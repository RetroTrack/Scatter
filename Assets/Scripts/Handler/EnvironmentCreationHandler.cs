using System;
using TMPro;
using UnityEngine;

public class EnvironmentCreationHandler : MonoBehaviour
{
    [SerializeField] private Environment2D _environment2D;
    [SerializeField] private TMP_InputField _heightInput;
    [SerializeField] private TMP_InputField _lengthInput;

    public void SetEnvironmentName(string name)
    {
        _environment2D.name = name;
    }
    public void SetEnvironmentHeight(string height)
    {
        if (string.IsNullOrWhiteSpace(height))
        {
            _heightInput.text = "0";
            _environment2D.maxHeight = 10;
            return;
        }

        _environment2D.maxHeight = int.Parse(height);

        if (_environment2D.maxHeight > 100)
        {
            _environment2D.maxHeight = 100;
            _heightInput.text = "100";
        }
        else if (_environment2D.maxHeight < 10)
        {
            _environment2D.maxHeight = 10;
        }
    }
    public void SetEnvironmentLength(string length)
    {
        if (string.IsNullOrWhiteSpace(length))
        {
            _lengthInput.text = "0";
            _environment2D.maxHeight = 20;
            return;
        }

        _environment2D.maxLength = int.Parse(length);

        if (_environment2D.maxLength > 200)
        {
            _environment2D.maxLength = 200;
            _lengthInput.text = "200";
        }
        else if (_environment2D.maxLength < 20)
        {
            _environment2D.maxLength = 20;
        }
    }

    public async void CreateEnvironment2D()
    {
        IWebRequestReponse webRequestResponse = await ApiManager.Instance.Environment2DApiClient.CreateEnvironment(_environment2D);

        switch (webRequestResponse)
        {
            case WebRequestData<Environment2D> dataResponse:
                _environment2D.id = dataResponse.Data.id;
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
