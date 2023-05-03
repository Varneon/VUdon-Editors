using UnityEditor;
using UnityEngine;

namespace Varneon.VUdon.Editors.Editor
{
    /// <summary>
    /// Descriptor for custom inspector header
    /// </summary>
    public class InspectorHeader
    {
        /// <summary>
        /// Title of the header
        /// </summary>
        private readonly string _title;

        /// <summary>
        /// Description of the header
        /// </summary>
        private readonly string _description;

        /// <summary>
        /// Does the header have URLs to show in the header
        /// </summary>
        private readonly bool _hasURLs;

        /// <summary>
        /// Label/URL string pairs for URLs to show in the header
        /// </summary>
        private readonly (string, string)[] _urls;

        /// <summary>
        /// Does the header have an icon
        /// </summary>
        private readonly bool _hasIcon;

        /// <summary>
        /// Header icon texture
        /// </summary>
        private readonly Texture _icon;

        public InspectorHeader(string title, string description, (string, string)[] urls = null, Texture icon = null)
        {
            _title = title;

            _description = description;

            _hasURLs = (_urls = urls) != null;

            _hasIcon = _icon = icon;
        }

        /// <summary>
        /// Draw the header during OnInspectorGUI
        /// </summary>
        /// <param name="editorDarkMode">Is the editor currently in dark mode</param>
        internal void Draw(bool editorDarkMode)
        {
            // Define the root scope for the header
            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                // If the editor is currently in dark mode, set the color for drawing the background to black
                if (editorDarkMode) { GUI.color = Color.black; }

                // Draw the background of the header
                GUI.Box(new Rect(Vector2.zero, new Vector2(Screen.width, scope.rect.height + 8f)), string.Empty);

                // Reset the color if in dark mode
                if (editorDarkMode) { GUI.color = Color.white; }

                // Draw the header icon if one has been assigned
                if (_hasIcon)
                {
                    // Get a rect for the icon
                    Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(40f), GUILayout.Width(40f));

                    // Draw the icon texture
                    GUI.DrawTexture(rect, _icon);
                }

                // Create a vertical scope for the text content
                using (new GUILayout.VerticalScope())
                {
                    // Create a horizontal scope for title and all URLs
                    using (new GUILayout.HorizontalScope())
                    {
                        // Draw the title of the header
                        GUILayout.Label(_title, EditorStyles.whiteLargeLabel);

                        // If the header has URLs, draw them
                        if (_hasURLs)
                        {
                            // Iterate through all URLs
                            foreach((string,string) url in _urls)
                            {
                                // If a URL is clicked, open it
                                if (GUILayout.Button(url.Item1, EditorStyles.linkLabel)) { Application.OpenURL(url.Item2); }
                            }
                        }
                    }

                    // Fill the space between the text rows
                    GUILayout.FlexibleSpace();

                    // Draw the header description
                    GUILayout.Label(_description, EditorStyles.wordWrappedLabel);
                }

                // Leave some room to the right side
                GUILayout.Space(4f);
            }

            // Header is drawn a little larger than the layout size, add more space to leave room for the next GUI element
            GUILayout.Space(4f);
        }
    }
}
