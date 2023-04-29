using System;

namespace Varneon.VUdon.Editors
{
    /// <summary>
    /// Add this attribute to set a custom label for a field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldLabelAttribute : Attribute
    {
        /// <summary>
        /// The custom field name
        /// </summary>
        public string FieldName;

        public FieldLabelAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}
