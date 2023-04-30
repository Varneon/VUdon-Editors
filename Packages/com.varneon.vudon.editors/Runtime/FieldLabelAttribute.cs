using UnityEngine;

namespace Varneon.VUdon.Editors
{
    /// <summary>
    /// Add this attribute to set a custom label for a field
    /// </summary>
    public class FieldLabelAttribute : PropertyAttribute
    {
        /// <summary>
        /// The custom field name
        /// </summary>
        public string FieldName;

        public FieldLabelAttribute(string fieldName)
        {
            FieldName = fieldName;

            this.order = 1;
        }
    }
}
