using System;

namespace TodoApp.Exceptions
{
    /// <summary>
    /// Исключение - профиль не найден
    /// </summary>
    public class ProfileNotFoundException : ProfileException
    {
        public string Login { get; }
        
        public ProfileNotFoundException(string login) 
            : base($"Профиль с логином '{login}' не найден")
        {
            Login = login;
        }
        
        public ProfileNotFoundException(string login, Exception innerException) 
            : base($"Профиль с логином '{login}' не найден", innerException)
        {
            Login = login;
        }
    }
}