using System.Linq;
using UnityEditor;
using UnityEditorInternal;
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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float defaultHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            return property.propertyType switch
            {
                SerializedPropertyType.Rect or SerializedPropertyType.RectInt => defaultHeight * 2 + spacing,
                SerializedPropertyType.Bounds or SerializedPropertyType.BoundsInt => defaultHeight * 3 + spacing * 2,
                _ => base.GetPropertyHeight(property, label),
            };
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get the abstract multi attribute
            MultiPropertyAttribute multiAttribute = (MultiPropertyAttribute)attribute;

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
                            multiAttribute.label = new GUIContent(((FieldLabelAttribute)attribute).Label, label.tooltip);
                            break;

                        // Set the property to be disabled when another boolean property's state matches conditions
                        case FieldAttributeType.Disable:
                            multiAttribute.disable = true;
                            FieldDisableAttribute disableAttribute = (FieldDisableAttribute)attribute;
                            multiAttribute.disabledCheckFunction = MultiPropertyAttribute.DisabledCheckFunction(disableAttribute.Logic, disableAttribute.Properties.Select(p => property.serializedObject.FindProperty(p)).ToArray());
                            break;

                        // Set range for a float or integer field
                        case FieldAttributeType.Range:
                            multiAttribute.isRange = true;
                            FieldRangeAttribute rangeAttribute = (FieldRangeAttribute)attribute;
                            multiAttribute.min = rangeAttribute.Min;
                            multiAttribute.max = rangeAttribute.Max;
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
                EditorGUI.BeginDisabledGroup(!multiAttribute.disabledCheckFunction.Invoke());
            }

            EditorGUI.BeginProperty(position, label, property);

            // Add alternative handling to ranged fields
            if (multiAttribute.isRange)
            {
                if (property.propertyType == SerializedPropertyType.Float)
                {
                    EditorGUI.Slider(position, property, multiAttribute.min, multiAttribute.max, label);
                }
                else if (property.propertyType == SerializedPropertyType.Integer)
                {
                    EditorGUI.IntSlider(position, property, (int)multiAttribute.min, (int)multiAttribute.max, label);
                }
                else
                {
                    EditorGUI.PropertyField(position, property, label, true);
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

                    label.image = multiAttribute.nullError ? errorIcon : warningIcon;
                }

                position = EditorGUI.PrefixLabel(position, label);

                GUI.color = Color.white;

                int indentLevel = EditorGUI.indentLevel;

                EditorGUI.indentLevel = 0;

                EditorGUI.ObjectField(position, property, GUIContent.none);

                EditorGUI.indentLevel = indentLevel;
            }
            // Draw the default field
            else
            {
                switch (property.propertyType)
                {
                    //case SerializedPropertyType.Generic:
                    //    EditorGUI.LabelField(position, "MultiPropertyDrawer does not support Generic fields yet.");
                    //    break;
                    case SerializedPropertyType.Integer:
                        property.intValue = EditorGUI.IntField(position, label, property.intValue);
                        break;
                    case SerializedPropertyType.Boolean:
                        property.boolValue = EditorGUI.Toggle(position, label, property.boolValue);
                        break;
                    case SerializedPropertyType.Float:
                        property.floatValue = EditorGUI.FloatField(position, label, property.floatValue);
                        break;
                    case SerializedPropertyType.String:
                        property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
                        break;
                    case SerializedPropertyType.Color:
                        property.colorValue = EditorGUI.ColorField(position, label, property.colorValue);
                        break;
                    case SerializedPropertyType.ObjectReference:
                        EditorGUI.ObjectField(position, property, label);
                        break;
                    case SerializedPropertyType.LayerMask:
                        int mask = EditorGUI.MaskField(position, label, InternalEditorUtility.LayerMaskToConcatenatedLayersMask(property.intValue), InternalEditorUtility.layers); ;
                        property.intValue = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(mask);
                        break;
                    case SerializedPropertyType.Enum:
                        position = EditorGUI.PrefixLabel(position, label);
                        int indentLevel = EditorGUI.indentLevel;
                        EditorGUI.indentLevel = 0;
                        property.enumValueIndex = EditorGUI.Popup(position, property.enumValueIndex, property.enumDisplayNames);
                        EditorGUI.indentLevel = indentLevel;
                        break;
                    case SerializedPropertyType.Vector2:
                        property.vector2Value = EditorGUI.Vector2Field(position, label, property.vector2Value);
                        break;
                    case SerializedPropertyType.Vector3:
                        property.vector3Value = EditorGUI.Vector3Field(position, label, property.vector3Value);
                        break;
                    case SerializedPropertyType.Vector4:
                        property.vector4Value = EditorGUI.Vector4Field(position, label, property.vector4Value);
                        break;
                    case SerializedPropertyType.Rect:
                        property.rectValue = EditorGUI.RectField(position, label, property.rectValue);
                        break;
                    //case SerializedPropertyType.ArraySize:
                    //    EditorGUI.LabelField(position, "MultiPropertyDrawer does not support Array fields yet.");
                    //    break;
                    //case SerializedPropertyType.Character:
                    //    EditorGUI.LabelField(position, "MultiPropertyDrawer does not support Character fields yet.");
                    //    break;
                    case SerializedPropertyType.AnimationCurve:
                        property.animationCurveValue = EditorGUI.CurveField(position, label, property.animationCurveValue);
                        break;
                    case SerializedPropertyType.Bounds:
                        property.boundsValue = EditorGUI.BoundsField(position, label, property.boundsValue);
                        break;
                    case SerializedPropertyType.Gradient:
                        property.gradientValue = EditorGUI.GradientField(position, label, property.gradientValue);
                        break;
                    case SerializedPropertyType.Quaternion:
                        property.quaternionValue = Quaternion.Euler(EditorGUI.Vector3Field(position, label, property.quaternionValue.eulerAngles));
                        break;
                    case SerializedPropertyType.Vector2Int:
                        property.vector2IntValue = EditorGUI.Vector2IntField(position, label, property.vector2IntValue);
                        break;
                    case SerializedPropertyType.Vector3Int:
                        property.vector3IntValue = EditorGUI.Vector3IntField(position, label, property.vector3IntValue);
                        break;
                    case SerializedPropertyType.RectInt:
                        property.rectIntValue = EditorGUI.RectIntField(position, label, property.rectIntValue);
                        break;
                    case SerializedPropertyType.BoundsInt:
                        property.boundsIntValue = EditorGUI.BoundsIntField(position, label, property.boundsIntValue);
                        break;
                    default:
                        // Draw the default property if the type hasn't already been implicitly implemented
                        // However, this can result in recursive drawing
                        EditorGUI.PropertyField(position, property, label, true);
                        break;
                }
            }

            EditorGUI.EndProperty();

            if (multiAttribute.disable)
            {
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}
