using System;
using TMPro;
using UnityEngine;

public class EnvironmentCreationHandler : MonoBehaviour
{
    public Environment2D environment2D;
    public TMP_InputField heightInput;
    public TMP_InputField lengthInput;

    public void SetEnvironmentName(string name)
    {
        environment2D.name = name;
    }
    public void SetEnvironmentHeight(string height)
    {
        if (string.IsNullOrWhiteSpace(height))
        {
            heightInput.text = "0";
            environment2D.maxHeight = 10;
            return;
        }

        environment2D.maxHeight = int.Parse(height);

        if (environment2D.maxHeight > 100)
        {
            environment2D.maxHeight = 100;
            heightInput.text = "100";
        }
        else if (environment2D.maxHeight < 10)
        {
            environment2D.maxHeight = 10;
        }
    }
    public void SetEnvironmentLength(string length)
    {
        if (string.IsNullOrWhiteSpace(length))
        {
            lengthInput.text = "0";
            environment2D.maxHeight = 20;
            return;
        }

        environment2D.maxLength = int.Parse(length);

        if (environment2D.maxLength > 200)
        {
            environment2D.maxLength = 200;
            lengthInput.text = "200";
        }
        else if (environment2D.maxLength < 20)
        {
            environment2D.maxLength = 20;
        }
    }

    public async void CreateEnvironment2D()
    {
        IWebRequestReponse webRequestResponse = await ApiManager.Instance.environment2DApiClient.CreateEnvironment(environment2D);

        switch (webRequestResponse)
        {
            case WebRequestData<Environment2D> dataResponse:
                environment2D.id = dataResponse.Data.id;
                SceneLoader.LoadScene("Worlds");
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
