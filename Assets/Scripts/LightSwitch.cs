using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSwitch : MonoBehaviour
{
    [SerializeField] private Light2D lampLight;

    private void Start()
    {
        if (lampLight == null) lampLight = GetComponent<Light2D>();
        if (lampLight != null) lampLight.enabled = false;
    }

    public void ToggleLight()
    {
        if (lampLight != null)
        {
            lampLight.enabled = !lampLight.enabled;
        }
    }
}

