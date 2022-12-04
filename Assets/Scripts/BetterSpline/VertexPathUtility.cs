using System.Collections.Generic;
using UnityEngine;

namespace BetterSpline
{
    public static class VertexPathUtility
    {
	    public static PathSplitData SplitBezierPathByAngleError(BezierPath bezierPath, float maxAngleError, float minVertexDst, float accuracy)
        {
			PathSplitData splitData = new PathSplitData();

            splitData.vertices.Add(bezierPath[0]);
            splitData.tangents.Add(CubicBezierUtility.EvaluateCurveDerivative(bezierPath.GetPointsInSegment(0), 0)
	            .normalized);
            splitData.cumulativeLength.Add(0);
            splitData.anchorVertexMap.Add(0);
			splitData.minMax.AddValue(bezierPath[0]);

            Vector3 prevPointOnPath = bezierPath[0];
            Vector3 lastAddedPoint = bezierPath[0];

            float currentPathLength = 0;
            float dstSinceLastVertex = 0;
            
            for (int segmentIndex = 0; segmentIndex < bezierPath.NumSegments; segmentIndex++)
            {
                Vector3[] segmentPoints = bezierPath.GetPointsInSegment(segmentIndex);
                float estimatedSegmentLength = CubicBezierUtility.EstimateCurveLength(segmentPoints[0],
	                segmentPoints[1], segmentPoints[2], segmentPoints[3]);
                int divisions = Mathf.CeilToInt(estimatedSegmentLength * accuracy);
                float increment = 1f / divisions;

                for (float t = increment; t <= 1; t += increment)
                {
                    bool isLastPointOnPath = t + increment > 1 && segmentIndex == bezierPath.NumSegments - 1;
                    if (isLastPointOnPath)
                    {
                        t = 1;
                    }
                    Vector3 pointOnPath = CubicBezierUtility.EvaluateCurve(segmentPoints, t);
                    Vector3 nextPointOnPath = CubicBezierUtility.EvaluateCurve(segmentPoints, t + increment);
                    
                    float localAngle = 180 - Utility.MinAngle(prevPointOnPath, pointOnPath, nextPointOnPath);
                    float angleFromPrevVertex =
	                    180 - Utility.MinAngle(lastAddedPoint, pointOnPath, nextPointOnPath);
                    float angleError = Mathf.Max(localAngle, angleFromPrevVertex);


                    if (angleError > maxAngleError && dstSinceLastVertex >= minVertexDst || isLastPointOnPath)
                    {
	                    currentPathLength += (lastAddedPoint - pointOnPath).magnitude;
                        splitData.cumulativeLength.Add(currentPathLength);
                        splitData.vertices.Add(pointOnPath);
                        splitData.tangents.Add(CubicBezierUtility.EvaluateCurveDerivative(segmentPoints, t).normalized);
						splitData.minMax.AddValue(pointOnPath);
                        dstSinceLastVertex = 0;
                        lastAddedPoint = pointOnPath;
                    }
                    else
                    {
                        dstSinceLastVertex += (pointOnPath - prevPointOnPath).magnitude;
                    }
                    prevPointOnPath = pointOnPath;
                }
                splitData.anchorVertexMap.Add(splitData.vertices.Count - 1);
            }
			return splitData;
		}

		public static PathSplitData SplitBezierPathEvenly(BezierPath bezierPath, float spacing, float accuracy)
        {
			PathSplitData splitData = new PathSplitData();

            splitData.vertices.Add(bezierPath[0]);
            splitData.tangents.Add(CubicBezierUtility.EvaluateCurveDerivative(bezierPath.GetPointsInSegment(0), 0)
	            .normalized);
            splitData.cumulativeLength.Add(0);
            splitData.anchorVertexMap.Add(0);
			splitData.minMax.AddValue(bezierPath[0]);

            Vector3 prevPointOnPath = bezierPath[0];
            Vector3 lastAddedPoint = bezierPath[0];

            float currentPathLength = 0;
            float dstSinceLastVertex = 0;
            
            for (int segmentIndex = 0; segmentIndex < bezierPath.NumSegments; segmentIndex++)
            {
                Vector3[] segmentPoints = bezierPath.GetPointsInSegment(segmentIndex);
                float estimatedSegmentLength = CubicBezierUtility.EstimateCurveLength(segmentPoints[0],
	                segmentPoints[1], segmentPoints[2], segmentPoints[3]);
                int divisions = Mathf.CeilToInt(estimatedSegmentLength * accuracy);
                float increment = 1f / divisions;

                for (float t = increment; t <= 1; t += increment)
                {
                    bool isLastPointOnPath = t + increment > 1 && segmentIndex == bezierPath.NumSegments - 1;
                    if (isLastPointOnPath)
                    {
                        t = 1;
                    }
                    Vector3 pointOnPath = CubicBezierUtility.EvaluateCurve(segmentPoints, t);
					dstSinceLastVertex += (pointOnPath - prevPointOnPath).magnitude;
					
					if (dstSinceLastVertex > spacing) 
					{
						float overshootDst = dstSinceLastVertex - spacing;
						pointOnPath += (prevPointOnPath-pointOnPath).normalized * overshootDst;
						t-=increment;
					}

                    if (dstSinceLastVertex >= spacing || isLastPointOnPath)
                    {
                        currentPathLength += (lastAddedPoint - pointOnPath).magnitude;
                        splitData.cumulativeLength.Add(currentPathLength);
                        splitData.vertices.Add(pointOnPath);
                        splitData.tangents.Add(CubicBezierUtility.EvaluateCurveDerivative(segmentPoints, t).normalized);
						splitData.minMax.AddValue(pointOnPath);
                        dstSinceLastVertex = 0;
                        lastAddedPoint = pointOnPath;
                    }
                    prevPointOnPath = pointOnPath;
                }
                splitData.anchorVertexMap.Add(splitData.vertices.Count - 1);
            }
			return splitData;
		}
		
	   public class PathSplitData 
	   {
		   public readonly List<Vector3> vertices = new();
		   public readonly List<Vector3> tangents = new();
		   public readonly List<float> cumulativeLength = new();
		   public readonly List<int> anchorVertexMap = new();
		   public readonly MinMax3D minMax = new();
	   }
    }
}