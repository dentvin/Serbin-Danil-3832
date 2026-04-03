using System;
using TodoApp.Exceptions;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class UndoCommand : ICommand
    {
        public void Execute()
        {
            if (AppInfo.UndoStack.Count == 0)
            {
                throw new EmptyStackException("Undo");
            }

            var command = AppInfo.UndoStack.Pop();
            command.Unexecute();
            AppInfo.RedoStack.Push(command);
        }
    }
}