using System;

namespace TodoApp.Exceptions
{
    /// <summary>
    /// Исключение для ошибок, связанных с профилями пользователей
    /// </summary>
    public class ProfileException : TodoAppException
    {
        public ProfileException() { }

        public ProfileException(string message) : base(message) { }

        public ProfileException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}