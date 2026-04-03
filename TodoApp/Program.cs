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
                FileManager.EnsureDataDirectory();
                AppInfo.Profiles = FileManager.LoadAllProfiles();
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

                    var profile = FileManager.LoadProfile(login, password);

                    AppInfo.CurrentProfile = profile;

                    string todoPath = FileManager.GetTodoFilePath(profile.Id);
                    if (File.Exists(todoPath))
                    {
                        AppInfo.UserTodos[profile.Id] = FileManager.LoadTodos(todoPath);
                    }
                    else
                    {
                        AppInfo.UserTodos[profile.Id] = new TodoList();
                        FileManager.SaveTodos(AppInfo.UserTodos[profile.Id], todoPath);
                    }

                    var todoList = AppInfo.UserTodos[profile.Id];
                    SubscribeToTodoEvents(todoList);

                    AppInfo.ClearUndoRedo();
                    return true;
                }
                catch (ProfileNotFoundException ex)
                {
                    attempts++;
                    Console.WriteLine($"[ОШИБКА] {ex.Message}. Осталось попыток: {maxAttempts - attempts}");
                }
                catch (InvalidPasswordException ex)
                {
                    attempts++;
                    Console.WriteLine($"[ОШИБКА] {ex.Message}. Осталось попыток: {maxAttempts - attempts}");
                }
                catch (DataAccessException ex)
                {
                    Console.WriteLine($"[ОШИБКА ФАЙЛА] {ex.Message}");
                    return false;
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
                    throw new DuplicateLoginException(login);
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
                FileManager.SaveProfile(profile);

                AppInfo.CurrentProfile = profile;
                AppInfo.UserTodos[profile.Id] = new TodoList();

                string todoPath = FileManager.GetTodoFilePath(profile.Id);
                FileManager.SaveTodos(AppInfo.UserTodos[profile.Id], todoPath);

                var todoList = AppInfo.UserTodos[profile.Id];
                SubscribeToTodoEvents(todoList);

                AppInfo.ClearUndoRedo();
                return true;
            }
            catch (DuplicateLoginException ex)
            {
                Console.WriteLine($"[ОШИБКА] {ex.Message}");
                return false;
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
            todoList.OnTodoAdded += FileManager.SaveTodoList;
            todoList.OnTodoDeleted += FileManager.SaveTodoList;
            todoList.OnTodoUpdated += FileManager.SaveTodoList;
            todoList.OnStatusChanged += FileManager.SaveTodoList;
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