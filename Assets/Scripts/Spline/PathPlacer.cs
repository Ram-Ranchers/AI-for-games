using System;
using UnityEngine;

namespace Spline
{
    public class PathPlacer : MonoBehaviour
    {
        public float spacing = .1f;
        public float resolution = 1;

        private void Start()
        {
            Vector2[] points = FindObjectOfType<PathCreator>().path.CalculateEvenlySpacePoints(spacing, resolution);
            foreach (Vector2 p in points)
            {
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                g.transform.position = p;
                g.transform.localScale = Vector3.one * spacing * .5f;
            }
        }
    }
}
