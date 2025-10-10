using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Работу выполнили Сербин Данил еееее");

        // Запрос данных пользователя
        Console.Write("Введите имя: ");
        string firstName = Console.ReadLine();

        Console.Write("Введите фамилию: ");
        string lastName = Console.ReadLine();

        Console.Write("Введите год рождения: ");
        int birthYear = int.Parse(Console.ReadLine());

        // Рассчёт возраста
        int currentYear = DateTime.Now.Year;
        int age = currentYear - birthYear;

        Console.WriteLine($"Добавлен пользователь {firstName} {lastName}, возраст - {age}");
    }
}
