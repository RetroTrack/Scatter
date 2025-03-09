using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentSelectionHandler : MonoBehaviour
{
    public static EnvironmentSelectionHandler Instance { get; private set; }

    public List<Environment2D> environments;
    [Header("Buttons")]
    public Transform parent;
    public GameObject environmentButtonPrefab;
    public List<GameObject> environmentButtons;


    [Header("Delete Button")]
    [SerializeField] private Button deleteButton;
    private bool isDeletingDeletingModeEnabled = false;
    [SerializeField] private Color deleteSelectedColor;
    [SerializeField] private Color deleteUnselectedColor;

    [Header("Share Button")]
    [SerializeField] private Button shareButton;
    private bool isSharingModeEnabled = false;
    [SerializeField] private Color shareSelectedColor;
    [SerializeField] private Color shareUnselectedColor;
    [Header("Share Panel")]
    [SerializeField] private GameObject sharePanel;
    [SerializeField] private TMP_InputField shareInputField;
    private string currentWorldId;
    [SerializeField] private TextMeshProUGUI shareWorldText;
    [Header("View Mode")]
    [SerializeField] private TextMeshProUGUI bottomButtonText;
    [SerializeField] private bool _isViewingShared = false;

    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        Instance = this;
        ReadPersonalEnvironment2Ds();
        ApiManager.Instance.isCurrentEnvironmentShared = false;
    }
    public async void ReadSharedEnvironment2Ds()
    {

        IWebRequestReponse webRequestResponse = await ApiManager.Instance.guestApiClient.ReadEnvironments();

        switch (webRequestResponse)
        {
            case WebRequestData<List<Environment2D>> dataResponse:
                List<Environment2D> environment2Ds = dataResponse.Data;
                environments = environment2Ds;
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

        IWebRequestReponse webRequestResponse = await ApiManager.Instance.environment2DApiClient.ReadEnvironment2Ds();

        switch (webRequestResponse)
        {
            case WebRequestData<List<Environment2D>> dataResponse:
                List<Environment2D> environment2Ds = dataResponse.Data;
                environments = environment2Ds;
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
        IWebRequestReponse webRequestResponse = await ApiManager.Instance.environment2DApiClient.DeleteEnvironment(id);

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
        if (isSharingModeEnabled)
            ToggleShareMode();
        isDeletingDeletingModeEnabled = !isDeletingDeletingModeEnabled;
        deleteButton.image.color = isDeletingDeletingModeEnabled ? deleteSelectedColor : deleteUnselectedColor;
    }

    public void ToggleShareMode()
    {
        if (isDeletingDeletingModeEnabled)
            ToggleDeleteMode();
        isSharingModeEnabled = !isSharingModeEnabled;
        shareButton.image.color = isSharingModeEnabled ? shareSelectedColor : shareUnselectedColor;
        sharePanel.SetActive(false);
    }

    public void SelectWorld(string worldId, GameObject gameObject)
    {
        if (isDeletingDeletingModeEnabled)
        {
            DeleteEnvironment2D(worldId);
            Destroy(gameObject);
            return;
        }

        if (isSharingModeEnabled)
        {
            sharePanel.SetActive(true);
            currentWorldId = worldId;
            shareWorldText.text = environments.Find(environment => environment.id == worldId).name;
            return;
        }

        ApiManager.Instance.currentEnvironment = environments.Find(environment => environment.id == worldId);
        ApiManager.Instance.shouldEnvironmentBeLoaded = true;
        if (_isViewingShared)
            ApiManager.Instance.isCurrentEnvironmentShared = true;
        SceneLoader.LoadScene("Game");
    }

    public async void ShareWorld()
    {
        sharePanel.SetActive(false);
        string username = shareInputField.text;
        await ApiManager.Instance.guestApiClient.ShareEnvironment(currentWorldId, username);
    }

    public void ToggleDisplayMode()
    {
        int childCount = parent.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
        _isViewingShared = !_isViewingShared;
        bottomButtonText.text = _isViewingShared ? "View My Worlds" : "View Shared Worlds";
        if (_isViewingShared)
        {
            ReadSharedEnvironment2Ds();
        }
        else
        {
            ReadPersonalEnvironment2Ds();
        }
    }


    private void LoadWorlds()
    {
        foreach (Environment2D environment in environments)
        {
            LoadWorldButton(environment);
        }
    }

    private void LoadWorldButton(Environment2D environment)
    {
        GameObject environmentButton = Instantiate(environmentButtonPrefab, parent);
        environmentButton.GetComponent<WorldSelectButton>().text.text = environment.name;
        environmentButton.GetComponent<WorldSelectButton>().worldId = environment.id;
        environmentButtons.Add(environmentButton);
    }
}
