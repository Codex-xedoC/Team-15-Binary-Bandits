//Name: Robert MacGillivray
//Date: Aug.05.2016
//File: PlazaPrefabEditor.cs
//Purpose: To create a clean custom inspector for the PlazaPrefab component

//Last Updated: Jul.01.2022 by Robert MacGillivray

using UnityEditor;

namespace UmbraEvolution.UmbraMazeMagician
{
    /// <summary>
    /// Makes the PlazaPrefab inspector pretty
    /// </summary>
    [CustomEditor(typeof(PlazaPrefab))]
    public class PlazaPrefabEditor : Editor
    {
        private SerializedProperty _widthProp;
        private SerializedProperty _lengthProp;
        private SerializedProperty _heightProp;
        private SerializedProperty _fixedPositionProp;
        private SerializedProperty _xPositionProp;
        private SerializedProperty _zPositionProp;

        private void OnEnable()
        {
            _widthProp = serializedObject.FindProperty("width");
            _lengthProp = serializedObject.FindProperty("length");
            _heightProp = serializedObject.FindProperty("height");
            _fixedPositionProp = serializedObject.FindProperty("fixedPosition");
            _xPositionProp = serializedObject.FindProperty("xPosition");
            _zPositionProp = serializedObject.FindProperty("zPosition");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_widthProp);
            EditorGUILayout.PropertyField(_lengthProp);
            EditorGUILayout.PropertyField(_heightProp);
            EditorGUILayout.PropertyField(_fixedPositionProp);
            if (_fixedPositionProp.boolValue)
            {
                EditorGUILayout.PropertyField(_xPositionProp);
                EditorGUILayout.PropertyField(_zPositionProp);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}