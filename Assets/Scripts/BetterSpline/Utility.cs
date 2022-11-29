using UnityEngine;
using UnityEditor;

namespace BetterSpline 
{
    public static class Utility 
    {
        public static Vector3 TransformPoint (Vector3 p, Transform t) 
        {
            float scale = Vector3.Dot (t.lossyScale, Vector3.one) / 3;
            Vector3 constrainedPos = t.position;
            Quaternion constrainedRot = t.rotation;
            return constrainedRot * p * scale + constrainedPos;
        }

        public static Vector3 InverseTransformPoint (Vector3 p, Transform t) 
        {
            Vector3 constrainedPos = t.position;
            Quaternion constrainedRot = t.rotation;

            float scale = Vector3.Dot (t.lossyScale, Vector3.one) / 3;
            var offset = p - constrainedPos;

            return Quaternion.Inverse (constrainedRot) * offset / scale;
        }
        
        public static Vector3 TransformDirection (Vector3 p, Transform t) 
        {
            Quaternion constrainedRot = t.rotation;
            return constrainedRot * p;
        }
        
        public static Vector2 ClosestPointOnLineSegment (Vector2 p, Vector2 a, Vector2 b) 
        {
            Vector2 aB = b - a;
            Vector2 aP = p - a;
            float sqrLenAB = aB.sqrMagnitude;

            if (sqrLenAB == 0)
            {
                return a;
            }
            
            float t = Mathf.Clamp01 (Vector2.Dot (aP, aB) / sqrLenAB);
            return a + aB * t;
        }

        public static float MinAngle (Vector3 a, Vector3 b, Vector3 c) 
        {
            return Vector3.Angle (a - b, c - b);
        }
        
        public static Vector3 GetMouseWorldPosition(float depthFor3DSpace = 10)
        {
            var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 worldMouse = Physics.Raycast(mouseRay, out var hitInfo, depthFor3DSpace * 2f) ? 
                hitInfo.point : mouseRay.GetPoint(depthFor3DSpace);
            
            float yDir = mouseRay.direction.y;
            if (yDir != 0)
            {
                float dstToXZPlane = Mathf.Abs(mouseRay.origin.y / yDir);
                worldMouse = mouseRay.GetPoint(dstToXZPlane);
            }
   
            return worldMouse;
        }
    }
}