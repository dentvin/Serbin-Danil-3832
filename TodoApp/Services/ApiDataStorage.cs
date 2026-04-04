using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TodoApp.Exceptions;
using TodoApp.Interfaces;
using TodoApp.Models;

namespace TodoApp.Services
{
    public class ApiDataStorage : IDataStorage
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public ApiDataStorage(string baseUrl = "http://localhost:5000/")
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public void SaveProfiles(IEnumerable<Profile> profiles)
        {
            var task = Task.Run(() => SaveProfilesAsync(profiles));
            task.Wait();
        }

        public IEnumerable<Profile> LoadProfiles()
        {
            var task = Task.Run(() => LoadProfilesAsync());
            return task.Result;
        }

        public void SaveTodos(Guid userId, IEnumerable<TodoItem> todos)
        {
            var task = Task.Run(() => SaveTodosAsync(userId, todos));
            task.Wait();
        }

        public IEnumerable<TodoItem> LoadTodos(Guid userId)
        {
            var task = Task.Run(() => LoadTodosAsync(userId));
            return task.Result;
        }

        public bool ProfilesExist()
        {
            var task = Task.Run(() => ProfilesExistAsync());
            return task.Result;
        }

        public bool TodosExist(Guid userId)
        {
            var task = Task.Run(() => TodosExistAsync(userId));
            return task.Result;
        }

        private async Task SaveProfilesAsync(IEnumerable<Profile> profiles)
        {
            try
            {
                // 1. Сериализация в JSON
                string json = JsonSerializer.Serialize(profiles);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

                // 2. Шифрование
                byte[] encryptedData = AesEncryption.EncryptBytes(jsonBytes);

                // 3. Отправка на сервер
                using var content = new ByteArrayContent(encryptedData);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                
                var response = await _httpClient.PostAsync(_baseUrl + "profiles", content);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new DataAccessException("Не удалось подключиться к серверу", _baseUrl, ex);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Ошибка при отправке профилей на сервер", _baseUrl, ex);
            }
        }

        private async Task<IEnumerable<Profile>> LoadProfilesAsync()
        {
            try
            {
                // 1. Получение данных с сервера
                var response = await _httpClient.GetAsync(_baseUrl + "profiles");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new List<Profile>();
                }
                
                response.EnsureSuccessStatusCode();
                
                byte[] encryptedData = await response.Content.ReadAsByteArrayAsync();
                
                // 2. Расшифровка
                byte[] jsonBytes = AesEncryption.DecryptBytes(encryptedData);
                
                // 3. Десериализация
                string json = Encoding.UTF8.GetString(jsonBytes);
                return JsonSerializer.Deserialize<List<Profile>>(json) ?? new List<Profile>();
            }
            catch (HttpRequestException ex)
            {
                throw new DataAccessException("Не удалось подключиться к серверу", _baseUrl, ex);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Ошибка при загрузке профилей с сервера", _baseUrl, ex);
            }
        }

        private async Task SaveTodosAsync(Guid userId, IEnumerable<TodoItem> todos)
        {
            try
            {
                // 1. Сериализация в JSON
                string json = JsonSerializer.Serialize(todos);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

                // 2. Шифрование
                byte[] encryptedData = AesEncryption.EncryptBytes(jsonBytes);

                // 3. Отправка на сервер
                using var content = new ByteArrayContent(encryptedData);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                
                var response = await _httpClient.PostAsync(_baseUrl + $"todos/{userId}", content);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new DataAccessException("Не удалось подключиться к серверу", _baseUrl, ex);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Ошибка при отправке задач на сервер", _baseUrl, ex);
            }
        }

        private async Task<IEnumerable<TodoItem>> LoadTodosAsync(Guid userId)
        {
            try
            {
                // 1. Получение данных с сервера
                var response = await _httpClient.GetAsync(_baseUrl + $"todos/{userId}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new List<TodoItem>();
                }
                
                response.EnsureSuccessStatusCode();
                
                byte[] encryptedData = await response.Content.ReadAsByteArrayAsync();
                
                // 2. Расшифровка
                byte[] jsonBytes = AesEncryption.DecryptBytes(encryptedData);
                
                // 3. Десериализация
                string json = Encoding.UTF8.GetString(jsonBytes);
                return JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
            }
            catch (HttpRequestException ex)
            {
                throw new DataAccessException("Не удалось подключиться к серверу", _baseUrl, ex);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Ошибка при загрузке задач с сервера", _baseUrl, ex);
            }
        }

        private async Task<bool> ProfilesExistAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl + "profiles");
                return response.StatusCode != System.Net.HttpStatusCode.NotFound && response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> TodosExistAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl + $"todos/{userId}");
                return response.StatusCode != System.Net.HttpStatusCode.NotFound && response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}