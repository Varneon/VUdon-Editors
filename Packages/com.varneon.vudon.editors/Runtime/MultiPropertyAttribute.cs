using UnityEngine;

namespace Varneon.VUdon.Editors
{
    /// <summary>
    /// Attribute for allowing multiple attribute on same field
    /// </summary>
    /// <remarks>
    /// <para>[Range] must be changed to [FieldRange]</para>
    /// </remarks>
    public abstract class MultiPropertyAttribute : PropertyAttribute
    {
#if !COMPILER_UDONSHARP
        /// <summary>
        /// Type of the attribute
        /// </summary>
        public abstract FieldAttributeType Type { get; }

        /// <summary>
        /// Is the attribute initialized for the PropertyDrawer
        /// </summary>
        internal bool initialized;

        /// <summary>
        /// All attached attributes on the property
        /// </summary>
        internal MultiPropertyAttribute[] attributes;

        /// <summary>
        /// Label of the property
        /// </summary>
        internal GUIContent label;

        /// <summary>
        /// Does the property have range attribute
        /// </summary>
        internal bool isRange;

        /// <summary>
        /// Does the property have disable attribute
        /// </summary>
        internal bool disable;

        /// <summary>
        /// Should the property disabled state be inverted
        /// </summary>
        internal bool disableWhenTrue;

        /// <summary>
        /// Should the property warn about null values on an ObjectField
        /// </summary>
        internal bool nullWarning;

        /// <summary>
        /// Should null reference on ObjectField warning be elevated to an error
        /// </summary>
        internal bool nullError;

#if UNITY_EDITOR
        /// <summary>
        /// Property for checking the state for disabled scope 
        /// </summary>
        internal UnityEditor.SerializedProperty disableProperty;
#endif
#endif
    }
}
