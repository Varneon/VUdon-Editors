﻿using UnityEditor;
using UnityEngine;

namespace Varneon.VUdon.Editors.Editor
{
    [CustomPropertyDrawer(typeof(MultiPropertyAttribute), true)]
    public class MultiPropertyAttributeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Error and warning textures
        /// </summary>
        private static readonly Texture
            errorIcon = EditorGUIUtility.IconContent("console.erroricon").image,
            warningIcon = EditorGUIUtility.IconContent("console.warnicon").image;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get the abstract multi attribute
            MultiPropertyAttribute multiAttribute = (MultiPropertyAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);

            // Initialize the attribute if it hasn't been already
            if (!multiAttribute.initialized)
            {
                // Get all multi attributes attached to the field
                multiAttribute.attributes = (MultiPropertyAttribute[])fieldInfo.GetCustomAttributes(typeof(MultiPropertyAttribute), false);

                // Iterate through all attributes
                foreach (MultiPropertyAttribute attribute in multiAttribute.attributes)
                {
                    switch (attribute.Type)
                    {
                        // Modify the property label
                        case FieldAttributeType.Label:
                            multiAttribute.label = new GUIContent(((FieldLabelAttribute)attribute).Label);
                            break;

                        // Set the property to be disabled when another boolean property's state matches conditions
                        case FieldAttributeType.Disable:
                            multiAttribute.disable = true;
                            FieldDisableAttribute disableAttribute = (FieldDisableAttribute)attribute;
                            multiAttribute.disableProperty = property.serializedObject.FindProperty(disableAttribute.Property);
                            multiAttribute.disableWhenTrue = disableAttribute.WhenTrue;
                            break;

                        // Set range for a float or integer field
                        case FieldAttributeType.Range:
                            multiAttribute.isRange = true;
                            break;

                        // Add warning or error for null reference on ObjectField
                        case FieldAttributeType.NullWarning:
                            multiAttribute.nullError = (multiAttribute.nullWarning = true) && ((FieldNullWarningAttribute)attribute).IsError;
                            break;
                    }
                }

                // If the label hasn't been modified, cache the original label
                if(multiAttribute.label == null) { multiAttribute.label = label; }

                // Set the initialized state
                multiAttribute.initialized = true;
            }

            // Override the label with cached one
            label = multiAttribute.label;

            // Begin disabled group if field has disable attribute
            if (multiAttribute.disable)
            {
                EditorGUI.BeginDisabledGroup(multiAttribute.disableWhenTrue == multiAttribute.disableProperty.boolValue);
            }

            // Add alternative handling to ranged fields
            if (multiAttribute.isRange)
            {
                FieldRangeAttribute rangeAttribute = (FieldRangeAttribute)multiAttribute;

                if (property.propertyType == SerializedPropertyType.Float)
                {
                    EditorGUI.Slider(position, property, rangeAttribute.Min, rangeAttribute.Max, label);
                }
                else if (property.propertyType == SerializedPropertyType.Integer)
                {
                    EditorGUI.IntSlider(position, property, (int)rangeAttribute.Min, (int)rangeAttribute.Max, label);
                }
                else
                {
                    EditorGUI.PropertyField(position, property, label);
                }
            }
            // Add alternative handling to fields with null warning
            else if (multiAttribute.nullWarning)
            {
                bool nullError = multiAttribute.nullError;

                bool isNull = property.objectReferenceValue == null;

                if (isNull)
                {
                    GUI.color = nullError ? Color.red : Color.yellow;

                    label = new GUIContent(label.text, multiAttribute.nullError ? errorIcon : warningIcon);
                }

                position = EditorGUI.PrefixLabel(position, label);

                GUI.color = Color.white;

                int indentLevel = EditorGUI.indentLevel;

                EditorGUI.indentLevel = 0;

                EditorGUI.PropertyField(position, property, GUIContent.none);

                EditorGUI.indentLevel = indentLevel;
            }
            // Draw the default field
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }

            if (multiAttribute.disable)
            {
                EditorGUI.EndDisabledGroup();
            }

            EditorGUI.EndProperty();
        }
    }
}
