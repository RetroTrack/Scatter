using System;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSelectionHandler : MonoBehaviour
{
    public static EnvironmentSelectionHandler Instance { get; private set; }

    public List<Environment2D> environments;
    public GameObject environmentButtonPrefab;
    public List<GameObject> environmentButtons;

    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        Instance = this;
    }

    public async void ReadEnvironment2Ds()
    {
        IWebRequestReponse webRequestResponse = await ApiManager.Instance.environment2DApiClient.ReadEnvironment2Ds();

        switch (webRequestResponse)
        {
            case WebRequestData<List<Environment2D>> dataResponse:
                List<Environment2D> environment2Ds = dataResponse.Data;
                environments = environment2Ds;
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

    public void SelectWorld(int world)
    {
        ApiManager.LoadScene("Game");
    }
}
