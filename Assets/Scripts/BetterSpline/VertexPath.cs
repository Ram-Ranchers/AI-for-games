using UnityEngine;

namespace BetterSpline 
{
    public class VertexPath 
    {
        private readonly Vector3[] localPoints;
        private readonly Vector3[] localTangents;
        private readonly Vector3[] localNormals;
        
        public readonly float[] times;
        private readonly float length;
        private readonly float[] cumulativeLengthAtEachVertex;
        private readonly Bounds bounds;
        public readonly Vector3 up;
        
        private const int accuracy = 10;
        private const float minVertexSpacing = .01f;

        private Transform transform;

        public VertexPath (BezierPath bezierPath, Transform transform, float maxAngleError = 0.3f, float minVertexDst = 0):
            this (bezierPath, VertexPathUtility.SplitBezierPathByAngleError (bezierPath, maxAngleError, minVertexDst, accuracy), transform) { }
        
        public VertexPath (BezierPath bezierPath, Transform transform, float vertexSpacing):
            this (bezierPath, VertexPathUtility.SplitBezierPathEvenly (bezierPath, Mathf.Max (vertexSpacing, minVertexSpacing), accuracy), transform) { }
        
        VertexPath (BezierPath bezierPath, VertexPathUtility.PathSplitData pathSplitData, Transform transform)
        {
            this.transform = transform;
            int numVerts = pathSplitData.vertices.Count;
            length = pathSplitData.cumulativeLength[numVerts - 1];

            localPoints = new Vector3[numVerts];
            localNormals = new Vector3[numVerts];
            localTangents = new Vector3[numVerts];
            cumulativeLengthAtEachVertex = new float[numVerts];
            times = new float[numVerts];
            bounds = new Bounds((pathSplitData.minMax.Min + pathSplitData.minMax.Max) / 2,
                pathSplitData.minMax.Max - pathSplitData.minMax.Min);
            
            up = bounds.size.z > bounds.size.y ? Vector3.up : -Vector3.forward;
            Vector3 lastRotationAxis = up;
            
            for (int i = 0; i < localPoints.Length; i++) {
                localPoints[i] = pathSplitData.vertices[i];
                localTangents[i] = pathSplitData.tangents[i];
                cumulativeLengthAtEachVertex[i] = pathSplitData.cumulativeLength[i];
                times[i] = cumulativeLengthAtEachVertex[i] / length;
                
                    if (i == 0) 
                    {
                        localNormals[0] = Vector3.Cross (lastRotationAxis, pathSplitData.tangents[0]).normalized;
                    } else
                    {
                        
                        Vector3 offset = (localPoints[i] - localPoints[i - 1]);
                        float sqrDst = offset.sqrMagnitude;
                        Vector3 r = lastRotationAxis - offset * 2 / sqrDst * Vector3.Dot(offset, lastRotationAxis);
                        Vector3 t = localTangents[i - 1] -
                                    offset * 2 / sqrDst * Vector3.Dot(offset, localTangents[i - 1]);

                        
                        Vector3 v2 = localTangents[i] - t;
                        float c2 = Vector3.Dot (v2, v2);

                        Vector3 finalRot = r - v2 * 2 / c2 * Vector3.Dot (v2, r);
                        Vector3 n = Vector3.Cross (finalRot, localTangents[i]).normalized;
                        localNormals[i] = n;
                        lastRotationAxis = finalRot;
                    }
            }

            float normalsAngleErrorAcrossJoin = Vector3.SignedAngle(localNormals[localNormals.Length - 1],
                localNormals[0], localTangents[0]);
                
            if (Mathf.Abs (normalsAngleErrorAcrossJoin) > 0.1f) 
            {
                for (int i = 1; i < localNormals.Length; i++) {
                    float t = (i / (localNormals.Length - 1f));
                    float angle = normalsAngleErrorAcrossJoin * t;
                    Quaternion rot = Quaternion.AngleAxis (angle, localTangents[i]);
                    localNormals[i] = rot * localNormals[i];
                }
            }
            
            for (int anchorIndex = 0; anchorIndex < pathSplitData.anchorVertexMap.Count - 1; anchorIndex++) 
            {
                int startVertIndex = pathSplitData.anchorVertexMap[anchorIndex];
                int endVertIndex = pathSplitData.anchorVertexMap[anchorIndex + 1];

                int num = endVertIndex - startVertIndex;
                if (anchorIndex == pathSplitData.anchorVertexMap.Count - 2) 
                {
                    num += 1;
                }
                for (int i = 0; i < num; i++) 
                {
                    float t = num == 1 ? 1f : i / (num - 1f);
                }
            }
        }
        
        public void UpdateTransform (Transform transform) 
        {
            this.transform = transform;
        }
        public int NumPoints => localPoints.Length;

        public Vector3 GetTangent (int index) 
        {
            return Utility.TransformDirection (localTangents[index], transform);
        }

        public Vector3 GetNormal (int index) 
        {
            return Utility.TransformDirection (localNormals[index], transform);
        }

        public Vector3 GetPoint (int index) 
        {
            return Utility.TransformPoint (localPoints[index], transform);
        }

        public Vector3 GetPointAtDistance (float dst) 
        {
            float t = dst / length;
            return GetPointAtTime (t);
        }

        public Vector3 GetDirectionAtDistance (float dst) 
        {
            float t = dst / length;
            return GetDirection (t);
        }

        public Vector3 GetNormalAtDistance (float dst) 
        {
            float t = dst / length;
            return GetNormal (t);
        }

        public Quaternion GetRotationAtDistance (float dst) 
        {
            float t = dst / length;
            return GetRotation (t);
        }
        
        public Vector3 GetPointAtTime (float t)
        {
            var data = CalculatePercentOnPathData (t);
            return Vector3.Lerp (GetPoint (data.previousIndex), GetPoint (data.nextIndex), data.percentBetweenIndices);
        }
        
        public Vector3 GetDirection (float t) 
        {
            var data = CalculatePercentOnPathData (t);
            Vector3 dir = Vector3.Lerp (localTangents[data.previousIndex], localTangents[data.nextIndex], data.percentBetweenIndices);
            return Utility.TransformDirection (dir, transform);
        }

        public Vector3 GetNormal (float t) 
        {
            var data = CalculatePercentOnPathData (t);
            Vector3 normal = Vector3.Lerp (localNormals[data.previousIndex], localNormals[data.nextIndex], data.percentBetweenIndices);
            return Utility.TransformDirection (normal, transform);
        }

        public Quaternion GetRotation (float t) 
        {
            var data = CalculatePercentOnPathData (t);
            Vector3 direction = Vector3.Lerp (localTangents[data.previousIndex], localTangents[data.nextIndex], data.percentBetweenIndices);
            Vector3 normal = Vector3.Lerp (localNormals[data.previousIndex], localNormals[data.nextIndex], data.percentBetweenIndices);
            return Quaternion.LookRotation (Utility.TransformDirection (direction, transform), Utility.TransformDirection (normal, transform));
        }

        public Vector3 GetClosestPointOnPath (Vector3 worldPoint) 
        {
            Vector3 localPoint = Utility.InverseTransformPoint(worldPoint, transform);

            TimeOnPathData data = CalculateClosestPointOnPathData (localPoint);
            Vector3 localResult = Vector3.Lerp (localPoints[data.previousIndex], localPoints[data.nextIndex], data.percentBetweenIndices);

            // Transform local result into world space
            return Utility.TransformPoint(localResult, transform);
        }

        public float GetClosestTimeOnPath (Vector3 worldPoint) 
        {
            Vector3 localPoint = Utility.InverseTransformPoint(worldPoint, transform);
            TimeOnPathData data = CalculateClosestPointOnPathData (localPoint);
            return Mathf.Lerp (times[data.previousIndex], times[data.nextIndex], data.percentBetweenIndices);
        }

        public float GetClosestDistanceAlongPath (Vector3 worldPoint) 
        {
            Vector3 localPoint = Utility.InverseTransformPoint(worldPoint, transform);
            TimeOnPathData data = CalculateClosestPointOnPathData(localPoint);
            return Mathf.Lerp (cumulativeLengthAtEachVertex[data.previousIndex], cumulativeLengthAtEachVertex[data.nextIndex], data.percentBetweenIndices);
        }
        
        TimeOnPathData CalculatePercentOnPathData (float t) 
        {
            int prevIndex = 0;
            int nextIndex = NumPoints - 1;
            int i = Mathf.RoundToInt (t * (NumPoints - 1));
            
            while (true) {
                if (t <= times[i]) 
                {
                    nextIndex = i;
                }
                else 
                {
                    prevIndex = i;
                }
                i = (nextIndex + prevIndex) / 2;

                if (nextIndex - prevIndex <= 1) 
                {
                    break;
                }
            }

            float abPercent = Mathf.InverseLerp (times[prevIndex], times[nextIndex], t);
            return new TimeOnPathData (prevIndex, nextIndex, abPercent);
        }

        TimeOnPathData CalculateClosestPointOnPathData (Vector3 localPoint) 
        {
            float minSqrDst = float.MaxValue;
            Vector3 closestPoint = Vector3.zero;
            int closestSegmentIndexA = 0;
            int closestSegmentIndexB = 0;

            for (int i = 0; i < localPoints.Length; i++) 
            {
                int nextI = i + 1;
                if (nextI >= localPoints.Length) 
                {
                    break;
                }

                Vector3 closestPointOnSegment = Utility.ClosestPointOnLineSegment (localPoint, localPoints[i], localPoints[nextI]);
                float sqrDst = (localPoint - closestPointOnSegment).sqrMagnitude;
                if (sqrDst < minSqrDst) {
                    minSqrDst = sqrDst;
                    closestPoint = closestPointOnSegment;
                    closestSegmentIndexA = i;
                    closestSegmentIndexB = nextI;
                }

            }
            float closestSegmentLength = (localPoints[closestSegmentIndexA] - localPoints[closestSegmentIndexB]).magnitude;
            float t = (closestPoint - localPoints[closestSegmentIndexA]).magnitude / closestSegmentLength;
            return new TimeOnPathData (closestSegmentIndexA, closestSegmentIndexB, t);
        }

        private struct TimeOnPathData 
        {
            public readonly int previousIndex;
            public readonly int nextIndex;
            public readonly float percentBetweenIndices;

            public TimeOnPathData (int prev, int next, float percentBetweenIndices) 
            {
                previousIndex = prev;
                nextIndex = next;
                this.percentBetweenIndices = percentBetweenIndices;
            }
        }
    }
}
