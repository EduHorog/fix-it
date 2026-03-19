using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSwitch : MonoBehaviour
{
    [SerializeField] private Light2D light2D;  // это Point Light 2D на LightCircle

    private void Start()
    {
        // глобальный свет пусть остаётся включён (можно вообще не трогать!)
    }

    public void TurnOnFlashlight()
    {
        if (light2D != null)
            light2D.enabled = true;
    }

    public void TurnOffFlashlight()
    {
        if (light2D != null)
            light2D.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (light2D != null)
                light2D.enabled = !light2D.enabled;
        }
    }
}
