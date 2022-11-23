using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ControlRoads : MonoBehaviour {

	private RoadNetwork network {get;set;}
	private RoadRenderer roadRenderer {get;set;}

	public List<RoadSegment> RoadSegments {get; private set;}
	public List<Intersection> Intersections {get; private set;}

	public int GridType;

    private void Start()
    {
		GenerateClick();
	}

	public void GenerateClick()
	{
		this.network = new RoadNetwork (100f);
		if(GridType == 0)
        {
			this.network.AddCityCentreX(new Vector2(0, 0), 120f);
		}
			
		else
        {
			this.network.AddCityCentreY(new Vector2(0, 0), 12f);
		}
	
		this.network.SplitSegments (0);
		this.network.SplitSegments (0);
		this.network.SplitSegments (1);
		this.network.SplitSegments (1);
		this.network.SplitSegments (2);
		this.network.SplitSegments (3);
		
		this.roadRenderer = this.GetComponent<RoadRenderer> ();
		this.roadRenderer.ClearData ();

		foreach (RoadSegment segment in this.network.RoadSegments)
            this.roadRenderer.AddRoadSegments(segment);

		foreach (Intersection inter in this.network.RoadIntersections)
			this.roadRenderer.AddIntersection (inter);

		this.RoadSegments = new List<RoadSegment> (this.network.RoadSegments);
	}
}
