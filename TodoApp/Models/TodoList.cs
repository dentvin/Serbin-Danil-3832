using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TodoApp.Models
{
	public class TodoList
	{
		private List<TodoItem> _items;

		// События
		public event Action<TodoItem> OnTodoAdded;
		public event Action<TodoItem> OnTodoDeleted;
		public event Action<TodoItem> OnTodoUpdated;
		public event Action<TodoItem> OnStatusChanged;

		public TodoList()
		{
			_items = new List<TodoItem>();
		}

		public void Add(TodoItem item)
		{
			_items.Add(item);
			OnTodoAdded?.Invoke(item);
		}

		public void InsertAt(int index, TodoItem item)
		{
			_items.Insert(index, item);
			OnTodoAdded?.Invoke(item);
		}

		public void Delete(int index)
		{
			if (index >= 0 && index < _items.Count)
			{
				var item = _items[index];
				_items.RemoveAt(index);
				OnTodoDeleted?.Invoke(item);
			}
		}

		public TodoItem GetItem(int index)
		{
			if (index >= 0 && index < _items.Count)
			{
				return _items[index];
			}
			return null;
		}

		public void SetStatus(int index, TodoStatus status)
		{
			if (index >= 0 && index < _items.Count)
			{
				_items[index].SetStatus(status);
				OnStatusChanged?.Invoke(_items[index]);
			}
		}

		public void UpdateItem(int index, string newText)
		{
			if (index >= 0 && index < _items.Count)
			{
				_items[index].UpdateText(newText);
				OnTodoUpdated?.Invoke(_items[index]);
			}
		}

		public int Count => _items.Count;

		public TodoItem this[int index]
		{
			set
			{
				if (index >= 0 && index < _items.Count)
				{
					_items[index] = value;
					OnTodoUpdated?.Invoke(value);
				}
			}
			get
			{
				return GetItem(index);
			}
		}

		public IEnumerator<TodoItem> GetEnumerator()
		{
			foreach (var item in _items)
			{
				yield return item;
			}
		}

		public List<TodoItem> GetAll()
		{
			return _items;
		}

		// ==================== LINQ МЕТОДЫ ====================

		// 1. WHERE - фильтрация по статусу (Method Syntax)
		public List<TodoItem> GetByStatus(TodoStatus status)
		{
			return _items.Where(item => item.Status == status).ToList();
		}

		// 2. WHERE - фильтрация по тексту (содержит подстроку)
		public List<TodoItem> GetByTextContains(string searchText)
		{
			return _items.Where(item => item.Text.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
		}

		// 3. ORDERBY - сортировка по дате
		public List<TodoItem> GetOrderedByDate(bool descending = false)
		{
			return descending
				? _items.OrderByDescending(item => item.LastUpdate).ToList()
				: _items.OrderBy(item => item.LastUpdate).ToList();
		}

		// 4. ORDERBY - сортировка по статусу
		public List<TodoItem> GetOrderedByStatus()
		{
			return _items.OrderBy(item => item.Status).ToList();
		}

		// 5. GROUPBY - группировка по статусу
		public Dictionary<TodoStatus, List<TodoItem>> GetGroupedByStatus()
		{
			return _items.GroupBy(item => item.Status)
						 .ToDictionary(group => group.Key, group => group.ToList());
		}

		// 6. GROUPBY - статистика по статусам (количество)
		public Dictionary<TodoStatus, int> GetStatusStatistics()
		{
			return _items.GroupBy(item => item.Status)
						 .ToDictionary(group => group.Key, group => group.Count());
		}

		// 7. ANY - есть ли выполненные задачи
		public bool HasCompletedTasks()
		{
			return _items.Any(item => item.Status == TodoStatus.Completed);
		}

		// 8. ALL - все ли задачи выполнены
		public bool AllTasksCompleted()
		{
			return _items.All(item => item.Status == TodoStatus.Completed);
		}

		// 9. COUNT с условием
		public int GetCountByStatus(TodoStatus status)
		{
			return _items.Count(item => item.Status == status);
		}

		// 10. SELECT - проекция (только текст задач)
		public List<string> GetTaskTexts()
		{
			return _items.Select(item => item.Text).ToList();
		}

		// 11. SELECT - проекция с анонимным типом (для статистики)
		public object GetTaskSummaries()
		{
			return _items.Select(item => new
			{
				item.Text,
				item.Status,
				DaysSinceUpdate = (DateTime.Now - item.LastUpdate).Days,
				IsRecent = (DateTime.Now - item.LastUpdate).Days < 3
			}).ToList();
		}

		// 12. First/FirstOrDefault
		public TodoItem GetFirstByStatus(TodoStatus status)
		{
			return _items.FirstOrDefault(item => item.Status == status);
		}

		// 13. Skip/Take - пагинация
		public List<TodoItem> GetPage(int pageNumber, int pageSize)
		{
			return _items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
		}

		// 14. QUERY SYNTAX - задачи, которые не обновлялись более N дней
		public List<TodoItem> GetOldTasks(int daysThreshold)
		{
			var threshold = DateTime.Now - TimeSpan.FromDays(daysThreshold);
			return (from item in _items
					where item.LastUpdate < threshold
					orderby item.LastUpdate ascending
					select item).ToList();
		}

		// 15. QUERY SYNTAX - задачи с группировкой (альтернативный стиль)
		public IEnumerable<IGrouping<TodoStatus, TodoItem>> GetGroupedByStatusQuerySyntax()
		{
			return from item in _items
				   group item by item.Status into statusGroup
				   select statusGroup;
		}

		// 16. Сложный LINQ с несколькими операциями (Fluent API)
		public List<TodoItem> GetTopIncompleteRecent(int count)
		{
			return _items.Where(item => item.Status != TodoStatus.Completed)
						 .OrderByDescending(item => item.LastUpdate)
						 .Take(count)
						 .ToList();
		}

		// 17. Демонстрация отложенного выполнения - возвращает IEnumerable (НЕ выполняется сразу!)
		public IEnumerable<TodoItem> GetNotStartedQuery()
		{
			return _items.Where(item => item.Status == TodoStatus.NotStarted);
		}

		public string GetTable(bool _showIndex = true, bool _showStatus = true, bool _showDate = true)
		{
			var table = new StringBuilder();
			int indexWidth = 5;
			int textWidth = 35;
			int statusWidth = 15;
			int dateWidth = 20;

			var columns = new List<int>();
			var headers = new List<string>();

			if (_showIndex)
			{
				columns.Add(indexWidth);
				headers.Add("INDEX");
			}

			columns.Add(textWidth);
			headers.Add("ЗАДАЧА");

			if (_showStatus)
			{
				columns.Add(statusWidth);
				headers.Add("СТАТУС");
			}
			if (_showDate)
			{
				columns.Add(dateWidth);
				headers.Add("ДАТА");
			}

			string BuildLine(char left, char mid, char right, char fill)
			{
				var sb = new StringBuilder();
				sb.Append(left);
				for (int i = 0; i < columns.Count; i++)
				{
					sb.Append(new string(fill, columns[i] + 2));
					sb.Append(i < columns.Count - 1 ? mid : right);
				}
				return sb.ToString();
			}

			table.AppendLine(BuildLine('╔', '╦', '╗', '═'));
			table.Append('║');
			for (int i = 0; i < columns.Count; i++)
				table.Append($" {headers[i].PadRight(columns[i])} ║");
			table.AppendLine();
			table.AppendLine(BuildLine('╠', '╬', '╣', '═'));

			int index = 0;
			foreach (var item in _items)
			{
				table.Append('║');
				int col = 0;

				if (_showIndex)
					table.Append($" {index.ToString().PadRight(columns[col++])} ║");

				string shortText = item.GetShortInfo();
				table.Append($" {shortText.PadRight(columns[col++])} ║");

				if (_showStatus)
					table.Append($" {item.Status.ToString().PadRight(columns[col++])} ║");

				if (_showDate)
					table.Append($" {item.LastUpdate.ToString("yyyy-MM-dd HH:mm").PadRight(columns[col++])} ║");

				table.AppendLine();

				if (index < _items.Count - 1)
					table.AppendLine(BuildLine('╠', '╬', '╣', '═'));

				index++;
			}

			table.AppendLine(BuildLine('╚', '╩', '╝', '═'));
			return table.ToString();
		}
	}
}