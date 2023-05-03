using System;
using System.Linq;
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
        /// The minimum allowed value
        /// </summary>
        internal float min;

        /// <summary>
        /// The maximum allowed value
        /// </summary>
        internal float max;

        /// <summary>
        /// Does the property have disable attribute
        /// </summary>
        internal bool disable;

        /// <summary>
        /// Should the property warn about null values on an ObjectField
        /// </summary>
        internal bool nullWarning;

        /// <summary>
        /// Should null reference on ObjectField warning be elevated to an error
        /// </summary>
        internal bool nullError;

        /// <summary>
        /// Function for checking whether the field should be enabled or not
        /// </summary>
        internal Func<bool> disabledCheckFunction;

#if UNITY_EDITOR
        internal static Func<bool> DisabledCheckFunction(LogicType logic, UnityEditor.SerializedProperty[] properties)
        {
            switch (logic)
            {
                case LogicType.AND:
                    return () => properties.All(p => p.boolValue);
                case LogicType.OR:
                    return () => properties.Any(p => p.boolValue);
                case LogicType.NAND:
                    return () => properties.Any(p => !p.boolValue);
                case LogicType.NOR:
                    return () => !properties.Any(p => p.boolValue);
                case LogicType.XOR:
                    return () => properties.Count(p => p.boolValue) == 1;
                case LogicType.XNOR:
                    return () => properties.All(p => p.boolValue) || properties.All(p => !p.boolValue);
                default:
                    throw new NotImplementedException();
            }
        }
#endif
#endif
    }
}
