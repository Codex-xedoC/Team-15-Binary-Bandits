// Name: Robert MacGillivray
// File: PathfinderToolEditor.cs
// Date: Dec.13.2019
// Purpose: To add some inspector functionality to the pathfinding demo tool

// Last Updated: Jul.01.2022 by Robert MacGillivray

using UnityEngine;
using UnityEditor;

namespace UmbraEvolution.UmbraMazeMagician
{
    /// <summary>
    /// Makes the pathfinding demo tool's inspector look pretty
    /// </summary>
    [CustomEditor(typeof(PathfinderTool))]
    public class PathfinderToolEditor : Editor
    {
        SerializedProperty _breadcrumbProperty;
        SerializedProperty _testMazeProperty;
        SerializedProperty _testPathfindingMethodProperty;
        SerializedProperty _startPositionProperty;
        SerializedProperty _endPositionProperty;
        SerializedProperty _startCoordinateProperty;
        SerializedProperty _endCoordinateProperty;
        SerializedProperty _startNodeProperty;
        SerializedProperty _endNodeProperty;

        private void OnEnable()
        {
            _breadcrumbProperty = serializedObject.FindProperty("Breadcrumb");
            _testMazeProperty = serializedObject.FindProperty("TestMaze");
            _testPathfindingMethodProperty = serializedObject.FindProperty("TestPathfindingMethod");
            _startPositionProperty = serializedObject.FindProperty("StartPosition");
            _endPositionProperty = serializedObject.FindProperty("EndPosition");
            _startCoordinateProperty = serializedObject.FindProperty("StartCoordinate");
            _endCoordinateProperty = serializedObject.FindProperty("EndCoordinate");
            _startNodeProperty = serializedObject.FindProperty("StartNode");
            _endNodeProperty = serializedObject.FindProperty("EndNode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_breadcrumbProperty);
            EditorGUILayout.PropertyField(_testMazeProperty);
            EditorGUILayout.PropertyField(_testPathfindingMethodProperty);
            switch((PathfinderTool.PathfindingType)_testPathfindingMethodProperty.intValue)
            {
                case PathfinderTool.PathfindingType.Positions:
                    EditorGUILayout.PropertyField(_startPositionProperty);
                    EditorGUILayout.PropertyField(_endPositionProperty);
                    break;
                case PathfinderTool.PathfindingType.Coordinates:
                    EditorGUILayout.PropertyField(_startCoordinateProperty);
                    EditorGUILayout.PropertyField(_endCoordinateProperty);
                    break;
                case PathfinderTool.PathfindingType.Nodes:
                    EditorGUILayout.PropertyField(_startNodeProperty);
                    EditorGUILayout.PropertyField(_endNodeProperty);
                    break;
            }

            if (GUILayout.Button("Run Pathfinding"))
            {
                ((PathfinderTool)target).RunTest();
            }

            if (GUILayout.Button("Clear Breadcrumbs"))
            {
                ((PathfinderTool)target).CleanUpTest();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
