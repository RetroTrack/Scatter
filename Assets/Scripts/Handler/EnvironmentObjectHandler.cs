using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scatter.Helpers;
using Scatter.World;
using UnityEngine;

public class EnvironmentObjectHandler : MonoBehaviour
{
    public static EnvironmentObjectHandler Instance { get; private set; }
    [SerializeField] private Environment2D _environment2D;
    public List<PlayerObject> loadedPlayerObjects = new();
    [SerializeField] private List<string> _deletedObjects = new();
    [SerializeField] private GameObject _rightPanel;
    [SerializeField] private GameObject _saveButton;

    public void Awake()
    {
        Instance = this;
    }

    public void SetEnvironment(Environment2D environment)
    {
        _environment2D = environment;
        CameraController.Instance.ChangeEnvironmentSize(new Vector2(environment.maxLength, environment.maxHeight));
    }

    public void DisablePersonalFunctions()
    {
        _rightPanel.SetActive(false);
        _saveButton.SetActive(false);
    }

    public async void LoadObjectsInEnvironment()
    {
        //Request objects from database
        List<Object2D> object2Ds = await ReadObject2Ds(_environment2D);

        if (object2Ds == null)
        {
            Debug.Log("No objects found in environment!");
            return;
        }
        //Convert objects to player objects
        ObjectHelper.ConvertObject2DsToPlayerObjects(object2Ds);
    }

    public async void SaveObjectsInEnvironment()
    {
        //Save all objects in the environment
        foreach (PlayerObject playerObject in loadedPlayerObjects)
        {
            if (string.IsNullOrEmpty(playerObject.ObjectId))
            {
                Object2D object2D = ObjectHelper.ConvertPlayerObjectToObject2D(_environment2D, playerObject);
                object2D.environmentId = _environment2D.id;
                string objectId = await CreateObject2D(object2D);
                playerObject.ObjectId = objectId;
            }
            else
            {
                Object2D object2D = ObjectHelper.ConvertPlayerObjectToObject2D(_environment2D, playerObject);
                await UpdateObject2D(object2D);
            }
        }

        //Delete all objects that are marked for deletion
        foreach (string objectId in _deletedObjects)
        {
            Object2D object2D = new Object2D();
            object2D.id = objectId;
            object2D.environmentId = _environment2D.id;
            await DeleteObject2D(object2D);
        }
        _deletedObjects.Clear();
    }

    public void AddDestroyed(PlayerObject playerObject)
    {
        if (!string.IsNullOrEmpty(playerObject.ObjectId))
        {
            _deletedObjects.Add(playerObject.ObjectId);
        }
        loadedPlayerObjects.Remove(playerObject);
    }

    public void AddPlayerObject(PlayerObject playerObject)
    {
        loadedPlayerObjects.Add(playerObject);
        Debug.Log("Added player object with id: " + playerObject.ObjectId);
        Debug.Log("Currently loaded player objects count: " + loadedPlayerObjects.Count);
    }


    #region Requests
    public async Task<List<Object2D>> ReadObject2Ds(Environment2D environment)
    {
        if (string.IsNullOrWhiteSpace(environment.id))
        {
            Debug.Log("Environment id is null or empty!");
            return null;
        }
        IWebRequestReponse webRequestResponse = ApiManager.Instance.isCurrentEnvironmentShared ?
            await ApiManager.Instance.GuestApiClient.ReadObject2Ds(environment.id)
            : await ApiManager.Instance.Object2DApiClient.ReadObject2Ds(environment.id);

        switch (webRequestResponse)
        {
            case WebRequestData<List<Object2D>> dataResponse:
                List<Object2D> object2Ds = dataResponse.Data;
                return object2Ds;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Read object2Ds error: " + errorMessage);
                // TODO: Error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
        return null;
    }

    public async Task<string> CreateObject2D(Object2D object2D)
    {
        IWebRequestReponse webRequestResponse = await ApiManager.Instance.Object2DApiClient.CreateObject2D(object2D);

        switch (webRequestResponse)
        {
            case WebRequestData<Object2D> dataResponse:
                object2D.id = dataResponse.Data.id;
                return object2D.id;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Create Object2D error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
        return null;
    }

    public async Task<bool> UpdateObject2D(Object2D object2D)
    {
        IWebRequestReponse webRequestResponse = await ApiManager.Instance.Object2DApiClient.UpdateObject2D(object2D);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                string responseData = dataResponse.Data;
                return true;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Update object2D error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
        return false;
    }

    public async Task<bool> DeleteObject2D(Object2D object2D)
    {
        IWebRequestReponse webRequestResponse = await ApiManager.Instance.Object2DApiClient.DeleteObject2D(object2D);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                string responseData = dataResponse.Data;
                return true;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Delete object2D error: " + errorMessage + ", object id: " + object2D.id);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
        return false;
    }
    #endregion
}

[Serializable]
public struct Prefab2D
{
    public string id;
    public GameObject prefab;
}