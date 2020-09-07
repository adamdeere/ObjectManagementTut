using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FloatRange))]
public class FloatRangeDraw : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var originalIndentLevel = EditorGUI.indentLevel;
        var originalLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);
        position.width /= 2f;
        EditorGUIUtility.labelWidth = position.width / 2f;
        EditorGUI.indentLevel = 1;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("min"));
        position.x += position.width;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("max"));
        EditorGUI.EndProperty();
        EditorGUI.indentLevel = originalIndentLevel;
        EditorGUIUtility.labelWidth = originalLabelWidth;
    }
}
