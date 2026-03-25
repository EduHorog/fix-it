using System;

[Serializable]
public class UIButtonData
{
    public string text;
    public Action onClick;
    public bool interactable = true;
}