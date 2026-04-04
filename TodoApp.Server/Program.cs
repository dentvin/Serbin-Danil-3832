using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace TodoApp.Server
{
    class Program
    {
        private static readonly string _dataDirectory = "server_data";
        private static readonly string _profilesFile = Path.Combine(_dataDirectory, "server_profiles.dat");
        private const string Url = "http://localhost:5000/";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Запуск TodoApp Сервера...");
            Console.WriteLine($"Сервер слушает: {Url}");
            Console.WriteLine("Нажмите Ctrl+C для остановки\n");

            Directory.CreateDirectory(_dataDirectory);

            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(Url);
                listener.Start();

                while (true)
                {
                    try
                    {
                        var context = await listener.GetContextAsync();
                        _ = Task.Run(() => ProcessRequestAsync(context));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ОШИБКА] {ex.Message}");
                    }
                }
            }
        }

        private static async Task ProcessRequestAsync(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {request.HttpMethod} {request.Url.AbsolutePath}");

            try
            {
                if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/profiles")
                {
                    await HandleSaveProfiles(request, response);
                }
                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/profiles")
                {
                    await HandleLoadProfiles(response);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath.StartsWith("/todos/"))
                {
                    await HandleSaveTodos(request, response);
                }
                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath.StartsWith("/todos/"))
                {
                    await HandleLoadTodos(request, response);
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    using (var writer = new StreamWriter(response.OutputStream))
                    {
                        await writer.WriteAsync("Not Found");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ОШИБКА] {ex.Message}");
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                using (var writer = new StreamWriter(response.OutputStream))
                {
                    await writer.WriteAsync($"Error: {ex.Message}");
                }
            }
            finally
            {
                response.Close();
            }
        }

        private static async Task HandleSaveProfiles(HttpListenerRequest request, HttpListenerResponse response)
        {
            string filePath = _profilesFile;
            
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await request.InputStream.CopyToAsync(fileStream);
            }
            
            response.StatusCode = (int)HttpStatusCode.OK;
            using (var writer = new StreamWriter(response.OutputStream))
            {
                await writer.WriteAsync("OK");
            }
            Console.WriteLine($"  -> Сохранены профили ({new FileInfo(filePath).Length} байт)");
        }

        private static async Task HandleLoadProfiles(HttpListenerResponse response)
        {
            string filePath = _profilesFile;
            
            if (!File.Exists(filePath))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                using (var writer = new StreamWriter(response.OutputStream))
                {
                    await writer.WriteAsync("Not Found");
                }
                return;
            }
            
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(response.OutputStream);
            }
            
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "application/octet-stream";
            Console.WriteLine($"  -> Отправлены профили ({new FileInfo(filePath).Length} байт)");
        }

        private static async Task HandleSaveTodos(HttpListenerRequest request, HttpListenerResponse response)
        {
            string userId = request.Url.AbsolutePath.Split('/')[2];
            string filePath = Path.Combine(_dataDirectory, $"server_todos_{userId}.dat");
            
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await request.InputStream.CopyToAsync(fileStream);
            }
            
            response.StatusCode = (int)HttpStatusCode.OK;
            using (var writer = new StreamWriter(response.OutputStream))
            {
                await writer.WriteAsync("OK");
            }
            Console.WriteLine($"  -> Сохранены задачи пользователя {userId} ({new FileInfo(filePath).Length} байт)");
        }

        private static async Task HandleLoadTodos(HttpListenerRequest request, HttpListenerResponse response)
        {
            string userId = request.Url.AbsolutePath.Split('/')[2];
            string filePath = Path.Combine(_dataDirectory, $"server_todos_{userId}.dat");
            
            if (!File.Exists(filePath))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                using (var writer = new StreamWriter(response.OutputStream))
                {
                    await writer.WriteAsync("Not Found");
                }
                return;
            }
            
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(response.OutputStream);
            }
            
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "application/octet-stream";
            Console.WriteLine($"  -> Отправлены задачи пользователя {userId} ({new FileInfo(filePath).Length} байт)");
        }
    }
}