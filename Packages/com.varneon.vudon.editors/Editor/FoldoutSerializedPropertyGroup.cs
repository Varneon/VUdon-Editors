using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Varneon.VUdon.Editors
{
    /// <summary>
    /// Group containing a list of SerializedProperties to render under a foldout in the inspector
    /// </summary>
    public class FoldoutSerializedPropertyGroup : ISerializedPropertyGroup
    {
        public bool IsFoldout => true;

        public readonly GUIContent LabelContent;

        public List<SerializedProperty> Properties => _properties;

        private readonly List<SerializedProperty> _properties;

        public FoldoutSerializedPropertyGroup(string name, string tooltip = null)
        {
            LabelContent = new GUIContent(name, tooltip);

            _properties = new List<SerializedProperty>();
        }
    }
}
