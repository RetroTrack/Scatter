using System;
using System.Collections.Generic;
using Scatter.Library;
using Scatter.World;
using UnityEngine;
using UnityEngine.UI;

namespace Scatter.Helpers
{
    public class LibraryHelper
    {
        #region Methods

        public static void GenerateSpriteCollider(GameObject newObject)
        {
            //Fit collider to sprite vertices
            List<Vector2> spriteVertices = new List<Vector2>();
            newObject.GetComponent<SpriteRenderer>().sprite.GetPhysicsShape(0, spriteVertices);
            newObject.GetComponent<PolygonCollider2D>().points = spriteVertices.ToArray();
        }

        #endregion

        #region Structs and Enums
        [Serializable]
        public struct LibraryCategory
        {
            public string name;
            public PlayerObject[] playerObjects;
        }

        #endregion
    }
    public class PointerHelper
    {
        #region Methods
        public static PointerMode StringToPointerMode(string mode)
        {
            return (PointerMode)Enum.Parse(typeof(PointerMode), mode);
        }
        #endregion

        #region Structs and Enums
        [Serializable]
        public struct PointerButton
        {
            public PointerMode pointerMode;
            public Button button;
        }

        [Serializable]
        public enum PointerMode
        {
            Place,
            Move,
            Scale,
            Rotate,
            Erase,
            Click
        }
        #endregion
    }

    public class ObjectHelper
    {
        #region Methods
        public static void ConvertObject2DsToPlayerObjects(List<Object2D> object2Ds)
        {
            //Instantiate player objects from object2D list
            List<PlayerObject> playerObjects = new List<PlayerObject>();
            foreach (Object2D object2D in object2Ds)
            {
                if (!LibraryManager.Instance.GetAllObjects().Exists(x => x.PrefabId.Equals(object2D.prefabId)))
                {
                    Debug.Log($"Prefab with id {object2D.prefabId} doesn't exist!");
                    continue;
                }
                PlayerObject playerObject = LibraryManager.Instance.GetAllObjects().Find(x => x.PrefabId == object2D.prefabId);
                GameObject newObject = LibraryManager.Instance.InstantiatePlayerObject(playerObject, false);
                newObject.transform.position = new Vector3(object2D.positionX, object2D.positionY, 0);
                newObject.transform.localScale = new Vector3(object2D.scaleX, object2D.scaleY, 1);
                newObject.transform.rotation = Quaternion.Euler(0, 0, object2D.rotationZ);
                newObject.GetComponent<SpriteRenderer>().sortingOrder = object2D.sortingLayer;
                newObject.GetComponent<PlayerObject>().ObjectId = object2D.id;
                newObject.GetComponent<PlayerObject>().PrefabId = object2D.prefabId;
                playerObjects.Add(newObject.GetComponent<PlayerObject>());
            }
            Debug.Log(playerObjects.Count);
        }

        public static List<Object2D> ConvertPlayerObjectsTo2DObjects(List<PlayerObject> playerObjects, Environment2D environment2D)
        {
            List<Object2D> object2Ds = new();
            foreach (PlayerObject playerObject in playerObjects)
            {
                Object2D object2D = ConvertPlayerObjectToObject2D(environment2D, playerObject);
                object2Ds.Add(object2D);
            }
            return object2Ds;
        }

        public static Object2D ConvertPlayerObjectToObject2D(Environment2D environment2D, PlayerObject playerObject)
        {
            return new()
            {
                id = playerObject.ObjectId,
                environmentId = environment2D.id,
                prefabId = playerObject.PrefabId,
                positionX = playerObject.transform.position.x,
                positionY = playerObject.transform.position.y,
                scaleX = playerObject.transform.localScale.x,
                scaleY = playerObject.transform.localScale.y,
                rotationZ = playerObject.transform.rotation.eulerAngles.z,
                sortingLayer = playerObject.GetComponent<SpriteRenderer>().sortingOrder
            };
        }
        #endregion
    }
}
