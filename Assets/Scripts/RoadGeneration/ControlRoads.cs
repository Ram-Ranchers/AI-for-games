using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ShittyRoadShit
{
	public class ControlRoads : MonoBehaviour
	{

		private RoadNetwork network { get; set; }
		private RoadRenderer roadRenderer { get; set; }

		public List<RoadSegment> RoadSegments { get; private set; }
		public List<Intersection> Intersections { get; private set; }

		public int GridType;
		public Terrain terrain;

		public void GenerateRoads()
		{
			network = new RoadNetwork(100f);
			if (GridType == 0)
			{
				network.AddCityCentreX(new Vector2(0, 0), 120f);
			}

			else
			{
				network.AddCityCentreY(new Vector2(0, 0), 12f);
			}

			network.SplitSegments(0);
			network.SplitSegments(0);
			network.SplitSegments(1);
			network.SplitSegments(1);
			network.SplitSegments(2);
			network.SplitSegments(3);

			roadRenderer = GetComponent<RoadRenderer>();
			roadRenderer.ClearData();

			foreach (RoadSegment segment in network.RoadSegments)
				roadRenderer.AddRoadSegments(segment);

			foreach (Intersection inter in network.RoadIntersections)
				roadRenderer.AddIntersection(inter);

			RoadSegments = new List<RoadSegment>(network.RoadSegments);
		}
	}
}

