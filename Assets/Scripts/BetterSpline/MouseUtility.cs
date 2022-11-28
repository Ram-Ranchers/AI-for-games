using UnityEditor;
using UnityEngine;

namespace BetterSpline
{
    public static class MouseUtility
    {
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
