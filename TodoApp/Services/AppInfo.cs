using System;
using System.Collections.Generic;
using System.Linq;
using TodoApp.Commands;
using TodoApp.Interfaces;
using TodoApp.Models;

namespace TodoApp.Services
{
    public static class AppInfo
    {
        public static IDataStorage? Storage { get; set; }
        public static List<Profile> Profiles { get; set; } = new();
        public static Profile? CurrentProfile { get; set; }
        public static Dictionary<Guid, TodoList> UserTodos { get; set; } = new();
        public static Stack<IUndoableCommand> UndoStack { get; set; } = new();
        public static Stack<IUndoableCommand> RedoStack { get; set; } = new();

        public static TodoList? GetCurrentTodoList()
        {
            if (CurrentProfile != null && UserTodos.ContainsKey(CurrentProfile.Id))
            {
                return UserTodos[CurrentProfile.Id];
            }
            return null;
        }

        public static void ClearUndoRedo()
        {
            UndoStack.Clear();
            RedoStack.Clear();
        }

        // Сохранение всех данных
        public static void SaveAllData()
        {
            if (Storage == null) return;

            Storage.SaveProfiles(Profiles);
            
            foreach (var userTodo in UserTodos)
            {
                Storage.SaveTodos(userTodo.Key, userTodo.Value.GetAll());
            }
        }

        // Загрузка всех данных
        public static void LoadAllData()
        {
            if (Storage == null) return;

            Profiles = Storage.LoadProfiles().ToList();
            
            foreach (var profile in Profiles)
            {
                var todos = Storage.LoadTodos(profile.Id).ToList();
                var todoList = new TodoList();
                foreach (var item in todos)
                {
                    todoList.Add(item);
                }
                UserTodos[profile.Id] = todoList;
            }
        }
    }
}