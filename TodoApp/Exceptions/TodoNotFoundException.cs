using System;

namespace TodoApp.Exceptions
{
    /// <summary>
    /// Исключение - задача не найдена
    /// </summary>
    public class TodoNotFoundException : TodoException
    {
        public int Index { get; }
        
        public TodoNotFoundException(int index) 
            : base($"Задача с индексом {index} не найдена")
        {
            Index = index;
        }
    }
}