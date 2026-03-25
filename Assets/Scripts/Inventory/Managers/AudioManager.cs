using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;
    
    [Header("Settings")]
    public AudioSource sfxSource;
    public AudioClip[] soundEffects; // Перетащи сюда звуки из проекта
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Если источник не назначен, создаем новый
            if (sfxSource == null) {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
        } else {
            Destroy(gameObject);
        }
    }

    // Проиграть звук по имени (ищет по названию клипа)
    public void PlaySound(string soundName) {
        foreach (AudioClip clip in soundEffects) {
            if (clip != null && clip.name.Contains(soundName)) {
                sfxSource.PlayOneShot(clip);
                return;
            }
        }
        // Если звук не найден - не страшно, просто пишем в лог
        Debug.LogWarning($"[Audio] Звук '{soundName}' не найден!");
    }
    
    // Проиграть случайный звук (для шагов, ударов)
    public void PlayRandomSound(AudioClip[] clips) {
        if (clips.Length > 0) {
            sfxSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
    }
}