using System;
using TodoApp.Exceptions;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class ErrorDemoCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                     ДЕМОНСТРАЦИЯ ОБРАБОТКИ ОШИБОК И ИСКЛЮЧЕНИЙ                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════╝\n");

            var todoList = AppInfo.GetCurrentTodoList();
            if (todoList == null)
            {
                Console.WriteLine("❌ Нет активного профиля. Сначала войдите в профиль.\n");
                return;
            }

            // ==================== ДЕМОНСТРАЦИЯ 1: try-catch-finally ====================
            Console.WriteLine("📌 1. try-catch-finally - обработка ошибок при удалении задачи:");
            Console.Write("   Введите индекс задачи для удаления (например, 999 для ошибки): ");
            string input = Console.ReadLine() ?? "";
            
            try
            {
                if (int.TryParse(input, out int index))
                {
                    var item = todoList.GetItem(index);
                    Console.WriteLine($"   ✓ Задача найдена: {item.GetShortInfo()}");
                    
                    try
                    {
                        todoList.Delete(index);
                        Console.WriteLine($"   ✓ Задача удалена");
                    }
                    finally
                    {
                        Console.WriteLine("   🔧 Блок finally выполнился (очистка ресурсов)");
                    }
                }
            }
            catch (TodoNotFoundException ex)
            {
                Console.WriteLine($"   ❌ Перехвачено исключение: {ex.Message}");
                Console.WriteLine($"   📍 Тип исключения: {ex.GetType().Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Неожиданная ошибка: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("   🔧 Блок finally выполнился в любом случае (успех или ошибка)");
            }

            // ==================== ДЕМОНСТРАЦИЯ 2: Иерархия исключений ====================
            Console.WriteLine("\n📌 2. Иерархия исключений (поймали конкретное, а потом общее):");
            
            try
            {
                Console.WriteLine("   ➕ Пробуем добавить задачу с пустым текстом...");
                todoList.UpdateItem(0, "");
            }
            catch (TodoNotFoundException ex)
            {
                Console.WriteLine($"   ❌ Поймано TodoNotFoundException (конкретное): {ex.Message}");
            }
            catch (TodoException ex)
            {
                Console.WriteLine($"   ❌ Поймано TodoException (общее для всех Todo ошибок): {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Поймано Exception (самое общее): {ex.Message}");
            }

            // ==================== ДЕМОНСТРАЦИЯ 3: throw и переброс исключений ====================
            Console.WriteLine("\n📌 3. throw - проброс исключения наверх:");
            
            try
            {
                TestThrowException(todoList);
            }
            catch (TodoException ex)
            {
                Console.WriteLine($"   ❌ Поймано исключение из метода: {ex.Message}");
            }

            // ==================== ДЕМОНСТРАЦИЯ 4: finally для очистки ====================
            Console.WriteLine("\n📌 4. finally - гарантированное выполнение кода:");
            
            try
            {
                Console.WriteLine("   📝 Пытаемся выполнить операцию...");
                throw new TodoException("Тестовая ошибка");
            }
            catch
            {
                Console.WriteLine("   ❌ Ошибка перехвачена");
            }
            finally
            {
                Console.WriteLine("   🧹 finally: закрытие файлов, соединений, очистка ресурсов");
            }

            // ==================== ДЕМОНСТРАЦИЯ 5: Свои исключения ====================
            Console.WriteLine("\n📌 5. Свои исключения (кастомные):");
            Console.WriteLine("   📦 Создана иерархия исключений:");
            Console.WriteLine("      Exception");
            Console.WriteLine("      └── TodoAppException (базовое для приложения)");
            Console.WriteLine("          ├── ProfileException");
            Console.WriteLine("          │   ├── ProfileNotFoundException");
            Console.WriteLine("          │   └── InvalidPasswordException");
            Console.WriteLine("          ├── TodoException");
            Console.WriteLine("          │   ├── TodoNotFoundException");
            Console.WriteLine("          │   └── InvalidTodoStatusException");
            Console.WriteLine("          └── DataAccessException");

            // ==================== ДЕМОНСТРАЦИЯ 6: Проверка статуса ====================
            Console.WriteLine("\n📌 6. Валидация статуса задачи:");
            Console.Write("   Введите неверный статус (например 'invalid'): ");
            string badStatus = Console.ReadLine() ?? "";
            
            try
            {
                if (Enum.TryParse<TodoStatus>(badStatus, ignoreCase: true, out var status))
                {
                    Console.WriteLine($"   ✓ Статус '{status}' корректен");
                }
                else
                {
                    throw new InvalidTodoStatusException(badStatus);
                }
            }
            catch (InvalidTodoStatusException ex)
            {
                Console.WriteLine($"   ❌ {ex.Message}");
            }

            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                           ДЕМОНСТРАЦИЯ ЗАВЕРШЕНА                                 ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════╝\n");
        }

        private void TestThrowException(TodoList todoList)
        {
            try
            {
                Console.WriteLine("   📝 Метод TestThrowException()");
                var item = todoList.GetItem(999);
            }
            catch (TodoNotFoundException ex)
            {
                Console.WriteLine($"   ⚠️ Перехватили внутри: {ex.Message}");
                Console.WriteLine("   🔄 Пробрасываем исключение дальше (throw)");
                throw;
            }
        }
    }
}