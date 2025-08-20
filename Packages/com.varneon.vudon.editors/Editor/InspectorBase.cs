using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Varneon.VUdon.Editors.Editor
{
    using Editor = UnityEditor.Editor;

    public abstract class InspectorBase : Editor
    {
        /// <summary>
        /// package.json file of the package for displaying the package name and version in the inspector footer
        /// </summary>
        [SerializeField]
        private PackageManifest packageManifest;

        /// <summary>
        /// <see cref="EditorPrefs"/> key under which the persistent state of the inspector will be stored.
        /// <para>By default the state of the inspector will be reset every time it gets reloaded, provide a key in order to make it persist</para>
        /// </summary>
        /// <remarks>
        /// <para>NOTE: Make sure to provide a unique name separated by forward slashes e.g.</para>
        /// <para>YOUR_NAME/PRODUCT_NAME/COMPONENT_NAME/InspectorState</para>
        /// </remarks>
        protected virtual string PersistenceKey => null;

        [Obsolete("Use PersistenceKey instead")]
        protected virtual string FoldoutPersistenceKey { get; }

        /// <summary>
        /// Header to be drawn in the top of the inspector
        /// <para>Use <see cref="InspectorHeaderBuilder"/> to construct the header with the desired parameters and pass the result of the Build() method to this property</para>
        /// </summary>
        protected abstract InspectorHeader Header { get; }

        private readonly HashSet<ISerializedPropertyGroup> propertyGroups = new HashSet<ISerializedPropertyGroup>();

        private bool[] foldoutStates;

        private bool drawFooter;

        private string footer;

        /// <summary>
        /// Shorthand for <see cref="EditorGUIUtility.isProSkin"/>
        /// </summary>
        protected bool EditorDarkMode => editorDarkMode;

        private bool editorDarkMode;

        private string persistenceKey;

        /// <summary>
        /// Override this property to set the number of persistent booleans stored in preferences
        /// </summary>
        /// <returns></returns>
        protected virtual int CustomPersistentBoolCount => 0;

        /// <summary>
        /// Custom persistent flags for storing data between inspector reloads
        /// </summary>
        protected bool[] customPersistentBools = new bool[0];

        private InspectorHeader header;

        private static GUIStyle HelpButtonStyle;
        private static Texture2D HelpIcon;

        protected virtual void OnEnable()
        {
            editorDarkMode = EditorGUIUtility.isProSkin;

            HelpIcon = (Texture2D)EditorGUIUtility.IconContent(editorDarkMode ? "d__Help" : "_Help@2x").image;

            HelpButtonStyle = new GUIStyle() { normal = { background = HelpIcon } };

            // Try getting the foldout persistence key safely in case it hasn't been implemented
            try
            {
#pragma warning disable CS0618 // Type or member is obsolete
                persistenceKey = FoldoutPersistenceKey;
#pragma warning restore CS0618 // Type or member is obsolete
            }
            catch { }

            if (string.IsNullOrWhiteSpace(persistenceKey))
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(PersistenceKey))
                    {
                        persistenceKey = PersistenceKey;
                    }
                }
                catch { }
            }

            // Try getting the header safely in case it hasn't been implemented
            try
            {
                header = Header;
            }
            catch
            {
                Debug.LogError("Exception occurred when trying to get inspector header! Make sure to follow instructions on the wiki: https://github.com/Varneon/VUdon-Editors/wiki/Creating-a-new-custom-inspector#2-2-provide-a-new-header-for-the-inspector");
            }

            drawFooter = TryBuildFooterString(out footer);

            Type targetType = target.GetType();

            SerializedProperty iterator = serializedObject.GetIterator();

            bool enterChildren = true;

            string foldoutHeader = null;

            HashSet<string> foldoutHeaders = new HashSet<string>();

            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.propertyPath == "m_Script") { enterChildren = false; continue; }

                FieldInfo fieldInfo = targetType.GetField(iterator.propertyPath, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (fieldInfo == null) { Debug.Log($"FieldInfo of {iterator.propertyPath} is null!"); enterChildren = false; continue; }

                if (Attribute.IsDefined(fieldInfo, typeof(FoldoutHeaderAttribute)))
                {
                    FoldoutHeaderAttribute foldoutAttribute = fieldInfo.GetCustomAttribute<FoldoutHeaderAttribute>();

                    foldoutHeader = foldoutAttribute.Header;

                    if (!string.IsNullOrWhiteSpace(foldoutHeader) && !foldoutHeaders.Contains(foldoutHeader))
                    {
                        foldoutHeaders.Add(foldoutHeader);

                        propertyGroups.Add(new FoldoutSerializedPropertyGroup(foldoutHeader, foldoutAttribute.Tooltip, foldoutAttribute.URL));
                    }
                }

                SerializedProperty property = iterator.Copy();

                if (foldoutHeader == null)
                {
                    ISerializedPropertyGroup group = propertyGroups.LastOrDefault();

                    if (group == null || group.GetType().Equals(typeof(FoldoutSerializedPropertyGroup)))
                    {
                        propertyGroups.Add(new RootSerializedPropertyGroup());
                    }
                }

                propertyGroups.Last().Properties.Add(property);

                enterChildren = false;
            }

            foldoutStates = new bool[propertyGroups.Count];

            customPersistentBools = new bool[CustomPersistentBoolCount];

            if (!string.IsNullOrWhiteSpace(persistenceKey) && EditorPrefs.HasKey(persistenceKey))
            {
                int states = EditorPrefs.GetInt(persistenceKey);

                int foldoutCount = foldoutStates.Length;

                for (int i = 0; i < foldoutCount; i++)
                {
                    foldoutStates[i] = (states & (1 << i)) != 0;
                }

                if(CustomPersistentBoolCount > 0)
                {
                    for (int i = 0; i < CustomPersistentBoolCount; i++)
                    {
                        customPersistentBools[i] = (states & (1 << (i + foldoutCount))) != 0;
                    }
                }
            }
        }

        /// <summary>
        /// Gets called before the inspector draws all of the default fields
        /// </summary>
        protected virtual void OnPreDrawFields() { }

        /// <summary>
        /// Gets called after the inspector is one drawing all of the default fields
        /// </summary>
        protected virtual void OnPostDrawFields() { }

        /// <summary>
        /// Gets called after the inspector has drawn the default footer
        /// </summary>
        protected virtual void OnPostDrawFooter() { }

        /// <summary>
        /// Gets called when serialized property group will begin to be drawn
        /// </summary>
        /// <param name="index"></param>
        protected virtual void OnPreDrawPropertyGroup(int index) { }

        /// <summary>
        /// Gets called when serialized property group has finished drawing
        /// </summary>
        /// <param name="index"></param>
        protected virtual void OnPostDrawPropertyGroup(int index) { }

        public sealed override void OnInspectorGUI()
        {
            header?.Draw(editorDarkMode);

            OnPreDrawFields();

            serializedObject.Update();

            for (int g = 0; g < propertyGroups.Count; g++)
            {
                ISerializedPropertyGroup group = propertyGroups.ElementAt(g);

                bool isFoldout = group.IsFoldout;

                bool expanded = !isFoldout || foldoutStates[g];

                if (isFoldout)
                {
                    FoldoutSerializedPropertyGroup foldoutGroup = (FoldoutSerializedPropertyGroup)group;

                    using (EditorGUI.ChangeCheckScope scope = new EditorGUI.ChangeCheckScope())
                    {
                        string helpURL = foldoutGroup.URL;

                        if (string.IsNullOrWhiteSpace(helpURL))
                        {
                            expanded = EditorGUILayout.BeginFoldoutHeaderGroup(expanded, foldoutGroup.LabelContent);
                        }
                        else
                        {
                            expanded = EditorGUILayout.BeginFoldoutHeaderGroup(expanded, foldoutGroup.LabelContent, null, (rect) => Application.OpenURL(helpURL), HelpButtonStyle);
                        }

                        if (scope.changed)
                        {
                            foldoutStates[g] = expanded;
                        }
                    }
                }

                if (expanded)
                {
                    if (isFoldout)
                    {
                        GUI.color = Color.black;

                        Rect rect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                        if (editorDarkMode) { GUI.Box(rect, string.Empty); }

                        GUI.color = Color.white;

                        EditorGUI.indentLevel++;
                    }

                    OnPreDrawPropertyGroup(g);

                    foreach (SerializedProperty property in group.Properties)
                    {
                        using (EditorGUI.ChangeCheckScope scope = new EditorGUI.ChangeCheckScope())
                        {
                            EditorGUILayout.PropertyField(property, true);

                            if (scope.changed)
                            {
                                serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }

                    OnPostDrawPropertyGroup(g);

                    if (isFoldout)
                    {
                        EditorGUILayout.EndVertical();

                        EditorGUI.indentLevel--;
                    }
                }

                if (isFoldout)
                {
                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
            }

            OnPostDrawFields();

            if (drawFooter) { DrawInspectorFooter(); }

            OnPostDrawFooter();
        }

        private void DrawInspectorFooter()
        {
            if(GUILayout.Button(footer, EditorStyles.centeredGreyMiniLabel))
            {
                EditorGUIUtility.PingObject(packageManifest);
            }
        }

        private bool TryBuildFooterString(out string footerString)
        {
            if (packageManifest)
            {
                JObject manifest = JsonConvert.DeserializeObject<JObject>(packageManifest.text);

                footerString = string.Concat(manifest.GetValue("name"), " - ", manifest.GetValue("version"));

                return true;
            }
            else
            {
                footerString = null;

                return false;
            }
        }

        protected virtual void OnDestroy()
        {
            if (string.IsNullOrWhiteSpace(persistenceKey)) { return; }

            if (foldoutStates == null) { return; }

            int states = 0;

            int foldoutCount = foldoutStates.Length;

            for (int i = 0; i < foldoutCount; i++)
            {
                if (foldoutStates[i])
                {
                    states |= 1 << i;
                }
            }

            if (CustomPersistentBoolCount > 0)
            {
                for (int i = 0; i < CustomPersistentBoolCount; i++)
                {
                    if (customPersistentBools[i])
                    {
                        states |= 1 << (i + foldoutCount);
                    }
                }
            }

            EditorPrefs.SetInt(persistenceKey, states);
        }
    }
}
