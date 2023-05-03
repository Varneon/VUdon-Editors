using System;

namespace Varneon.VUdon.Editors
{
    /// <summary>
    /// Add this attribute to disable a field if the condition isn't met based on boolean properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldDisableAttribute : MultiPropertyAttribute
    {
#if !COMPILER_UDONSHARP
        public override FieldAttributeType Type => FieldAttributeType.Disable;
#endif

        /// <summary>
        /// Logic for enabling the field
        /// </summary>
        public readonly LogicType Logic;

        /// <summary>
        /// Name of the boolean field
        /// </summary>
        public readonly string[] Properties;

        /// <summary>
        /// Add this attribute to disable a field if the condition isn't met based on boolean properties
        /// </summary>
        /// <param name="properties">Names of the boolean fields</param>
        public FieldDisableAttribute(params string[] properties) : this(LogicType.AND, properties) { }

        /// <summary>
        /// Add this attribute to disable a field if the condition isn't met based on boolean properties
        /// </summary>
        /// <param name="logic">Logic gate type for disable</param>
        /// <param name="properties">Names of the boolean fields</param>
        public FieldDisableAttribute(LogicType logic, params string[] properties)
        {
            Logic = logic;

            Properties = properties;

            this.order = 1;
        }
    }
}
