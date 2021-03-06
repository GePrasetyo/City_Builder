using TriLibCore.General;
using CityBuilderCore;
using TriLibCore;
using UnityEngine;
using TriLibCore.Extensions;
using UnityEngine.UI;

public class ModelFileLoader : MonoBehaviour
{
    public static ModelFileLoader instance;

    /// <summary>
    /// The last loaded GameObject.
    /// </summary>
    public GameObject _loadedGameObject;
    [SerializeField] private GameObject panelLoadObject;
    [SerializeField] private ToolsActivator tools;
    [SerializeField] private BuildingBuilder builder;

    /// <summary>
    /// The load Model Button.
    /// </summary>
    [SerializeField] private Button _loadModelButton;
    [SerializeField] private Button _openPanel;

    /// <summary>
    /// The progress indicator Text;
    /// </summary>
    [SerializeField] private Text _progressText;
    [SerializeField] private InputField xSize, ySize;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        _loadModelButton.onClick.AddListener(LoadModel);

        _openPanel.onClick.AddListener(() => panelLoadObject.SetActive(!panelLoadObject.activeInHierarchy));
    }    

    /// <summary>
    /// Creates the AssetLoaderOptions instance and displays the Model file-picker.
    /// </summary>
    /// <remarks>
    /// You can create the AssetLoaderOptions by right clicking on the Assets Explorer and selecting "TriLib->Create->AssetLoaderOptions->Pre-Built AssetLoaderOptions".
    /// </remarks>
    public void LoadModel() {
        if (xSize.text == "" || ySize.text == "" || xSize.text == "0" || ySize.text == "0")
            return;

        panelLoadObject.gameObject.SetActive(false);

        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        var assetLoaderFilePicker = AssetLoaderFilePicker.Create();
        assetLoaderFilePicker.LoadModelFromFilePickerAsync("Select a Model file", OnLoad, OnMaterialsLoad, OnProgress, OnBeginLoad, OnError, null, assetLoaderOptions);
    }

    /// <summary>
    /// Called when the the Model begins to load.
    /// </summary>
    /// <param name="filesSelected">Indicates if any file has been selected.</param>
    private void OnBeginLoad(bool filesSelected) {
        _loadModelButton.interactable = !filesSelected;
        _progressText.enabled = filesSelected;
    }

    /// <summary>
    /// Called when any error occurs.
    /// </summary>
    /// <param name="obj">The contextualized error, containing the original exception and the context passed to the method where the error was thrown.</param>
    private void OnError(IContextualizedError obj) {
        Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
    }

    /// <summary>
    /// Called when the Model loading progress changes.
    /// </summary>
    /// <param name="assetLoaderContext">The context used to load the Model.</param>
    /// <param name="progress">The loading progress.</param>
    private void OnProgress(AssetLoaderContext assetLoaderContext, float progress) {
        _progressText.text = $"Progress: {progress:P}";
    }

    /// <summary>
    /// Called when the Model (including Textures and Materials) has been fully loaded.
    /// </summary>
    /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
    /// <param name="assetLoaderContext">The context used to load the Model.</param>
    private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext) {
        if (assetLoaderContext.RootGameObject != null) {
            Debug.Log("Model fully loaded.");
        }
        else {
            Debug.Log("Model could not be loaded.");
        }
        _loadModelButton.interactable = true;
        _progressText.enabled = false;
    }

    /// <summary>
    /// Called when the Model Meshes and hierarchy are loaded.
    /// </summary>
    /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
    /// <param name="assetLoaderContext">The context used to load the Model.</param>
    private void OnLoad(AssetLoaderContext assetLoaderContext) {
        _loadedGameObject = assetLoaderContext.RootGameObject;
        if (_loadedGameObject != null) {

            //cacheBuilding.cachedObject = _loadedGameObject;
            builder.BuildingInfo.Size = new Vector2Int(int.Parse(xSize.text), int.Parse(ySize.text));
            tools.SetToolActive(true);
        }
    }
}
