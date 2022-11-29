using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace BetterSpline
{
	[CustomEditor(typeof(PathCreator))]
	public class PathEditor : Editor
	{
		const float segmentSelectDistanceThreshold = 10f;
		const float screenPolylineMaxAngleError = .3f;
		const float screenPolylineMinVertexDst = .01f;
		
		const float constantHandleScale = .01f;
		const float normalsSpacing = .2f;

		PathCreator creator;
		Editor globalDisplaySettingsEditor;
		ScreenSpacePolyLine screenSpaceLine;
		ScreenSpacePolyLine.MouseInfo pathMouseInfo;
		GlobalDisplaySettings globalDisplaySettings;
		PathHandle.HandleColours splineAnchorColours;
		PathHandle.HandleColours splineControlColours;
		Dictionary<GlobalDisplaySettings.HandleType, Handles.CapFunction> capFunctions;
		ArcHandle anchorAngleHandle = new();
		VertexPath normalsVertexPath;
		
		int selectedSegmentIndex;
		int draggingHandleIndex;
		int mouseOverHandleIndex;
		int handleIndexToDisplayAsTransform;

		bool shiftLastFrame;
		bool hasUpdatedScreenSpaceLine;
		bool hasUpdatedNormalsVertexPath;
		bool editingNormalsOld;

		Vector3 transformPos;
		Vector3 transformScale;
		Quaternion transformRot;

		Color handlesStartCol;
		
		public override void OnInspectorGUI()
		{
			Undo.RecordObject(creator, "Path settings changed");
			
			if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed")
			{
				data.PathModifiedByUndo();
			}
		}
		
		void OnSceneGUI()
		{
			if (!globalDisplaySettings.visibleBehindObjects)
			{
				Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
			}

			EventType eventType = Event.current.type;

			using (var check = new EditorGUI.ChangeCheckScope())
			{
				handlesStartCol = Handles.color;

				if (eventType != EventType.Repaint && eventType != EventType.Layout)
				{
					ProcessBezierPathInput(Event.current);
				}

				DrawBezierPathSceneEditor();

				if (eventType == EventType.Layout)
				{
					HandleUtility.AddDefaultControl(0);
				}

				if (check.changed)
				{
					EditorApplication.QueuePlayerLoopUpdate();
				}
			}

			SetTransformState();
		}
		
		void ProcessBezierPathInput(Event e)
		{
			int previousMouseOverHandleIndex = (mouseOverHandleIndex == -1) ? 0 : mouseOverHandleIndex;
			mouseOverHandleIndex = -1;
			for (int i = 0; i < bezierPath.NumPoints; i += 3)
			{

				int handleIndex = (previousMouseOverHandleIndex + i) % bezierPath.NumPoints;
				float handleRadius = GetHandleDiameter(globalDisplaySettings.anchorSize * data.bezierHandleScale, bezierPath[handleIndex]) / 2f;
				Vector3 pos = Utility.TransformPoint(bezierPath[handleIndex], creator.transform);
				float dst = HandleUtility.DistanceToCircle(pos, handleRadius);
				if (dst == 0)
				{
					mouseOverHandleIndex = handleIndex;
					break;
				}
			}
			
			if (mouseOverHandleIndex == -1)
			{
				if (e.type == EventType.MouseDown && e.button == 0 && e.shift)
				{
					UpdatePathMouseInfo();
		
					if (selectedSegmentIndex != -1 && selectedSegmentIndex < bezierPath.NumSegments)
					{
						Vector3 newPathPoint = pathMouseInfo.closestWorldPointToMouse;
						newPathPoint = Utility.InverseTransformPoint(newPathPoint, creator.transform);
						Undo.RecordObject(creator, "Split segment");
						bezierPath.SplitSegment(newPathPoint, selectedSegmentIndex, pathMouseInfo.timeOnBezierSegment);
					}
					else
					{

						var pointIdx = e.control || e.command ? 0 : bezierPath.NumPoints - 1;
		
						var endPointLocal = bezierPath[pointIdx];
						var endPointGlobal =
							Utility.TransformPoint(endPointLocal, creator.transform);
						var distanceCameraToEndpoint = (Camera.current.transform.position - endPointGlobal).magnitude;
						var newPointGlobal =
							Utility.GetMouseWorldPosition(distanceCameraToEndpoint);
						var newPointLocal =
							Utility.InverseTransformPoint(newPointGlobal, creator.transform);

						Undo.RecordObject(creator, "Add segment");
						if (e.control || e.command)
						{
							bezierPath.AddSegmentToStart(newPointLocal);
						}
						else
						{
							bezierPath.AddSegmentToEnd(newPointLocal);
						}

					}

				}
			}
			
			if (e.keyCode == KeyCode.Backspace || e.keyCode == KeyCode.Delete || (e.control || e.command) && e.type == EventType.MouseDown && e.button == 0)
			{

				if (mouseOverHandleIndex != -1)
				{
					Undo.RecordObject(creator, "Delete segment");
					bezierPath.DeleteSegment(mouseOverHandleIndex);
					if (mouseOverHandleIndex == handleIndexToDisplayAsTransform)
					{
						handleIndexToDisplayAsTransform = -1;
					}
					mouseOverHandleIndex = -1;
					Repaint();
				}
			}
			
			if (draggingHandleIndex == -1 && mouseOverHandleIndex == -1)
			{
				bool shiftDown = e.shift && !shiftLastFrame;
				if (shiftDown || (e.type == EventType.MouseMove || e.type == EventType.MouseDrag) && e.shift)
				{
					UpdatePathMouseInfo();
					bool notSplittingAtControlPoint = pathMouseInfo.timeOnBezierSegment > 0 && pathMouseInfo.timeOnBezierSegment < 1;
					if (pathMouseInfo.mouseDstToLine < segmentSelectDistanceThreshold && notSplittingAtControlPoint)
					{
						if (pathMouseInfo.closestSegmentIndex != selectedSegmentIndex)
						{
							selectedSegmentIndex = pathMouseInfo.closestSegmentIndex;
							HandleUtility.Repaint();
						}
					}
					else
					{
						selectedSegmentIndex = -1;
						HandleUtility.Repaint();
					}

				}
			}

			shiftLastFrame = e.shift;

		}

		void DrawBezierPathSceneEditor()
		{
			Bounds bounds = bezierPath.CalculateBoundsWithTransform(creator.transform);

			if (Event.current.type == EventType.Repaint)
			{
				for (int i = 0; i < bezierPath.NumSegments; i++)
				{
					Vector3[] points = bezierPath.GetPointsInSegment(i);
					for (int j = 0; j < points.Length; j++)
					{
						points[j] = Utility.TransformPoint(points[j], creator.transform);
					}

					if (data.showPerSegmentBounds)
					{
						Bounds segmentBounds = CubicBezierUtility.CalculateSegmentBounds(points[0], points[1], points[2], points[3]);
						Handles.color = globalDisplaySettings.segmentBounds;
						Handles.DrawWireCube(segmentBounds.center, segmentBounds.size);
					}

					bool highlightSegment = i == selectedSegmentIndex && Event.current.shift &&
					                        draggingHandleIndex == -1 && mouseOverHandleIndex == -1;
					Color segmentCol = highlightSegment
						? globalDisplaySettings.highlightedPath
						: globalDisplaySettings.bezierPath;
					Handles.DrawBezier(points[0], points[3], points[1], points[2], segmentCol, null, 2);
				}

				if (data.showPathBounds)
				{
					Handles.color = globalDisplaySettings.bounds;
					Handles.DrawWireCube(bounds.center, bounds.size);
				}
				
				if (data.showNormals)
				{
					if (!hasUpdatedNormalsVertexPath)
					{
						normalsVertexPath = new VertexPath(bezierPath, creator.transform, normalsSpacing);
						hasUpdatedNormalsVertexPath = true;
					}

					if (editingNormalsOld != data.showNormals)
					{
						editingNormalsOld = data.showNormals;
						Repaint();
					}

					Vector3[] normalLines = new Vector3[normalsVertexPath.NumPoints * 2];
					Handles.color = globalDisplaySettings.normals;
					for (int i = 0; i < normalsVertexPath.NumPoints; i++)
					{
						normalLines[i * 2] = normalsVertexPath.GetPoint(i);
						normalLines[i * 2 + 1] = normalsVertexPath.GetPoint(i) + normalsVertexPath.GetNormal(i) * globalDisplaySettings.normalsLength;
					}
					Handles.DrawLines(normalLines);
				}
			}

			if (data.displayAnchorPoints)
			{
				for (int i = 0; i < bezierPath.NumPoints; i += 3)
				{
					DrawHandle(i);
				}
			}
		}

		void DrawHandle(int i)
		{
			Vector3 handlePosition = Utility.TransformPoint(bezierPath[i], creator.transform);

			float anchorHandleSize = GetHandleDiameter(globalDisplaySettings.anchorSize * data.bezierHandleScale, bezierPath[i]);
			float controlHandleSize = GetHandleDiameter(globalDisplaySettings.controlSize * data.bezierHandleScale, bezierPath[i]);

			bool isAnchorPoint = i % 3 == 0;
			float handleSize = isAnchorPoint ? anchorHandleSize : controlHandleSize;
			bool doTransformHandle = i == handleIndexToDisplayAsTransform;

			PathHandle.HandleColours handleColours = isAnchorPoint ? splineAnchorColours : splineControlColours;
			if (i == handleIndexToDisplayAsTransform)
			{
				handleColours.defaultColour = isAnchorPoint ? globalDisplaySettings.anchorSelected : globalDisplaySettings.controlSelected;
			}
			var cap = capFunctions[isAnchorPoint ? globalDisplaySettings.anchorShape : globalDisplaySettings.controlShape];
			PathHandle.HandleInputType handleInputType;
			handlePosition = PathHandle.DrawHandle(handlePosition, isAnchorPoint, handleSize, cap, handleColours, out handleInputType, i);

			if (doTransformHandle)
			{
				if (data.showNormals && Tools.current == Tool.Rotate && isAnchorPoint)
				{
					Handles.color = handlesStartCol;

					int attachedControlIndex = (i == bezierPath.NumPoints - 1) ? i - 1 : i + 1;
					Vector3 dir = (bezierPath[attachedControlIndex] - handlePosition).normalized;
					float handleRotOffset = (360 + bezierPath.GlobalNormalsAngle) % 360;
					anchorAngleHandle.radius = handleSize * 3;
					anchorAngleHandle.angle = handleRotOffset + bezierPath.GetAnchorNormalAngle(i / 3);
					Vector3 handleDirection = Vector3.Cross(dir, Vector3.up);
					Matrix4x4 handleMatrix = Matrix4x4.TRS(
						handlePosition,
						Quaternion.LookRotation(handleDirection, dir),
						Vector3.one
					);

					using (new Handles.DrawingScope(handleMatrix))
					{
						// draw the handle
						EditorGUI.BeginChangeCheck();
						anchorAngleHandle.DrawHandle();
						if (EditorGUI.EndChangeCheck())
						{
							Undo.RecordObject(creator, "Set angle");
							bezierPath.SetAnchorNormalAngle(i / 3, anchorAngleHandle.angle - handleRotOffset);
						}
					}

				}
				else
				{
					handlePosition = Handles.DoPositionHandle(handlePosition, Quaternion.identity);
				}

			}

			switch (handleInputType)
			{
				case PathHandle.HandleInputType.LMBDrag:
					draggingHandleIndex = i;
					handleIndexToDisplayAsTransform = -1;
					Repaint();
					break;
				case PathHandle.HandleInputType.LMBRelease:
					draggingHandleIndex = -1;
					handleIndexToDisplayAsTransform = -1;
					Repaint();
					break;
				case PathHandle.HandleInputType.LMBClick:
					draggingHandleIndex = -1;
					if (Event.current.shift)
					{
						handleIndexToDisplayAsTransform = -1;
					}
					else
					{
						if (handleIndexToDisplayAsTransform == i)
						{
							handleIndexToDisplayAsTransform = -1;
						}
						else
						{
							handleIndexToDisplayAsTransform = i;
						}
					}
					Repaint();
					break;
				case PathHandle.HandleInputType.LMBPress:
					if (handleIndexToDisplayAsTransform != i)
					{
						handleIndexToDisplayAsTransform = -1;
						Repaint();
					}
					break;
			}

			Vector3 localHandlePosition = Utility.InverseTransformPoint(handlePosition, creator.transform);

			if (bezierPath[i] != localHandlePosition)
			{
				Undo.RecordObject(creator, "Move point");
				bezierPath.MovePoint(i, localHandlePosition);

			}

		}

		void OnDisable()
		{
			Tools.hidden = false;
		}

		void OnEnable()
		{
			creator = (PathCreator)target;
			creator.InitializeEditorData();

			data.bezierCreated -= ResetState;
			data.bezierCreated += ResetState;
			Undo.undoRedoPerformed -= OnUndoRedo;
			Undo.undoRedoPerformed += OnUndoRedo;

			LoadDisplaySettings();
			UpdateGlobalDisplaySettings();
			ResetState();
			SetTransformState(true);
		}

		void SetTransformState(bool initialize = false)
		{
			Transform t = creator.transform;
			if (!initialize)
			{
				if (transformPos != t.position || t.localScale != transformScale || t.rotation != transformRot)
				{
					data.PathTransformed();
				}
			}
			transformPos = t.position;
			transformScale = t.localScale;
			transformRot = t.rotation;
		}

		void OnUndoRedo()
		{
			hasUpdatedScreenSpaceLine = false;
			hasUpdatedNormalsVertexPath = false;
			selectedSegmentIndex = -1;

			Repaint();
		}
		
		void LoadDisplaySettings()
		{
			globalDisplaySettings = GlobalDisplaySettings.Load();

			capFunctions = new Dictionary<GlobalDisplaySettings.HandleType, Handles.CapFunction>();
			capFunctions.Add(GlobalDisplaySettings.HandleType.Circle, Handles.CylinderHandleCap);
			capFunctions.Add(GlobalDisplaySettings.HandleType.Sphere, Handles.SphereHandleCap);
			capFunctions.Add(GlobalDisplaySettings.HandleType.Square, Handles.CubeHandleCap);
		}

		void UpdateGlobalDisplaySettings()
		{
			var gds = globalDisplaySettings;
			splineAnchorColours = new PathHandle.HandleColours(gds.anchor, gds.anchorHighlighted, gds.anchorSelected, gds.handleDisabled);
			splineControlColours = new PathHandle.HandleColours(gds.control, gds.controlHighlighted, gds.controlSelected, gds.handleDisabled);

			anchorAngleHandle.fillColor = new Color(1, 1, 1, .05f);
			anchorAngleHandle.wireframeColor = Color.grey;
			anchorAngleHandle.radiusHandleColor = Color.clear;
			anchorAngleHandle.angleHandleColor = Color.white;
		}

		void ResetState()
		{
			selectedSegmentIndex = -1;
			draggingHandleIndex = -1;
			mouseOverHandleIndex = -1;
			handleIndexToDisplayAsTransform = -1;
			hasUpdatedScreenSpaceLine = false;
			hasUpdatedNormalsVertexPath = false;

			bezierPath.OnModified -= OnPathModifed;
			bezierPath.OnModified += OnPathModifed;

			SceneView.RepaintAll();
			EditorApplication.QueuePlayerLoopUpdate();
		}

		void OnPathModifed()
		{
			hasUpdatedScreenSpaceLine = false;
			hasUpdatedNormalsVertexPath = false;

			RepaintUnfocusedSceneViews();
		}

		void RepaintUnfocusedSceneViews()
		{
			if (SceneView.sceneViews.Count > 1)
			{
				foreach (SceneView sv in SceneView.sceneViews)
				{
					if (EditorWindow.focusedWindow != sv)
					{
						sv.Repaint();
					}
				}
			}
		}

		void UpdatePathMouseInfo()
		{

			if (!hasUpdatedScreenSpaceLine || screenSpaceLine != null && screenSpaceLine.TransformIsOutOfDate())
			{
				screenSpaceLine = new ScreenSpacePolyLine(bezierPath, creator.transform, screenPolylineMaxAngleError, screenPolylineMinVertexDst);
				hasUpdatedScreenSpaceLine = true;
			}
			pathMouseInfo = screenSpaceLine.CalculateMouseInfo();
		}

		float GetHandleDiameter(float diameter, Vector3 handlePosition)
		{
			float scaledDiameter = diameter * constantHandleScale;
			if (data.keepConstantHandleSize)
			{
				scaledDiameter *= HandleUtility.GetHandleSize(handlePosition) * 2.5f;
			}
			return scaledDiameter;
		}

		BezierPath bezierPath => data.bezierPath;

		PathCreatorData data => creator.EditorData;
	}
}