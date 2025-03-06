using System;
using System.Collections.Generic;
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
    public Button deleteButton;
    public bool isDeleting = false;
    public Color selectedColor;
    public Color unselectedColor;

    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        Instance = this;
        ReadEnvironment2Ds();
    }

    public async void ReadEnvironment2Ds()
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
        isDeleting = !isDeleting;
        deleteButton.image.color = isDeleting ? selectedColor : unselectedColor;
    }

    public void SelectWorld(string worldId, GameObject gameObject)
    {
        if(isDeleting)
        {
            DeleteEnvironment2D(worldId);
            Destroy(gameObject);
            return;
        }

        ApiManager.Instance.currentEnvironment = environments.Find(environment => environment.id == worldId);
        ApiManager.Instance.shouldEnvironmentBeLoaded = true;
        SceneLoader.LoadScene("Game");
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
