#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using CellResizeType = RW.UI.SnapScrollerPlugin.SnapScroller.CellResizeType;

namespace RW.UI.SnapScrollerPlugin {

    [CustomEditor(typeof(SnapScroller))]
    [CanEditMultipleObjects]
    public class SnapScrollerEditor : Editor {

        SerializedProperty scrollDirection;
        SerializedProperty snapScrollerCellTemplate;
        SerializedProperty spacing;
        SerializedProperty loop;

        SerializedProperty cellResizeType;
        AnimBool isShow_cellScaleParams;
        SerializedProperty cellScaleForOnFocus;
        SerializedProperty cellScaleForOthers;
        AnimBool isShow_cellSizeParams;
        SerializedProperty cellSizeForOnFocus;
        SerializedProperty cellSizeForOthers;

        SerializedProperty moveSpeed;

        SerializedProperty onValueChanged;

        private void OnEnable() {
            scrollDirection = serializedObject.FindProperty("scrollDirection");
            snapScrollerCellTemplate = serializedObject.FindProperty("snapScrollerCellTemplate");
            spacing = serializedObject.FindProperty("spacing");
            loop = serializedObject.FindProperty("loop");

            cellResizeType = serializedObject.FindProperty("cellResizeType");
            cellScaleForOnFocus = serializedObject.FindProperty("cellScaleForOnFocus");
            cellScaleForOthers = serializedObject.FindProperty("cellScaleForOthers");
            cellSizeForOnFocus = serializedObject.FindProperty("cellSizeForOnFocus");
            cellSizeForOthers = serializedObject.FindProperty("cellSizeForOthers");
            var resizeType = (CellResizeType)cellResizeType.intValue;
            isShow_cellScaleParams = new AnimBool(!cellResizeType.hasMultipleDifferentValues && resizeType == CellResizeType.Scale);
            isShow_cellSizeParams = new AnimBool(!cellResizeType.hasMultipleDifferentValues && resizeType == CellResizeType.WidthAndHeight);

            moveSpeed = serializedObject.FindProperty("moveSpeed");

            onValueChanged = serializedObject.FindProperty("onValueChanged");
        }

        public override void OnInspectorGUI() {

            GUILayout.Box("A snap type scroller", GUILayout.ExpandWidth(true));

            serializedObject.Update();
            EditorGUILayout.PropertyField(snapScrollerCellTemplate);
            EditorGUILayout.PropertyField(scrollDirection);
            EditorGUILayout.PropertyField(loop);
            EditorGUILayout.PropertyField(spacing);
            EditorGUILayout.PropertyField(moveSpeed);

            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(cellResizeType);
            var resizeType = (CellResizeType)cellResizeType.intValue;
            isShow_cellScaleParams.target = (!cellResizeType.hasMultipleDifferentValues && resizeType == CellResizeType.Scale);
            isShow_cellSizeParams.target = (!cellResizeType.hasMultipleDifferentValues && resizeType == CellResizeType.WidthAndHeight);
            if (EditorGUILayout.BeginFadeGroup(isShow_cellScaleParams.faded)) {
                EditorGUILayout.PropertyField(cellScaleForOnFocus);
                EditorGUILayout.PropertyField(cellScaleForOthers);
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorGUILayout.BeginFadeGroup(isShow_cellSizeParams.faded)) {
                EditorGUILayout.PropertyField(cellSizeForOnFocus);
                EditorGUILayout.PropertyField(cellSizeForOthers);
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(onValueChanged);

            base.serializedObject.ApplyModifiedProperties();

            //base.OnInspectorGUI();
        }

    }
}
#endif