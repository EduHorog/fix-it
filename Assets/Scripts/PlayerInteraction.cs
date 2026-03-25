using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactRadius = 2f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            LightSwitch[] allLamps = FindObjectsOfType<LightSwitch>();
            
            LightSwitch nearestLamp = null;
            float nearestDistance = float.MaxValue;
            
            foreach (var lamp in allLamps)
            {
                float distance = Vector2.Distance(transform.position, lamp.transform.position);
                
                if (distance <= interactRadius && distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestLamp = lamp;
                }
            }
            
            if (nearestLamp != null)
            {
                nearestLamp.ToggleLight();
            }
        }
    }
}