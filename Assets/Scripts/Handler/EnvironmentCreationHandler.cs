using System;
using Scatter.Api;
using Scatter.Api.Models;
using Scatter.Api.Responses;
using Scatter.Helpers;
using TMPro;
using UnityEngine;

namespace Scatter.Handler
{
    public class EnvironmentCreationHandler : MonoBehaviour
    {
        [field: SerializeField] public Environment2D Environment2D { get; private set; }
        [SerializeField] private TMP_InputField _heightInput;
        [SerializeField] private TMP_InputField _lengthInput;
        [SerializeField] private TMPro.TextMeshProUGUI _errorText;

        #region Variable Setters
        public void SetErrorText(string text)
        {
            _errorText.text = text;
            _errorText.gameObject.SetActive(true);
        }

        public void SetEnvironmentName(string name)
        {
            Environment2D.name = name;
        }
        public void SetEnvironmentHeight(string height)
        {
            if (string.IsNullOrWhiteSpace(height))
            {
                if(_heightInput != null)
                    _heightInput.text = "0";
                Environment2D.maxHeight = 10;
                return;
            }

            Environment2D.maxHeight = int.Parse(height);

            if (Environment2D.maxHeight > 100)
            {
                Environment2D.maxHeight = 100;
                if (_heightInput != null)
                    _heightInput.text = "100";
            }
            else if (Environment2D.maxHeight < 10)
            {
                Environment2D.maxHeight = 10;
            }
        }
        public void SetEnvironmentLength(string length)
        {
            if (string.IsNullOrWhiteSpace(length))
            {
                if (_heightInput != null)
                    _lengthInput.text = "0";
                Environment2D.maxHeight = 20;
                return;
            }

            Environment2D.maxLength = int.Parse(length);

            if (Environment2D.maxLength > 200)
            {
                Environment2D.maxLength = 200;
                if (_heightInput != null)
                    _lengthInput.text = "200";
            }
            else if (Environment2D.maxLength < 20)
            {
                Environment2D.maxLength = 20;
            }
        }
        #endregion

        #region WebRequests
        public async void CreateEnvironment2D()
        {
            IWebRequestReponse webRequestResponse = await ApiManager.Instance.Environment2DApiClient.CreateEnvironment(Environment2D);

            switch (webRequestResponse)
            {
                case WebRequestData<Environment2D> dataResponse:
                    Environment2D.id = dataResponse.Data.id;
                    SceneLoader.LoadScene("Worlds");
                    break;
                case WebRequestError errorResponse:
                    string errorMessage = errorResponse.ErrorMessage;
                    SetErrorText("Couldn't create world, are you sure that the values are correct and the name is original?");
                    break;
                default:
                    throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
            }
        }
        #endregion
    }
}
