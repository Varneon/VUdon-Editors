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
        [SerializeField]
        private PackageManifest packageManifest;

        protected abstract string FoldoutPersistenceKey { get; }

        protected abstract InspectorHeader Header { get; }

        private readonly HashSet<ISerializedPropertyGroup> propertyGroups = new HashSet<ISerializedPropertyGroup>();

        private bool[] foldoutStates;

        private bool drawFooter;

        private string footer;

        private bool editorDarkMode;

        private string foldoutPersistenceKey;

        private InspectorHeader header;

        protected virtual void OnEnable()
        {
            editorDarkMode = EditorGUIUtility.isProSkin;

            // Try getting the foldout persistence key safely in case it hasn't been implemented
            try
            {
                foldoutPersistenceKey = FoldoutPersistenceKey;
            }
            catch { }

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
                    foldoutHeader = fieldInfo.GetCustomAttribute<FoldoutHeaderAttribute>().Header;

                    if (!string.IsNullOrWhiteSpace(foldoutHeader) && !foldoutHeaders.Contains(foldoutHeader))
                    {
                        foldoutHeaders.Add(foldoutHeader);

                        propertyGroups.Add(new FoldoutSerializedPropertyGroup(foldoutHeader));
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

            if (!string.IsNullOrWhiteSpace(foldoutPersistenceKey) && EditorPrefs.HasKey(foldoutPersistenceKey))
            {
                int states = EditorPrefs.GetInt(FoldoutPersistenceKey);

                for (int i = 0; i < foldoutStates.Length; i++)
                {
                    foldoutStates[i] = (states & (1 << i)) != 0;
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

        public sealed override void OnInspectorGUI()
        {
            header?.Draw(editorDarkMode);

            serializedObject.Update();

            OnPreDrawFields();

            for (int g = 0; g < propertyGroups.Count; g++)
            {
                ISerializedPropertyGroup group = propertyGroups.ElementAt(g);

                bool isFoldout = group.IsFoldout;

                bool expanded = !isFoldout || foldoutStates[g];

                if (isFoldout)
                {
                    using (EditorGUI.ChangeCheckScope scope = new EditorGUI.ChangeCheckScope())
                    {
                        expanded = EditorGUILayout.BeginFoldoutHeaderGroup(expanded, ((FoldoutSerializedPropertyGroup)group).FoldoutName);

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

                        GUI.Box(rect, string.Empty);

                        GUI.color = Color.white;

                        EditorGUI.indentLevel++;
                    }

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

        private void OnDestroy()
        {
            if (string.IsNullOrWhiteSpace(foldoutPersistenceKey)) { return; }

            if (foldoutStates == null) { return; }

            int states = 0;

            for (int i = 0; i < foldoutStates.Length; i++)
            {
                if (foldoutStates[i])
                {
                    states |= 1 << i;
                }
            }

            EditorPrefs.SetInt(foldoutPersistenceKey, states);
        }
    }
}
