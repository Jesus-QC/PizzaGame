using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Scriptable Objects/Dialogue")]
public class Dialogue : ScriptableObject
{
    public List<DialogueLine> lines;
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)] public string text;
}
