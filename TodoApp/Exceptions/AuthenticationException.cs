using System;

namespace TodoApp.Exceptions
{
    public class AuthenticationException : TodoAppException
    {
        public AuthenticationException() : base("Пользователь не авторизован. Сначала войдите в профиль (profile) или создайте новый.")
        {
        }

        public AuthenticationException(string message) : base(message)
        {
        }
    }
}