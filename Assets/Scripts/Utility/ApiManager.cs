using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApiManager : MonoBehaviour
{
    [Header("Api's")]
    public static ApiManager Instance { get; private set; }
    public UserApiClient userApiClient;
    public Environment2DApiClient environment2DApiClient;
    public Object2DApiClient object2DApiClient;

    [Header("Variables")]
    public Environment2D currentEnvironment;
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
            shouldEnvironmentBeLoaded = false;
        }
    }
}
