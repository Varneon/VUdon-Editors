using UnityEngine;

namespace Varneon.VUdon.Editors
{
    /// <summary>
    /// Add this attribute to ignore a field in serialization by InspectorBase
    /// </summary>
    /// <remarks>
    /// Useful when you're hooking onto the provided callbacks, e.g. rendering pre/post group and want to add special logic to a specific field's serialization
    /// </remarks>
    public class FieldIgnoreAttribute : PropertyAttribute { }
}
