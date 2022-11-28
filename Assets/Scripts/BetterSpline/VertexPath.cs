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
            bounds = new Bounds ((pathSplitData.minMax.Min + pathSplitData.minMax.Max) / 2, pathSplitData.minMax.Max - pathSplitData.minMax.Min);
            
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
                        Vector3 r = lastRotationAxis - offset * 2 / sqrDst * Vector3.Dot (offset, lastRotationAxis);
                        Vector3 t = localTangents[i - 1] - offset * 2 / sqrDst * Vector3.Dot (offset, localTangents[i - 1]);

                        
                        Vector3 v2 = localTangents[i] - t;
                        float c2 = Vector3.Dot (v2, v2);

                        Vector3 finalRot = r - v2 * 2 / c2 * Vector3.Dot (v2, r);
                        Vector3 n = Vector3.Cross (finalRot, localTangents[i]).normalized;
                        localNormals[i] = n;
                        lastRotationAxis = finalRot;
                    }
            }
            
            float normalsAngleErrorAcrossJoin = Vector3.SignedAngle (localNormals[localNormals.Length - 1], localNormals[0], localTangents[0]);
                
            if (Mathf.Abs (normalsAngleErrorAcrossJoin) > 0.1f) 
            {
                for (int i = 1; i < localNormals.Length; i++) {
                    float t = (i / (localNormals.Length - 1f));
                    float angle = normalsAngleErrorAcrossJoin * t;
                    Quaternion rot = Quaternion.AngleAxis (angle, localTangents[i]);
                    localNormals[i] = rot * localNormals[i] * ((bezierPath.FlipNormals) ? -1 : 1);
                }
            }
            
            for (int anchorIndex = 0; anchorIndex < pathSplitData.anchorVertexMap.Count - 1; anchorIndex++) 
            {
                int nextAnchorIndex = anchorIndex + 1;

                float startAngle = bezierPath.GetAnchorNormalAngle (anchorIndex) + bezierPath.GlobalNormalsAngle;
                float endAngle = bezierPath.GetAnchorNormalAngle (nextAnchorIndex) + bezierPath.GlobalNormalsAngle;
                float deltaAngle = Mathf.DeltaAngle (startAngle, endAngle);

                int startVertIndex = pathSplitData.anchorVertexMap[anchorIndex];
                int endVertIndex = pathSplitData.anchorVertexMap[anchorIndex + 1];

                int num = endVertIndex - startVertIndex;
                if (anchorIndex == pathSplitData.anchorVertexMap.Count - 2) 
                {
                    num += 1;
                }
                for (int i = 0; i < num; i++) 
                {
                    int vertIndex = startVertIndex + i;
                    float t = num == 1 ? 1f : i / (num - 1f);
                    float angle = startAngle + deltaAngle * t;
                    Quaternion rot = Quaternion.AngleAxis (angle, localTangents[vertIndex]);
                    localNormals[vertIndex] = rot * localNormals[vertIndex] * (bezierPath.FlipNormals ? -1 : 1);
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
            return MathUtility.TransformDirection (localTangents[index], transform);
        }

        public Vector3 GetNormal (int index) 
        {
            return MathUtility.TransformDirection (localNormals[index], transform);
        }

        public Vector3 GetPoint (int index) 
        {
            return MathUtility.TransformPoint (localPoints[index], transform);
        }
    }
}
