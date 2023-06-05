using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Inria.Avatar.AvatarReady
{
    public class AvatarRuntimeImportUi : MonoBehaviour
    {
        private const string AvatarRuntimeImportUiPrefabName = "AvatarRuntimeImportUi";

        /// <summary>
        /// Load and instantiate the UI prefab from Resources
        /// </summary>
        public static void AddUi()
        {
            if (FindObjectOfType<AvatarRuntimeImportUi>() == null)
            {
                GameObject avatarRuntimeImportUi = Resources.Load<GameObject>(AvatarRuntimeImportUiPrefabName);

                if (avatarRuntimeImportUi == null)
                    throw new MissingComponentException(nameof(AvatarRuntimeImportUiPrefabName));

                Instantiate(avatarRuntimeImportUi);
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
        public Dropdown TargetDropdown;
        public Button RefreshTargetDropdownButton;

        [Space]
        public Dropdown AvatarSetDropdown;

        [Space]
        public Button FilterFemaleButton;
        public Button FilterMaleButton;

        public Button Filter10Button;
        public Button Filter20Button;
        public Button Filter30Button;
        public Button Filter40Button;
        public Button Filter50Button;

        public Transform AvatarListContent;

        [Space]
        public Toggle AutomaticCalibrationToggle;

        [Space]
        public Button SelectAvatarButton;


        private GameObject _avatarItemPrefab;
        public GameObject AvatarItemPrefab
        {
            get
            {
                if (_avatarItemPrefab == null)
                {
                    _avatarItemPrefab = Resources.Load<GameObject>("Prefabs/AvatarItem");
                    if (_avatarItemPrefab == null)
                        Debug.LogError("Missing prefab");
                }
                return _avatarItemPrefab;
            }
        }

        private Texture _defaultPreviewTexture;
        private Texture DefaultPreviewTexture
        {
            get
            {
                if (_defaultPreviewTexture == null)
                {
                    _defaultPreviewTexture = Resources.Load<Texture>("Ui/person");
                    if (_defaultPreviewTexture == null)
                        Debug.LogError("Missing texture");
                }
                return _defaultPreviewTexture;
            }
        }

        #region Filters
        private bool FilterAvailable
        {
            set
            {
                FilterFemaleButton.interactable = value;
                FilterMaleButton.interactable = value;
                Filter10Button.interactable = value;
                Filter20Button.interactable = value;
                Filter30Button.interactable = value;
                Filter40Button.interactable = value;
                Filter50Button.interactable = value;
            }
        }

        private bool _filterMaleActivated = false;
        private bool FilterMaleActivated
        {
            get
            {
                return _filterMaleActivated;
            }
            set
            {
                _filterMaleActivated = value;
                FilterMaleButton.image.color = (_filterMaleActivated ? selectedButtonColor : Color.white);
            }
        }

        private bool _filterFemaleActivated = false;
        private bool FilterFemaleActivated
        {
            get
            {
                return _filterFemaleActivated;
            }
            set
            {
                _filterFemaleActivated = value;
                FilterFemaleButton.image.color = (_filterFemaleActivated ? selectedButtonColor : Color.white);
            }
        }


        private bool _filter10Activated = false;
        private bool Filter10Activated
        {
            get
            {
                return _filter10Activated;
            }
            set
            {
                _filter10Activated = value;
                Filter10Button.image.color = (_filter10Activated ? selectedButtonColor : Color.white);
            }
        }

        private bool _filter20Activated = false;
        private bool Filter20Activated
        {
            get
            {
                return _filter20Activated;
            }
            set
            {
                _filter20Activated = value;
                Filter20Button.image.color = (_filter20Activated ? selectedButtonColor : Color.white);
            }
        }

        private bool _filter30Activated = false;
        private bool Filter30Activated
        {
            get
            {
                return _filter30Activated;
            }
            set
            {
                _filter30Activated = value;
                Filter30Button.image.color = (_filter30Activated ? selectedButtonColor : Color.white);
            }
        }

        private bool _filter40Activated = false;
        private bool Filter40Activated
        {
            get
            {
                return _filter40Activated;
            }
            set
            {
                _filter40Activated = value;
                Filter40Button.image.color = (_filter40Activated ? selectedButtonColor : Color.white);
            }
        }

        private bool _filter50Activated = false;
        private bool Filter50Activated
        {
            get
            {
                return _filter50Activated;
            }
            set
            {
                _filter50Activated = value;
                Filter50Button.image.color = (_filter50Activated ? selectedButtonColor : Color.white);
            }
        }
        #endregion

        private AvatarReadySetup selectedTarget;

        private string selectedAvatar;
        private GameObject selectedAvatarItem;

        private bool running = false;

        private readonly Color defaultButtonColor = new Color(1, 1, 1, 0.3921569f);
        private readonly Color selectedButtonColor = new Color(0.3529412f, 0.7960784f, 1);
        private readonly Color waitingButtonColor = new Color(0.37f, 0.76f, 0.91f);

        private void Awake()
        {
            Assert.IsNotNull(CollapseButton);

            Assert.IsNotNull(TargetDropdown);
            Assert.IsNotNull(RefreshTargetDropdownButton);

            Assert.IsNotNull(AvatarSetDropdown);

            Assert.IsNotNull(FilterFemaleButton);
            Assert.IsNotNull(FilterMaleButton);

            Assert.IsNotNull(Filter10Button);
            Assert.IsNotNull(Filter20Button);
            Assert.IsNotNull(Filter30Button);
            Assert.IsNotNull(Filter40Button);
            Assert.IsNotNull(Filter50Button);

            Assert.IsNotNull(AvatarListContent);

            Assert.IsNotNull(AutomaticCalibrationToggle);

            Assert.IsNotNull(SelectAvatarButton);
        }

        private void Start()
        {
            // CollapseButton settings
            CollapseButton.onClick.AddListener(OnCollapseButtonClick);

            // TargetDropdown settings
            TargetDropdown.onValueChanged.AddListener(OnTargetDropdownValueChanged);
            ResetTargetDropdown();
            RefreshTargetDropdownButton.onClick.AddListener(OnRefreshTargetDropdownButtonClick);

            // SourceDropdown settings
            AvatarSetDropdown.onValueChanged.AddListener(OnAvatarSetDropdownValueChanged);
            ResetAvatarSetDropdown();

            // Filters settings
            FilterFemaleButton.onClick.AddListener(OnFilterFemaleButtonClick);
            FilterMaleButton.onClick.AddListener(OnFilterMaleButtonClick);

            Filter10Button.onClick.AddListener(OnFilter10ButtonClick);
            Filter20Button.onClick.AddListener(OnFilter20ButtonClick);
            Filter30Button.onClick.AddListener(OnFilter30ButtonClick);
            Filter40Button.onClick.AddListener(OnFilter40ButtonClick);
            Filter50Button.onClick.AddListener(OnFilter50ButtonClick);

            // AvatarListContent
            ResetAvatarListContent();
            ResetAvatarListContext();

            // SelectAvatarButton settings
            SelectAvatarButton.onClick.AddListener(OnSelectAvatarButtonClick);
        }

        private void Update()
        {
            SelectAvatarButton.interactable = selectedTarget != null && !string.IsNullOrWhiteSpace(selectedAvatar) && !running;
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
                rectTransform.anchoredPosition = new Vector2(-rectTransform.sizeDelta.x, 0);
                CollapseButton.GetComponentInChildren<Text>().text = ">>";
            }
            else
            {
                rectTransform.anchoredPosition = Vector2.zero;
                CollapseButton.GetComponentInChildren<Text>().text = "<<";
            }
        }
        #endregion

        #region Target
        private AvatarReadySetup[] _avatarReadySetups;
        private AvatarReadySetup[] AvatarReadySetups
        {
            get
            {
                if (_avatarReadySetups == null)
                    _avatarReadySetups = FindObjectsOfType<AvatarReadySetup>().Where(ars => ars.AvatarType == AvatarReadySetup.AvatarTypes.RuntimeImport).ToArray();
                return _avatarReadySetups;
            }
        }

        private void ResetTargetDropdown()
        {
            TargetDropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            options.Add(new Dropdown.OptionData("None"));
            foreach (AvatarReadySetup ars in AvatarReadySetups)
            {
                options.Add(new Dropdown.OptionData(ars.name));
            }
            TargetDropdown.AddOptions(options);

            if (TargetDropdown.options.Count > 1)
                TargetDropdown.value = 1;
            else
                TargetDropdown.value = 0;
            OnTargetDropdownValueChanged(TargetDropdown.value);
        }

        private void OnTargetDropdownValueChanged(int newValue)
        {
            if (TargetDropdown.value == 0)
            {
                selectedTarget = null;
            }
            else
            {
                selectedTarget = AvatarReadySetups[TargetDropdown.value - 1];
            }
        }

        private void OnRefreshTargetDropdownButtonClick()
        {
            _avatarReadySetups = FindObjectsOfType<AvatarReadySetup>().Where(ars => ars.AvatarType == AvatarReadySetup.AvatarTypes.RuntimeImport).ToArray();
            ResetTargetDropdown();
        }
        #endregion

        #region Set
        private string selectedAvatarSet;

        private List<JSONObject> possibleAvatars = new List<JSONObject>();

        private const string optionAvatarGalleryLabel = "Avatar Gallery";
        private const string optionLocalAvatarsLabel = "Local avatars";

        private void ResetAvatarSetDropdown()
        {
            AvatarSetDropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            options.Add(new Dropdown.OptionData(optionAvatarGalleryLabel));
            options.Add(new Dropdown.OptionData(optionLocalAvatarsLabel));

            // TODO search for local configuration

            AvatarSetDropdown.AddOptions(options);

            AvatarSetDropdown.value = 0;
            OnAvatarSetDropdownValueChanged(AvatarSetDropdown.value);
        }

        private void OnAvatarSetDropdownValueChanged(int newValue)
        {
            ResetAvatarListContent();
            ResetAvatarListContext();

            selectedAvatarSet = AvatarSetDropdown.options[AvatarSetDropdown.value].text;

            if (selectedAvatarSet == optionAvatarGalleryLabel)
            {
                FilterAvailable = true;

                // Request AvatarGallery API
                StartCoroutine(RequestAvatarGalleryApi());
            }
            else if (selectedAvatarSet == optionLocalAvatarsLabel)
            {
                FilterAvailable = false;

                // Look for all avatars in cache, ie. saved locally
                PopulateLocalAvatarItems();

            }
            else
            {
                FilterAvailable = true;

                // TODO look for local subset files
            }
        }

        private IEnumerator RequestAvatarGalleryApi()
        {
            JSONArray avatars = null;

            using (UnityWebRequest apiRequest = UnityWebRequest.Get("https://avatar-gallery.irisa.fr/api.php"))
            {
                apiRequest.timeout = 3;

                yield return apiRequest.SendWebRequest();

                if (apiRequest.result == UnityWebRequest.Result.Success)
                {
                    string textResponse = apiRequest.downloadHandler.text;

                    JSONNode json = JSON.Parse(textResponse);

                    if (!json.IsArray)
                        throw new Exception("Json response must be an array");

                    avatars = json.AsArray;
                }
                else
                {
                    Debug.LogError($"Fail to reach 'https://avatar-gallery.irisa.fr'. You have to be inside the Inria network." +
                        $"\nResult='{apiRequest.result}', Error='{apiRequest.error}', Code='{apiRequest.responseCode}'");

                    Debug.LogWarning("Switching to 'Local avatars' set");

                    AvatarSetDropdown.value = 1;
                    OnAvatarSetDropdownValueChanged(AvatarSetDropdown.value);
                }
            }

            if (avatars == null)
                yield break;

            ProcessJson(avatars);

            PopulateAvatarGalleryAvatarItems();
        }

        private void ProcessJson(JSONArray avatars)
        {
            foreach (JSONNode jsonAvatar in avatars.Values)
            {
                if (!jsonAvatar.IsObject)
                {
                    throw new Exception("content must be JSONObject");
                }

                JSONObject avatar = jsonAvatar.AsObject;

                if (avatar[AvatarCache.CurrentRenderPipeline] != "not_available")
                {
                    possibleAvatars.Add(avatar);
                }
            }
        }
        #endregion

        #region AvatarList
        private void ResetAvatarListContent()
        {
            // reset selection
            selectedAvatar = null;
            selectedAvatarItem = null;

            // clear content
            foreach (Transform child in AvatarListContent)
                DestroyUtils.Destroy(child.gameObject);

            // show loading item
            Instantiate(AvatarItemPrefab, AvatarListContent);
        }

        private void ResetAvatarListContext()
        {
            // reset filters
            FilterMaleActivated = false;
            FilterFemaleActivated = false;
            Filter10Activated = false;
            Filter20Activated = false;
            Filter30Activated = false;
            Filter40Activated = false;
            Filter50Activated = false;

            // clear json
            possibleAvatars.Clear();
        }

        private void PopulateAvatarGalleryAvatarItems()
        {
            // clear content
            foreach (Transform child in AvatarListContent)
                DestroyUtils.Destroy(child.gameObject);

            foreach (JSONObject avatar in possibleAvatars)
            {
                // Apply filters
                if (FilterMaleActivated && avatar["metadata"]["gender"].Value != "male")
                    continue;
                if (FilterFemaleActivated && avatar["metadata"]["gender"].Value != "female")
                    continue;

                if (Filter10Activated && avatar["metadata"]["age"].Value != "10_20")
                    continue;
                if (Filter20Activated && avatar["metadata"]["age"].Value != "20_30")
                    continue;
                if (Filter30Activated && avatar["metadata"]["age"].Value != "30_40")
                    continue;
                if (Filter40Activated && avatar["metadata"]["age"].Value != "40_50")
                    continue;
                if (Filter50Activated && avatar["metadata"]["age"].Value != "50_60")
                    continue;

                // Add item
                GameObject item = Instantiate(AvatarItemPrefab, AvatarListContent);

                item.transform.Find("Name").GetComponent<Text>().text = avatar["name"].Value;

                if (!avatar["preview"].IsNull && !avatar["preview"]["body"].IsNull)
                {
                    StartCoroutine(DownloadPreviewImage(item.transform.Find("Preview").GetComponent<RawImage>(), avatar["preview"]["body"]));
                }
                else
                {
                    SetPreviewImage(item.transform.Find("Preview").GetComponent<RawImage>(), DefaultPreviewTexture);
                }

                item.GetComponent<Button>().onClick.AddListener(() => OnAvatarItemClick(item, avatar["name"].Value));
            }
        }

        private void PopulateLocalAvatarItems()
        {
            // clear content
            foreach (Transform child in AvatarListContent)
                DestroyUtils.Destroy(child.gameObject);

            foreach (string avatar in AvatarCache.Keys)
            {
                // Add item
                GameObject item = Instantiate(AvatarItemPrefab, AvatarListContent);

                item.transform.Find("Name").GetComponent<Text>().text = avatar;

                SetPreviewImage(item.transform.Find("Preview").GetComponent<RawImage>(), DefaultPreviewTexture);

                item.GetComponent<Button>().onClick.AddListener(() => OnAvatarItemClick(item, avatar));
            }
        }

        private void OnAvatarItemClick(GameObject item, string avatarName)
        {
            if (selectedAvatarItem != null)
                selectedAvatarItem.GetComponent<Button>().image.color = defaultButtonColor;

            selectedAvatar = avatarName;
            selectedAvatarItem = item;

            if (selectedAvatarItem != null)
                selectedAvatarItem.GetComponent<Button>().image.color = selectedButtonColor;
        }

        private Dictionary<string, Texture> previewImages = new Dictionary<string, Texture>();

        private IEnumerator DownloadPreviewImage(RawImage target, string url)
        {
            if (!previewImages.ContainsKey(url))
            {
                using (UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://avatar-gallery.irisa.fr" + url))
                {
                    yield return www.SendWebRequest();

                    // avoid concurent assignment
                    if (!previewImages.ContainsKey(url))
                        previewImages.Add(url, DownloadHandlerTexture.GetContent(www));
                }
            }

            // avoid concurent assignment
            if (target == null || target.Equals(null))
                yield break;

            SetPreviewImage(target, previewImages[url]);
        }

        private void SetPreviewImage(RawImage target, Texture tex)
        {
            target.texture = tex;

            // scale image
            int textureWidth = target.texture.width;
            int textureHeight = target.texture.height;

            if (textureWidth == textureHeight)
                return;

            float panelWidth = target.rectTransform.rect.width;
            float panelHeight = target.rectTransform.rect.height;

            if (textureWidth > textureHeight)
            {
                float v = textureHeight * panelWidth / textureWidth;

                target.rectTransform.sizeDelta = new Vector2(target.rectTransform.sizeDelta.x, target.rectTransform.sizeDelta.y - v / 2);

            }
            else
            {
                float v = textureWidth * panelHeight / textureHeight;

                target.rectTransform.sizeDelta = new Vector2(target.rectTransform.sizeDelta.x - v / 2, target.rectTransform.sizeDelta.y);
            }
        }
        #endregion

        #region Filters
        private void OnFilterFemaleButtonClick()
        {
            FilterFemaleActivated = !FilterFemaleActivated;
            FilterMaleActivated = false;

            ResetAvatarListContent();
            PopulateAvatarGalleryAvatarItems();
        }

        private void OnFilterMaleButtonClick()
        {
            FilterMaleActivated = !FilterMaleActivated;
            FilterFemaleActivated = false;

            ResetAvatarListContent();
            PopulateAvatarGalleryAvatarItems();
        }

        private void OnFilter10ButtonClick()
        {
            Filter10Activated = !Filter10Activated;
            Filter20Activated = false;
            Filter30Activated = false;
            Filter40Activated = false;
            Filter50Activated = false;

            ResetAvatarListContent();
            PopulateAvatarGalleryAvatarItems();
        }

        private void OnFilter20ButtonClick()
        {
            Filter10Activated = false;
            Filter20Activated = !Filter20Activated;
            Filter30Activated = false;
            Filter40Activated = false;
            Filter50Activated = false;

            ResetAvatarListContent();
            PopulateAvatarGalleryAvatarItems();
        }

        private void OnFilter30ButtonClick()
        {
            Filter10Activated = false;
            Filter20Activated = false;
            Filter30Activated = !Filter30Activated;
            Filter40Activated = false;
            Filter50Activated = false;

            ResetAvatarListContent();
            PopulateAvatarGalleryAvatarItems();
        }

        private void OnFilter40ButtonClick()
        {
            Filter10Activated = false;
            Filter20Activated = false;
            Filter30Activated = false;
            Filter40Activated = !Filter40Activated;
            Filter50Activated = false;

            ResetAvatarListContent();
            PopulateAvatarGalleryAvatarItems();
        }

        private void OnFilter50ButtonClick()
        {
            Filter10Activated = false;
            Filter20Activated = false;
            Filter30Activated = false;
            Filter40Activated = false;
            Filter50Activated = !Filter50Activated;

            ResetAvatarListContent();
            PopulateAvatarGalleryAvatarItems();
        }
        #endregion

        #region Selection
        private void OnSelectAvatarButtonClick()
        {
            if (selectedTarget == null)
                return;

            if (selectedAvatar == null)
                return;

            if (running)
                return;

            running = true;
            SelectAvatarButton.image.color = waitingButtonColor;


            StartCoroutine(ApplyAvatar());
        }

        private IEnumerator ApplyAvatar()
        {
            // Get the current avatar
            AvatarReady previousAvatarReady = selectedTarget.GetComponentInChildren<AvatarReady>();
            if (previousAvatarReady == null)
            {
                Debug.LogError("Cannot find AvatarReady on previous avatar");
                yield break;
            }

            // Download the next avatar if needed
            if (!AvatarCache.Contains(selectedAvatar))
            {
                yield return AvatarCache.Download(selectedAvatar);
            }

            yield return null;

            // Load the next avatar
            AssetBundle avatarAssetBundle = AvatarCache.Get(selectedAvatar);
            if (avatarAssetBundle == null)
            {
                Debug.LogError("No AssetBundle for " + selectedAvatar);
            }

            yield return null;

            // Look for avatar prefab
            GameObject[] objs = avatarAssetBundle.LoadAllAssets<GameObject>();
            if (objs.Length == 0)
            {
                Debug.LogError("No GameObject on the AssetBundle of " + selectedAvatar);
                yield break;
            }
            if (objs.Length > 1)
            {
                Debug.LogWarning("More than one GameObject on the AssetBundle of " + selectedAvatar + ", picking the first.");
            }
            GameObject avatarPrefab = objs.First();

            // SetActive(false) to delay Awake calls until the end of the setup
            avatarPrefab.SetActive(false);

            yield return null;

            // Call AvatarReadyRuntimeLink OnAvatarDestroy
            selectedTarget.GetComponent<AvatarReadyRuntimeLink>().OnAvatarDestroy.Invoke();

            yield return null;

            // Save old AvatarReady config
            string previousAvatarReadyConfig = JsonUtility.ToJson(previousAvatarReady);

            // Destroy previous avatar
            DestroyUtils.Destroy(previousAvatarReady.gameObject);

            yield return null;

            // Instantiate avatar
            GameObject avatarInstance = Instantiate(avatarPrefab, selectedTarget.transform);

            // Dispose AssetBundle
            avatarAssetBundle.Unload(false);

            // Fix Unity bug with Material/Shader and AssetBundle
            FixAssetBundleMaterials(avatarInstance);

            // add AvatarReady
            AvatarReady instanceAvatarReady = avatarInstance.AddComponent<AvatarReady>();

            // apply parameter from previous avatar
            JsonUtility.FromJsonOverwrite(previousAvatarReadyConfig, instanceAvatarReady);

            yield return null;

            // Setup AvatarReady
            instanceAvatarReady.SetupAvatar();
            instanceAvatarReady.activated = true;

            yield return null;

            avatarInstance.SetActive(true);

            yield return null;

            // Call AvatarReadyRuntimeLink
            selectedTarget.GetComponent<AvatarReadyRuntimeLink>().OnAvatarLoaded.Invoke(avatarInstance);

            yield return null;

            // Calibration
            CalibrationUi calibrationUi = FindObjectOfType<CalibrationUi>();
            if (calibrationUi != null)
            {
                calibrationUi.OnRefreshAvatarsDropdownButtonClick();
                if (AutomaticCalibrationToggle.isOn)
                {
                    if (calibrationUi.UserDropdown.value != 0)
                    {
                        calibrationUi.OnCalibrateUsingProfileButtonClick();
                    }
                }
            }

            // Allow for another avatar selection
            SelectAvatarButton.image.color = defaultButtonColor;
            running = false;
        }

        private void FixAssetBundleMaterials(GameObject obj)
        {
            // https://forum.unity.com/threads/hdrp-lit-material-on-exported-gameobjects-do-not-render-correctly.868558/
            // https://issuetracker.unity3d.com/issues/texture-maps-are-not-applicable-to-the-material-when-scene-is-loaded-from-asset-bundles

            foreach (Renderer r in obj.GetComponentsInChildren<Renderer>())
            {
                foreach (Material mat in r.materials)
                {
                    mat.shader = Shader.Find(mat.shader.name);
                }
            }
        }
        #endregion
    }
}