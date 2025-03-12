using Scatter.Handler;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Scatter.Canvas
{
    public class WorldSelectButton : MonoBehaviour
    {
        public Image image;
        public TextMeshProUGUI text;
        public string worldId;

        // OnSelect is called when the button is clicked
        public void OnSelect()
        {
            gameObject.GetComponent<Button>().interactable = false;
            EnvironmentSelectionHandler.Instance.SelectWorld(worldId, gameObject);
        }
    }
}