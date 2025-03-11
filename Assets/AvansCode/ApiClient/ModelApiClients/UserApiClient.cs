using System;
using UnityEngine;

public class UserApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> Register(User user)
    {
        string route = "/account/register";
        string data = JsonUtility.ToJson(user);

        return await webClient.SendPostRequest(route, data);
    }

    public async Awaitable<IWebRequestReponse> Login(User user)
    {
        string route = "/account/login";
        string data = JsonUtility.ToJson(user);

        IWebRequestReponse response = await webClient.SendPostRequest(route, data);
        return ProcessLoginResponse(response);
    }

    public async Awaitable<IWebRequestReponse> RefreshToken(string refreshToken)
    {
        string route = "/account/refresh";
        AuthRefreshToken token = new AuthRefreshToken(refreshToken);
        string data = JsonUtility.ToJson(token);

        IWebRequestReponse response = await webClient.SendPostRequest(route, data);
        return ProcessLoginResponse(response);
    }


    private IWebRequestReponse ProcessLoginResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                string token = JsonHelper.ExtractToken(data.Data);
                webClient.SetToken(token);
                ApiManager.Instance.SetRefreshToken(JsonHelper.ExtractRefreshToken(data.Data));
                return new WebRequestData<string>("Succes");
            default:
                return webRequestResponse;
        }
    }

    
}

