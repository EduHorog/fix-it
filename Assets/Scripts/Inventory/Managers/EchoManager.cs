using UnityEngine;

public class EchoManager : MonoBehaviour {
    public static EchoManager Instance;
    public int currentEcho = 0;
    
    [Header("Atmosphere")]
    public Light globalLight;
    public float minLight = 0.2f;
    public float maxLight = 1.0f;
    
    [Header("Thresholds")]
    public int goodEndingThreshold = 0;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddEcho(int value) {
        currentEcho += value;
        Debug.Log($"Эхо: {currentEcho}");
        UpdateAtmosphere();
    }

    void UpdateAtmosphere() {
        if (globalLight != null) {
            float intensity = Mathf.Lerp(minLight, maxLight, 
                Mathf.InverseLerp(-50, 50, currentEcho));
            globalLight.intensity = intensity;
        }
    }

    public void CheckEnding() {
        if (currentEcho >= goodEndingThreshold) {
            UIManager.Instance.ShowEnding("good");
        } else {
            UIManager.Instance.ShowEnding("bad");
        }
    }
}