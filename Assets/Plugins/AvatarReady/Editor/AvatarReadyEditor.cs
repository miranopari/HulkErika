using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inria.Avatar.AvatarReady
{
    [CustomEditor(typeof(AvatarReady))]
    public class AvatarReadyEditor : Editor
    {
        private AvatarReady _target;
        private AvatarReady Target
        {
            get
            {
                if (_target == null)
                    _target = target as AvatarReady;
                return _target;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Help
            Target.showHelp = EditorGUILayout.BeginFoldoutHeaderGroup(Target.showHelp, "Help");
            if (Target.showHelp)
            {
                GUIStyle style = new GUIStyle(EditorStyles.textArea);
                style.richText = true;
                // TODO use EditorGUI.hyperLinkClicked for Unity 2022+
                EditorGUILayout.TextArea(
                    "AvatarReady works best with third-party packages (if present in your project, they are automaticaly added to AvatarReady)\n" +
                    "- FinalIK (from Inria Npm Registry or from AssetStore) to add better Inverse Kinematics Animations features\n" +
                    "- XSens MvnLive (from Inria Npm Registry) to add Mocap features\n" +
                    "- SteamVR (from Inria Npm Registry or from Github) to add some VR features\n" +
                    "- Azure Kinect (from Microsoft website) to add AR features\n" +
                    "(If you don't know about the Inria Npm Registry contact me at adrien.reuzeau@inria.fr or @adreuzea on Inria's Mattermost)\n" +
                    "\n" +
                    "To configure your avatar, select the options from top to bottom depending on your usage\n" +
                    "Then click 'Setup Avatar' to validate, you can then modify the added components if needed\n" +
                    "(Be aware that clicking 'Reset Avatar' will remove every component added by AvatarReady so you may lost your modification)\n" +
                    "\n" +
                    "If you need Avatars, you can find some at <a href=\"https://avatar-gallery.irisa.fr/\">https://avatar-gallery.irisa.fr/</a>\n" +
                    "and follow the 'How to import these avatars' section to ensure best visual quality", style);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(Target.activated);

            // AnimationType
            DynamicPopup(typeof(AnimationSystem), true, Target.AnimationType, t => Target.AnimationType = t, new GUIContent("Animation Type", "Global type of animation"));

            EditorGUI.indentLevel++;

            // AnimationSystem
            DynamicPopup(Target.AnimationType, false, Target.AnimationSystem, t => Target.AnimationSystem = t, new GUIContent("Animation System", "Specific system used to animate the avatar"), null, () =>
                  {
                      Target.SystemStringOptions.Clear();
                      Target.SystemObjectOptions.Clear();

                      Target.AnimationConfig = null;
                  });

            // AnimationSystem options
            if (Target.AnimationSystem != null && Attribute.IsDefined(Target.AnimationSystem, typeof(AvatarReadyOptionAttribute)))
            {
                EditorGUILayout.LabelField("Options:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                AvatarReadyOptionAttribute[] systemOptions = Attribute.GetCustomAttributes(Target.AnimationSystem, typeof(AvatarReadyOptionAttribute)) as AvatarReadyOptionAttribute[];

                systemOptions = systemOptions.OrderBy(t => t.order).ToArray();

                for (int i = 0; i < systemOptions.Length; i++)
                {
                    if (systemOptions[i] is AvatarReadyConstOptionAttribute constConfigOption)
                    {
                        if (!Target.SystemStringOptions.ContainsKey(constConfigOption.variableName))
                            Target.SystemStringOptions.Add(constConfigOption.variableName, constConfigOption.defaultValue.ToString());

                        DynamicConstField(constConfigOption.variableType, Target.SystemStringOptions[constConfigOption.variableName], (str) => Target.SystemStringOptions[constConfigOption.variableName] = str, new GUIContent(constConfigOption.displayName));
                    }
                    else if (systemOptions[i] is AvatarReadyReflectionOptionAttribute variableConfigOption)
                    {
                        if (!Target.SystemStringOptions.ContainsKey(variableConfigOption.variableName))
                            Target.SystemStringOptions.Add(variableConfigOption.variableName, string.Empty);

                        DynamicReflectionField(Target.AnimationSystem, variableConfigOption.methodOptions, Target.SystemStringOptions[variableConfigOption.variableName], (str) => Target.SystemStringOptions[variableConfigOption.variableName] = str, new GUIContent(variableConfigOption.displayName));
                    }
                    else if (systemOptions[i] is AvatarReadyObjectOptionAttribute objectConfigOption)
                    {
                        if (!Target.SystemObjectOptions.ContainsKey(objectConfigOption.variableName))
                            Target.SystemObjectOptions.Add(objectConfigOption.variableName, null);

                        DynamicObjectField(objectConfigOption.ObjectType, Target.SystemObjectOptions[objectConfigOption.variableName], (obj) => Target.SystemObjectOptions[objectConfigOption.variableName] = obj, new GUIContent(objectConfigOption.displayName));
                    }
                }

                EditorGUI.indentLevel--;
            }


            EditorGUI.indentLevel++;

            // AnimationConfig
            DynamicOptionalPopup(typeof(AnimationConfig), false, Target.AnimationConfig, t => Target.AnimationConfig = t, new GUIContent("Configuration", "The configuration to apply to the animation system"), t =>
            {
                if (Attribute.IsDefined(t, typeof(AvatarReadySupportedSystemAttribute)))
                {
                    AvatarReadySupportedSystemAttribute[] attributes = Attribute.GetCustomAttributes(t, typeof(AvatarReadySupportedSystemAttribute)) as AvatarReadySupportedSystemAttribute[];
                    return attributes.Any(attribute => attribute.supportedSystem.IsAssignableFrom(Target.AnimationSystem));
                }
                else
                {
                    Debug.LogWarning(t.Name + " does not have the " + nameof(AvatarReadySupportedSystemAttribute) + " attribute and thus can not be use");
                    return false;
                }
            }, () =>
            {
                if (Target.AnimationConfig != null)
                {
                    int trackerNumber = Attribute.GetCustomAttributes(Target.AnimationConfig, typeof(AvatarReadySupportedTrackerAttribute)).Length;
                    Target.Trackers = new Transform[trackerNumber];
                    Target.TrackerTypes = new AvatarReady.TypeArray(trackerNumber);
                }
                else
                {
                    Target.Trackers = null;
                    Target.TrackerTypes = null;

                    Target.ConfigStringOptions.Clear();
                    Target.ConfigObjectOptions.Clear();
                }
            });

            EditorGUI.indentLevel++;

            // AnimationConfig Trackers
            if (Target.AnimationConfig != null && Attribute.IsDefined(Target.AnimationConfig, typeof(AvatarReadySupportedTrackerAttribute)))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Trackers:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("(Tracker Types might be override at runtime)");
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel++;

                AvatarReadySupportedTrackerAttribute[] supportedTrackers = Attribute.GetCustomAttributes(Target.AnimationConfig, typeof(AvatarReadySupportedTrackerAttribute)) as AvatarReadySupportedTrackerAttribute[];

                supportedTrackers = supportedTrackers.OrderBy(t => t.order).ToArray();

                for (int i = 0; i < supportedTrackers.Length; i++)
                {
                    // Transform
                    Transform nextValue = EditorGUILayout.ObjectField(supportedTrackers[i].trackerName + (supportedTrackers[i].necessity == AvatarReadySupportedTrackerAttribute.Necessity.Optional ? " (Optional)" : ""), Target.Trackers[i], typeof(Transform), true) as Transform;
                    if (Target.Trackers[i] != nextValue)
                    {
                        Target.Trackers[i] = nextValue;
                        EditorUtility.SetDirty(Target);
                    }

                    // TrackerType
                    EditorGUI.indentLevel++;
                    DynamicOptionalPopup(supportedTrackers[i].trackerType, false, Target.TrackerTypes[i], t => Target.TrackerTypes[i] = t, new GUIContent("Tracker Type", "What kind of tracker do you use. Used to define offsets"));
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            // AnimationConfig Options
            if (Target.AnimationConfig != null && Attribute.IsDefined(Target.AnimationConfig, typeof(AvatarReadyOptionAttribute)))
            {
                EditorGUILayout.LabelField("Options:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                AvatarReadyOptionAttribute[] configOptions = Attribute.GetCustomAttributes(Target.AnimationConfig, typeof(AvatarReadyOptionAttribute)) as AvatarReadyOptionAttribute[];

                configOptions = configOptions.OrderBy(t => t.order).ToArray();

                for (int i = 0; i < configOptions.Length; i++)
                {
                    if (configOptions[i] is AvatarReadyConstOptionAttribute constConfigOption)
                    {
                        if (!Target.ConfigStringOptions.ContainsKey(constConfigOption.variableName))
                            Target.ConfigStringOptions.Add(constConfigOption.variableName, constConfigOption.defaultValue.ToString());

                        DynamicConstField(constConfigOption.variableType, Target.ConfigStringOptions[constConfigOption.variableName], (str) => Target.ConfigStringOptions[constConfigOption.variableName] = str, new GUIContent(constConfigOption.displayName));
                    }
                    else if (configOptions[i] is AvatarReadyReflectionOptionAttribute variableConfigOption)
                    {
                        if (!Target.ConfigStringOptions.ContainsKey(variableConfigOption.variableName))
                            Target.ConfigStringOptions.Add(variableConfigOption.variableName, string.Empty);

                        DynamicReflectionField(Target.AnimationConfig, variableConfigOption.methodOptions, Target.ConfigStringOptions[variableConfigOption.variableName], (str) => Target.ConfigStringOptions[variableConfigOption.variableName] = str, new GUIContent(variableConfigOption.displayName));
                    }
                    else if (configOptions[i] is AvatarReadyObjectOptionAttribute objectConfigOption)
                    {
                        if (!Target.ConfigObjectOptions.ContainsKey(objectConfigOption.variableName))
                            Target.ConfigObjectOptions.Add(objectConfigOption.variableName, null);

                        DynamicObjectField(objectConfigOption.ObjectType, Target.ConfigObjectOptions[objectConfigOption.variableName], (obj) => Target.ConfigObjectOptions[objectConfigOption.variableName] = obj, new GUIContent(objectConfigOption.displayName));
                    }
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            // CalibrationTechnique
            DynamicOptionalPopup(typeof(CalibrationTechnique), false, Target.CalibrationTechnique, t => Target.CalibrationTechnique = t, new GUIContent("Calibration Technique"), t =>
               {
                   if (Attribute.IsDefined(t, typeof(AvatarReadySupportedSystemAttribute)))
                   {
                       AvatarReadySupportedSystemAttribute[] attributes = Attribute.GetCustomAttributes(t, typeof(AvatarReadySupportedSystemAttribute)) as AvatarReadySupportedSystemAttribute[];
                       return attributes.Any(attribute => attribute.supportedSystem.IsAssignableFrom(Target.AnimationSystem));
                   }
                   else
                   {
                       Debug.LogWarning(t.Name + " does not have the " + nameof(AvatarReadySupportedSystemAttribute) + " attribute and thus can not be use");
                       return false;
                   }
               });

            EditorGUILayout.Space();

            // AvatarProvider
            DynamicPopup(typeof(AvatarProvider), false, Target.AvatarProvider, t => Target.AvatarProvider = t, new GUIContent("Avatar Provider", "The software used to create the avatar"), null, () =>
                {
                    Target.SelectedFeatures = new AvatarReady.TypeList();

                    // Add enabled by default Features
                    Type[] defaultAvatarFeatures = ReflectionUtils.GetSubclassesOf(typeof(AvatarFeature)).Where(t =>
                    {
                        if (Attribute.IsDefined(t, typeof(AvatarReadySupportedProviderAttribute)))
                        {
                            AvatarReadySupportedProviderAttribute[] attributes = Attribute.GetCustomAttributes(t, typeof(AvatarReadySupportedProviderAttribute)) as AvatarReadySupportedProviderAttribute[];
                            return attributes.Where(attribute => attribute.supportedProvider.IsAssignableFrom(Target.AvatarProvider)).Any(attribute => attribute.enabledByDefault);
                        }
                        else
                        {
                            Debug.LogWarning(t.Name + " does not have the " + nameof(AvatarReadySupportedProviderAttribute) + " attribute and thus cannot be used");
                            return false;
                        }
                    }).ToArray();

                    foreach (Type feature in defaultAvatarFeatures)
                    {
                        Target.SelectedFeatures.Add(feature);
                    }
                });

            EditorGUI.indentLevel++;

            // Features
            Type[] avatarFeatures = ReflectionUtils.GetSubclassesOf(typeof(AvatarFeature)).Where(t =>
            {
                if (Attribute.IsDefined(t, typeof(AvatarReadySupportedProviderAttribute)))
                {
                    AvatarReadySupportedProviderAttribute[] attributes = Attribute.GetCustomAttributes(t, typeof(AvatarReadySupportedProviderAttribute)) as AvatarReadySupportedProviderAttribute[];
                    return attributes.Any(attribute => attribute.supportedProvider.IsAssignableFrom(Target.AvatarProvider));
                }
                else
                {
                    Debug.LogWarning(t.Name + " does not have the " + nameof(AvatarReadySupportedProviderAttribute) + " attribute and thus cannot be used");
                    return false;
                }
            }).ToArray();

            if (avatarFeatures.Length > 0)
            {
                EditorGUILayout.LabelField("Feature:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                string[] avatarFeaturesOptions = ReflectionUtils.GetAvatarReadyNames(avatarFeatures);
                foreach (Type feature in avatarFeatures)
                {
                    if (EditorGUILayout.ToggleLeft(new GUIContent(ReflectionUtils.GetAvatarReadyName(feature)), Target.SelectedFeatures.Contains(feature)))
                    {
                        if (!Target.SelectedFeatures.Contains(feature))
                        {
                            Target.SelectedFeatures.Add(feature);
                            EditorUtility.SetDirty(Target);
                        }
                    }
                    else
                    {
                        if (Target.SelectedFeatures.Contains(feature))
                        {
                            Target.SelectedFeatures.Remove(feature);
                            EditorUtility.SetDirty(Target);
                        }
                    }
                    ShowDesc(feature, true);
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;

            if (!Target.runtimeImportMode)
            {
                // SETUP
                if (GUILayout.Button("Setup Avatar"))
                {
                    // TODO check required options
                    // TODO check required trackers

                    Target.SetupAvatar();

                    Target.activated = true;
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();

                if (GUILayout.Button("Reset"))
                {
                    Target.ResetAvatar();

                    Target.activated = false;
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }

                // Calibration Extra Settings
                Target.showCalibrationExtraSettings = EditorGUILayout.BeginFoldoutHeaderGroup(Target.showCalibrationExtraSettings, "Calibration extra settings");
                if (Target.showCalibrationExtraSettings)
                {
                    Target.calibrationOnStart = EditorGUILayout.Toggle(new GUIContent("Calibration on start", "If true, the selected profile will be used on start. Otherwise, you have to use the integrated UI."), Target.calibrationOnStart);
                    if (Target.calibrationOnStart)
                    {
                        CalibrationProfilesManager calibrationProfilesManager = CalibrationProfilesManager.Instance;

                        string[] options = calibrationProfilesManager.CalibrationProfiles.Select(cp => cp.Name).ToArray();

                        Target.selectedCalibrationOnStartProfile = EditorGUILayout.Popup(new GUIContent("Selected profile", "The profile that will be use on start"), Target.selectedCalibrationOnStartProfile, options);
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(Target);
        }

        #region UiUtils
        private void DynamicPopup(Type baseClass, bool IsAbstract, Type inputValue, Action<Type> setNextValue, GUIContent label, Func<Type, bool> filter = null, Action OnValueChanged = null)
        {
            Type[] types = ReflectionUtils.GetSubclassesOf(baseClass, IsAbstract);
            if (filter != null)
                types = types.Where(t => filter(t)).ToArray();
            string[] options = ReflectionUtils.GetAvatarReadyNames(types);
            int index = Array.IndexOf(types, inputValue);
            if (index < 0) index = 0;
            if (index > options.Length) index = 0;
            index = EditorGUILayout.Popup(label, index, options);
            Type nextValue = types[index];
            if (inputValue != nextValue)
            {
                setNextValue(nextValue);
                EditorUtility.SetDirty(Target);
                if (OnValueChanged != null)
                    OnValueChanged();
            }

            ShowDesc(nextValue);
        }

        private void DynamicOptionalPopup(Type baseClass, bool IsAbstract, Type inputValue, Action<Type> setNextValue, GUIContent label, Func<Type, bool> filter = null, Action OnValueChanged = null)
        {
            Type[] types = ReflectionUtils.GetSubclassesOf(baseClass, IsAbstract);
            if (filter != null)
                types = types.Where(t => filter(t)).ToArray();
            string[] options = new string[] { "None" };
            ArrayUtility.AddRange(ref options, ReflectionUtils.GetAvatarReadyNames(types));
            int index = (inputValue == null ? 0 : Array.IndexOf(types, inputValue) + 1);
            if (index < 0) index = 0;
            if (index > options.Length) index = 0;
            index = EditorGUILayout.Popup(label, index, options);
            Type nextValue = (index == 0 ? null : types[index - 1]);
            if (inputValue != nextValue)
            {
                setNextValue(nextValue);
                EditorUtility.SetDirty(Target);
                if (OnValueChanged != null)
                    OnValueChanged();
            }

            ShowDesc(nextValue);
        }

        private void DynamicConstField(Type baseClass, string inputValue, Action<string> setNextValue, GUIContent label, Action OnValueChanged = null)
        {
            string nextValue;
            if (baseClass.IsEnum)
                nextValue = EditorGUILayout.EnumPopup(label, (Enum)Enum.Parse(baseClass, inputValue)).ToString();
            else if (baseClass == typeof(bool))
                nextValue = EditorGUILayout.Toggle(label, bool.Parse(inputValue)).ToString();
            else if (baseClass == typeof(int))
                nextValue = EditorGUILayout.IntField(label, int.Parse(inputValue)).ToString();
            else if (baseClass == typeof(float))
                nextValue = EditorGUILayout.FloatField(label, float.Parse(inputValue)).ToString();
            else if (baseClass == typeof(double))
                nextValue = EditorGUILayout.DoubleField(label, double.Parse(inputValue)).ToString();
            else if (baseClass == typeof(string))
                nextValue = EditorGUILayout.TextField(label, inputValue);
            else
                throw new NotSupportedException(baseClass.Name);

            if (inputValue != nextValue)
            {
                setNextValue(nextValue);
                EditorUtility.SetDirty(Target);
                if (OnValueChanged != null)
                    OnValueChanged();
            }
        }

        private void DynamicReflectionField(Type configClass, string methodOptions, string inputValue, Action<string> setNextValue, GUIContent label, Action OnValueChanged = null)
        {
            MethodInfo method = configClass.GetMethod(methodOptions, BindingFlags.Public | BindingFlags.Static);
            if (method == null)
                throw new InvalidOperationException($"{methodOptions} must be the name of a public static method that return a string[] and takes no parameter inside your AnimationConfig");

            object retval = method.Invoke(null, null);
            if (retval is string[] options)
            {
                options = options.Prepend("None").ToArray();

                if (string.IsNullOrWhiteSpace(inputValue))
                    inputValue = "None";

                int index = Array.IndexOf(options, inputValue);
                if (index == -1)
                    index = 0;

                int selectedIndex = EditorGUILayout.Popup(label, index, options);

                string nextValue = options.ElementAt(selectedIndex);
                if (nextValue == "None")
                    nextValue = string.Empty;

                if (inputValue != nextValue)
                {
                    setNextValue(nextValue);
                    EditorUtility.SetDirty(Target);
                    if (OnValueChanged != null)
                        OnValueChanged();
                }
            }
            else
            {
                throw new InvalidOperationException($"{methodOptions} must be the name of a public static method that return a string[] and takes no parameter inside your AnimationConfig");
            }
        }

        private void DynamicObjectField(Type baseClass, UnityEngine.Object inputValue, Action<UnityEngine.Object> setNextValue, GUIContent label, Action OnValueChanged = null)
        {
            UnityEngine.Object nextValue = EditorGUILayout.ObjectField(label, inputValue, baseClass, true);

            if (inputValue != nextValue)
            {
                setNextValue(nextValue);
                EditorUtility.SetDirty(Target);
                if (OnValueChanged != null)
                    OnValueChanged();
            }
        }

        private void ShowDesc(Type value, bool wide = false)
        {
            if (value != null && Attribute.IsDefined(value, typeof(AvatarReadyNameAttribute)))
            {
                AvatarReadyNameAttribute name = Attribute.GetCustomAttribute(value, typeof(AvatarReadyNameAttribute)) as AvatarReadyNameAttribute;
                if (!string.IsNullOrWhiteSpace(name.desc))
                    EditorGUILayout.HelpBox(name.desc, MessageType.None, wide);
            }
        }
        #endregion
    }
}
