namespace TodoListApp;

public class ProfileCommand : ICommand
{
    public Profile Profile { get; set; }

    public void Execute()
    {
        if (Profile == null)
        {
            Console.WriteLine("Профиль недоступен.");
            return;
        }

        Console.WriteLine(Profile.GetInfo());
    }
}
