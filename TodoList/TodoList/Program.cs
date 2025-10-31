using System;

class Program
{
    static void Main()
    {
        Console.Write("Введите имя: ");
        string firstName = Console.ReadLine();

        Console.Write("Введите фамилию: ");
        string lastName = Console.ReadLine();

        Console.Write("Введите год рождения: ");
        int birthYear = int.TryParse(Console.ReadLine(), out int year) ? year : 2000;

        var profile = new Profile { FirstName = firstName, LastName = lastName, BirthYear = birthYear };
        var todoList = new TodoList();

        while (true)
        {
            Console.Write("\nВведите команду: ");
            string input = Console.ReadLine();

            ICommand command = CommandParser.Parse(input, todoList, profile);
            command.Execute();

            if (command is ExitCommand) break;
        }
    }
}
