using System;

namespace TodoApp.Exceptions
{
    /// <summary>
    /// Исключение - неверный пароль
    /// </summary>
    public class InvalidPasswordException : ProfileException
    {
        public string Login { get; }
        
        public InvalidPasswordException(string login) 
            : base($"Неверный пароль для профиля '{login}'")
        {
            Login = login;
        }
    }
}