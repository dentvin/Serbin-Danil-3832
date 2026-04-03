using System;

namespace TodoApp.Exceptions
{
    public class InvalidArgumentException : TodoAppException
    {
        public string? ArgumentName { get; }
        public string? ArgumentValue { get; }

        public InvalidArgumentException(string argumentName, string argumentValue, string message) 
            : base($"Ошибка в аргументе '{argumentName}': {message}. Получено: '{argumentValue}'")
        {
            ArgumentName = argumentName;
            ArgumentValue = argumentValue;
        }

        public InvalidArgumentException(string message) : base(message)
        {
            ArgumentName = null;
            ArgumentValue = null;
        }
    }
}