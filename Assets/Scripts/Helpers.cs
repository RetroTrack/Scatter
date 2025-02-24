using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scatter.Helpers
{
    public class LibraryHelper
    {
        #region Methods

        public static void GenerateSpriteCollider(Sprite sprite, GameObject newObject)
        {
            //Fit collider to sprite vertices
            newObject.GetComponent<SpriteRenderer>().sprite = sprite;
            List<Vector2> spriteVertices = new List<Vector2>();
            sprite.GetPhysicsShape(0, spriteVertices);
            newObject.GetComponent<PolygonCollider2D>().points = spriteVertices.ToArray();
        }

        #endregion

        #region Structs and Enums
        [Serializable]
        public struct LibraryCategory
        {
            public string name;
            public LibraryObject[] libraryObjects;
        }

        [Serializable]
        public struct LibraryObject
        {
            public string name;
            public Sprite sprite;
            public GameObject prefab;
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
            Erase
        }
        #endregion
    }
}
