    using UnityEngine;
    using System.Collections.Generic;

    public class InteractableObject : MonoBehaviour
    {
        [Header("Настройки взаимодействия")]
        public string objectName = "Объект";
        public string promptText = "Нажмите [E] для взаимодействия";
        public float interactionRange = 3f;
        
        [Header("Визуал")]
        public SpriteRenderer spriteRenderer;
        public Sprite defaultSprite; // ✅ Спрайт по умолчанию

        [Header("Настройки")]
        public bool oneTimeInteraction = true; // ✅ Можно настроить в инспекторе
        private bool hasBeenUsed = false;      // ✅ Флаг: было ли взаимодействие
        
        [Header("Действия")]
        public List<InteractionChoice> choices = new List<InteractionChoice>();
        
        [Header("События")]
        public UnityEngine.Events.UnityEvent onInteractStart;
        public UnityEngine.Events.UnityEvent onInteractEnd;

        private bool isPlayerNear = false;
        private Sprite originalSprite; // ✅ Запоминаем оригинальный спрайт

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
            // Сохраняем исходный спрайт для возможного возврата
            if (spriteRenderer != null)
            {
                originalSprite = spriteRenderer.sprite;
                if (defaultSprite != null)
                    spriteRenderer.sprite = defaultSprite;
            }
        }

        private void Update()
        {
            // ✅ Добавляем проверку: если объект уже использован и он одноразовый — не реагируем
            if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
            {
                if (oneTimeInteraction && hasBeenUsed)
                {
                    // Опционально: показать сообщение, что объект уже использован
                    // GameUIManager.Instance?.ShowMessage("Вы уже взаимодействовали с этим объектом");
                    return; 
                }
                OpenInteractionMenu();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // ✅ Проверяем: если объект одноразовый и уже использован — ничего не делаем
                if (oneTimeInteraction && hasBeenUsed)
                    return;
                    
                isPlayerNear = true;
                GameUIManager.Instance?.ShowPrompt(promptText);
                onInteractStart?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // ✅ Та же проверка при выходе
                if (oneTimeInteraction && hasBeenUsed)
                    return;
                    
                isPlayerNear = false;
                GameUIManager.Instance?.HidePrompt();
                GameUIManager.Instance?.CloseInteractionMenu();
                onInteractEnd?.Invoke();
            }
        }

        public void OpenInteractionMenu()
        {
            if (GameUIManager.Instance == null)
            {
                Debug.LogError("GameUIManager не найден!");
                return;
            }
            GameUIManager.Instance.HidePrompt();
            List<UIButtonData> buttons = new List<UIButtonData>();
            
            foreach (var choice in choices)
            {
                // Создаём локальную копию для замыкания
                InteractionChoice selectedChoice = choice;
                
                string buttonText = selectedChoice.IsItemInInventory 
                    ? $"Использовать {selectedChoice.choiceName}" 
                    : $"Использовать ***";
                
                buttons.Add(new UIButtonData
                {
                    text = buttonText,
                    onClick = () => ExecuteChoice(selectedChoice), // ← Теперь работает!
                    interactable = selectedChoice.IsItemInInventory
                });
            }

            GameUIManager.Instance.ShowInteractionMenu(objectName, buttons);
        }

        private void ExecuteChoice(InteractionChoice choice)
        {
            // 🔹 Проверка инвентаря (без изменений)
            if (!choice.IsItemInInventory)
            {
                GameUIManager.Instance?.ShowMessage("У вас нет нужного предмета!");
                return;
            }

            // 🔹 Расход предмета
            if (choice.consumeItem && choice.requiredItem != null)
            {
                InventoryManager.Instance.RemoveOneItem(choice.requiredItem);
            }

            // 🔹 Сообщение
            if (!string.IsNullOrEmpty(choice.successMessage))
            {
                GameUIManager.Instance?.ShowMessage(choice.successMessage);
            }

            // 🔹 Звук и эффекты
            if (choice.successSound != null)
                AudioSource.PlayClipAtPoint(choice.successSound, transform.position);
            
            if (choice.successEffect != null)
                choice.successEffect.Play();

            // 🔹 Смена спрайта
            if (choice.newSprite != null)
            {
                ChangeSprite(choice.newSprite);
            }

            // 🔹 Уничтожение объекта
            if (choice.destroyObject)
            {
                Destroy(gameObject, 0.3f);
            }
            
            // 🔹 Событие тени
            if (choice.triggerShadow && choice.shadowEvent != null)
            {
                choice.shadowEvent.TriggerShadowEvent();
            }

            // ✅ НОВОЕ: Применяем карму, если указано
            if (choice.karmaChange != 0 || choice.isCorrectChoice)
            {
                KarmaManager.Instance?.AddKarma(choice.karmaChange, choice.isCorrectChoice);
            }

            if (oneTimeInteraction)
            {
                MarkAsUsed();
            }

            // 🔹 Закрытие меню
            GameUIManager.Instance?.CloseInteractionMenu();
        }

         public void MarkAsUsed()
        {
            hasBeenUsed = true;
            
            // Опционально: визуальная обратная связь
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(0.7f, 0.7f, 0.7f, 1f); // Легкое затемнение
            }
            
            // Опционально: отключить коллайдер, чтобы вообще не реагировал на триггеры
            // GetComponent<Collider2D>().enabled = false;
        }

        public void ResetInteraction()
        {
            hasBeenUsed = false;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }
        }

        // ✅ Новый метод для смены спрайта
        public void ChangeSprite(Sprite newSprite)
        {
            if (spriteRenderer != null && newSprite != null)
            {
                spriteRenderer.sprite = newSprite;
            }
        }

        // ✅ Метод для возврата к исходному спрайту (опционально)
        public void ResetSprite()
        {
            if (spriteRenderer != null && originalSprite != null)
            {
                spriteRenderer.sprite = originalSprite;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }