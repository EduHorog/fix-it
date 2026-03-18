using UnityEngine;
using System.Collections;

public class MirrorShadowEvent : MonoBehaviour
{
    [Header("Настройки тени")]
    public GameObject shadowPrefab;
    public Transform[] spawnPoints;
    
    [Header("Тайминг")]
    [Tooltip("Сколько тень видна на экране")]
    public float shadowVisibleTime = 0.5f;
    
    [Tooltip("Как часто тень появляется (интервал между появлениями)")]
    public float spawnInterval = 3f; // Каждые 3 секунды
    
    [Header("Радиус активации")]
    [Tooltip("Тень появляется только если игрок в этом радиусе")]
    public float activationRadius = 5f;
    
    [Header("Аудио")]
    public AudioClip shadowSound;

    private Transform player;
    private bool isMirrorCursed = false; // ✅ Флаг: зеркало "активировано"
    private Coroutine shadowLoop;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    /// <summary>
    /// Вызывается после использования ткани — запускает цикл появления тени
    /// </summary>
    public void TriggerShadowEvent()
    {
        if (isMirrorCursed) return; // Уже активировано
        
        isMirrorCursed = true;
        Debug.Log("🪞 Зеркало активировано — тень начинает появляться периодически");
        
        // Запускаем цикл появления тени
        shadowLoop = StartCoroutine(ShadowLoop());
    }

    /// <summary>
    /// Останавливает появление тени (если нужно починить зеркало клеем)
    /// </summary>
    public void StopShadowEvent()
    {
        if (shadowLoop != null)
            StopCoroutine(shadowLoop);
        
        isMirrorCursed = false;
        Debug.Log("🪞 Зеркало деактивировано — тень больше не появляется");
    }

    private IEnumerator ShadowLoop()
    {
        while (isMirrorCursed)
        {
            // ✅ Проверяем: игрок в радиусе?
            if (player != null)
            {
                float distance = Vector2.Distance(transform.position, player.position);
                
                if (distance <= activationRadius)
                {
                    // Игрок рядом — показываем тень
                    SpawnShadowOnce();
                }
            }
            
            // Ждём интервал перед следующей попыткой
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnShadowOnce()
    {
        if (spawnPoints.Length == 0 || shadowPrefab == null) return;

        // Выбираем случайную точку
        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject shadow = Instantiate(shadowPrefab, randomPoint.position, Quaternion.identity);
        
        // Настраиваем спрайт тени
        SpriteRenderer shadowSprite = shadow.GetComponent<SpriteRenderer>();
        if (shadowSprite != null)
        {
            Color c = shadowSprite.color;
            c.a = 0.8f;
            shadowSprite.color = c;
        }

        // Звук (опционально)
        if (shadowSound != null)
            AudioSource.PlayClipAtPoint(shadowSound, randomPoint.position);

        // ✅ Уничтожаем тень через 0.5 секунды
        Destroy(shadow, shadowVisibleTime);
    }

    // ✅ Отладка: визуализация в редакторе
    private void OnDrawGizmosSelected()
    {
        // Радиус активации
        Gizmos.color = isMirrorCursed ? new Color(1f, 0.2f, 0.2f, 0.4f) : new Color(1f, 0.8f, 0.2f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, activationRadius);
        
        // Точки спавна
        Gizmos.color = Color.red;
        foreach (var point in spawnPoints)
        {
            if (point != null)
                Gizmos.DrawSphere(point.position, 0.15f);
        }
    }
}