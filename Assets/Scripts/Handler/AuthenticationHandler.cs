using System;
using Scatter.Api;
using Scatter.Api.Models;
using Scatter.Api.Responses;
using Scatter.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Scatter.Handler
{
    public class AuthenticationHandler : MonoBehaviour
    {
        private User _user = new User();
        [SerializeField] private TMPro.TextMeshProUGUI _errorText;
        [SerializeField] private Button _confirmButton;

        #region Variable Setters
        public void SetUserMail(string mail)
        {
            _user.Email = mail;
        }
        public void SetUserPassword(string password)
        {
            _user.Password = password;
        }
        public void SetErrorText(string text)
        {
            _errorText.text = text;
            _errorText.gameObject.SetActive(true);
        }
        #endregion

        #region WebRequests

        public async void Register()
        {
            _confirmButton.interactable = false;
            IWebRequestReponse webRequestResponse = await ApiManager.Instance.UserApiClient.Register(_user);

            switch (webRequestResponse)
            {
                case WebRequestData<string>:
                    SceneLoader.LoadScene("Login");
                    break;
                case WebRequestError:
                    SetErrorText("Email address already exists!");
                    _confirmButton.interactable = true;
                    break;
                default:
                    throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
            }
        }

        public async void Login()
        {
            _confirmButton.interactable = false;
            IWebRequestReponse webRequestResponse = await ApiManager.Instance.UserApiClient.Login(_user);

            switch (webRequestResponse)
            {
                case WebRequestData<string>:
                    SceneLoader.LoadScene("MainMenu");
                    break;
                case WebRequestError:
                    SetErrorText("Email or password incorrect!");
                    _confirmButton.interactable = true;
                    break;
                default:
                    throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
            }
        }
        #endregion
    }
}