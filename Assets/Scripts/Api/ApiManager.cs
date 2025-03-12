using System;
using Scatter.Api.Clients;
using Scatter.Api.Models;
using Scatter.Handler;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scatter.Api
{
    public class ApiManager : MonoBehaviour
    {
        [Header("Api's")]
        public static ApiManager Instance { get; private set; }
        [field: SerializeField] public UserApiClient UserApiClient { get; private set; }
        [field: SerializeField] public Environment2DApiClient Environment2DApiClient { get; private set; }
        [field: SerializeField] public Object2DApiClient Object2DApiClient { get; private set; }
        [field: SerializeField] public GuestApiClient GuestApiClient { get; private set; }

        [Header("Variables")]
        public Environment2D CurrentEnvironment { get; set; }
        public bool IsCurrentEnvironmentShared { get; set; } = false;
        public bool ShouldEnvironmentBeLoaded { get; set; } = false;

        private string _refreshToken;
        private DateTime _timeLastRefreshToken;

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance.gameObject);
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void SetRefreshToken(string refreshToken)
        {
            _refreshToken = refreshToken;
            _timeLastRefreshToken = DateTime.Now;
        }

        public void Update()
        {
            // Auto refresh token every 59 minutes
            if (_refreshToken != null && _timeLastRefreshToken != null)
            {
                if (DateTime.Now.Subtract(_timeLastRefreshToken).TotalMinutes >= 59)
                {
                    _ = UserApiClient.RefreshToken(_refreshToken);
                    _timeLastRefreshToken = DateTime.Now;
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            // Load environment objects in the game scene
            if (scene.name.Equals("Game") && CurrentEnvironment != null && ShouldEnvironmentBeLoaded)
            {
                EnvironmentObjectHandler.Instance.SetEnvironment(CurrentEnvironment);
                EnvironmentObjectHandler.Instance.LoadObjectsInEnvironment();
                if (IsCurrentEnvironmentShared)
                    EnvironmentObjectHandler.Instance.DisablePersonalFunctions();
                ShouldEnvironmentBeLoaded = false;
            }
        }
    }
}