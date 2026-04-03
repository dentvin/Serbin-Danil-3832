using System;

namespace TodoApp.Exceptions
{
    public class InvalidCommandException : TodoAppException
    {
        public string CommandName { get; }

        public InvalidCommandException(string commandName) 
            : base($"Неизвестная команда: '{commandName}'. Введите 'help' для списка команд.")
        {
            CommandName = commandName;
        }

        public InvalidCommandException(string commandName, string message) 
            : base(message)
        {
            CommandName = commandName;
        }
    }
}