using System;

namespace TodoListApp
{
    public class Profile
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public int BirthYear { get; set; }

        public Profile() { }

        public Profile(string firstName, string lastName, int birthYear)
        {
            FirstName = firstName;
            LastName = lastName;
            BirthYear = birthYear;
        }

        public string GetInfo()
        {
            int age = DateTime.Now.Year - BirthYear;
            return $"{FirstName} {LastName}, возраст {age}";
        }
    }
}
