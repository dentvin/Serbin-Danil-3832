using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class AsyncDemoCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    ДЕМОНСТРАЦИЯ АСИНХРОННОСТИ И МНОГОПОТОЧНОСТИ                      ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════════════════╝\n");

            Console.WriteLine("📌 Асинхронность и многопоточность");
            Console.WriteLine("   Для полной демонстрации создайте файл AsyncDemoCommand.cs");
            Console.WriteLine("   или используйте команды: undo, redo, linq, error\n");
            
            Console.WriteLine("   Основные темы:");
            Console.WriteLine("   - Thread / ThreadPool");
            Console.WriteLine("   - Task / async / await");
            Console.WriteLine("   - Parallel");
            Console.WriteLine("   - lock / Mutex / Semaphore");
            Console.WriteLine("   - Concurrent коллекции");
            Console.WriteLine("   - CancellationToken / Таймаут");
            Console.WriteLine("   - I/O-bound vs CPU-bound\n");
            
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                           ДЕМОНСТРАЦИЯ ЗАВЕРШЕНА                                        ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════════════════╝\n");
        }
    }
}