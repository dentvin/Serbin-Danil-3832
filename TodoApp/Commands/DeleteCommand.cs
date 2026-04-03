using System;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Commands
{
	public class DeleteCommand : IUndoableCommand
	{
		private int _index;
		private int _originalIndex;
		private TodoItem _deletedItem;
		private TodoList _todos;

		public DeleteCommand(int index)
		{
			_index = index;
		}

		public void Execute()
		{
			_todos = AppInfo.GetCurrentTodoList();
			if (_todos == null) return;

			if (_index < 0 || _index >= _todos.Count)
			{
				Console.WriteLine($"Ошибка: задача с индексом {_index} не найдена.");
				return;
			}

			_deletedItem = _todos[_index];
			_originalIndex = _index;
			_todos.Delete(_index);

			Console.WriteLine($"Задача удалена: {_deletedItem.Text}");
		}

		public void Unexecute()
		{
			_todos = AppInfo.GetCurrentTodoList();
			if (_todos == null || _deletedItem == null) return;

			_todos.InsertAt(_originalIndex, _deletedItem);
			Console.WriteLine("Отменено удаление задачи");
		}
	}
}