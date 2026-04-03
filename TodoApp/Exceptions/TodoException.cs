using System;

namespace TodoApp.Exceptions
{
    /// <summary>
    /// Исключение для ошибок, связанных с задачами (Todo)
    /// </summary>
    public class TodoException : TodoAppException
    {
        public TodoException() { }
        
        public TodoException(string message) : base(message) { }
        
        public TodoException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}