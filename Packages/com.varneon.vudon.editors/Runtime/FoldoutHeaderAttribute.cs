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

        public readonly string Tooltip;

        public FoldoutHeaderAttribute(string header, string tooltip = null)
        {
            Header = header;

            Tooltip = tooltip;
        }
    }
}
