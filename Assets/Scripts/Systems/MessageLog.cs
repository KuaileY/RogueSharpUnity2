using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

public class MessageLog
{
    private readonly Queue<string> _lines;
    private StringBuilder str;
    private static Text text;

    public MessageLog()
    {
        _lines = new Queue<string>();
        str = new StringBuilder();
        text = Game.text;
    }

    public void Add(string message)
    {
        _lines.Enqueue(message);
        if (_lines.Count > 9)
        {
            _lines.Dequeue();
        }
    }

    public void Draw()
    {
        str.Length = 0;
        string[] lines = _lines.ToArray();
        for (int i = 0; i < lines.Count(); i++)
        {
            str.Append(lines[i] + "\n");
        }
        text.text = str.ToString();
    }
}
