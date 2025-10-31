using System;

public class Profile
{
    private string firstName;
    private string lastName;
    private int birthYear;

    public Profile(string firstName, string lastName, int birthYear)
    {
        this.firstName = firstName;
        this.lastName = lastName;
        this.birthYear = birthYear;
    }

    public string GetInfo()
    {
        int age = DateTime.Now.Year - birthYear;
        return $"{firstName} {lastName}, возраст {age}";
    }
}
