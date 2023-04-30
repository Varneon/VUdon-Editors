using System.Collections.Generic;
using UnityEditor;

namespace Varneon.VUdon.Editors
{
    /// <summary>
    /// Interface for a group containing a list of SerializedProperties to draw
    /// </summary>
    public interface ISerializedPropertyGroup
    {
        /// <summary>
        /// Is this group a foldout type
        /// </summary>
        bool IsFoldout { get; }

        /// <summary>
        /// SerializedProperties to draw on inspector
        /// </summary>
        List<SerializedProperty> Properties { get; }
    }
}
