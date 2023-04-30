using UnityEditor;
using UnityEngine;

namespace Varneon.VUdon.Editors
{
    [CustomPropertyDrawer(typeof(FieldLabelAttribute), true)]
    public class FieldLabelAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, new GUIContent(((FieldLabelAttribute)attribute).FieldName));
        }
    }
}
