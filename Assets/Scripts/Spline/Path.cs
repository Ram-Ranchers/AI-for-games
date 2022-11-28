using System.Collections.Generic;
using UnityEngine;

namespace Spline
{
    [System.Serializable]
    public class Path
    {
        [SerializeField, HideInInspector]
        private List<Vector3> points;
        [SerializeField, HideInInspector]
        private bool autoSetControlPoints;
    
        public Path(Vector3 centre)
        {
            points = new List<Vector3>
            {
                centre + Vector3.left * 2f,
                centre + Vector3.left * 1f + Vector3.forward * .5f,
                centre + Vector3.right * 1f - Vector3.forward * .5f,
                centre + Vector3.right * 2f
            };
        }

        public Vector3 this[int i] => points[i];

        public bool AutoSetControlPoints
        {
            get
            {
                return autoSetControlPoints;
            }
            set
            {
                if (autoSetControlPoints != value)
                {
                    autoSetControlPoints = value;
                    if (autoSetControlPoints)
                    {
                        AutoSetAllControlPoints();
                    }
                }
            }
        }
    
        public int NumPoints => points.Count;

        public int NumSegments => points.Count / 3;

        public void AddSegment(Vector3 anchorPos)
        {
            points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
            points.Add((points[points.Count - 1] + anchorPos) * .5f);
            points.Add(anchorPos);

            if (autoSetControlPoints)
            {
                AutoSetAllAffectedControlPoints(points.Count - 1);
            }
        }

        public void SplitSegment(Vector3 anchorPos, int segmentIndex)
        {
            points.InsertRange(segmentIndex * 3 + 2, new Vector3[]{Vector2.zero, anchorPos, Vector2.zero});
            if (autoSetControlPoints)
            {
                AutoSetAllAffectedControlPoints(segmentIndex * 3 + 3);
            }
            else
            {
                AutoSetAnchorControlPoints(segmentIndex * 3 + 3);
            }
        }
        
        public void DeleteSegment(int anchorIndex)
        {
            if (NumSegments > 2 || NumSegments > 1)
            {
                if (anchorIndex == 0)
                {
                    points.RemoveRange(0, 3);
                }
                else if (anchorIndex == points.Count - 1)
                {
                    points.RemoveRange(anchorIndex - 2, 3);
                }
                else
                {
                    points.RemoveRange(anchorIndex - 1, 3);
                }
            }
        }
        
        public Vector3[] GetPointsInSegment(int i)
        {
            return new [] { points[i * 3], points[i * 3 + 1], points[i * 3 + 2], points[LoopIndex(i * 3 + 3)] };
        }

        public void MovePoint(int i, Vector3 pos)
        {
            pos.y = 0;
            
            Vector3 deltaMove = pos - points[i];

            if (i % 3 == 0 || !autoSetControlPoints)
            {
                points[i] = pos;

                if (autoSetControlPoints)
                {
                    AutoSetAllAffectedControlPoints(i);
                }
                else
                {
                    if (i % 3 == 0)
                    {
                        if (i + 1 < points.Count)
                        {
                            points[LoopIndex(i + 1)] += deltaMove;
                        }

                        if (i - 1 >= 0)
                        {
                            points[LoopIndex(i - 1)] += deltaMove;
                        }
                    }
                    else
                    {
                        bool nexPointIsAnchor = (i + 1) % 3 == 0;
                        int correspondingControlIndex = (nexPointIsAnchor) ? i + 2 : i - 2;
                        int anchorIndex = (nexPointIsAnchor) ? i + 1 : i - 1;

                        if (correspondingControlIndex >= 0 && correspondingControlIndex < points.Count)
                        {
                            float dst = (points[LoopIndex(anchorIndex)] - points[LoopIndex(correspondingControlIndex)])
                                .magnitude;
                            Vector3 dir = (points[LoopIndex(anchorIndex)] - pos).normalized;
                            points[LoopIndex(correspondingControlIndex)] = points[LoopIndex(anchorIndex)] + dir * dst;
                        }
                    }
                }
            }
        }

        public Vector3[] CalculateEvenlySpacePoints(float spacing, float resolution = 1)
        {
            List<Vector3> evenlySpacedPoints = new List<Vector3>();
            evenlySpacedPoints.Add(points[0]);
            Vector3 previousPoint = points[0];
            float dstSinceLastEvenPoint = 0;

            for (int segmentIndex = 0; segmentIndex < NumSegments; segmentIndex++)
            {
                Vector3[] p = GetPointsInSegment(segmentIndex);
                float controlNetLength = Vector3.Distance(p[0],
                    p[1]) + Vector3.Distance(p[1], p[2]) + Vector3.Distance(p[2], p[3]);
                float estimatedCurveLength = Vector3.Distance(p[0], p[3]) + controlNetLength / 2f;
                int divisions = Mathf.CeilToInt(estimatedCurveLength * resolution * 10);
                float t = 0;
                while (t <= 1)
                {
                    t += 1f / divisions;
                    Vector3 pointOnCurve = Bezier.EvaluateCubic(p[0], p[1], p[2], p[3], t);
                    dstSinceLastEvenPoint += Vector3.Distance(previousPoint, pointOnCurve);

                    while (dstSinceLastEvenPoint >= spacing)
                    {
                        float overshootDst = dstSinceLastEvenPoint - spacing;
                        Vector3 newEvenlySpacePoint =
                            pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDst;
                        evenlySpacedPoints.Add(newEvenlySpacePoint);
                        dstSinceLastEvenPoint = overshootDst;
                        previousPoint = newEvenlySpacePoint;
                    }
                    
                    previousPoint = pointOnCurve;
                }
            }
            
            evenlySpacedPoints.Add(points[points.Count - 1]);

            return evenlySpacedPoints.ToArray();
        }
        
        private void AutoSetAllAffectedControlPoints(int updatedAnchorIndex)
        {
            for (int i = updatedAnchorIndex - 3; i <= updatedAnchorIndex + 3; i += 3)
            {
                if (i >= 0 && i < points.Count)
                {
                    AutoSetAnchorControlPoints(LoopIndex(i));
                }
            }
        
            AutoSetStartAndEndControls();
        }
    
        private void AutoSetAllControlPoints()
        {
            for (int i = 0; i < points.Count; i += 3)
            {
                AutoSetAnchorControlPoints(i);
            }
        
            AutoSetStartAndEndControls();
        }
    
        private void AutoSetAnchorControlPoints(int anchorIndex)
        {
            Vector3 anchorPos = points[anchorIndex];
            Vector3 dir = Vector3.zero;
            float[] neighbourDistances = new float[2];

            if (anchorIndex - 3 >= 0)
            {
                Vector3 offset = points[LoopIndex(anchorIndex - 3)] - anchorPos;
                dir += offset.normalized;
                neighbourDistances[0] = offset.magnitude;
            }
            if (anchorIndex + 3 >= 0)
            {
                Vector3 offset = points[LoopIndex(anchorIndex + 3)] - anchorPos;
                dir -= offset.normalized;
                neighbourDistances[1] = -offset.magnitude;
            }

            dir.Normalize();
        
            for (int i = 0; i < 2; i++)
            {
                int controlIndex = anchorIndex + i * 2 - 1;
                if (controlIndex >= 0 && controlIndex < points.Count)
                {
                    points[LoopIndex(controlIndex)] = anchorPos + dir * neighbourDistances[i] * .5f;
                }
            }
        }
 
        private void AutoSetStartAndEndControls()
        {
            points[1] = (points[0] + points[2]) * .5f;
            points[points.Count - 2] = (points[points.Count - 1] + points[points.Count - 3]) * .5f;
        }
    
        private int LoopIndex(int i)
        {
            return (i + points.Count) % points.Count;
        }
    }
}
