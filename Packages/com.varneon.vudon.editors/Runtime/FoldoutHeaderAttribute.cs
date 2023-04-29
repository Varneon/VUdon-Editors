using System;

namespace Varneon.VUdon.Editors
{
    /// <summary>
    /// Add this attribute to a field to draw it under a foldout
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class FoldoutHeaderAttribute : Attribute
    {
        /// <summary>
        /// The header text
        /// </summary>
        public string Header { get; }

        public FoldoutHeaderAttribute(string header)
        {
            Header = header;
        }
    }
}
