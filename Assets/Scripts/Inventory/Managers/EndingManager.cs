using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance { get; private set; }

    [Header("Сцены концовок")]
    [SerializeField] private string goodEndingScene = "Ending_Good";
    [SerializeField] private string neutralEndingScene = "Ending_Neutral";
    [SerializeField] private string badEndingScene = "Ending_Bad";

    [Header("Настройки")]
    [SerializeField] private bool autoTriggerOnLevelEnd = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TriggerEnding()
    {
        // 🔹 Безопасная проверка
        if (KarmaManager.Instance == null)
        {
            Debug.LogWarning("[Ending] KarmaManager не найден! Загружаем нейтральную концовку.");
            LoadScene(neutralEndingScene);
            return;
        }

        var endingType = KarmaManager.Instance.GetEndingType();
        
        switch (endingType)
        {
            case KarmaManager.EndingType.Good:
                Debug.Log("[Ending] 🕊️ ХОРОШАЯ концовка");
                LoadScene(goodEndingScene);
                break;
            case KarmaManager.EndingType.Bad:
                Debug.Log("[Ending] 💀 ПЛОХАЯ концовка");
                LoadScene(badEndingScene);
                break;
            default:
                Debug.Log("[Ending] ⚖️ НЕЙТРАЛЬНАЯ концовка");
                LoadScene(neutralEndingScene);
                break;
        }
    }

    public void ForceEnding(KarmaManager.EndingType type)
    {
        switch (type)
        {
            case KarmaManager.EndingType.Good: LoadScene(goodEndingScene); break;
            case KarmaManager.EndingType.Bad: LoadScene(badEndingScene); break;
            default: LoadScene(neutralEndingScene); break;
        }
    }

    private void LoadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).IsValid())
            SceneManager.LoadScene(sceneName);
        else
        {
            Debug.LogWarning($"[Ending] Сцена '{sceneName}' не найдена!");
            SceneManager.LoadScene(0);
        }
    }

    private void OnEnable()
    {
        if (autoTriggerOnLevelEnd)
        {
            // Пример: LevelManager.OnLevelComplete += TriggerEnding;
        }
    }
}