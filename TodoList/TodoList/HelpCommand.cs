namespace TodoListApp;

public class HelpCommand : ICommand
{
    public void Execute()
    {
        Console.WriteLine("\nДоступные команды:");
        Console.WriteLine(" help                     - показать список команд");
        Console.WriteLine(" profile                  - показать данные пользователя");
        Console.WriteLine(" add \"текст\"              - добавить задачу");
        Console.WriteLine(" add -m / --multiline     - добавить многострочную задачу");
        Console.WriteLine(" view                     - показать задачи");
        Console.WriteLine(" view -i                  - показать индексы");
        Console.WriteLine(" view -s                  - показать статус");
        Console.WriteLine(" view -d                  - показать дату");
        Console.WriteLine(" view -a                  - показать всё");
        Console.WriteLine(" read <номер>             - полное описание задачи");
        Console.WriteLine(" update <номер> \"текст\"   - изменить задачу");
        Console.WriteLine(" done <номер>             - отметить выполненной");
        Console.WriteLine(" delete <номер>           - удалить задачу");
        Console.WriteLine(" exit                     - выход\n");
    }
}
