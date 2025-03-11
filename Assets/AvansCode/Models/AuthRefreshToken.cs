using System;

[Serializable]
public class AuthRefreshToken
{
    public string RefreshToken;

    public AuthRefreshToken(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}