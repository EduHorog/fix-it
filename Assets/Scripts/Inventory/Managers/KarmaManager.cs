using UnityEngine;
using System;

public class KarmaManager : MonoBehaviour
{
    public static KarmaManager Instance { get; private set; }

    [Header("Настройки кармы")]
    [SerializeField] private int startingKarma = 0;
    [SerializeField] private int positiveThreshold = 10;   // Для хорошей концовки
    [SerializeField] private int negativeThreshold = -10;  // Для плохой концовки
    [SerializeField] private int karmaClamp = 100;         // Ограничение [-100; 100]

    // События для UI и других систем
    public event Action<int> OnKarmaChanged;
    public event Action<KarmaLevel> OnKarmaLevelChanged;
    public event Action<bool> OnCorrectChoiceMade; // Для подсчёта "правильных ответов"

    public int CurrentKarma { get; private set; }
    public KarmaLevel CurrentLevel { get; private set; }
    public int CorrectChoicesCount { get; private set; } // ✅ Счётчик правильных выборов

    public enum KarmaLevel { Neutral, Positive, Negative }
    public enum EndingType { Good, Neutral, Bad }

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
            return;
        }

        CurrentKarma = startingKarma;
        UpdateKarmaLevel();
    }

    /// <summary>
    /// Добавить карму и/или засчитать правильный выбор
    /// </summary>
    public void AddKarma(int amount, bool isCorrect = false)
    {
        CurrentKarma = Mathf.Clamp(CurrentKarma + amount, -karmaClamp, karmaClamp);
        
        if (isCorrect)
        {
            CorrectChoicesCount++;
            OnCorrectChoiceMade?.Invoke(true);
        }
        
        UpdateKarmaLevel();
        OnKarmaChanged?.Invoke(CurrentKarma);
        
        Debug.Log($"[Karma] {amount:+#;-#;0} | Всего: {CurrentKarma} | Правильных: {CorrectChoicesCount}");
    }

    public void SetKarma(int value)
    {
        CurrentKarma = Mathf.Clamp(value, -karmaClamp, karmaClamp);
        UpdateKarmaLevel();
        OnKarmaChanged?.Invoke(CurrentKarma);
    }

    public void ResetKarma()
    {
        CurrentKarma = startingKarma;
        CorrectChoicesCount = 0;
        UpdateKarmaLevel();
        OnKarmaChanged?.Invoke(CurrentKarma);
    }

    // ✅ Проверки для концовок
    public bool CanGetGoodEnding() => CurrentKarma >= positiveThreshold;
    public bool CanGetBadEnding() => CurrentKarma <= negativeThreshold;
    public EndingType GetEndingType()
    {
        if (CurrentKarma >= positiveThreshold) return EndingType.Good;
        if (CurrentKarma <= negativeThreshold) return EndingType.Bad;
        return EndingType.Neutral;
    }

    private void UpdateKarmaLevel()
    {
        KarmaLevel newLevel = CurrentKarma >= positiveThreshold ? KarmaLevel.Positive :
                              CurrentKarma <= negativeThreshold ? KarmaLevel.Negative :
                              KarmaLevel.Neutral;

        if (newLevel != CurrentLevel)
        {
            CurrentLevel = newLevel;
            OnKarmaLevelChanged?.Invoke(CurrentLevel);
        }
    }

    // ✅ Для отладки в инспекторе
    private void OnValidate()
    {
        if (positiveThreshold < 0) positiveThreshold = 0;
        if (negativeThreshold > 0) negativeThreshold = 0;
    }
}