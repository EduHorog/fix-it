using UnityEngine;

public class PickupItem2D : MonoBehaviour
{
    public ItemData itemData;
    public float rotationSpeed = 50f;
    public float floatSpeed = 1f;
    public float floatHeight = 0.3f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Вращение в 2D (вокруг Z)
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        
        // Покачивание вверх-вниз
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatHeight;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (InventoryManager.Instance.AddItem(itemData))
            {
                Destroy(gameObject);
            }
        }
    }
}