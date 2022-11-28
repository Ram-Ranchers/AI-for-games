using UnityEngine;

namespace BetterSpline 
{
    public class PathCreator : MonoBehaviour 
    {
        public event System.Action pathUpdated;

        [SerializeField, HideInInspector]
        PathCreatorData editorData;
        [SerializeField, HideInInspector]
        bool initialized;

        GlobalDisplaySettings globalEditorDisplaySettings;
        
        public VertexPath path 
        {
            get 
            {
                if (!initialized) 
                {
                    InitializeEditorData();
                }
                return editorData.GetVertexPath(transform);
            }
        }
        
        public void InitializeEditorData() 
        {
            if (editorData == null) 
            {
                editorData = new PathCreatorData ();
            }
            editorData.bezierOrVertexPathModified -= TriggerPathUpdate;
            editorData.bezierOrVertexPathModified += TriggerPathUpdate;

            editorData.Initialize();
            initialized = true;
        }

        public PathCreatorData EditorData => editorData;

        private void TriggerPathUpdate() 
        {
            if (pathUpdated != null) 
            {
                pathUpdated();
            }
        }

#if UNITY_EDITOR
        
        void OnDrawGizmos() 
        {
            GameObject selectedObj = UnityEditor.Selection.activeGameObject;
            if (selectedObj != gameObject) 
            {
                if (path != null) 
                {
                    path.UpdateTransform (transform);

                    if (globalEditorDisplaySettings == null) 
                    {
                        globalEditorDisplaySettings = GlobalDisplaySettings.Load ();
                    }

                    if (globalEditorDisplaySettings.visibleWhenNotSelected) 
                    {
                        Gizmos.color = globalEditorDisplaySettings.bezierPath;

                        for (int i = 0; i < path.NumPoints; i++) 
                        {
                            int nextI = i + 1;
                            if (nextI >= path.NumPoints) 
                            {
                                break;
                            }
                            Gizmos.DrawLine (path.GetPoint (i), path.GetPoint (nextI));
                        }
                    }
                }
            }
        }
#endif
    }
}