using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inria.Avatar.AvatarReady
{
    public class CalibrationUi : MonoBehaviour
    {
        private const string CalibrationUiPrefabName = "CalibrationUi";

        /// <summary>
        /// Load and instantiate the UI prefab from Resources
        /// </summary>
        public static void AddUi()
        {
            if (FindObjectOfType<CalibrationUi>() == null)
            {
                GameObject calibrationUi = Resources.Load<GameObject>(CalibrationUiPrefabName);

                if (calibrationUi == null)
                    throw new MissingComponentException(nameof(CalibrationUiPrefabName));

                Instantiate(calibrationUi);
            }

            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }
        }

        public Button CollapseButton;

        [Space]
        public Dropdown AvatarsDropdown;
        public Button RefreshAvatarsDropdownButton;

        [Space]
        public InputField CalibrationTechniqueNameValue;
        public InputField CalibrationTechniqueDescValue;

        [Space]
        public RectTransform TrackersContent;

        [Space]
        public Dropdown UserDropdown;
        public Button AddUserButton;
        public Button DeleteUserButton;
        public RectTransform UserDataContent;

        [Space]
        public Button CalibrateUsingProfileButton;
        public Button QuickCalibrateButton;
        public Button DelayedQuickCalibrateButton;

        [Space]
        public GameObject NewProfilePanel;
        public InputField NewProfileNameValue;
        public Button CreateNewProfileButton;
        public Button CancelNewProfileButton;

        [Space]
        public GameObject MessagePanel;
        public Text MessageText;


        private CalibrationProfilesManager CalibrationProfilesManager;

        private AvatarReady selectedAvatarReady = null;
        private CalibrationProfile selectedCalibrationProfile = null;

        private void Awake()
        {
            Assert.IsNotNull(CollapseButton);

            Assert.IsNotNull(AvatarsDropdown);
            Assert.IsNotNull(RefreshAvatarsDropdownButton);

            Assert.IsNotNull(CalibrationTechniqueNameValue);
            Assert.IsNotNull(CalibrationTechniqueDescValue);

            Assert.IsNotNull(TrackersContent);

            Assert.IsNotNull(UserDropdown);
            Assert.IsNotNull(AddUserButton);
            Assert.IsNotNull(DeleteUserButton);
            Assert.IsNotNull(UserDataContent);

            Assert.IsNotNull(CalibrateUsingProfileButton);
            Assert.IsNotNull(QuickCalibrateButton);
            Assert.IsNotNull(DelayedQuickCalibrateButton);

            Assert.IsNotNull(NewProfilePanel);
            Assert.IsNotNull(NewProfileNameValue);
            Assert.IsNotNull(CreateNewProfileButton);
            Assert.IsNotNull(CancelNewProfileButton);

            Assert.IsNotNull(MessagePanel);
            Assert.IsNotNull(MessageText);
        }

        private void Start()
        {
            CalibrationProfilesManager = CalibrationProfilesManager.Instance;

            // CollapseButton settings
            CollapseButton.onClick.AddListener(OnCollapseButtonClick);

            // AvatarDropdown settings
            AvatarsDropdown.onValueChanged.AddListener(OnAvatarDropdownValueChanged);
            ResetAvatarDropdown();
            RefreshAvatarsDropdownButton.onClick.AddListener(OnRefreshAvatarsDropdownButtonClick);

            // UserDropDown settings
            UserDropdown.onValueChanged.AddListener(OnUserDropdownValueChanged);
            ResetUserDropdown();

            // AddUserButton settings
            AddUserButton.onClick.AddListener(OnAddUserButtonClick);

            // DeleteUserButton settings
            DeleteUserButton.onClick.AddListener(OnDeleteUserButtonClick);

            // Calibrate buttons settings
            CalibrateUsingProfileButton.onClick.AddListener(OnCalibrateUsingProfileButtonClick);
            QuickCalibrateButton.onClick.AddListener(OnQuickCalibrateButtonClick);
            DelayedQuickCalibrateButton.onClick.AddListener(OnDelayedQuickCalibrateButtonClick);

            // NewProfilePanel settings
            NewProfilePanel.SetActive(false);

            // CreateNewProfileButton settings
            CreateNewProfileButton.onClick.AddListener(OnCreateNewProfileButtonClick);

            // CancelNewProfileButton settings
            CancelNewProfileButton.onClick.AddListener(OnCancelNewProfileButtonClick);

            // MessagePanel settings
            MessagePanel.SetActive(false);
        }

        private void Update()
        {
            CalibrateUsingProfileButton.interactable = selectedAvatarReady != null && selectedCalibrationProfile != null && !NewProfilePanel.activeSelf;
            QuickCalibrateButton.interactable = selectedAvatarReady != null && !NewProfilePanel.activeSelf;
            DelayedQuickCalibrateButton.interactable = QuickCalibrateButton.interactable;

        }

        #region CollapseButton
        /// <summary>
        /// Collapse/Show the panel
        /// </summary>
        private void OnCollapseButtonClick()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform.anchoredPosition.x == 0)
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x, 0);
                CollapseButton.GetComponentInChildren<Text>().text = "<<";
            }
            else
            {
                rectTransform.anchoredPosition = Vector2.zero;
                CollapseButton.GetComponentInChildren<Text>().text = ">>";
            }
        }
        #endregion

        #region Avatar
        private AvatarReady[] _avatarReadys;
        private AvatarReady[] AvatarReadys
        {
            get
            {
                if (_avatarReadys == null)
                    _avatarReadys = FindObjectsOfType<AvatarReady>().Where(ar => ar.activated).ToArray();
                return _avatarReadys;
            }
        }

        /// <summary>
        /// Search for every CalibrationTechniqueBehaviour in the scene
        /// </summary>
        private void ResetAvatarDropdown()
        {
            AvatarsDropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            options.Add(new Dropdown.OptionData("None"));
            foreach (AvatarReady ar in AvatarReadys)
            {
                options.Add(new Dropdown.OptionData(ar.transform.parent.name));
            }
            AvatarsDropdown.AddOptions(options);

            if (AvatarsDropdown.options.Count > 1)
                AvatarsDropdown.value = 1;
            else
                AvatarsDropdown.value = 0;
            OnAvatarDropdownValueChanged(AvatarsDropdown.value);
        }

        /// <summary>
        /// Called by RefreshAvatarsDropdownButton on click.
        /// Search for new CalibrationTechniqueBehaviour in scene and refresh the ui
        /// </summary>
        public void OnRefreshAvatarsDropdownButtonClick()
        {
            _avatarReadys = FindObjectsOfType<AvatarReady>().Where(ar => ar.activated).ToArray();
            ResetAvatarDropdown();
        }

        /// <summary>
        /// Update informations of the selected CalibrationTechniqueBehaviour
        /// </summary>
        private void OnAvatarDropdownValueChanged(int newValue)
        {
            if (AvatarsDropdown.value == 0)
            {
                selectedAvatarReady = null;
            }
            else
            {
                selectedAvatarReady = AvatarReadys[AvatarsDropdown.value - 1];
            }

            UpdateCalibrationTechniquePanel();
            UpdateTrackersPanel();

            // Update User Profile Panel
            OnUserDropdownValueChanged(UserDropdown.value);
        }
        #endregion

        #region CalibrationTechniquePanel
        private void UpdateCalibrationTechniquePanel()
        {
            if (selectedAvatarReady == null)
            {
                CalibrationTechniqueNameValue.text = "";
                CalibrationTechniqueDescValue.text = "";
            }
            else
            {
                CalibrationTechniqueBehaviour selectedCalibrationTechniqueBehaviour = selectedAvatarReady.GetComponent<CalibrationTechniqueBehaviour>();
                if (selectedCalibrationTechniqueBehaviour != null)
                {
                    if (Attribute.IsDefined(selectedCalibrationTechniqueBehaviour.GetType(), typeof(AvatarReadyNameAttribute)))
                    {
                        AvatarReadyNameAttribute arna = Attribute.GetCustomAttribute(selectedCalibrationTechniqueBehaviour.GetType(), typeof(AvatarReadyNameAttribute)) as AvatarReadyNameAttribute;
                        CalibrationTechniqueNameValue.text = arna.displayName;
                        CalibrationTechniqueDescValue.text = string.IsNullOrWhiteSpace(arna.desc) ? "" : arna.desc;
                    }
                }
                else
                {
                    CalibrationTechniqueNameValue.text = "";
                    CalibrationTechniqueDescValue.text = "";
                }
            }
        }
        #endregion

        #region Trackers
        private void UpdateTrackersPanel()
        {
            // Remove all children
            foreach (Transform child in TrackersContent)
            {
                DestroyUtils.Destroy(child.gameObject);
            }

            if (selectedAvatarReady != null && selectedAvatarReady.AnimationConfig != null)
            {
                AvatarReadySupportedTrackerAttribute[] supportedTrackers = Attribute.GetCustomAttributes(selectedAvatarReady.AnimationConfig, typeof(AvatarReadySupportedTrackerAttribute)) as AvatarReadySupportedTrackerAttribute[];
                supportedTrackers = supportedTrackers.OrderBy(t => t.order).ToArray();
                for (int i = 0; i < supportedTrackers.Length; i++)
                {
                    if (selectedAvatarReady.Trackers[i] != null)
                    {
                        AddTrackerDataItem(
                            supportedTrackers[i].trackerName,
                            supportedTrackers[i].trackerType,
                            selectedAvatarReady.TrackerTypes,
                            i
                        );
                    }
                }
            }
        }

        public GameObject _trackerDataPrefab;
        public GameObject TrackerDataPrefab
        {
            get
            {
                if (_trackerDataPrefab == null)
                    _trackerDataPrefab = Resources.Load<GameObject>("Prefabs/TrackerData");
                return _trackerDataPrefab;
            }
        }

        private void AddTrackerDataItem(string trackerName, Type trackerType, AvatarReady.TypeArray inOutTrackerType, int index)
        {
            Type initialTrackerType = inOutTrackerType[index];

            GameObject data = Instantiate(TrackerDataPrefab, TrackersContent, false);
            data.transform.Find("TrackerLabel").GetComponent<Text>().text = trackerName;

            Dropdown dropdown = data.transform.Find("TrackerTypeDropdown").GetComponent<Dropdown>();
            dropdown.ClearOptions();

            Type[] types = ReflectionUtils.GetSubclassesOf(trackerType);
            List<string> options = new List<string>();
            options.Add("None");
            options.AddRange(ReflectionUtils.GetAvatarReadyNames(types));

            dropdown.AddOptions(options);

            // set input value
            int value = (initialTrackerType == null ? 0 : Array.IndexOf(types, initialTrackerType) + 1);
            if (value < 0) value = 0;
            if (value > options.Count) value = 0;
            dropdown.value = value;


            // registrer onValueChanged
            dropdown.onValueChanged.AddListener((nextValue) =>
            {
                Type nextType = (nextValue == 0 ? null : types[nextValue - 1]);
                if (inOutTrackerType[index] != nextType)
                    inOutTrackerType[index] = nextType;
            });

        }
        #endregion

        #region User
        /// <summary>
        /// Get every CalibrationProfile from the manager
        /// </summary>
        private void ResetUserDropdown()
        {
            UserDropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            options.Add(new Dropdown.OptionData("None"));
            foreach (CalibrationProfile cp in CalibrationProfilesManager.CalibrationProfiles)
            {
                options.Add(new Dropdown.OptionData(cp.Name));
            }
            UserDropdown.AddOptions(options);

            if (UserDropdown.options.Count > 1)
                UserDropdown.value = 1;
            else
                UserDropdown.value = 0;
            OnUserDropdownValueChanged(UserDropdown.value);
        }

        /// <summary>
        /// Show InputField according to the CalibrationTechniqueBehaviour and the data inside the profile
        /// </summary>
        private void OnUserDropdownValueChanged(int newValue)
        {
            // Remove all children
            foreach (Transform child in UserDataContent)
            {
                DestroyUtils.Destroy(child.gameObject);
            }

            if (UserDropdown.value != 0)
            {
                CalibrationTechniqueBehaviour selectedCalibrationTechniqueBehaviour = null;
                if (selectedAvatarReady != null)
                    selectedCalibrationTechniqueBehaviour = selectedAvatarReady.GetComponent<CalibrationTechniqueBehaviour>();

                selectedCalibrationProfile = CalibrationProfilesManager.CalibrationProfiles[UserDropdown.value - 1];

                if (selectedCalibrationTechniqueBehaviour == null)
                {
                    // if no selected calibration technique, show readonly data inside the profile
                    foreach (CalibrationProfile.CalibrationData cd in selectedCalibrationProfile.AllData())
                    {
                        AddDataUi(cd.Key, cd.Value);
                    }
                }
                else
                {
                    // if there is a selected calibration technique,
                    // show data used by calibration technique and allow to modify them
                    AvatarReadyCalibrationData[] arcds = Attribute.GetCustomAttributes(selectedCalibrationTechniqueBehaviour.GetType(), typeof(AvatarReadyCalibrationData)) as AvatarReadyCalibrationData[];
                    foreach (AvatarReadyCalibrationData arcd in arcds)
                    {
                        if (selectedCalibrationProfile.ContainsData(arcd.DataKey))
                        {
                            AddDataUi(arcd.DataKey, selectedCalibrationProfile.Get(arcd.DataKey), true);
                        }
                        else
                        {
                            AddDataUi(arcd.DataKey, float.NaN, true);
                        }
                    }

                    AddDataUiSeparator();

                    // then show every other data not used by the calibration technique inside the profile as readonly
                    foreach (CalibrationProfile.CalibrationData cd in selectedCalibrationProfile.AllData().Where(cp => !arcds.Select(arcd => arcd.DataKey).Contains(cp.Key)))
                    {
                        AddDataUi(cd.Key, cd.Value);
                    }
                }
            }
            else
            {
                selectedCalibrationProfile = null;
            }
        }

        public GameObject _activeDataPrefab;
        public GameObject ActiveDataPrefab
        {
            get
            {
                if (_activeDataPrefab == null)
                    _activeDataPrefab = Resources.Load<GameObject>("Prefabs/ActiveData");
                return _activeDataPrefab;
            }
        }
        public GameObject _otherDataPrefab;
        public GameObject OtherDataPrefab
        {
            get
            {
                if (_otherDataPrefab == null)
                    _otherDataPrefab = Resources.Load<GameObject>("Prefabs/OtherData");
                return _otherDataPrefab;
            }
        }

        /// <summary>
        /// Instantiates an UI prefab to display a profile data.
        /// </summary>
        private void AddDataUi(string key, float value, bool active = false)
        {
            GameObject data;
            if (active)
            {
                data = Instantiate(ActiveDataPrefab, UserDataContent, false);
            }
            else
            {
                data = Instantiate(OtherDataPrefab, UserDataContent, false);
            }

            data.transform.Find("DataLabel").GetComponent<Text>().text = Nicify(key);

            InputField dataValueInputField = data.transform.Find("DataValue").GetComponent<InputField>();
            if (value != float.NaN)
                dataValueInputField.text = value.ToString("0.00");
            dataValueInputField.interactable = active;
            dataValueInputField.onEndEdit.AddListener((inputValue) =>
            {
                float userInputValue;
                if (float.TryParse(dataValueInputField.text, out userInputValue))
                {
                    selectedCalibrationProfile.Set(key, userInputValue);
                    CalibrationProfilesManager.UpdateCalibrationProfile(selectedCalibrationProfile);
                }
                else
                {
                    ShowMessage("Error! Wrong format");
                    if (selectedCalibrationProfile.ContainsData(key))
                    {
                        dataValueInputField.text = selectedCalibrationProfile.Get(key).ToString("0.00");
                    }
                    else
                    {
                        if (value != float.NaN)
                            dataValueInputField.text = value.ToString("0.00");
                        else
                            dataValueInputField.text = null;
                    }
                }

            });

            if (active)
            {
                data.transform.Find("RefreshDataButton").GetComponent<Button>().onClick.AddListener(() =>
                {
                    RetrieveCalibrationData(dataValueInputField, key);
                });

                data.transform.Find("DelayedRefreshDataButton").GetComponent<Button>().onClick.AddListener(() =>
                {
                    StopAllCoroutines();
                    StartCoroutine(DelayRetrieveCalibrationData(dataValueInputField, key));
                });
            }
        }

        /// <summary>
        /// Add an empty UI object as visual separator
        /// </summary>
        private void AddDataUiSeparator()
        {
            GameObject separator = new GameObject("Separator", typeof(RectTransform));
            separator.transform.SetParent(UserDataContent);
            RectTransform separatorRectTransform = separator.GetComponent<RectTransform>();
            separatorRectTransform.sizeDelta = new Vector2(0, 10);

            Image separatorImage = separator.AddComponent<Image>();
            separatorImage.color = new Color(0, 0, 0, 0.2f);
        }

        /// <summary>
        /// Waits 5 seconds and calls <see cref="RetrieveCalibrationData"/>
        /// </summary>
        private IEnumerator DelayRetrieveCalibrationData(InputField inputField, string key)
        {
            yield return new WaitForSeconds(5);
            RetrieveCalibrationData(inputField, key);
        }

        /// <summary>
        /// Retrieves the data from the selected calibrationTechniqueBehaviour and set the value in the inputField
        /// </summary>
        private void RetrieveCalibrationData(InputField inputField, string key)
        {
            try
            {
                CalibrationTechniqueBehaviour selectedCalibrationTechniqueBehaviour = selectedAvatarReady.GetComponent<CalibrationTechniqueBehaviour>();
                if (selectedCalibrationTechniqueBehaviour != null)
                {
                    float computedValue = selectedCalibrationTechniqueBehaviour.GetData(key);

                    inputField.text = computedValue.ToString("0.00");
                    inputField.onEndEdit.Invoke(inputField.text);
                }
                else
                {
                    Debug.LogError("ERROR: Should not happen");
                }
            }
            catch (KeyNotFoundException e)
            {
                ShowMessage($"Error! Can't get data named '{key}' from calibration technique");
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Transforms a variable name into a nice name that can be display.
        /// </summary>
        private string Nicify(string str)
        {
            StringBuilder retval = new StringBuilder();
            string input = str.Trim();
            for (int i = 0; i < input.Length; i++)
            {
                // If this character is a capital and it is not the first character add a space.
                if (char.IsUpper(input[i]) == true && i != 0)
                {
                    retval.Append(" ");
                }

                // Add the character to our new string
                if (i == 0)
                    retval.Append(char.ToUpper(input[i]));
                else
                    retval.Append(input[i]);
            }
            return retval.ToString();
        }

        /// <summary>
        /// Display the NewProfilePanel
        /// </summary>
        private void OnAddUserButtonClick()
        {
            NewProfilePanel.SetActive(true);
            CalibrateUsingProfileButton.interactable = false;
            QuickCalibrateButton.interactable = false;
        }

        /// <summary>
        /// Delete the object and file of the selected profile, and then reload the dropdown
        /// </summary>
        private void OnDeleteUserButtonClick()
        {
            // TODO add confirmation

            CalibrationProfilesManager.DeleteCalibrationProfile(selectedCalibrationProfile);

            ResetUserDropdown();
        }
        #endregion

        #region NewProfilePanel
        private void OnCreateNewProfileButtonClick()
        {
            if (string.IsNullOrWhiteSpace(NewProfileNameValue.text))
            {
                ShowMessage("Profile name is required");
                return;
            }
            // TODO sanitize user input

            CalibrationProfile cp = new CalibrationProfile();
            cp.Name = NewProfileNameValue.text;

            CalibrationProfilesManager.AddCalibrationProfile(cp);

            ResetUserDropdown();

            NewProfilePanel.SetActive(false);
        }

        private void OnCancelNewProfileButtonClick()
        {
            NewProfilePanel.SetActive(false);
        }
        #endregion

        #region CalibrationButtons
        public void OnCalibrateUsingProfileButtonClick()
        {
            TargetHelper.ReapplyOffsets(selectedAvatarReady);

            if (selectedAvatarReady.GetComponent<CalibrationTechniqueBehaviour>() != null)
                selectedAvatarReady.GetComponent<CalibrationTechniqueBehaviour>().Calibrate(selectedCalibrationProfile);

            TargetHelper.ApplyDynamicOffsets(selectedAvatarReady.gameObject);
        }
        private void OnQuickCalibrateButtonClick()
        {
            TargetHelper.ReapplyOffsets(selectedAvatarReady);

            if (selectedAvatarReady.GetComponent<CalibrationTechniqueBehaviour>() != null)
                selectedAvatarReady.GetComponent<CalibrationTechniqueBehaviour>().Calibrate(null);

            TargetHelper.ApplyDynamicOffsets(selectedAvatarReady.gameObject);
        }
        private void OnDelayedQuickCalibrateButtonClick()
        {
            StopAllCoroutines();
            StartCoroutine(OnDelayedQuickCalibrateButtonClickCoroutine());
        }

        private IEnumerator OnDelayedQuickCalibrateButtonClickCoroutine()
        {
            yield return new WaitForSeconds(5);
            OnQuickCalibrateButtonClick();
        }
        #endregion

        #region MessagePanelEvents
        private void ShowMessage(string msg)
        {
            MessageText.text = msg;
            StartCoroutine(ToastMessage());
        }

        private IEnumerator ToastMessage()
        {
            MessagePanel.SetActive(true);
            yield return new WaitForSeconds(3);
            MessagePanel.SetActive(false);
        }
        #endregion
    }
}
