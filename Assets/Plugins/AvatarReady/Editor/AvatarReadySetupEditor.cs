using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inria.Avatar.AvatarReady
{
    [CustomEditor(typeof(AvatarReadySetup))]
    public class AvatarReadySetupEditor : Editor
    {
        private AvatarReadySetup _target;
        private AvatarReadySetup Target
        {
            get
            {
                if (_target == null)
                    _target = target as AvatarReadySetup;
                return _target;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(Target.activated);

            // Avatar Type
            //EnumField(Target.AvatarType, e => Target.AvatarType = (AvatarReadySetup.AvatarTypes)e, new GUIContent("Avatar type"));
            EnumField(ref Target.AvatarType, new GUIContent("Avatar type"));

            /*
             * if static
             *      show the list of prefab that look like Avatar (search for CC_Avatars folder? Animator with Humanoid rig? 
             *      check if avatar is already a child?
             *      
             * if runtimeImport
             *      prompt a msg to indicates the steps and constraints that apply
             *      
             * if runtimeImport
             *      add events linked to UI and allow extenal msg
             */

            if (Target.AvatarType == AvatarReadySetup.AvatarTypes.Static)
            {
                EditorGUI.indentLevel++;

                // children animator
                Animator[] potentialChildrenAvatars = Target.GetComponentsInChildren<Animator>();

                // prefabs
                var potentialPrefabAvatars = AssetDatabase.FindAssets("t:prefab")
                    .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                    .Select(assetPath => AssetDatabase.LoadAssetAtPath<GameObject>(assetPath))
                    .Where(obj => obj.GetComponent<Animator>() != null)
                    .Where(obj => obj.GetComponent<Animator>().isHuman);

                List<string> options = new List<string>();
                foreach (Animator animator in potentialChildrenAvatars)
                    options.Add("Children/" + animator.name);
                foreach (GameObject prefab in potentialPrefabAvatars)
                    options.Add("Prefabs/" + prefab.name);

                DropdownField(options.ToArray(), ref Target.SelectedStaticAvatar, new GUIContent("Avatar"));

                EditorGUI.indentLevel--;
            }
            else if (Target.AvatarType == AvatarReadySetup.AvatarTypes.RuntimeImport)
            {
                EditorGUI.BeginDisabledGroup(true);
                // TODO explain the way
                EditorGUILayout.TextArea(
                    "If you choose to use the Avatar Runtime Import feature:\n" +
                    "Edit the AvatarReady config as normal on the dummy avatar.\n" +
                    "You must not add or modify components in Editor on the dummy avatar.\n" +
                    "If you want to add or modify components, please use the event provided\n" +
                    $"in the {nameof(AvatarReadyRuntimeLink)} and do it by script.");
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                throw new NotSupportedException(Target.AvatarType.ToString());
            }

            EditorGUILayout.Space();

            // SETUP
            if (GUILayout.Button("Setup AvatarReady"))
            {
                SetupAvatarReady();

                Target.activated = true;
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            if (GUILayout.Button("Reset"))
            {
                ResetAvatarReady();

                Target.activated = false;
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(Target);
        }

        private void SetupAvatarReady()
        {
            /*
             * if static
             *      get the prefab and instantiate it if provided
             *      add AvatarReady script
             *      
             * if runtimeImport
             *      get the default dummy prefab, instantiate it
             *      add AvatarReady script
             *      instantiate Import Ui to select avatar from website
             */
            if (Target.AvatarType == AvatarReadySetup.AvatarTypes.Static)
            {
                Debug.Log($"Setting up AvatarReady for static avatar {Target.SelectedStaticAvatar}");

                if (Target.SelectedStaticAvatar == null)
                {

                }
                else
                {
                    string[] split = Target.SelectedStaticAvatar.Split('/');
                    string type = split[0].ToLower();
                    string name = split[1];

                    if (type == "children")
                    {
                        GameObject child = Target.transform.GetComponentsInChildren<Animator>().Where(a => a.name == name).First().gameObject;

                        if (child == null)
                            throw new InvalidOperationException($"No child named {name} found");

                        child.GetOrAddComponent<AvatarReady>();
                    }
                    else if (type == "prefabs")
                    {
                        GameObject prefab = AssetDatabase.FindAssets("t:prefab")
                            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                            .Select(assetPath => AssetDatabase.LoadAssetAtPath<GameObject>(assetPath))
                            .Where(obj => obj.GetComponent<Animator>() != null)
                            .Where(obj => obj.GetComponent<Animator>().isHuman)
                            .Where(obj => obj.GetComponent<Animator>().name == name).First();

                        if (prefab == null)
                            throw new InvalidOperationException($"No prefab named {name} found");

                        GameObject instance = Instantiate(prefab, Target.transform);

                        instance.GetOrAddComponent<AvatarReady>();
                    }
                    else
                    {
                        throw new NotSupportedException(type);
                    }
                }
            }
            else if (Target.AvatarType == AvatarReadySetup.AvatarTypes.RuntimeImport)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/CC_DummyAvatar");

                if (prefab == null)
                    throw new InvalidOperationException("Missing CC_DummyAvatar");

                GameObject instance = Instantiate(prefab, Target.transform);

                AvatarReady avatarReady = instance.GetOrAddComponent<AvatarReady>();
                avatarReady.runtimeImportMode = true;

                Target.gameObject.GetOrAddComponent<AvatarReadyRuntimeLink>();

                // Add UI Prefab
                AvatarRuntimeImportUi.AddUi();
            }
        }

        private void ResetAvatarReady()
        {
            if (Target.AvatarType == AvatarReadySetup.AvatarTypes.Static)
            {

            }
            else if (Target.AvatarType == AvatarReadySetup.AvatarTypes.RuntimeImport)
            {
                DestroyUtils.Destroy(Target.GetComponentInChildren<AvatarReady>().gameObject);
                DestroyUtils.Destroy(Target.GetComponent<AvatarReadyRuntimeLink>());
            }
        }

        private void DropdownField(string[] options, ref string value, GUIContent label, Action OnValueChanged = null)
        {

            int index = value == null ? 0 : Array.IndexOf(options, value);
            if (index < 0) index = 0;
            if (index > options.Length) index = 0;
            index = EditorGUILayout.Popup(label, index, options);
            string nextValue = options[index];
            if (value != nextValue)
            {
                value = nextValue;
                EditorUtility.SetDirty(Target);
                if (OnValueChanged != null)
                    OnValueChanged();
            }
        }

        private void EnumField<T>(ref T inputValue, GUIContent label, Action OnValueChanged = null) where T : Enum
        {
            T nextValue = (T)EditorGUILayout.EnumPopup(label, inputValue);

            if (!inputValue.Equals(nextValue))
            {
                inputValue = nextValue;
                EditorUtility.SetDirty(Target);
                if (OnValueChanged != null)
                    OnValueChanged();
            }
        }

    }
}

