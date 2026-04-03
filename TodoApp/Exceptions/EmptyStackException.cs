using System;

namespace TodoApp.Exceptions
{
    public class EmptyStackException : TodoAppException
    {
        public string StackName { get; }

        public EmptyStackException(string stackName) 
            : base($"Невозможно выполнить операцию: стек '{stackName}' пуст.")
        {
            StackName = stackName;
        }
    }
}