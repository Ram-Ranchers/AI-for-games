using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnchangedSplines;
using TerrainGeneration;
using UnityEngine.UI;

public class Road : MonoBehaviour
{
    public int lanesLeft, lanesRight;
    public Vector3 end1, end2;
    public List<Vehicle> leftVehicles, rightVehicles;
    [SerializeField] private float maxSpeed = 60;
    private int size;
    private PathCreator pathCreator;
    private EndOfPathInstruction end;
    public RawImage image;
    private Texture2D heightMap;
    public float average = 0;

    // Start is called before the first frame update
    void Start()
    {
        pathCreator = gameObject.GetComponent<PathCreator>();
        end = EndOfPathInstruction.Stop;
        end1 = pathCreator.path.GetPointAtDistance(0, end);
        end2 = pathCreator.path.GetPointFromEnd(0, end);

        heightMap = image.GetComponent<RawImage>().texture as Texture2D;

        for (int i = 0; i < pathCreator.bezierPath.NumPoints; i++)
        {
            average += heightMap.GetPixel((int)pathCreator.bezierPath[i].x, (int)pathCreator.bezierPath[i].y).grayscale;
        }

        average = average / pathCreator.bezierPath.NumPoints;

        maxSpeed = maxSpeed * average;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }
}
