using System;

namespace Varneon.VUdon.Editors
{
    /// <summary>
    /// Add this attribute to add a warning if an object field's reference is null
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldNullWarningAttribute : MultiPropertyAttribute
    {
#if !COMPILER_UDONSHARP
        public override FieldAttributeType Type => FieldAttributeType.NullWarning;
#endif

        /// <summary>
        /// Should the severity of null state be error
        /// </summary>
        public readonly bool IsError;

        /// <summary>
        /// Add this attribute to add a warning if an object field's reference is null
        /// </summary>
        /// <param name="isError">Should the severity of null state be error</param>
        public FieldNullWarningAttribute(bool isError = false)
        {
            IsError = isError;

            this.order = 1;
        }
    }
}
