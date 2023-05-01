using System;

namespace Varneon.VUdon.Editors
{
    /// <summary>
    /// Add this attribute to set a custom label for a field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldLabelAttribute : MultiPropertyAttribute
    {
#if !COMPILER_UDONSHARP
        public override FieldAttributeType Type => FieldAttributeType.Label;
#endif

        /// <summary>
        /// The custom field label
        /// </summary>
        public readonly string Label;

        /// <summary>
        /// Add this attribute to set a custom label for a field
        /// </summary>
        /// <param name="label">The custom field label</param>
        public FieldLabelAttribute(string label)
        {
            Label = label;

            this.order = 1;
        }
    }
}
