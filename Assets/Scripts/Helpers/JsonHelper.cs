using System;
using System.Collections.Generic;
using Scatter.Api.Clients;
using UnityEngine;
namespace Scatter.Helpers
{
    public static class JsonHelper
    {
        public static List<T> ParseJsonArray<T>(string jsonArray)
        {
            string extendedJson = "{\"list\":" + jsonArray + "}";
            JsonList<T> parsedList = JsonUtility.FromJson<JsonList<T>>(extendedJson);
            return parsedList.list;
        }

        public static string ExtractToken(string data)
        {
            Token token = JsonUtility.FromJson<Token>(data);
            return token.accessToken;
        }
        public static string ExtractRefreshToken(string data)
        {
            Token token = JsonUtility.FromJson<Token>(data);
            return token.refreshToken;
        }
    }

    [Serializable]
    public class JsonList<T>
    {
        public List<T> list;
    }
}