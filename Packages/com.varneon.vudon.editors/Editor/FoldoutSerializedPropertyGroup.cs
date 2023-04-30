using System.Collections.Generic;
using UnityEditor;

namespace Varneon.VUdon.Editors
{
    /// <summary>
    /// Group containing a list of SerializedProperties to render under a foldout in the inspector
    /// </summary>
    public class FoldoutSerializedPropertyGroup : ISerializedPropertyGroup
    {
        public bool IsFoldout => true;

        public string FoldoutName { get; set; }

        public List<SerializedProperty> Properties => _properties;

        private readonly List<SerializedProperty> _properties;

        public FoldoutSerializedPropertyGroup(string name)
        {
            FoldoutName = name;

            _properties = new List<SerializedProperty>();
        }
    }
}
