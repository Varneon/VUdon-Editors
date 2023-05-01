using System;

namespace Varneon.VUdon.Editors
{
    /// <summary>
    /// Add this attribute to disable a field based on boolean field's state
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldDisableAttribute : MultiPropertyAttribute
    {
#if !COMPILER_UDONSHARP
        public override FieldAttributeType Type => FieldAttributeType.Disable;
#endif

        /// <summary>
        /// Name of the boolean field
        /// </summary>
        public readonly string Property;

        /// <summary>
        /// Should the field be disabled when the value is true
        /// </summary>
        public readonly bool WhenTrue;

        /// <summary>
        /// Add this attribute to disable a field based on boolean property's state
        /// </summary>
        /// <param name="property">Name of the boolean field</param>
        /// <param name="whenTrue">Should the field be disabled when the value is true</param>
        public FieldDisableAttribute(string property, bool whenTrue = false)
        {
            Property = property;

            WhenTrue = whenTrue;

            this.order = 1;
        }
    }
}
