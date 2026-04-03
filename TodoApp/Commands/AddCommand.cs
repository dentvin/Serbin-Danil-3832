using System;
using System.Collections.Generic;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class AddCommand : IUndoableCommand
    {
        private string _text;
        private bool _isMultiline;
        private TodoItem _addedItem;
        private TodoList _todos;
        private int _addedIndex;

        public AddCommand(string text, bool isMultiline)
        {
            _text = text;
            _isMultiline = isMultiline;
        }

        public void Execute()
        {
            _todos = AppInfo.GetCurrentTodoList();
            if (_todos == null) return;

            if (_isMultiline)
            {
                _text = ReadMultilineInput();
            }

            _addedItem = new TodoItem(_text);
            _addedIndex = _todos.Count;
            _todos.Add(_addedItem);

            Console.WriteLine($"Задача добавлена: {_text}");
        }

        public void Unexecute()
        {
            _todos = AppInfo.GetCurrentTodoList();
            if (_todos == null || _addedItem == null) return;

            // Находим индекс добавленного элемента
            var items = _todos.GetAll();
            int index = items.FindIndex(item => item == _addedItem);
            if (index != -1)
            {
                _todos.Delete(index);
                Console.WriteLine("Отменено добавление задачи");
            }
        }

        private string ReadMultilineInput()
        {
            var lines = new List<string>();
            Console.WriteLine("Введите строки задачи (завершите вводом '!end'):");

            while (true)
            {
                Console.Write("> ");
                string line = Console.ReadLine();
                if (line == "!end")
                    break;
                lines.Add(line);
            }

            return string.Join("\n", lines);
        }
    }
}