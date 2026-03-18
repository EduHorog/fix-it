using UnityEngine;

public class DebugSpawner : MonoBehaviour
{
    public ItemData[] itemsToSpawn;

    void Update()
    {
        for (int i = 0; i < itemsToSpawn.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (itemsToSpawn[i] != null)
                {
                    InventoryManager.Instance.AddItem(itemsToSpawn[i]);
                }
            }
        }
    }
}