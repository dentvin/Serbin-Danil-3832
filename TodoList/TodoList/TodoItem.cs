using System;

public class TodoItem
{
    private string text;
    private bool isDone;
    private DateTime lastUpdate;

    public string Text => text;
    public bool IsDone => isDone;
    public DateTime LastUpdate => lastUpdate;

    public TodoItem(string text)
    {
        this.text = text ?? "";
        this.isDone = false;
        this.lastUpdate = DateTime.Now;
    }

    public void MarkDone()
    {
        isDone = true;
        lastUpdate = DateTime.Now;
    }

    public void UpdateText(string newText)
    {
        if (!string.IsNullOrEmpty(newText))
        {
            text = newText;
            lastUpdate = DateTime.Now;
        }
    }

    public string GetShortInfo()
    {
        string shortText = text.Length > 30 ? text.Substring(0, 30) + "..." : text.PadRight(30);
        string status = isDone ? "Done" : "Not done";
        string date = lastUpdate.ToString("dd.MM.yyyy HH:mm");
        return $"{shortText} | {status,-7} | {date}";
    }

    public string GetFullInfo()
    {
        string status = isDone ? "Done" : "Not done";
        return $"Task: {text}\nStatus: {status}\nLast updated: {lastUpdate:dd.MM.yyyy HH:mm}";
    }
}
