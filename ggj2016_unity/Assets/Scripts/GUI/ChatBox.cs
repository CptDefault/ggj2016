using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChatBox : MonoBehaviour
{
    public static ChatBox Instance;

    public UILabel chatLabel;

    private Queue<string> _lines = new Queue<string>();

    protected void Awake()
    {
        Instance = this;
    }

    public void AddChatMessage(string newLine)
    {
        if (_lines.Count > 5)
            _lines.Dequeue();
        
        _lines.Enqueue(newLine);

        chatLabel.text = "";
        int index = 0;
        foreach (var line in _lines)
        {
            chatLabel.text += line;
            if (index != _lines.Count - 1)
                chatLabel.text += "\n";

            index++;
        }
    }
}
