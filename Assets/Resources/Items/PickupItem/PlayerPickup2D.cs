using UnityEngine;

public class PlayerPickup2D : MonoBehaviour
{
    public float pickupRange = 2f;
    public LayerMask itemLayer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryPickup();
        }
    }

    void TryPickup()
    {
        // Луч в сторону движения игрока
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, pickupRange, itemLayer);
        
        // Если игрок смотрит влево, луч тоже должен смотреть влево
        if (transform.localScale.x < 0)
        {
            hit = Physics2D.Raycast(transform.position, -transform.right, pickupRange, itemLayer);
        }

        if (hit.collider != null)
        {
            PickupItem2D pickupItem = hit.collider.GetComponent<PickupItem2D>();
            if (pickupItem != null)
            {
                if (InventoryManager.Instance.AddItem(pickupItem.itemData))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }

        // Отладка луча (видна в игре)
        Debug.DrawRay(transform.position, transform.right * pickupRange, Color.red, 0.1f);
    }
}