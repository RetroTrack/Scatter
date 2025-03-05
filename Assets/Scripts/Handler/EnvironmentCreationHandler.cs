using System;
using UnityEngine;

public class EnvironmentCreationHandler : MonoBehaviour
{
    public Environment2D environment2D;


    public void SetEnvironmentName(string name)
    {
        environment2D.name = name;
    }
    public void SetEnvironmentHeight(string height)
    {
        environment2D.maxHeight = int.Parse(height);
    }
    public void SetEnvironmentLength(string length)
    {
        environment2D.maxLength = int.Parse(length);
    }

    public async void CreateEnvironment2D()
    {
        IWebRequestReponse webRequestResponse = await ApiManager.Instance.environment2DApiClient.CreateEnvironment(environment2D);

        switch (webRequestResponse)
        {
            case WebRequestData<Environment2D> dataResponse:
                environment2D.id = dataResponse.Data.id;
                // TODO: Handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Create environment2D error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }
}
