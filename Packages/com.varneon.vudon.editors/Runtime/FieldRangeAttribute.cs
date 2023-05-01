using System;

namespace Varneon.VUdon.Editors
{
    /// <summary>
    /// Attribute used to make a float or int variable in a script be restricted to a specific range
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldRangeAttribute : MultiPropertyAttribute
    {
#if !COMPILER_UDONSHARP
        public override FieldAttributeType Type => FieldAttributeType.Range;
#endif

        /// <summary>
        /// The minimum allowed value
        /// </summary>
        public readonly float Min;

        /// <summary>
        /// The maximum allowed value
        /// </summary>
        public readonly float Max;

        /// <summary>
        /// Attribute used to make a float or int variable in a script be restricted to a specific range
        /// </summary>
        /// <param name="min">The minimum allowed value</param>
        /// <param name="max">The maximum allowed value</param>
        public FieldRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;

            this.order = 1;
        }
    }
}
