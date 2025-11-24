using System.Collections.Generic;


[System.Serializable]
public class DialogueEntry
{
    public int id;
    public string characterName;
    public string text;
    public int nextId;
}

[System.Serializable]
public class DialogueData
{
    public List<DialogueEntry> dialogues;
}
