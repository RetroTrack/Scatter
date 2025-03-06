using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSelectButton : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI text;
    public string worldId;
    public void OnSelect()
    {
        EnvironmentSelectionHandler.Instance.SelectWorld(worldId, gameObject);
    }
}
