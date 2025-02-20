using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scatter.Helpers
{
    public class LibraryHelper
    {
        #region Structs and Enums
        //Structs for Library Objects
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
