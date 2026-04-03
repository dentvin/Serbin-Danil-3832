using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Exceptions;

namespace TodoApp.Commands
{
    public class LoadCommand : ICommand
    {
        private readonly int _downloadsCount;
        private readonly int _downloadSize;
        private static readonly object _consoleLock = new object();

        public LoadCommand(int downloadsCount, int downloadSize)
        {
            _downloadsCount = downloadsCount;
            _downloadSize = downloadSize;
        }

        public void Execute()
        {
            RunAsync().Wait();
        }

        private async Task RunAsync()
        {
            // Очищаем область под прогресс-бары
            int startRow = Console.CursorTop;
            
            // Проверяем, чтобы не выйти за пределы буфера
            if (startRow + _downloadsCount >= Console.BufferHeight - 1)
            {
                Console.WriteLine("\n⚠️ Недостаточно места в консоли. Увеличьте высоту буфера или уменьшите количество загрузок.\n");
                return;
            }
            
            // Резервируем строки под прогресс-бары
            for (int i = 0; i < _downloadsCount; i++)
            {
                Console.WriteLine();
            }
            
            // Создаём и запускаем задачи параллельно
            var tasks = new List<Task>();
            for (int i = 0; i < _downloadsCount; i++)
            {
                int index = i;
                int row = startRow + index;
                tasks.Add(DownloadAsync(index, row, _downloadSize));
            }
            
            // Ждём завершения всех задач
            await Task.WhenAll(tasks);
            
            // Перемещаем курсор в конец и выводим сообщение
            int finalRow = startRow + _downloadsCount;
            if (finalRow < Console.BufferHeight)
            {
                Console.SetCursorPosition(0, finalRow);
            }
            Console.WriteLine("\n✅ Все загрузки завершены.");
        }

        private async Task DownloadAsync(int index, int row, int totalSteps)
        {
            var random = new Random(index + DateTime.Now.Millisecond + Environment.TickCount);
            
            for (int step = 0; step <= totalSteps; step++)
            {
                // Вычисляем процент (0-100)
                int percent = (step * 100) / totalSteps;
                
                // Создаём прогресс-бар
                string bar = CreateProgressBar(percent);
                string output = $"Загрузка {index + 1}: {bar}";
                
                // Потокобезопасная запись в консоль
                lock (_consoleLock)
                {
                    try
                    {
                        // Проверяем, что строка в пределах буфера
                        if (row >= 0 && row < Console.BufferHeight)
                        {
                            Console.SetCursorPosition(0, row);
                            // Очищаем строку перед записью
                            Console.Write(new string(' ', Console.WindowWidth - 1));
                            Console.SetCursorPosition(0, row);
                            Console.Write(output);
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // Игнорируем ошибку выхода за пределы буфера
                    }
                }
                
                // Имитация загрузки со случайной задержкой (20-100 мс)
                if (step < totalSteps)
                {
                    await Task.Delay(random.Next(20, 100));
                }
            }
        }

        private string CreateProgressBar(int percent)
        {
            const int barLength = 20;
            int filledLength = (percent * barLength) / 100;
            
            // Ограничиваем значения
            filledLength = Math.Clamp(filledLength, 0, barLength);
            percent = Math.Clamp(percent, 0, 100);
            
            string filled = new string('#', filledLength);
            string empty = new string('-', barLength - filledLength);
            
            return $"[{filled}{empty}] {percent}%";
        }
    }
}