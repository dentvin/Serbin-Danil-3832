using System;
using System.Collections.Generic;
using System.Linq;
using TodoApp.Models;
using System.Net.Http;
using System.Threading.Tasks;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class SyncCommand : ICommand
    {
        private readonly bool _pull;
        private readonly bool _push;

        public SyncCommand(bool pull, bool push)
        {
            _pull = pull;
            _push = push;
        }

        public void Execute()
        {
            RunAsync().Wait();
        }

        private async Task RunAsync()
        {
            Console.WriteLine("\n🔄 Синхронизация с сервером...\n");

            // Проверка доступности сервера
            if (!await IsServerAvailable())
            {
                Console.WriteLine("❌ Ошибка: сервер недоступен.");
                Console.WriteLine("   Запустите TodoApp.Server перед синхронизацией.\n");
                return;
            }

            Console.WriteLine("✅ Сервер доступен\n");

            var apiStorage = new ApiDataStorage();

            if (_pull)
            {
                Console.WriteLine("📥 Режим PULL - загрузка данных с сервера...\n");
                
                try
                {
                    // Загрузка профилей
                    var profiles = apiStorage.LoadProfiles();
                    AppInfo.Profiles = new List<Profile>(profiles);
                    Console.WriteLine($"   ✅ Загружено профилей: {AppInfo.Profiles.Count}");
                    
                    // Загрузка задач для каждого профиля
                    foreach (var profile in AppInfo.Profiles)
                    {
                        var todos = apiStorage.LoadTodos(profile.Id);
                        var todoList = new TodoList();
                        foreach (var item in todos)
                        {
                            todoList.Add(item);
                        }
                        AppInfo.UserTodos[profile.Id] = todoList;
                        Console.WriteLine($"   ✅ Загружены задачи для {profile.Login}: {todos.Count()}");
                    }
                    
                    Console.WriteLine("\n📌 Данные успешно загружены с сервера!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ Ошибка при загрузке: {ex.Message}");
                }
            }
            else if (_push)
            {
                Console.WriteLine("📤 Режим PUSH - отправка данных на сервер...\n");
                
                try
                {
                    // Отправка профилей
                    apiStorage.SaveProfiles(AppInfo.Profiles);
                    Console.WriteLine($"   ✅ Отправлено профилей: {AppInfo.Profiles.Count}");
                    
                    // Отправка задач для каждого профиля
                    foreach (var profile in AppInfo.Profiles)
                    {
                        var todoList = AppInfo.UserTodos.ContainsKey(profile.Id) 
                            ? AppInfo.UserTodos[profile.Id] 
                            : new TodoList();
                        apiStorage.SaveTodos(profile.Id, todoList.GetAll());
                        Console.WriteLine($"   ✅ Отправлены задачи для {profile.Login}: {todoList.Count}");
                    }
                    
                    Console.WriteLine("\n📌 Данные успешно отправлены на сервер!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ Ошибка при отправке: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("⚠️ Используйте: sync --pull или sync --push");
            }
            
            Console.WriteLine();
        }

        private async Task<bool> IsServerAvailable()
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(2);
                var response = await client.GetAsync("http://localhost:5000/profiles");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}