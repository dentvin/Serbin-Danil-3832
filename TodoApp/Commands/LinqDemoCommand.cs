using System;
using System.Linq;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class LinqDemoCommand : ICommand
    {
        public void Execute()
        {
            var todoList = AppInfo.GetCurrentTodoList();
            if (todoList == null || todoList.Count == 0)
            {
                Console.WriteLine("\n❌ Нет задач для демонстрации LINQ. Сначала добавьте несколько задач командой add\n");
                return;
            }

            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                           ДЕМОНСТРАЦИЯ LINQ ОПЕРАЦИЙ                           ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════╝\n");

            // ==================== 1. WHERE ====================
            Console.WriteLine("📌 1. WHERE - фильтрация (задачи со статусом InProgress):");
            var inProgress = todoList.GetByStatus(TodoStatus.InProgress);
            if (inProgress.Any())
                foreach (var task in inProgress)
                    Console.WriteLine($"   ✓ {task.GetShortInfo()}");
            else
                Console.WriteLine("   ➜ Нет задач в статусе InProgress");

            // ==================== 2. ORDERBY ====================
            Console.WriteLine("\n📌 2. ORDERBY - сортировка по дате (сначала новые):");
            var ordered = todoList.GetOrderedByDate(descending: true);
            foreach (var task in ordered.Take(3))
                Console.WriteLine($"   ✓ {task.GetShortInfo()} ({(DateTime.Now - task.LastUpdate).Days} дней назад)");

            // ==================== 3. GROUPBY ====================
            Console.WriteLine("\n📌 3. GROUPBY - группировка по статусам (статистика):");
            var statistics = todoList.GetStatusStatistics();
            foreach (var stat in statistics)
                Console.WriteLine($"   ✓ {stat.Key}: {stat.Value} задач");

            // ==================== 4. COUNT с условием ====================
            Console.WriteLine("\n📌 4. COUNT - количество задач по статусам:");
            foreach (TodoStatus status in Enum.GetValues(typeof(TodoStatus)))
            {
                int count = todoList.GetCountByStatus(status);
                if (count > 0)
                    Console.WriteLine($"   ✓ {status}: {count} шт.");
            }

            // ==================== 5. ANY / ALL ====================
            Console.WriteLine("\n📌 5. ANY / ALL - проверка условий:");
            Console.WriteLine($"   ✓ Есть выполненные задачи? {(todoList.HasCompletedTasks() ? "Да" : "Нет")}");
            Console.WriteLine($"   ✓ Все задачи выполнены? {(todoList.AllTasksCompleted() ? "Да" : "Нет")}");

            // ==================== 6. FIRST / FIRSTORDEFAULT ====================
            Console.WriteLine("\n📌 6. FIRSTORDEFAULT - первая задача со статусом NotStarted:");
            var firstNotStarted = todoList.GetFirstByStatus(TodoStatus.NotStarted);
            if (firstNotStarted != null)
                Console.WriteLine($"   ✓ {firstNotStarted.GetShortInfo()}");
            else
                Console.WriteLine("   ➜ Нет задач со статусом NotStarted");

            // ==================== 7. SKIP / TAKE ====================
            Console.WriteLine("\n📌 7. SKIP / TAKE - пагинация (первые 2 задачи):");
            var firstPage = todoList.GetPage(1, 2);
            foreach (var task in firstPage)
                Console.WriteLine($"   ✓ {task.GetShortInfo()}");

            // ==================== 8. SELECT ====================
            Console.WriteLine("\n📌 8. SELECT - проекция (только текст задач):");
            var texts = todoList.GetTaskTexts();
            for (int i = 0; i < Math.Min(3, texts.Count); i++)
                Console.WriteLine($"   ✓ {texts[i].Substring(0, Math.Min(30, texts[i].Length))}...");

            // ==================== 9. QUERY SYNTAX ====================
            Console.WriteLine("\n📌 9. QUERY SYNTAX (SQL-стиль) - задачи старше 7 дней:");
            var oldTasks = todoList.GetOldTasks(7);
            if (oldTasks.Any())
                foreach (var task in oldTasks)
                    Console.WriteLine($"   ✓ {task.GetShortInfo()} ({(DateTime.Now - task.LastUpdate).Days} дней)");
            else
                Console.WriteLine("   ➜ Нет задач старше 7 дней");

            // ==================== 10. ОТЛОЖЕННОЕ ВЫПОЛНЕНИЕ ====================
            Console.WriteLine("\n📌 10. ОТЛОЖЕННОЕ ВЫПОЛНЕНИЕ (Deferred Execution):");
            Console.WriteLine("   ⚠️  Создаём запрос, но НЕ выполняем его...");
            
            var deferredQuery = todoList.GetNotStartedQuery();
            Console.WriteLine("   📝 Запрос создан, результат ещё не вычислен");
            
            Console.WriteLine("   ➕ Добавляем новую задачу со статусом NotStarted...");
            todoList.Add(new TodoItem("[DEMO] Задача для демонстрации отложенного выполнения"));
            
            Console.WriteLine($"   📊 Результат запроса ПОСЛЕ добавления: {deferredQuery.Count()} задач");
            Console.WriteLine("   ✨ Запрос выполнился ТОЛЬКО при вызове .Count() — это и есть ОТЛОЖЕННОЕ ВЫПОЛНЕНИЕ!");
            Console.WriteLine("   💡 При использовании .ToList() выполнение было бы немедленным");

            // ==================== 11. CHAINING (Fluent API) ====================
            Console.WriteLine("\n📌 11. FLUENT API (цепочка методов) - топ-3 невыполненные задачи:");
            var topIncomplete = todoList.GetTopIncompleteRecent(3);
            foreach (var task in topIncomplete)
                Console.WriteLine($"   ✓ {task.GetShortInfo()}");

            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                              ДЕМОНСТРАЦИЯ ЗАВЕРШЕНА                              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════╝\n");
        }
    }
}