using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BetterSpline
{
	[System.Serializable]
	public class BezierPath
	{
		public event System.Action OnModified;

		[SerializeField, HideInInspector]
		List<Vector3> points;
		[SerializeField, HideInInspector]
		float autoControlLength = .3f;
		[SerializeField, HideInInspector]
		bool boundsUpToDate;
		
		[SerializeField, HideInInspector]
		List<float> perAnchorNormalsAngle;
		[SerializeField, HideInInspector]
		float globalNormalsAngle;
		[SerializeField, HideInInspector]
		bool flipNormals;
		
		public BezierPath(Vector3 centre)
		{
			points = new List<Vector3> 
			{
				centre + Vector3.left * 2f,
				centre + Vector3.left * 1f + Vector3.forward * .5f,
				centre + Vector3.right * 1f - Vector3.forward * .5f,
				centre + Vector3.right * 2f
			};

			perAnchorNormalsAngle = new List<float> { 0, 0 };
		}
		
		public BezierPath(IEnumerable<Vector3> points)
		{
			Vector3[] pointsArray = points.ToArray();

			if (pointsArray.Length < 2)
			{
				Debug.LogError("Path requires at least 2 anchor points.");
			}
			else
			{
				this.points = new List<Vector3> { pointsArray[0], Vector3.zero, Vector3.zero, pointsArray[1] };
				perAnchorNormalsAngle = new List<float>(new float[] { 0, 0 });

				for (int i = 2; i < pointsArray.Length; i++)
				{
					AddSegmentToEnd(pointsArray[i]);
					perAnchorNormalsAngle.Add(0);
				}
			}
		}
		
		public Vector3 this[int i] => points[i];
		
		public void SetPoint(int i, Vector3 localPosition, bool suppressPathModifiedEvent = false)
		{
			points[i] = localPosition;
			if (!suppressPathModifiedEvent)
			{
				NotifyPathModified();
			}
		}
		
		public int NumPoints => points.Count;
		
		public int NumAnchorPoints => (points.Count + 2) / 3;
		
		public int NumSegments => points.Count / 3;

		public void AddSegmentToEnd(Vector3 anchorPos)
		{
			int lastAnchorIndex = points.Count - 1;
			
			Vector3 secondControlForOldLastAnchorOffset = points[lastAnchorIndex] - points[lastAnchorIndex - 1];
			Vector3 secondControlForOldLastAnchor = points[lastAnchorIndex] + secondControlForOldLastAnchorOffset;
			Vector3 controlForNewAnchor = (anchorPos + secondControlForOldLastAnchor) * .5f;

			points.Add(secondControlForOldLastAnchor);
			points.Add(controlForNewAnchor);
			points.Add(anchorPos);
			perAnchorNormalsAngle.Add(perAnchorNormalsAngle[perAnchorNormalsAngle.Count - 1]);

			AutoSetAllAffectedControlPoints(points.Count - 1);
			
			NotifyPathModified();
		}
		
		public void AddSegmentToStart(Vector3 anchorPos)
		{
			Vector3 secondControlForOldFirstAnchorOffset = points[0] - points[1];

			Vector3 secondControlForOldFirstAnchor = points[0] + secondControlForOldFirstAnchorOffset;
			Vector3 controlForNewAnchor = (anchorPos + secondControlForOldFirstAnchor) * .5f;
			points.Insert(0, anchorPos);
			points.Insert(1, controlForNewAnchor);
			points.Insert(2, secondControlForOldFirstAnchor);
			perAnchorNormalsAngle.Insert(0, perAnchorNormalsAngle[0]);
			
			AutoSetAllAffectedControlPoints(0);
			
			NotifyPathModified();
		}
		
		public void SplitSegment(Vector3 anchorPos, int segmentIndex, float splitTime)
		{
			if (float.IsNaN(splitTime))
			{
				Debug.Log("Trying to split segment, but given value was invalid");
				return;
			}

			splitTime = Mathf.Clamp01(splitTime);
			
			points.InsertRange(segmentIndex * 3 + 2, new[] { Vector3.zero, anchorPos, Vector3.zero });
			AutoSetAllAffectedControlPoints(segmentIndex * 3 + 3);
			
			int newAnchorAngleIndex = (segmentIndex + 1) % perAnchorNormalsAngle.Count;
			float anglePrev = perAnchorNormalsAngle[segmentIndex];
			float angleNext = perAnchorNormalsAngle[newAnchorAngleIndex];
			float splitAngle = Mathf.LerpAngle(anglePrev, angleNext, splitTime);
			perAnchorNormalsAngle.Insert(newAnchorAngleIndex, splitAngle);

			NotifyPathModified();
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

				perAnchorNormalsAngle.RemoveAt(anchorIndex / 3);

				AutoSetAllControlPoints();

				NotifyPathModified();
			}
		}
		
		public Vector3[] GetPointsInSegment(int segmentIndex)
		{
			segmentIndex = Mathf.Clamp(segmentIndex, 0, NumSegments - 1);
			return new[]
			{
				this[segmentIndex * 3], this[segmentIndex * 3 + 1], this[segmentIndex * 3 + 2],
				this[LoopIndex(segmentIndex * 3 + 3)]
			};
		}

		public void MovePoint(int i, Vector3 pointPos, bool suppressPathModifiedEvent = false)
		{
			pointPos.y = 0;

			if (i % 3 == 0)
			{
				points[i] = pointPos;

				AutoSetAllAffectedControlPoints(i);

				if (!suppressPathModifiedEvent)
				{
					NotifyPathModified();
				}
			}
		}

		public Bounds CalculateBoundsWithTransform(Transform transform)
		{
			MinMax3D minMax = new MinMax3D();

			for (int i = 0; i < NumSegments; i++)
			{
				Vector3[] p = GetPointsInSegment(i);
				for (int j = 0; j < p.Length; j++)
				{
					p[j] = Utility.TransformPoint(p[j], transform);
				}

				minMax.AddValue(p[0]);
				minMax.AddValue(p[3]);

				List<float> extremePointTimes = CubicBezierUtility.ExtremePointTimes(p[0], p[1], p[2], p[3]);
				foreach (float t in extremePointTimes)
				{
					minMax.AddValue(CubicBezierUtility.EvaluateCurve(p, t));
				}
			}

			return new Bounds((minMax.Min + minMax.Max) / 2, minMax.Max - minMax.Min);
		}
		
		public bool FlipNormals
		{
			get => flipNormals;
			set
			{
				if (flipNormals != value)
				{
					flipNormals = value;
					NotifyPathModified();
				}
			}
		}
		
		public float GlobalNormalsAngle
		{
			get => globalNormalsAngle;
			set
			{
				if (value != globalNormalsAngle)
				{
					globalNormalsAngle = value;
					NotifyPathModified();
				}
			}
		}
		
		public float GetAnchorNormalAngle(int anchorIndex)
		{
			return perAnchorNormalsAngle[anchorIndex] % 360;
		}
		
		public void SetAnchorNormalAngle(int anchorIndex, float angle)
		{
			angle = (angle + 360) % 360;
			if (perAnchorNormalsAngle[anchorIndex] != angle)
			{
				perAnchorNormalsAngle[anchorIndex] = angle;
				NotifyPathModified();
			}
		}
		
		public void ResetNormalAngles()
		{
			for (int i = 0; i < perAnchorNormalsAngle.Count; i++)
			{
				perAnchorNormalsAngle[i] = 0;
			}
			globalNormalsAngle = 0;
			NotifyPathModified();
		}
		
		void UpdateBounds()
		{
			if (boundsUpToDate)
			{
				return;
			}
			
			MinMax3D minMax = new MinMax3D();

			for (int i = 0; i < NumSegments; i++)
			{
				Vector3[] p = GetPointsInSegment(i);
				minMax.AddValue(p[0]);
				minMax.AddValue(p[3]);

				List<float> extremePointTimes = CubicBezierUtility.ExtremePointTimes(p[0], p[1], p[2], p[3]);
				foreach (float t in extremePointTimes)
				{
					minMax.AddValue(CubicBezierUtility.EvaluateCurve(p, t));
				}
			}

			boundsUpToDate = true;
		}
		
		void AutoSetAllAffectedControlPoints(int updatedAnchorIndex)
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
		
		void AutoSetAllControlPoints()
		{
			if (NumAnchorPoints > 2)
			{
				for (int i = 0; i < points.Count; i += 3)
				{
					AutoSetAnchorControlPoints(i);
				}
			}

			AutoSetStartAndEndControls();
		}

		void AutoSetAnchorControlPoints(int anchorIndex)
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
					points[LoopIndex(controlIndex)] = anchorPos + dir * neighbourDistances[i] * autoControlLength;
				}
			}
		}
		
		void AutoSetStartAndEndControls()
		{
			if (NumAnchorPoints == 2)
			{
				points[1] = points[0] + (points[3] - points[0]) * .25f;
				points[2] = points[3] + (points[0] - points[3]) * .25f;
			}
			else
			{
				points[1] = (points[0] + points[2]) * .5f;
				points[points.Count - 2] = (points[points.Count - 1] + points[points.Count - 3]) * .5f;
			}
		}

		int LoopIndex(int i)
		{
			return (i + points.Count) % points.Count;
		}

		public void NotifyPathModified()
		{
			boundsUpToDate = false;
			if (OnModified != null)
			{
				OnModified();
			}
		}
	}
}