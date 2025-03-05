using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApiManager : MonoBehaviour
{
    [Header("Api's")]
    public static ApiManager Instance { get; private set; }
    public UserApiClient userApiClient;
    public Environment2DApiClient environment2DApiClient;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public static void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public static void Quit()
    {
        Application.Quit();
    }
}
