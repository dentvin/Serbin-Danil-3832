using System;
using System.IO;
using System.Linq;
using TodoApp.Commands;
using TodoApp.Exceptions;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Clear();

            try
            {
                // Инициализация FileManager
                AppInfo.Storage = new FileManager("data");
                
                // Загрузка данных через новый Storage
                AppInfo.LoadAllData();
                
                MainLoop();
            }
            catch (DataAccessException ex)
            {
                Console.WriteLine($"[КРИТИЧЕСКАЯ ОШИБКА] {ex.Message}");
                Console.WriteLine($"   Файл: {ex.FilePath}");
                Console.WriteLine("Программа будет закрыта.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[НЕИЗВЕСТНАЯ ОШИБКА] {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey();
            }
        }

        private static void EnsureAuthenticated()
        {
            if (AppInfo.CurrentProfile == null)
            {
                throw new AuthenticationException();
            }
        }

        private static bool SelectOrCreateProfile()
        {
            while (true)
            {
                Console.WriteLine("Войти в существующий профиль? [y/n]");
                Console.Write("> ");

                string choice = Console.ReadLine()?.ToLower() ?? "n";

                if (choice == "y")
                {
                    return LoginProfile();
                }
                else if (choice == "n")
                {
                    return CreateProfile();
                }
                else
                {
                    Console.WriteLine("Пожалуйста, введите 'y' или 'n'");
                }
            }
        }

        private static bool LoginProfile()
        {
            int attempts = 0;
            const int maxAttempts = 3;

            while (attempts < maxAttempts)
            {
                try
                {
                    if (AppInfo.Profiles.Count == 0)
                    {
                        Console.WriteLine("Нет сохранённых профилей. Пожалуйста, создайте новый.");
                        return false;
                    }

                    Console.Write("Логин: ");
                    string login = Console.ReadLine() ?? "";

                    Console.Write("Пароль: ");
                    string password = Console.ReadLine() ?? "";

                    var profile = AppInfo.Profiles.FirstOrDefault(p => p.Login == login && p.Password == password);

                    if (profile == null)
                    {
                        attempts++;
                        Console.WriteLine($"Неверный логин или пароль. Осталось попыток: {maxAttempts - attempts}");
                        continue;
                    }

                    AppInfo.CurrentProfile = profile;

                    if (!AppInfo.UserTodos.ContainsKey(profile.Id))
                    {
                        AppInfo.UserTodos[profile.Id] = new TodoList();
                    }

                    var todoList = AppInfo.UserTodos[profile.Id];
                    SubscribeToTodoEvents(todoList);

                    AppInfo.ClearUndoRedo();
                    return true;
                }
                catch (Exception ex)
                {
                    attempts++;
                    Console.WriteLine($"[ОШИБКА] {ex.Message}. Осталось попыток: {maxAttempts - attempts}");
                }
            }

            Console.WriteLine("Превышено количество попыток входа. Возврат в главное меню.");
            return false;
        }

        private static bool CreateProfile()
        {
            try
            {
                Console.Write("Логин: ");
                string login = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(login))
                {
                    Console.WriteLine("Логин не может быть пустым");
                    return false;
                }

                if (AppInfo.Profiles.Any(p => p.Login == login))
                {
                    Console.WriteLine("Этот логин уже занят.");
                    return false;
                }

                Console.Write("Пароль: ");
                string password = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Пароль не может быть пустым");
                    return false;
                }

                Console.Write("Имя: ");
                string firstName = Console.ReadLine() ?? "";

                Console.Write("Фамилия: ");
                string lastName = Console.ReadLine() ?? "";

                Console.Write("Год рождения: ");
                if (!int.TryParse(Console.ReadLine(), out int birthYear))
                {
                    Console.WriteLine("Неверный формат года.");
                    return false;
                }

                if (birthYear < 1900 || birthYear > DateTime.Now.Year)
                {
                    Console.WriteLine("Неверный год рождения (должен быть от 1900 до текущего года)");
                    return false;
                }

                var profile = new Profile(login, password, firstName, lastName, birthYear);
                AppInfo.Profiles.Add(profile);
                AppInfo.Storage?.SaveProfiles(AppInfo.Profiles);

                AppInfo.CurrentProfile = profile;
                AppInfo.UserTodos[profile.Id] = new TodoList();
                AppInfo.Storage?.SaveTodos(profile.Id, AppInfo.UserTodos[profile.Id].GetAll());

                var todoList = AppInfo.UserTodos[profile.Id];
                SubscribeToTodoEvents(todoList);

                AppInfo.ClearUndoRedo();
                return true;
            }
            catch (DataAccessException ex)
            {
                Console.WriteLine($"[ОШИБКА] Не удалось сохранить профиль: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ОШИБКА] {ex.Message}");
                return false;
            }
        }

        private static void SubscribeToTodoEvents(TodoList todoList)
        {
            todoList.OnTodoAdded += (item) => SaveCurrentTodos();
            todoList.OnTodoDeleted += (item) => SaveCurrentTodos();
            todoList.OnTodoUpdated += (item) => SaveCurrentTodos();
            todoList.OnStatusChanged += (item) => SaveCurrentTodos();
        }

        private static void SaveCurrentTodos()
        {
            if (AppInfo.CurrentProfile != null && AppInfo.Storage != null)
            {
                var todoList = AppInfo.GetCurrentTodoList();
                if (todoList != null)
                {
                    AppInfo.Storage.SaveTodos(AppInfo.CurrentProfile.Id, todoList.GetAll());
                }
            }
        }

        private static void MainLoop()
        {
            while (true)
            {
                try
                {
                    if (AppInfo.CurrentProfile is null)
                    {
                        if (!SelectOrCreateProfile())
                        {
                            return;
                        }

                        Console.WriteLine($"\nДобро пожаловать, {AppInfo.CurrentProfile?.FirstName}!\n");
                    }

                    Console.Write("> ");
                    string input = Console.ReadLine() ?? "";

                    if (input.ToLower() == "exit")
                    {
                        Console.WriteLine("До свидания!");
                        break;
                    }

                    // Проверка авторизации для всех команд кроме profile, help, exit
                    if (input.ToLower() != "profile" && input.ToLower() != "help" && input.ToLower() != "exit")
                    {
                        EnsureAuthenticated();
                    }

                    ICommand command = CommandParser.Parse(input);
                    command.Execute();

                    if (command is IUndoableCommand undoableCmd)
                    {
                        AppInfo.UndoStack.Push(undoableCmd);
                        AppInfo.RedoStack.Clear();
                    }
                }
                catch (AuthenticationException ex)
                {
                    Console.WriteLine($"[ОШИБКА АВТОРИЗАЦИИ] {ex.Message}");
                    Console.WriteLine("   Пожалуйста, войдите в профиль командой 'profile'");
                }
                catch (ProfileNotFoundException ex)
                {
                    Console.WriteLine($"[ОШИБКА ПРОФИЛЯ] {ex.Message}");
                }
                catch (DuplicateLoginException ex)
                {
                    Console.WriteLine($"[ОШИБКА РЕГИСТРАЦИИ] {ex.Message}");
                }
                catch (TodoNotFoundException ex)
                {
                    Console.WriteLine($"[ОШИБКА ЗАДАЧИ] {ex.Message}");
                }
                catch (InvalidCommandException ex)
                {
                    Console.WriteLine($"[ОШИБКА КОМАНДЫ] {ex.Message}");
                }
                catch (InvalidArgumentException ex)
                {
                    Console.WriteLine($"[ОШИБКА АРГУМЕНТА] {ex.Message}");
                }
                catch (EmptyStackException ex)
                {
                    Console.WriteLine($"[ОШИБКА UNDO/REDO] {ex.Message}");
                }
                catch (DataAccessException ex)
                {
                    Console.WriteLine($"[ОШИБКА ФАЙЛА] {ex.Message}");
                    if (ex.FilePath != null)
                        Console.WriteLine($"   Файл: {ex.FilePath}");
                }
                catch (TodoAppException ex)
                {
                    Console.WriteLine($"[ОШИБКА ПРИЛОЖЕНИЯ] {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[НЕИЗВЕСТНАЯ ОШИБКА] {ex.Message}");
                    Console.WriteLine($"   {ex.GetType().Name}");
                }
            }
        }
    }
}