using System;

class Program
{
    static void Main()
    {
        // Данные пользователя
        Console.Write("Введите имя: ");
        string firstName = Console.ReadLine();

        Console.Write("Введите фамилию: ");
        string lastName = Console.ReadLine();

        Console.Write("Введите год рождения: ");
        int birthYear = int.Parse(Console.ReadLine());

        // Массив задач
        string[] todos = new string[2]; // начальная длина
        int todoCount = 0;

        Console.WriteLine("\nПрограмма TodoList запущена! Введите help для списка команд.");
        while (true)
        {
            Console.Write("\nВведите команду (help для списка команд): ");
            string command = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(command))
                continue;

            string cmd = command.Split(' ')[0].ToLower();

            switch (cmd)
            {
                case "help":
                    Console.WriteLine("Доступные команды:");
                    Console.WriteLine("help — показать список команд");
                    Console.WriteLine("profile — показать данные пользователя");
                    Console.WriteLine("add \"текст задачи\" — добавить новую задачу");
                    Console.WriteLine("view — показать все задачи");
                    Console.WriteLine("exit — выйти из программы");
                    break;

                case "profile":
                    Console.WriteLine($"{firstName} {lastName}, {birthYear}");
                    break;

                default:
                    Console.WriteLine("Неизвестная команда. Введите help для списка команд.");
                    break;
                case "add":
                    if (command.Length <= 4)
                    {
                        Console.WriteLine("Ошибка: текст задачи не указан.");
                        break;
                    }
                    string taskText = command.Substring(4).Trim(); // всё после "add"
                    // проверка, хватает ли места в массиве
                    if (todoCount >= todos.Length)
                    {
                        string[] newTodos = new string[todos.Length * 2]; // новый массив в 2 раза больше
                        for (int i = 0; i < todos.Length; i++)
                            newTodos[i] = todos[i];
                        todos = newTodos;
                    }
                    todos[todoCount] = taskText;
                    todoCount++;
                    Console.WriteLine("Задача добавлена!");
                    break;
            }
        }

    }
}
