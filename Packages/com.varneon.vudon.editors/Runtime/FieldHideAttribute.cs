using System;

namespace Varneon.VUdon.Editors
{
    /// <summary>
    /// Add this attribute to hide a field if the condition isn't met based on boolean properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldHideAttribute : MultiPropertyAttribute
    {
#if !COMPILER_UDONSHARP
        public override FieldAttributeType Type => FieldAttributeType.Hide;
#endif

        /// <summary>
        /// Logic for hiding the field
        /// </summary>
        public readonly LogicType Logic;

        /// <summary>
        /// Names of the boolean fields
        /// </summary>
        public readonly string[] Properties;

        /// <summary>
        /// Add this attribute to hide a field if the condition isn't met based on boolean properties
        /// </summary>
        /// <param name="properties">Names of the boolean fields</param>
        public FieldHideAttribute(params string[] properties) : this(LogicType.AND, properties) { }

        /// <summary>
        /// Add this attribute to hide a field if the condition isn't met based on boolean properties
        /// </summary>
        /// <param name="logic">Logic gate type for hide</param>
        /// <param name="properties">Names of the boolean fields</param>
        public FieldHideAttribute(LogicType logic, params string[] properties)
        {
            Logic = logic;

            Properties = properties;

            this.order = 1;
        }
    }
}
