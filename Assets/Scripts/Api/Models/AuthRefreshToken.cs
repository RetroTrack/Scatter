using System;
namespace Scatter.Api.Models
{
    [Serializable]
    public class AuthRefreshToken
    {
        public string RefreshToken;

        public AuthRefreshToken(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}