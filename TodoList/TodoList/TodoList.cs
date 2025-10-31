using System;

namespace TodoList
{
    public class TodoList
    {
        private TodoItem[] items;
        private int count;
        private const int INITIAL_CAPACITY = 2;

        public TodoList()
        {
            items = new TodoItem[INITIAL_CAPACITY];
            count = 0;
        }

        public void Add(TodoItem item)
        {
            if (count >= items.Length)
                IncreaseArray();
            items[count++] = item;
        }

        public void Delete(int index)
        {
            if (index < 0 || index >= count)
            {
                Console.WriteLine("Ошибка: задачи с таким номером не существует.");
                return;
            }

            for (int i = index; i < count - 1; i++)
                items[i] = items[i + 1];

            count--;
            Console.WriteLine($"Задача №{index + 1} удалена!");
        }

        public void View(bool showIndex, bool showDone, bool showDate)
        {
            if (count == 0)
            {
                Console.WriteLine("Список задач пуст.");
                return;
            }

            if (showIndex) Console.Write("| №  ");
            Console.Write("| Task Text                      ");
            if (showDone) Console.Write("| Status ");
            if (showDate) Console.Write("| Last Update        ");
            Console.WriteLine("|");

            for (int i = 0; i < count; i++)
            {
                if (showIndex) Console.Write($"| {i + 1,-3}");
                string text = items[i].Text.Length > 30 ? items[i].Text.Substring(0, 30) + "..." : items[i].Text.PadRight(30);
                Console.Write($"| {text}");
                if (showDone)
                {
                    string status = items[i].IsDone ? "Done" : "Not done";
                    Console.Write($"| {status,-7}");
                }
                if (showDate) Console.Write($"| {items[i].LastUpdate:dd.MM.yyyy HH:mm}");
                Console.WriteLine("|");
            }
        }

        public void Read(int index)
        {
            if (index < 0 || index >= count)
            {
                Console.WriteLine("Ошибка: задачи с таким номером не существует.");
                return;
            }

            Console.WriteLine(items[index].GetFullInfo());
        }

        private void IncreaseArray()
        {
            int newSize = items.Length * 2;
            TodoItem[] newItems = new TodoItem[newSize];
            for (int i = 0; i < items.Length; i++)
                newItems[i] = items[i];
            items = newItems;
        }

        public int Count => count;
        public TodoItem GetItem(int index) => index >= 0 && index < count ? items[index] : null;
    }
}