using System.Collections.Generic;
using UnityEditor;

namespace Varneon.VUdon.Editors
{
    /// <summary>
    /// Group containing a list of SerializedProperties to render at the root of the inspector
    /// </summary>
    public class RootSerializedPropertyGroup : ISerializedPropertyGroup
    {
        public bool IsFoldout => false;

        public List<SerializedProperty> Properties => _properties;

        private readonly List<SerializedProperty> _properties;

        public RootSerializedPropertyGroup()
        {
            _properties = new List<SerializedProperty>();
        }
    }
}
