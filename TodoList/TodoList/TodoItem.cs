using System;

namespace TodoListApp
{
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
            this.text = text;
            isDone = false;
            lastUpdate = DateTime.Now;
        }

        public void MarkDone()
        {
            isDone = true;
            lastUpdate = DateTime.Now;
        }

        public void UpdateText(string newText)
        {
            text = newText;
            lastUpdate = DateTime.Now;
        }

        public string GetShortInfo()
        {
            string shortText = text.Length > 30 ? text.Substring(0, 30) + "..." : text.PadRight(30);
            string status = isDone ? "Done" : "Not done";
            return $"{shortText} | {status} | {lastUpdate:dd.MM.yyyy HH:mm}";
        }

        public string GetFullInfo()
        {
            string status = isDone ? "Done" : "Not done";
            return $"Task: {text}\nStatus: {status}\nLast update: {lastUpdate:dd.MM.yyyy HH:mm:ss}";
        }
    }
}
