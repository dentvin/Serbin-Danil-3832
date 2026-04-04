using System;
using System.Collections.Generic;
using System.Linq;
using TodoApp.Data;
using TodoApp.Exceptions;
using TodoApp.Models;

namespace TodoApp.Services
{
    public class ProfileRepository
    {
        public List<Profile> GetAll()
        {
            using var context = new AppDbContext();
            return context.Profiles.ToList();
        }

        public Profile GetById(Guid id)
        {
            using var context = new AppDbContext();
            var profile = context.Profiles.FirstOrDefault(p => p.Id == id);
            if (profile == null)
                throw new ProfileNotFoundException(id.ToString());
            return profile;
        }

        public Profile GetByLogin(string login)
        {
            using var context = new AppDbContext();
            var profile = context.Profiles.FirstOrDefault(p => p.Login == login);
            if (profile == null)
                throw new ProfileNotFoundException(login);
            return profile;
        }

        public void Add(Profile profile)
        {
            using var context = new AppDbContext();
            if (context.Profiles.Any(p => p.Login == profile.Login))
                throw new DuplicateLoginException(profile.Login);
            
            context.Profiles.Add(profile);
            context.SaveChanges();
        }

        public void Update(Profile profile)
        {
            using var context = new AppDbContext();
            context.Profiles.Update(profile);
            context.SaveChanges();
        }

        public bool Exists(string login)
        {
            using var context = new AppDbContext();
            return context.Profiles.Any(p => p.Login == login);
        }

        public bool Exists(Guid id)
        {
            using var context = new AppDbContext();
            return context.Profiles.Any(p => p.Id == id);
        }
    }
}