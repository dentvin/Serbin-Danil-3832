const int INITIAL_CAPACITY = 2;

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
        case "help": ShowHelp(); break;
        case "profile": ShowProfile(userFirstName, userLastName, userBirthYear); break;
        case "exit": return;
        default: Console.WriteLine("Неизвестная команда."); break;
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
    Console.WriteLine("exit — выйти из программы");
}

static void ShowProfile(string firstName, string lastName, int birthYear)
{
    Console.WriteLine($"{firstName} {lastName}, {birthYear}");
}
