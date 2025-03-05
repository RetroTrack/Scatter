using UnityEngine;

public class WorldSelectButton : MonoBehaviour
{
    public void OnSelect(int world)
    {
        EnvironmentSelectionHandler.Instance.SelectWorld(world);
    }
}
