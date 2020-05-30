using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FloatRangeSliderAttribute))]
public class FloatRangeSliderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var originIndentLevel = EditorGUI.indentLevel;
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(
            position, GUIUtility.GetControlID(FocusType.Passive), label
        );
        EditorGUI.indentLevel = 0;
        var minProperty = property.FindPropertyRelative("min");
        var maxProperty = property.FindPropertyRelative("max");
        var minValue = minProperty.floatValue;
        var maxValue = maxProperty.floatValue;
        var fieldWidth = position.width / 4f - 4f;
        var sliderWidth = position.width / 2f;
        position.width = fieldWidth;
        minValue = EditorGUI.FloatField(position, minValue);
        position.x += fieldWidth + 4f;
        position.width = sliderWidth;
        var limit = attribute as FloatRangeSliderAttribute;
        EditorGUI.MinMaxSlider(
            position, ref minValue, ref maxValue, limit.Min, limit.Max
        );
        position.x += sliderWidth + 4f;
        position.width = fieldWidth;
        maxValue = EditorGUI.FloatField(position, maxValue);
        if (minValue < limit.Min) {
            minValue = limit.Min;
        }
        if (maxValue < minValue) {
            maxValue = minValue;
        }
        else if (maxValue > limit.Max) {
            maxValue = limit.Max;
        }
        minProperty.floatValue = minValue;
        maxProperty.floatValue = maxValue;

        EditorGUI.EndProperty();
        EditorGUI.indentLevel = originIndentLevel;
    }
}
