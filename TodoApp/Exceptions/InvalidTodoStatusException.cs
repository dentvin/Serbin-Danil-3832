using System;

namespace TodoApp.Exceptions
{
    /// <summary>
    /// Исключение - неверный статус задачи
    /// </summary>
    public class InvalidTodoStatusException : TodoException
    {
        public string StatusValue { get; }
        
        public InvalidTodoStatusException(string statusValue) 
            : base($"Неверный статус задачи: '{statusValue}'. Доступные статусы: NotStarted, InProgress, Completed, Postponed, Failed")
        {
            StatusValue = statusValue;
        }
    }
}