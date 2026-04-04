using System;
using System.Collections.Generic;
using System.Linq;
using TodoApp.Data;
using TodoApp.Exceptions;
using TodoApp.Models;

namespace TodoApp.Services
{
    public class TodoRepository
    {
        public List<TodoItem> GetAll(Guid profileId)
        {
            using var context = new AppDbContext();
            return context.Todos
                .Where(t => t.ProfileId == profileId)
                .OrderBy(t => t.Id)
                .ToList();
        }

        public TodoItem GetById(int id, Guid profileId)
        {
            using var context = new AppDbContext();
            var item = context.Todos.FirstOrDefault(t => t.Id == id && t.ProfileId == profileId);
            if (item == null)
                throw new TodoNotFoundException(id);
            return item;
        }

        public void Add(TodoItem item, Guid profileId)
        {
            using var context = new AppDbContext();
            item.ProfileId = profileId;
            context.Todos.Add(item);
            context.SaveChanges();
        }

        public void Update(int id, string newText, Guid profileId)
        {
            using var context = new AppDbContext();
            var item = GetById(id, profileId);
            item.UpdateText(newText);
            context.SaveChanges();
        }

        public void Delete(int id, Guid profileId)
        {
            using var context = new AppDbContext();
            var item = GetById(id, profileId);
            context.Todos.Remove(item);
            context.SaveChanges();
        }

        public void SetStatus(int id, TodoStatus status, Guid profileId)
        {
            using var context = new AppDbContext();
            var item = GetById(id, profileId);
            item.SetStatus(status);
            context.SaveChanges();
        }

        public int GetCount(Guid profileId)
        {
            using var context = new AppDbContext();
            return context.Todos.Count(t => t.ProfileId == profileId);
        }
    }
}