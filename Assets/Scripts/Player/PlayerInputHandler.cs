using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    void Update()
    {
        // Исправлено: Alpha1 = слот 0, Alpha2 = слот 1, и т.д.
        if (Input.GetKeyDown(KeyCode.Alpha1)) InventoryManager.Instance.SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) InventoryManager.Instance.SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) InventoryManager.Instance.SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) InventoryManager.Instance.SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) InventoryManager.Instance.SelectSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) InventoryManager.Instance.SelectSlot(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) InventoryManager.Instance.SelectSlot(6);

        

        if (Input.GetKeyDown(KeyCode.Q))
        {
            InventoryManager.Instance.DropItem();
        }
    }
}