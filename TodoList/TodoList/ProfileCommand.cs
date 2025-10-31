using System;

public class ProfileCommand : ICommand
{
    public Profile Profile { get; set; }
    public void Execute()
    {
        Console.WriteLine(Profile.GetInfo());
    }
}
