using System;

namespace TodoApp.Exceptions
{
    public class DuplicateLoginException : ProfileException
    {
        public string Login { get; }

        public DuplicateLoginException(string login) 
            : base($"Логин '{login}' уже существует. Пожалуйста, выберите другой логин.")
        {
            Login = login;
        }
    }
}