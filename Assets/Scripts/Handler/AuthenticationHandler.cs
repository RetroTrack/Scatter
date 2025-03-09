using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthenticationHandler : MonoBehaviour
{
    private User _user = new User();
    [SerializeField] private TMPro.TextMeshProUGUI _errorText;

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

    public async void Register()
    {
        IWebRequestReponse webRequestResponse = await ApiManager.Instance.UserApiClient.Register(_user);

        switch (webRequestResponse)
        {
            case WebRequestData<string>:
                SceneLoader.LoadScene("Login");
                break;
            case WebRequestError:
                SetErrorText("Email address already exists!");
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    public async void Login()
    {
        IWebRequestReponse webRequestResponse = await ApiManager.Instance.UserApiClient.Login(_user);

        switch (webRequestResponse)
        {
            case WebRequestData<string>:
                SceneLoader.LoadScene("MainMenu");
                break;
            case WebRequestError:
                SetErrorText("Email or password incorrect!");
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }
}