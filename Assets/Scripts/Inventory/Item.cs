using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject {
    public string itemName;
    public Sprite icon;
    public bool isQuestItem = false;
    public string[] combineWith;
    public string resultItemID;
    
    [TextArea(2, 4)]
    public string description;
}