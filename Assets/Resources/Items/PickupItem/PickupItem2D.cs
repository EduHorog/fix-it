using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PickupItem2D : MonoBehaviour
{
    [Header("Item Data")]
    public ItemData itemData;

    [Header("Animation Settings")]
    public float rotationSpeed = 50f;   // Скорость вращения вокруг Z
    public float floatSpeed = 1f;       // Скорость покачивания
    public float floatHeight = 0.3f;    // Амплитуда покачивания

    [Header("Visual References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector3 startPos;
    private bool isInitialized = false;

    private void Awake()
    {
        // Автопоиск SpriteRenderer, если не назначен
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Initialize();
    }

    private void OnValidate()
    {
        // Обновление визуала в редакторе при изменении itemData
        if (!Application.isPlaying)
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
            ApplyItemVisuals();
        }
    }

    private void Initialize()
    {
        if (isInitialized) return;
        
        startPos = transform.position;
        ApplyItemVisuals();
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        // Вращение в 2D (вокруг оси Z)
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        
        // Плавное покачивание вверх-вниз
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = startPos + Vector3.up * yOffset;
    }

    /// <summary>
    /// Применяет визуальные настройки из ItemData к спрайту
    /// </summary>
    public void ApplyItemVisuals()
    {
        if (itemData == null || spriteRenderer == null) 
            return;

        // Применяем иконку предмета
        if (itemData.icon != null)
        {
            spriteRenderer.sprite = itemData.icon;
        }

        // Опционально: дополнительные визуальные настройки
        // spriteRenderer.color = itemData.tintColor;
        // transform.localScale = Vector3.one * itemData.displayScale;
    }

    /// <summary>
    /// Динамическая смена предмета (если префаб переиспользуется)
    /// </summary>
    public void SetItemData(ItemData newData)
    {
        itemData = newData;
        ApplyItemVisuals();
    }

    /// <summary>
    /// Сброс позиции при повторном использовании (для объектного пула)
    /// </summary>
    public void ResetPosition(Vector3 newPosition)
    {
        startPos = newPosition;
        transform.position = newPosition;
    }
}