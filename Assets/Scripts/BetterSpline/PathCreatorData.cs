using UnityEngine;

namespace BetterSpline 
{
    [System.Serializable]
    public class PathCreatorData 
    {
        public event System.Action bezierOrVertexPathModified;
        public event System.Action bezierCreated;

        [SerializeField]
        BezierPath _bezierPath;
        VertexPath _vertexPath;

        [SerializeField]
        bool vertexPathUpToDate;
        
        public float vertexPathMaxAngleError = .3f;
        public float vertexPathMinVertexSpacing = 0.01f;
        
        public bool showPathBounds;
        public bool showPerSegmentBounds;
        public bool displayAnchorPoints = true;
        public float bezierHandleScale = 1;
        public bool keepConstantHandleSize;
        
        public bool showNormals;

        public void Initialize () 
        {
            if (_bezierPath == null) 
            {
                CreateBezier(Vector3.zero);
            }
            vertexPathUpToDate = false;
            _bezierPath.OnModified -= BezierPathEdited;
            _bezierPath.OnModified += BezierPathEdited;
        }

        public void ResetBezierPath (Vector3 centre) 
        {
            CreateBezier (centre);
        }

        void CreateBezier (Vector3 centre) 
        {
            if (_bezierPath != null) 
            {
                _bezierPath.OnModified -= BezierPathEdited;
            }
            
            _bezierPath = new BezierPath (centre);

            _bezierPath.OnModified += BezierPathEdited;
            vertexPathUpToDate = false;

            if (bezierOrVertexPathModified != null) 
            {
                bezierOrVertexPathModified();
            }
            if (bezierCreated != null) {
                bezierCreated();
            }
        }

        public BezierPath bezierPath 
        {
            get => _bezierPath;
            set 
            {
                _bezierPath.OnModified -= BezierPathEdited;
                vertexPathUpToDate = false;
                _bezierPath = value;
                _bezierPath.OnModified += BezierPathEdited;

                if (bezierOrVertexPathModified != null) {
                    bezierOrVertexPathModified();
                }
                if (bezierCreated != null) {
                    bezierCreated();
                }
            }
        }
        
        public VertexPath GetVertexPath (Transform transform) 
        {
            if (!vertexPathUpToDate || _vertexPath == null) 
            {
                vertexPathUpToDate = true;
                _vertexPath = new VertexPath(bezierPath, transform, vertexPathMaxAngleError,
                    vertexPathMinVertexSpacing);
            }
            return _vertexPath;
        }

        public void PathTransformed() 
        {
            if (bezierOrVertexPathModified != null) 
            {
                bezierOrVertexPathModified();
            }
        }

        public void VertexPathSettingsChanged()
        {
            vertexPathUpToDate = false;
            if (bezierOrVertexPathModified != null) 
            {
                bezierOrVertexPathModified();
            }
        }

        public void PathModifiedByUndo() 
        {
            vertexPathUpToDate = false;
            if (bezierOrVertexPathModified != null) 
            {
                bezierOrVertexPathModified();
            }
        }

        void BezierPathEdited() 
        {
            vertexPathUpToDate = false;
            if (bezierOrVertexPathModified != null) 
            {
                bezierOrVertexPathModified();
            }
        }
    }
}