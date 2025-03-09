using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApiManager : MonoBehaviour
{
    [Header("Api's")]
    public static ApiManager Instance { get; private set; }
    [field:SerializeField] public UserApiClient UserApiClient { get; private set; }
    [field: SerializeField] public Environment2DApiClient Environment2DApiClient { get; private set; }
    [field: SerializeField] public Object2DApiClient Object2DApiClient { get; private set; }
    [field: SerializeField] public GuestApiClient GuestApiClient { get; private set; }

    [Header("Variables")]
    public Environment2D currentEnvironment;
    public bool isCurrentEnvironmentShared = false;
    public bool shouldEnvironmentBeLoaded = false;

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.name.Equals("Game") && currentEnvironment != null && shouldEnvironmentBeLoaded)
        {
            EnvironmentObjectHandler.Instance.SetEnvironment(currentEnvironment);
            EnvironmentObjectHandler.Instance.LoadObjectsInEnvironment();
            if(isCurrentEnvironmentShared) 
                EnvironmentObjectHandler.Instance.DisablePersonalFunctions();
            shouldEnvironmentBeLoaded = false;
        }
    }
}
