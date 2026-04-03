using System;

namespace TodoApp.Exceptions
{
    /// <summary>
    /// Базовое исключение для всего приложения
    /// </summary>
    public class TodoAppException : Exception
    {
        public TodoAppException() { }

        public TodoAppException(string message) : base(message) { }

        public TodoAppException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}