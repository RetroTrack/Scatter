using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentSelectionHandler : MonoBehaviour
{
    public static EnvironmentSelectionHandler Instance { get; private set; }

    private List<Environment2D> _environments = new List<Environment2D>();
    [Header("Buttons")]
    [SerializeField] private Transform _parent;
    [SerializeField] private GameObject _environmentButtonPrefab;
    private List<GameObject> _environmentButtons = new List<GameObject>();


    [Header("Delete Button")]
    [SerializeField] private Button _deleteButton;
    private bool _isDeletingDeletingModeEnabled = false;
    [SerializeField] private Color _deleteSelectedColor;
    [SerializeField] private Color _deleteUnselectedColor;

    [Header("Share Button")]
    [SerializeField] private Button _shareButton;
    private bool _isSharingModeEnabled = false;
    [SerializeField] private Color _shareSelectedColor;
    [SerializeField] private Color _shareUnselectedColor;

    [Header("Share Panel")]
    [SerializeField] private GameObject _sharePanel;
    [SerializeField] private TMP_InputField _shareInputField;
    private string _currentWorldId;
    [SerializeField] private TextMeshProUGUI _shareWorldText;

    [Header("View Mode")]
    [SerializeField] private TextMeshProUGUI _bottomButtonText;
    private bool _isViewingShared = false;

    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        Instance = this;
        ReadPersonalEnvironment2Ds();
        ApiManager.Instance.isCurrentEnvironmentShared = false;
    }
    public async void ReadSharedEnvironment2Ds()
    {

        IWebRequestReponse webRequestResponse = await ApiManager.Instance.GuestApiClient.ReadEnvironments();

        switch (webRequestResponse)
        {
            case WebRequestData<List<Environment2D>> dataResponse:
                List<Environment2D> environment2Ds = dataResponse.Data;
                _environments = environment2Ds;
                LoadWorlds();
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Read environment2Ds error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    public async void ReadPersonalEnvironment2Ds()
    {

        IWebRequestReponse webRequestResponse = await ApiManager.Instance.Environment2DApiClient.ReadEnvironment2Ds();

        switch (webRequestResponse)
        {
            case WebRequestData<List<Environment2D>> dataResponse:
                List<Environment2D> environment2Ds = dataResponse.Data;
                _environments = environment2Ds;
                LoadWorlds();
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Read environment2Ds error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    public async void DeleteEnvironment2D(string id)
    {
        IWebRequestReponse webRequestResponse = await ApiManager.Instance.Environment2DApiClient.DeleteEnvironment(id);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                string responseData = dataResponse.Data;
                // TODO: Handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Delete environment error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    public void ToggleDeleteMode()
    {
        if (_isSharingModeEnabled)
            ToggleShareMode();
        _isDeletingDeletingModeEnabled = !_isDeletingDeletingModeEnabled;
        _deleteButton.image.color = _isDeletingDeletingModeEnabled ? _deleteSelectedColor : _deleteUnselectedColor;
    }

    public void ToggleShareMode()
    {
        if (_isDeletingDeletingModeEnabled)
            ToggleDeleteMode();
        _isSharingModeEnabled = !_isSharingModeEnabled;
        _shareButton.image.color = _isSharingModeEnabled ? _shareSelectedColor : _shareUnselectedColor;
        _sharePanel.SetActive(false);
    }

    public void SelectWorld(string worldId, GameObject gameObject)
    {
        if (_isDeletingDeletingModeEnabled)
        {
            DeleteEnvironment2D(worldId);
            Destroy(gameObject);
            return;
        }

        if (_isSharingModeEnabled)
        {
            _sharePanel.SetActive(true);
            _currentWorldId = worldId;
            _shareWorldText.text = _environments.Find(environment => environment.id == worldId).name;
            return;
        }

        ApiManager.Instance.currentEnvironment = _environments.Find(environment => environment.id == worldId);
        ApiManager.Instance.shouldEnvironmentBeLoaded = true;
        if (_isViewingShared)
            ApiManager.Instance.isCurrentEnvironmentShared = true;
        SceneLoader.LoadScene("Game");
    }

    public async void ShareWorld()
    {
        _sharePanel.SetActive(false);
        string username = _shareInputField.text;
        await ApiManager.Instance.GuestApiClient.ShareEnvironment(_currentWorldId, username);
    }

    public void ToggleDisplayMode()
    {
        int childCount = _parent.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(_parent.GetChild(i).gameObject);
        }
        _isViewingShared = !_isViewingShared;
        _bottomButtonText.text = _isViewingShared ? "View My Worlds" : "View Shared Worlds";
        if (_isViewingShared)
        {
            ReadSharedEnvironment2Ds();
            _deleteButton.gameObject.SetActive(false);
            _shareButton.gameObject.SetActive(false);
            _sharePanel.SetActive(false);
            _isSharingModeEnabled = false;
            _isDeletingDeletingModeEnabled = false;
            _shareButton.image.color = _shareUnselectedColor;
            _deleteButton.image.color = _deleteUnselectedColor;
        }
        else
        {
            ReadPersonalEnvironment2Ds();
            _deleteButton.gameObject.SetActive(true);
            _shareButton.gameObject.SetActive(true);
        }
    }


    private void LoadWorlds()
    {
        foreach (Environment2D environment in _environments)
        {
            LoadWorldButton(environment);
        }
    }

    private void LoadWorldButton(Environment2D environment)
    {
        GameObject environmentButton = Instantiate(_environmentButtonPrefab, _parent);
        environmentButton.GetComponent<WorldSelectButton>().text.text = environment.name;
        environmentButton.GetComponent<WorldSelectButton>().worldId = environment.id;
        _environmentButtons.Add(environmentButton);
    }
}
