using System;

class Program
{
    const int INITIAL_CAPACITY = 2;

    static void Main()
    {
        string userFirstName = GetInput("Введите имя: ");
        string userLastName = GetInput("Введите фамилию: ");
        int userBirthYear = int.Parse(GetInput("Введите год рождения: "));

        string[] tasks = new string[INITIAL_CAPACITY];
        int taskCount = 0;

        while (true)
        {
            string command = GetInput("\nВведите команду (help для списка команд): ");
            if (string.IsNullOrWhiteSpace(command))
                continue;

            string cmd = command.Split(' ')[0].ToLower();

            switch (cmd)
            {
                case "help":
                    ShowHelp();
                    break;
                case "profile":
                    ShowProfile(userFirstName, userLastName, userBirthYear);
                    break;
                case "add":
                    AddTask(command, ref tasks, ref taskCount);
                    break;
                case "exit":
                    Console.WriteLine("Программа завершена.");
                    return;
                default:
                    Console.WriteLine("Неизвестная команда.");
                    break;
            }
        }
    }

    static string GetInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    static void ShowHelp()
    {
        Console.WriteLine("Доступные команды:");
        Console.WriteLine("help — показать список команд");
        Console.WriteLine("profile — показать данные пользователя");
        Console.WriteLine("add \"текст задачи\" — добавить новую задачу");
        Console.WriteLine("exit — выйти из программы");
    }

    static void ShowProfile(string firstName, string lastName, int birthYear)
    {
        Console.WriteLine($"{firstName} {lastName}, {birthYear}");
    }

    static void AddTask(string command, ref string[] tasks, ref int taskCount)
    {
        if (command.Length <= 4)
        {
            Console.WriteLine("Ошибка: текст задачи не указан.");
            return;
        }

        string taskText = command.Substring(4).Trim();

        EnsureCapacity(ref tasks, taskCount);

        tasks[taskCount] = taskText;
        taskCount++;
        Console.WriteLine("Задача добавлена!");
    }

    static void EnsureCapacity(ref string[] tasks, int taskCount)
    {
        if (taskCount >= tasks.Length)
        {
            string[] newTasks = new string[tasks.Length * 2];
            for (int i = 0; i < tasks.Length; i++)
                newTasks[i] = tasks[i];
            tasks = newTasks;
        }
    }
}
