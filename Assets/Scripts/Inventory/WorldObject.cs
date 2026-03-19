
// using UnityEngine;
// using System.Collections.Generic;
// using UnityEngine.Events;

// public class WorldObject : MonoBehaviour
// {
//     [Header("Info")]
//     public string objectName;
//     public string interactionPrompt = "Нажмите [E] для взаимодействия";
    
//     [Header("Item Actions")]
//     [SerializeField] private List<ItemInteraction> interactions = new();
    
//     [Header("Visuals")]
//     [SerializeField] private SpriteRenderer spriteRenderer;
//     [SerializeField] private Sprite defaultSprite;
//     [SerializeField] private Sprite[] resultSprites; // 0 = bad, 1 = good
    
//     [Header("Echo")]
//     [SerializeField] private int echoOnSuccess = 5;
//     [SerializeField] private int echoOnFail = -3;
    
//     private bool isInteracted = false;
//     private bool isPlayerNearby = false;
    
//     [System.Serializable]
//     public class ItemInteraction
//     {
//         public Item requiredItem;
//         public InteractionResult result;
//         public string successMessage;
//         public string failMessage;
//         public UnityEvent<int> onEchoChange; // кастомное событие для изменения эха
//     }
    
//     public enum InteractionResult { Break, Fix, Nothing }

//     private void Update()
//     {
//         if (isPlayerNearby && !isInteracted && Input.GetKeyDown(KeyCode.E))
//         {
//             TryInteract();
//         }
        
//         // Показываем подсказку при приближении
//         if (isPlayerNearby && !isInteracted)
//             UIManager.Instance?.ShowNotification(interactionPrompt);
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Player")) isPlayerNearby = true;
//     }

//     private void OnTriggerExit2D(Collider2D other)
//     {
//         if (other.CompareTag("Player")) isPlayerNearby = false;
//     }

//     private void TryInteract()
//     {
//         foreach (var interaction in interactions)
//         {
//             if (InventoryManager.Instance.TryUseItem(interaction.requiredItem.name))
//             {
//                 ExecuteInteraction(interaction);
//                 return;
//             }
//         }
        
//         // Если ни один предмет не подошёл
//         UIManager.Instance.ShowNotification("Этот предмет не подходит");
//     }

//     private void ExecuteInteraction(ItemInteraction interaction)
//     {
//         isInteracted = true;
        
//         // Меняем спрайт
//         if (spriteRenderer != null)
//         {
//             spriteRenderer.sprite = interaction.result == InteractionResult.Fix 
//                 ? resultSprites[1] 
//                 : interaction.result == InteractionResult.Break 
//                     ? resultSprites[0] 
//                     : defaultSprite;
//         }
        
//         // Удаляем предмет из инвентаря (опционально)
//         // InventoryManager.Instance.RemoveItem(interaction.requiredItem.name);
        
//         // Эхо + уведомление
//         int echoChange = interaction.result == InteractionResult.Fix ? echoOnSuccess : 
//                         interaction.result == InteractionResult.Break ? echoOnFail : 0;
        
//         EchoManager.Instance?.AddEcho(echoChange);
        
//         string message = interaction.result == InteractionResult.Fix ? interaction.successMessage :
//                         interaction.result == InteractionResult.Break ? interaction.failMessage :
//                         "Ничего не произошло";
        
//         if (echoChange != 0) 
//             message += $" ({(echoChange > 0 ? "+" : "")}{echoChange} Эхо)";
            
//         UIManager.Instance.ShowNotification(message);
//     }
    
//     // === Для отладки: добавить взаимодействие через инспектор ===
//     #if UNITY_EDITOR
//     private void OnValidate()
//     {
//         if (interactions == null) interactions = new();
//     }
//     #endif
// }