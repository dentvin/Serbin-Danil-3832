using System;
using System.Linq;
using TodoApp.Commands;
using TodoApp.Exceptions;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp
{
    class Program
    {
        private static ProfileRepository _profileRepo = new ProfileRepository();
        private static TodoRepository _todoRepo = new TodoRepository();
        private static Profile? _currentProfile;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Clear();

            try
            {
                MainLoop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[КРИТИЧЕСКАЯ ОШИБКА] {ex.Message}");
                Console.ReadKey();
            }
        }

        private static void EnsureAuthenticated()
        {
            if (_currentProfile == null)
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
                    Console.Write("Логин: ");
                    string login = Console.ReadLine() ?? "";

                    Console.Write("Пароль: ");
                    string password = Console.ReadLine() ?? "";

                    var profile = _profileRepo.GetByLogin(login);
                    
                    if (profile.Password != password)
                    {
                        throw new InvalidPasswordException(login);
                    }

                    _currentProfile = profile;
                    AppInfo.CurrentProfile = profile;
                    return true;
                }
                catch (ProfileNotFoundException)
                {
                    attempts++;
                    Console.WriteLine($"Неверный логин. Осталось попыток: {maxAttempts - attempts}");
                }
                catch (InvalidPasswordException)
                {
                    attempts++;
                    Console.WriteLine($"Неверный пароль. Осталось попыток: {maxAttempts - attempts}");
                }
            }

            Console.WriteLine("Превышено количество попыток входа.");
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

                if (_profileRepo.Exists(login))
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
                    Console.WriteLine("Неверный год рождения");
                    return false;
                }

                var profile = new Profile
                {
                    Login = login,
                    Password = password,
                    FirstName = firstName,
                    LastName = lastName,
                    BirthYear = birthYear
                };
                
                _profileRepo.Add(profile);
                _currentProfile = profile;
                AppInfo.CurrentProfile = profile;
                
                return true;
            }
            catch (DuplicateLoginException ex)
            {
                Console.WriteLine($"[ОШИБКА] {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ОШИБКА] {ex.Message}");
                return false;
            }
        }

        private static void MainLoop()
        {
            while (true)
            {
                try
                {
                    // Если нет профиля — всегда предлагаем вход/регистрацию
                    if (_currentProfile is null)
                    {
                        Console.WriteLine("\n=== НЕОБХОДИМА АВТОРИЗАЦИЯ ===");
                        if (!SelectOrCreateProfile())
                        {
                            Console.WriteLine("Выход из программы...");
                            return;
                        }
                        Console.WriteLine($"\nДобро пожаловать, {_currentProfile.FirstName}!\n");
                        continue;
                    }

                    Console.Write("> ");
                    string input = Console.ReadLine() ?? "";

                    if (input.ToLower() == "exit")
                    {
                        Console.WriteLine("До свидания!");
                        break;
                    }

                    // Обработка выхода из профиля
                    if (input.ToLower() == "profile -o" || input.ToLower() == "profile --out")
                    {
                        _currentProfile = null;
                        AppInfo.CurrentProfile = null;
                        AppInfo.ClearUndoRedo();
                        Console.WriteLine("Вы вышли из профиля.");
                        continue;
                    }

                    // Проверка авторизации для остальных команд
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