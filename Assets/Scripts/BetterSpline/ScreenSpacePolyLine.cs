using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BetterSpline
{
	public class ScreenSpacePolyLine
	{
		private const int accuracyMultiplier = 10;

		private const float intermediaryThreshold = .2f;

		private readonly List<Vector3> verticesWorld;
	
		private readonly List<int> vertexToPathSegmentMap;

		private readonly int[] segmentStartIndices;

		private readonly float pathLengthWorld;
		private readonly float[] cumululativeLengthWorld;

		private Vector2[] points;

		private Vector3 prevCamPos;
		private Quaternion prevCamRot;
		private bool prevCamIsOrtho;

		private readonly Transform transform;
		private readonly Vector3 transformPosition;
		private readonly Quaternion transformRotation;
		private readonly Vector3 transformScale;

		public ScreenSpacePolyLine(BezierPath bezierPath, Transform transform, float maxAngleError, float minVertexDst, float accuracy = 1)
		{
			this.transform = transform;
			transformPosition = transform.position;
			transformRotation = transform.rotation;
			transformScale = transform.localScale;
			
			verticesWorld = new List<Vector3>();
			vertexToPathSegmentMap = new List<int>();
			segmentStartIndices = new int[bezierPath.NumSegments + 1];

			verticesWorld.Add(bezierPath[0]);
			vertexToPathSegmentMap.Add(0);
			Vector3 prevPointOnPath = bezierPath[0];
			float dstSinceLastVertex;
			Vector3 lastAddedPoint = prevPointOnPath;
			float dstSinceLastIntermediary;

			for (int segmentIndex = 0; segmentIndex < bezierPath.NumSegments; segmentIndex++)
			{
				Vector3[] segmentPoints = bezierPath.GetPointsInSegment(segmentIndex);
				verticesWorld.Add(segmentPoints[0]);
				vertexToPathSegmentMap.Add(segmentIndex);
				segmentStartIndices[segmentIndex] = verticesWorld.Count - 1;

				prevPointOnPath = segmentPoints[0];
				lastAddedPoint = prevPointOnPath;
				dstSinceLastVertex = 0;
				dstSinceLastIntermediary = 0;

				float estimatedSegmentLength = CubicBezierUtility.EstimateCurveLength(segmentPoints[0],
					segmentPoints[1], segmentPoints[2], segmentPoints[3]);
				int divisions = Mathf.CeilToInt(estimatedSegmentLength * accuracy * accuracyMultiplier);
				float increment = 1f / divisions;

				for (float t = increment; t <= 1; t += increment)
				{
					Vector3 pointOnPath = CubicBezierUtility.EvaluateCurve(segmentPoints[0], segmentPoints[1],
						segmentPoints[2], segmentPoints[3], t);
					Vector3 nextPointOnPath = CubicBezierUtility.EvaluateCurve(segmentPoints[0], segmentPoints[1],
						segmentPoints[2], segmentPoints[3], t + increment);
					
					float localAngle = 180 - Utility.MinAngle(prevPointOnPath, pointOnPath, nextPointOnPath);

					float angleFromPrevVertex =
						180 - Utility.MinAngle(lastAddedPoint, pointOnPath, nextPointOnPath);
					float angleError = Mathf.Max(localAngle, angleFromPrevVertex);


					if (angleError > maxAngleError && dstSinceLastVertex >= minVertexDst)
					{
						dstSinceLastVertex = 0;
						dstSinceLastIntermediary = 0;
						verticesWorld.Add(pointOnPath);
						vertexToPathSegmentMap.Add(segmentIndex);
						lastAddedPoint = pointOnPath;
					}
					else
					{
						if (dstSinceLastIntermediary > intermediaryThreshold)
						{
							verticesWorld.Add(pointOnPath);
							vertexToPathSegmentMap.Add(segmentIndex);
							dstSinceLastIntermediary = 0;
						}
						else
						{
							dstSinceLastIntermediary += (pointOnPath - prevPointOnPath).magnitude;
						}
						dstSinceLastVertex += (pointOnPath - prevPointOnPath).magnitude;
					}
					prevPointOnPath = pointOnPath;
				}
			}

			segmentStartIndices[bezierPath.NumSegments] = verticesWorld.Count;

			verticesWorld.Add(bezierPath[bezierPath.NumPoints - 1]);
			cumululativeLengthWorld = new float[verticesWorld.Count];
			for (int i = 0; i < verticesWorld.Count; i++)
			{
				verticesWorld[i] = Utility.TransformPoint(verticesWorld[i], transform);
				if (i > 0)
				{
					pathLengthWorld += (verticesWorld[i - 1] - verticesWorld[i]).magnitude;
					cumululativeLengthWorld[i] = pathLengthWorld;
				}
			}

		}

		void ComputeScreenSpace()
		{
			if (Camera.current.transform.position != prevCamPos || Camera.current.transform.rotation != prevCamRot || Camera.current.orthographic != prevCamIsOrtho)
			{
				points = new Vector2[verticesWorld.Count];
				for (int i = 0; i < verticesWorld.Count; i++)
				{
					points[i] = HandleUtility.WorldToGUIPoint(verticesWorld[i]);
				}

				prevCamPos = Camera.current.transform.position;
				prevCamRot = Camera.current.transform.rotation;
				prevCamIsOrtho = Camera.current.orthographic;
			}
		}

		public MouseInfo CalculateMouseInfo()
		{
			ComputeScreenSpace();

			Vector2 mousePos = Event.current.mousePosition;
			float minDst = float.MaxValue;
			int closestPolyLineSegmentIndex = 0;
			int closestBezierSegmentIndex = 0;

			for (int i = 0; i < points.Length - 1; i++)
			{
				float dst = HandleUtility.DistancePointToLineSegment(mousePos, points[i], points[i + 1]);

				if (dst < minDst)
				{
					minDst = dst;
					closestPolyLineSegmentIndex = i;
					closestBezierSegmentIndex = vertexToPathSegmentMap[i];
				}
			}

			Vector2 closestPointOnLine = Utility.ClosestPointOnLineSegment(mousePos,
				points[closestPolyLineSegmentIndex], points[closestPolyLineSegmentIndex + 1]);
			float dstToPointOnLine = (points[closestPolyLineSegmentIndex] - closestPointOnLine).magnitude;

			float d = (points[closestPolyLineSegmentIndex] - points[closestPolyLineSegmentIndex + 1]).magnitude;
			float percentBetweenVertices = (d == 0) ? 0 : dstToPointOnLine / d;
			Vector3 closestPoint3D = Vector3.Lerp(verticesWorld[closestPolyLineSegmentIndex],
				verticesWorld[closestPolyLineSegmentIndex + 1], percentBetweenVertices);

			float distanceAlongPathWorld = cumululativeLengthWorld[closestPolyLineSegmentIndex] +
			                               Vector3.Distance(verticesWorld[closestPolyLineSegmentIndex], closestPoint3D);

			int bezierSegmentStartIndex = segmentStartIndices[closestBezierSegmentIndex];
			int bezierSegmentEndIndex = segmentStartIndices[closestBezierSegmentIndex + 1];
			float bezierSegmentLength = cumululativeLengthWorld[bezierSegmentEndIndex] -
			                            cumululativeLengthWorld[bezierSegmentStartIndex];
			float distanceAlongBezierSegment =
				distanceAlongPathWorld - cumululativeLengthWorld[bezierSegmentStartIndex];
			float timeAlongBezierSegment = distanceAlongBezierSegment / bezierSegmentLength;

			return new MouseInfo(minDst, closestPoint3D, timeAlongBezierSegment, closestBezierSegmentIndex);
		}

		public bool TransformIsOutOfDate()
		{
			return transform.position != transformPosition || transform.rotation != transformRotation ||
			       transform.localScale != transformScale;
		}
		
		public struct MouseInfo
		{
			public readonly float mouseDstToLine;
			public readonly Vector3 closestWorldPointToMouse;
			public readonly float timeOnBezierSegment;
			public readonly int closestSegmentIndex;
			
			public MouseInfo(float mouseDstToLine, Vector3 closestWorldPointToMouse, float timeOnBezierSegment, int closestSegmentIndex)
			{
				this.mouseDstToLine = mouseDstToLine;
				this.closestWorldPointToMouse = closestWorldPointToMouse;
				this.timeOnBezierSegment = timeOnBezierSegment;
				this.closestSegmentIndex = closestSegmentIndex;
			}
		}
	}
}