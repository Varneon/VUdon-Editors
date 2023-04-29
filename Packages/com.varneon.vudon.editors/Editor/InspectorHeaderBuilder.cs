using System.Collections.Generic;
using UnityEngine;

namespace Varneon.VUdon.Editors.Editor
{
    /// <summary>
    /// Builder for InspectorHeader
    /// </summary>
    public class InspectorHeaderBuilder
    {
        /// <summary>
        /// Title of the header
        /// </summary>
        public string Title;

        /// <summary>
        /// Description of the header
        /// </summary>
        public string Description;

        /// <summary>
        /// Label/URL string pairs for URLs to show in the header
        /// </summary>
        public List<(string, string)> URLs = new List<(string, string)>();

        /// <summary>
        /// Header icon texture
        /// </summary>
        public Texture2D Icon;

        public InspectorHeaderBuilder() : this(string.Empty, string.Empty) { }

        public InspectorHeaderBuilder(string title) : this(title, null) { }

        public InspectorHeaderBuilder(string title, string description)
        {
            Title = title;

            Description = description;
        }
    }

    public static class InspectorHeaderBuilderExtensions
    {
        /// <summary>
        /// Sets the title of the header
        /// </summary>
        /// <param name="builder">Builder to modify</param>
        /// <param name="title">Title of the header</param>
        /// <returns>Modified InspectorHeaderBuilder</returns>
        public static InspectorHeaderBuilder WithTitle(this InspectorHeaderBuilder builder, string title)
        {
            builder.Title = title;

            return builder;
        }

        /// <summary>
        /// Sets the description of the header
        /// </summary>
        /// <param name="builder">Builder to modify</param>
        /// <param name="description">Description of the header</param>
        /// <returns>Modified InspectorHeaderBuilder</returns>
        public static InspectorHeaderBuilder WithDescription(this InspectorHeaderBuilder builder, string description)
        {
            builder.Description = description;

            return builder;
        }

        /// <summary>
        /// Adds a URL to the header
        /// </summary>
        /// <param name="builder">Builder to modify</param>
        /// <param name="label">URL label</param>
        /// <param name="url">Full URL</param>
        /// <returns>Modified InspectorHeaderBuilder</returns>
        public static InspectorHeaderBuilder WithURL(this InspectorHeaderBuilder builder, string label, string url)
        {
            builder.URLs.Add((label, url));

            return builder;
        }

        /// <summary>
        /// Sets the icon of the header
        /// </summary>
        /// <param name="builder">Builder to modify</param>
        /// <param name="icon">Icon of the header</param>
        /// <returns>Modified InspectorHeaderBuilder</returns>
        public static InspectorHeaderBuilder WithIcon(this InspectorHeaderBuilder builder, Texture2D icon)
        {
            builder.Icon = icon;

            return builder;
        }

        /// <summary>
        /// Builds the header
        /// </summary>
        /// <param name="builder">Builder to build</param>
        /// <returns>Built InspectorHeader</returns>
        public static InspectorHeader Build(this InspectorHeaderBuilder builder)
        {
            return new InspectorHeader(builder.Title, builder.Description, builder.URLs.ToArray(), builder.Icon);
        }
    }
}
