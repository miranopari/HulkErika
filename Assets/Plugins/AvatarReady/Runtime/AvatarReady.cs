using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    [RequireComponent(typeof(Animator))]
    public class AvatarReady : MonoBehaviour
    {
        public bool activated;

        public bool runtimeImportMode;

        #region SerializedInspector
        [SerializeField]
        private string _animationType;
        public Type AnimationType
        {
            get => !string.IsNullOrWhiteSpace(_animationType) ? Type.GetType(_animationType) : null;
            set => _animationType = value != null ? value.AssemblyQualifiedName : null;
        }

        [SerializeField]
        private string _animationSystem;
        public Type AnimationSystem
        {
            get => !string.IsNullOrWhiteSpace(_animationSystem) ? Type.GetType(_animationSystem) : null;
            set => _animationSystem = value != null ? value.AssemblyQualifiedName : null;
        }

        [SerializeField]
        private string _animationConfig;
        public Type AnimationConfig
        {
            get => !string.IsNullOrWhiteSpace(_animationConfig) ? Type.GetType(_animationConfig) : null;
            set => _animationConfig = value != null ? value.AssemblyQualifiedName : null;
        }

        [SerializeField]
        private string _avatarProvider;
        public Type AvatarProvider
        {
            get => !string.IsNullOrWhiteSpace(_avatarProvider) ? Type.GetType(_avatarProvider) : null;
            set => _avatarProvider = value != null ? value.AssemblyQualifiedName : null;
        }

        [SerializeField]
        private string _calibrationTechnique;
        public Type CalibrationTechnique
        {
            get => !string.IsNullOrWhiteSpace(_calibrationTechnique) ? Type.GetType(_calibrationTechnique) : null;
            set => _calibrationTechnique = value != null ? value.AssemblyQualifiedName : null;
        }

        public TypeList SelectedFeatures;

        public Transform[] Trackers;
        public TypeArray TrackerTypes;

        public OptionDict<string> SystemStringOptions;
        public OptionDict<UnityEngine.Object> SystemObjectOptions;

        public OptionDict<string> ConfigStringOptions;
        public OptionDict<UnityEngine.Object> ConfigObjectOptions;
        #endregion

        public bool showHelp;

        #region CalibrationExtraSettings
        public bool showCalibrationExtraSettings;

        public bool calibrationOnStart;
        public int selectedCalibrationOnStartProfile;
        #endregion

        #region EasyAccess
        private Animator _animator;
        public Animator Animator
        {
            get
            {
                if (_animator == null)
                    _animator = GetComponent<Animator>();
                return _animator;
            }
        }
        #endregion

        #region Utils
        [Serializable]
        public class TypeList : IEnumerable
        {
            [SerializeField]
            private List<string> serializedList;

            public TypeList()
            {
                serializedList = new List<string>();
            }

            public bool Contains(Type item)
            {
                return serializedList.Contains(item.AssemblyQualifiedName);
            }

            public void Add(Type item)
            {
                serializedList.Add(item.AssemblyQualifiedName);
            }

            public void Remove(Type item)
            {
                serializedList.Remove(item.AssemblyQualifiedName);
            }

            public IEnumerator GetEnumerator()
            {
                return serializedList.Where(str => !string.IsNullOrWhiteSpace(str)).Select(str => Type.GetType(str)).GetEnumerator();
            }
        }

        [Serializable]
        public class TypeArray
        {
            [SerializeField]
            private string[] serializedArray;

            public TypeArray(int capacity)
            {
                serializedArray = new string[capacity];
            }

            public Type this[int key]
            {
                get => serializedArray[key] != null ? Type.GetType(serializedArray[key]) : null;
                set => serializedArray[key] = (value != null ? value.AssemblyQualifiedName : null);
            }

            public int Length
            {
                get => serializedArray.Length;
            }
        }

        [Serializable]
        public class OptionDict<T> : IDictionary<string, T>
        {
            [Serializable]
            public class ConfigOption
            {
                public string name;
                public T value;
            }

            [SerializeField]
            private List<ConfigOption> serializedDict;

            public ICollection<string> Keys { get => serializedDict.Select(co => co.name).ToList(); }
            public ICollection<T> Values { get => serializedDict.Select(co => co.value).ToList(); }
            public int Count { get => serializedDict.Count; }
            public bool IsReadOnly { get => false; }

            public T this[string key]
            {
                get
                {
                    if (ContainsKey(key))
                        return serializedDict.Find(co => co.name == key).value;
                    else
                        throw new KeyNotFoundException(key);
                }
                set
                {
                    if (ContainsKey(key))
                        serializedDict.Find(co => co.name == key).value = value;
                    else
                        serializedDict.Add(new ConfigOption() { name = key, value = value });
                }
            }

            public OptionDict()
            {
                serializedDict = new List<ConfigOption>();
            }

            public void Add(string key, T value)
            {
                this[key] = value;
            }

            public bool ContainsKey(string key)
            {
                return serializedDict.Any(co => co.name == key);
            }

            public bool Remove(string key)
            {
                return serializedDict.RemoveAll(co => co.name == key) >= 1;
            }

            public bool TryGetValue(string key, out T value)
            {
                throw new NotImplementedException();
            }
            public void Clear()
            {
                serializedDict.Clear();
            }

            public void Add(KeyValuePair<string, T> item)
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<string, T> item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<string, T> item)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return serializedDict.GetEnumerator();
            }
        }
        #endregion

        private IEnumerator Start()
        {
            yield return null;

            if (calibrationOnStart)
            {
                CalibrationTechniqueBehaviour ctb = GetComponent<CalibrationTechniqueBehaviour>();
                if (ctb != null)
                {
                    CalibrationProfile cp = CalibrationProfilesManager.Instance.CalibrationProfiles[selectedCalibrationOnStartProfile];
                    ctb.Calibrate(cp);
                }
            }
        }

        public void SetupAvatar()
        {
            // Create Targets & Offsets
            Dictionary<HumanBodyBones, TargetHelper.TrackerDescriptor> trackerDescriptors = TargetHelper.CreateTargetsAndOffsets(this);

            // Add AnimationSystem 
            AnimationSystem animationSystem = Activator.CreateInstance(AnimationSystem) as AnimationSystem;
            animationSystem.Init(this);

            // Use AnimationConfig
            if (AnimationConfig != null)
            {
                AnimationConfig animationConfig = Activator.CreateInstance(AnimationConfig) as AnimationConfig;
                animationConfig.ApplyConfiguration(this, trackerDescriptors);
            }

            // apply calibration technique
            if (CalibrationTechnique != null)
            {
                if (typeof(CalibrationTechniqueSetting).IsAssignableFrom(CalibrationTechnique))
                {
                    // apply the setting
                    CalibrationTechniqueSetting calibrationTechnique = Activator.CreateInstance(CalibrationTechnique) as CalibrationTechniqueSetting;
                    calibrationTechnique.ApplySetting(this);

                    // assuming calibration is done on start, call dynamic offset afterward
                    Animator.gameObject.AddComponent<ApplyDynamicOffsetsOnStart>();
                }
                else if (typeof(CalibrationTechniqueBehaviour).IsAssignableFrom(CalibrationTechnique))
                {
                    // Add the UI in the scene if missing
                    CalibrationUi.AddUi();

                    // Add the MonoBehaviour
                    CalibrationTechniqueBehaviour calibrationTechnique = gameObject.AddComponent(CalibrationTechnique) as CalibrationTechniqueBehaviour;
                    calibrationTechnique.InitBehaviour(this, trackerDescriptors);

                    // the dynamicOffsets are applied after calibration
                }
                else
                {
                    throw new NotSupportedException("should not happen");
                }
            }
            else
            {
                // if no calibration technique, call dynamic offset on start
                Animator.gameObject.AddComponent<ApplyDynamicOffsetsOnStart>();
            }

            // Add Feature
            foreach (Type selectedFeature in SelectedFeatures)
            {
                AvatarFeature avatarFeature = gameObject.AddComponent(selectedFeature) as AvatarFeature;
                avatarFeature.InitFeature();
            }
        }

        public void ResetAvatar()
        {
            // call Reset() on everything to destroy every created gameobject and remove script or settings

            // remove Feature
            foreach (Type selectedFeature in SelectedFeatures)
            {
                AvatarFeature avatarFeature = gameObject.GetComponent(selectedFeature) as AvatarFeature;
                if (avatarFeature != null)
                {
                    avatarFeature.ResetFeature();
                    DestroyUtils.Destroy(avatarFeature);
                }

            }

            // remove calibrationTechnique
            if (CalibrationTechnique != null)
            {
                if (typeof(CalibrationTechniqueSetting).IsAssignableFrom(CalibrationTechnique))
                {
                    // reset the setting
                    CalibrationTechniqueSetting calibrationTechnique = Activator.CreateInstance(CalibrationTechnique) as CalibrationTechniqueSetting;
                    calibrationTechnique.ResetSetting(this);

                    DestroyUtils.Destroy(Animator.gameObject.GetComponent<ApplyDynamicOffsetsOnStart>());
                }
                else if (typeof(CalibrationTechniqueBehaviour).IsAssignableFrom(CalibrationTechnique))
                {

                    // remove the MonoBehaviour
                    CalibrationTechniqueBehaviour calibrationTechnique = gameObject.GetComponent<CalibrationTechniqueBehaviour>();
                    if (calibrationTechnique != null)
                    {
                        calibrationTechnique.ResetBehaviour();
                        DestroyUtils.Destroy(calibrationTechnique);
                    }
                }
                else
                {
                    throw new NotSupportedException("should not happen");
                }
            }
            else
            {
                // remove it
                DestroyUtils.Destroy(Animator.gameObject.GetComponent<ApplyDynamicOffsetsOnStart>());
            }

            // remove AnimationConfig
            if (AnimationConfig != null)
            {
                AnimationConfig animationConfig = Activator.CreateInstance(AnimationConfig) as AnimationConfig;
                animationConfig.ResetConfiguration(this);
            }

            // remove AnimationSystem 
            AnimationSystem animationSystem = Activator.CreateInstance(AnimationSystem) as AnimationSystem;
            animationSystem.Reset(this);

            // remove targets
            TargetHelper.RemoveTargets(this);
        }
    }
}
