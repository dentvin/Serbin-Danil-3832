using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TodoApp.Exceptions;
using TodoApp.Models;

namespace TodoApp.Services
{
    public static class FileManager
    {
        private const string DataDir = "data";
        private const string ProfilesFileName = "profiles.csv";

        public static void EnsureDataDirectory()
        {
            if (!Directory.Exists(DataDir))
            {
                try
                {
                    Directory.CreateDirectory(DataDir);
                }
                catch (Exception ex)
                {
                    throw new DataAccessException("Не удалось создать папку для данных", DataDir, ex);
                }
            }
        }

        public static void SaveProfile(Profile profile)
        {
            try
            {
                EnsureDataDirectory();
                string filePath = Path.Combine(DataDir, ProfilesFileName);

                var profiles = LoadAllProfiles();
                var existing = profiles.FirstOrDefault(p => p.Id == profile.Id);

                if (existing != null)
                {
                    profiles.Remove(existing);
                }
                profiles.Add(profile);

                SaveAllProfiles(profiles, filePath);
            }
            catch (DataAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Ошибка при сохранении профиля", DataDir, ex);
            }
        }

        public static Profile LoadProfile(string login, string password)
        {
            try
            {
                var profiles = LoadAllProfiles();
                var profile = profiles.FirstOrDefault(p => p.Login == login);
                
                if (profile == null)
                {
                    throw new ProfileNotFoundException(login);
                }
                
                if (profile.Password != password)
                {
                    throw new InvalidPasswordException(login);
                }
                
                return profile;
            }
            catch (ProfileException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"Ошибка при загрузке профиля '{login}'", DataDir, ex);
            }
        }

        public static List<Profile> LoadAllProfiles()
        {
            EnsureDataDirectory();
            string filePath = Path.Combine(DataDir, ProfilesFileName);
            var profiles = new List<Profile>();

            try
            {
                if (!File.Exists(filePath))
                {
                    return profiles;
                }

                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split(';');
                    if (parts.Length == 6)
                    {
                        var profile = new Profile
                        {
                            Id = Guid.Parse(parts[0]),
                            Login = parts[1],
                            Password = parts[2],
                            FirstName = parts[3],
                            LastName = parts[4],
                            BirthYear = int.Parse(parts[5])
                        };
                        profiles.Add(profile);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Ошибка при загрузке всех профилей", filePath, ex);
            }

            return profiles;
        }

        private static void SaveAllProfiles(List<Profile> profiles, string filePath)
        {
            try
            {
                var lines = new List<string>();
                foreach (var profile in profiles)
                {
                    string line = $"{profile.Id};{profile.Login};{profile.Password};{profile.FirstName};{profile.LastName};{profile.BirthYear}";
                    lines.Add(line);
                }
                File.WriteAllLines(filePath, lines);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Ошибка при сохранении профилей", filePath, ex);
            }
        }

        public static void SaveTodos(TodoList todos, string filePath)
        {
            try
            {
                var lines = new List<string>();
                int index = 0;

                foreach (var item in todos.GetAll())
                {
                    string escapedText = EscapeCsv(item.Text);
                    string line = $"{index};{escapedText};{item.Status};{item.LastUpdate:yyyy-MM-ddTHH:mm:ss}";
                    lines.Add(line);
                    index++;
                }

                File.WriteAllLines(filePath, lines);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Ошибка при сохранении задач", filePath, ex);
            }
        }

        public static TodoList LoadTodos(string filePath)
        {
            var todos = new TodoList();

            try
            {
                if (!File.Exists(filePath))
                {
                    return todos;
                }

                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = ParseCsvLine(line);
                    if (parts.Count >= 4)
                    {
                        string text = UnescapeCsv(parts[1]);
                        var status = Enum.Parse<TodoStatus>(parts[2]);
                        DateTime.TryParse(parts[3], out DateTime lastUpdate);

                        var item = new TodoItem(text)
                        {
                            Status = status,
                            LastUpdate = lastUpdate
                        };
                        todos.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Ошибка при загрузке задач", filePath, ex);
            }

            return todos;
        }

        private static string EscapeCsv(string text)
        {
            return "\"" + text.Replace("\"", "\"\"").Replace("\n", "\\n") + "\"";
        }

        private static string UnescapeCsv(string text)
        {
            return text.Trim('"').Replace("\\n", "\n").Replace("\"\"", "\"");
        }

        private static List<string> ParseCsvLine(string line)
        {
            var parts = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    current.Append(c);
                }
                else if (c == ';' && !inQuotes)
                {
                    parts.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            parts.Add(current.ToString());
            return parts;
        }

        public static string GetTodoFilePath(Guid profileId)
        {
            return Path.Combine(DataDir, $"todos_{profileId}.csv");
        }

        public static void SaveTodoList(TodoItem item)
        {
            try
            {
                var profile = AppInfo.CurrentProfile;
                if (profile != null)
                {
                    var todoList = AppInfo.GetCurrentTodoList();
                    if (todoList != null)
                    {
                        string filePath = GetTodoFilePath(profile.Id);
                        SaveTodos(todoList, filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ОШИБКА] Не удалось сохранить изменения: {ex.Message}");
            }
        }
    }
}