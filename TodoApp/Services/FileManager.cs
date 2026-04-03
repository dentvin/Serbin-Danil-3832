using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TodoApp.Exceptions;
using TodoApp.Interfaces;
using TodoApp.Models;

namespace TodoApp.Services
{
    public class FileManager : IDataStorage
    {
        private readonly string _dataDirectory;
        private readonly string _profilesFile;

        public FileManager(string dataDirectory = "data")
        {
            _dataDirectory = dataDirectory;
            _profilesFile = Path.Combine(_dataDirectory, "profiles.enc");
            EnsureDataDirectory();
        }

        private void EnsureDataDirectory()
        {
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }
        }

        private string GetTodoFilePath(Guid userId)
        {
            return Path.Combine(_dataDirectory, $"todos_{userId}.enc");
        }

        public void SaveProfiles(IEnumerable<Profile> profiles)
        {
            try
            {
                using (var fileStream = new FileStream(_profilesFile, FileMode.Create, FileAccess.Write))
                using (var bufferedStream = new BufferedStream(fileStream, 8192))
                using (var cryptoStream = AesEncryption.CreateEncryptStream(bufferedStream))
                using (var writer = new StreamWriter(cryptoStream, Encoding.UTF8))
                {
                    foreach (var profile in profiles)
                    {
                        string line = $"{profile.Id}|{profile.Login}|{profile.Password}|{profile.FirstName}|{profile.LastName}|{profile.BirthYear}";
                        writer.WriteLine(line);
                    }
                }
            }
            catch (IOException ex)
            {
                throw new DataAccessException("Ошибка при сохранении профилей", _profilesFile, ex);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Неизвестная ошибка при сохранении профилей", _profilesFile, ex);
            }
        }

        public IEnumerable<Profile> LoadProfiles()
        {
            var profiles = new List<Profile>();

            if (!File.Exists(_profilesFile))
            {
                return profiles;
            }

            try
            {
                using (var fileStream = new FileStream(_profilesFile, FileMode.Open, FileAccess.Read))
                using (var bufferedStream = new BufferedStream(fileStream, 8192))
                using (var cryptoStream = AesEncryption.CreateDecryptStream(bufferedStream))
                using (var reader = new StreamReader(cryptoStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        var parts = line.Split('|');
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
            }
            catch (CryptographicException ex)
            {
                throw new DataAccessException("Ошибка расшифровки файла профилей", _profilesFile, ex);
            }
            catch (IOException ex)
            {
                throw new DataAccessException("Ошибка доступа к файлу профилей", _profilesFile, ex);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Неизвестная ошибка при загрузке профилей", _profilesFile, ex);
            }

            return profiles;
        }

        public void SaveTodos(Guid userId, IEnumerable<TodoItem> todos)
        {
            string filePath = GetTodoFilePath(userId);

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (var bufferedStream = new BufferedStream(fileStream, 8192))
                using (var cryptoStream = AesEncryption.CreateEncryptStream(bufferedStream))
                using (var writer = new StreamWriter(cryptoStream, Encoding.UTF8))
                {
                    foreach (var item in todos)
                    {
                        string escapedText = EscapeForCsv(item.Text);
                        string line = $"{escapedText}|{item.Status}|{item.LastUpdate:yyyy-MM-ddTHH:mm:ss}";
                        writer.WriteLine(line);
                    }
                }
            }
            catch (IOException ex)
            {
                throw new DataAccessException("Ошибка при сохранении задач", filePath, ex);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Неизвестная ошибка при сохранении задач", filePath, ex);
            }
        }

        public IEnumerable<TodoItem> LoadTodos(Guid userId)
        {
            var todos = new List<TodoItem>();
            string filePath = GetTodoFilePath(userId);

            if (!File.Exists(filePath))
            {
                return todos;
            }

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var bufferedStream = new BufferedStream(fileStream, 8192))
                using (var cryptoStream = AesEncryption.CreateDecryptStream(bufferedStream))
                using (var reader = new StreamReader(cryptoStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        var parts = line.Split('|');
                        if (parts.Length >= 3)
                        {
                            string text = UnescapeFromCsv(parts[0]);
                            if (Enum.TryParse<TodoStatus>(parts[1], out var status))
                            {
                                DateTime.TryParse(parts[2], out DateTime lastUpdate);

                                var item = new TodoItem(text)
                                {
                                    Status = status,
                                    LastUpdate = lastUpdate
                                };
                                todos.Add(item);
                            }
                        }
                    }
                }
            }
            catch (CryptographicException ex)
            {
                throw new DataAccessException("Ошибка расшифровки файла задач", filePath, ex);
            }
            catch (IOException ex)
            {
                throw new DataAccessException("Ошибка доступа к файлу задач", filePath, ex);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Неизвестная ошибка при загрузке задач", filePath, ex);
            }

            return todos;
        }

        public bool ProfilesExist()
        {
            return File.Exists(_profilesFile);
        }

        public bool TodosExist(Guid userId)
        {
            return File.Exists(GetTodoFilePath(userId));
        }

        private string EscapeForCsv(string text)
        {
            return text.Replace("|", "||").Replace("\n", "\\n");
        }

        private string UnescapeFromCsv(string text)
        {
            return text.Replace("||", "|").Replace("\\n", "\n");
        }
    }
}