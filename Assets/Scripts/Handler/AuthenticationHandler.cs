using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthenticationHandler : MonoBehaviour
{
    private User _user = new User();
    [SerializeField] private TMPro.TextMeshProUGUI errorText;

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
        errorText.text = text;
        errorText.gameObject.SetActive(true);
    }

    public async void Register()
    {
        IWebRequestReponse webRequestResponse = await ApiManager.Instance.userApiClient.Register(_user);

        switch (webRequestResponse)
        {
            case WebRequestData<string>:
                ApiManager.LoadScene("Login");
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
        IWebRequestReponse webRequestResponse = await ApiManager.Instance.userApiClient.Login(_user);

        switch (webRequestResponse)
        {
            case WebRequestData<string>:
                ApiManager.LoadScene("MainMenu");
                break;
            case WebRequestError:
                SetErrorText("Email or password incorrect!");
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }
}