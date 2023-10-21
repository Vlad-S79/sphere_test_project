using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Core.Common.SerializedCollections.Editor
{
    [CustomPropertyDrawer(typeof(SerializedHashSet<>), true)]
    public class SerializedHashSetPropertyDrawer : PropertyDrawer
    {
        private const float LineHeight = 20f;
        private const float Spacing = 5f;
        private const float IconWidth = 15f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.indentLevel = 0;
            
            SerializedProperty valuesProperty = property.FindPropertyRelative("values");
            
            Rect nameRect = new Rect(position.x, position.y, position.width, LineHeight);
            EditorGUI.LabelField(nameRect, label, EditorStyles.boldLabel);

            Dictionary<string, int> keyCounts = GetKeyCounts(valuesProperty);
            for (int i = 0; i < valuesProperty.arraySize; i++)
            {
                string key = GetKey(valuesProperty.GetArrayElementAtIndex(i));
                Rect valueRect;

                Rect removeRect = new Rect(position.x + position.width - IconWidth - Spacing, position.y + (i * (LineHeight + Spacing)) + LineHeight, IconWidth, LineHeight);               
                Rect warningRect = new Rect();
                
                if (keyCounts[key] > 1)
                {
                    valueRect = new Rect(position.x + IconWidth + Spacing, position.y + (i * (LineHeight + Spacing)) + LineHeight, position.width - Spacing - IconWidth - Spacing - IconWidth - Spacing, LineHeight);
                    warningRect = new Rect(position.x, position.y + (i * (LineHeight + Spacing)) + 1f + LineHeight + 2f, IconWidth, IconWidth);
                }
                else
                {
                    valueRect = new Rect(position.x, position.y + (i * (LineHeight + Spacing)) + LineHeight, position.width - Spacing - IconWidth - Spacing, LineHeight);
                }
                
                EditorGUI.PropertyField(valueRect, valuesProperty.GetArrayElementAtIndex(i), GUIContent.none);

                // Draw the warning icon for duplicate keys
                if (keyCounts[key] > 1)
                {
                    GUI.DrawTexture(warningRect, EditorGUIUtility.IconContent("console.warnicon.sml").image);
                }

                if (GUI.Button(removeRect, EditorGUIUtility.IconContent("Toolbar Minus", "Remove Key"), EditorStyles.miniButtonRight))
                {
                    valuesProperty.DeleteArrayElementAtIndex(i);
                    break;
                }
            }

            Rect addRect = new Rect(position.x, position.y + (valuesProperty.arraySize) * (LineHeight + Spacing) + LineHeight, position.width, LineHeight);
            if (GUI.Button(addRect, "Add Value", EditorStyles.miniButton))
            {
                valuesProperty.InsertArrayElementAtIndex(valuesProperty.arraySize);
            }

            EditorGUI.EndProperty();
        }

        private Dictionary<string, int> GetKeyCounts(SerializedProperty keysProperty)
        {
            Dictionary<string, int> keyCounts = new Dictionary<string, int>();
            for (int i = 0; i < keysProperty.arraySize; i++)
            {
                string key = GetKey(keysProperty.GetArrayElementAtIndex(i));
                
                if (!keyCounts.ContainsKey(key))
                {
                    keyCounts[key] = 1;
                }
                else
                {
                    keyCounts[key]++;
                }
            }
            return keyCounts;
        }

        private string GetKey(SerializedProperty serializedProperty)
        {
            switch (serializedProperty.propertyType)
            {
                case SerializedPropertyType.String:
                    return serializedProperty.stringValue;
                case SerializedPropertyType.ObjectReference:
                    var reference = serializedProperty.objectReferenceValue;
                    if (reference == null) return "";
                    return reference.GetInstanceID().ToString();
                case SerializedPropertyType.Enum:
                    return serializedProperty.enumDisplayNames[serializedProperty.enumValueIndex];
                default: return "";
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty valuesProperty = property.FindPropertyRelative("values");
            return (valuesProperty.arraySize + 1) * (LineHeight + Spacing) + LineHeight;
        }
    }
}