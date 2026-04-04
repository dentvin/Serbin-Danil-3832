using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class Profile
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Логин обязателен")]
        [MaxLength(50, ErrorMessage = "Логин не может быть длиннее 50 символов")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [MaxLength(100, ErrorMessage = "Пароль не может быть длиннее 100 символов")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Имя обязательно")]
        [MaxLength(50, ErrorMessage = "Имя не может быть длиннее 50 символов")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Фамилия обязательна")]
        [MaxLength(50, ErrorMessage = "Фамилия не может быть длиннее 50 символов")]
        public string LastName { get; set; } = string.Empty;

        [Range(1900, 2025, ErrorMessage = "Год рождения должен быть между 1900 и 2025")]
        public int BirthYear { get; set; }

        // Навигационное свойство (один ко многим)
        public virtual ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();

        // Конструктор по умолчанию
        public Profile()
        {
            Id = Guid.NewGuid();
        }

        public string GetInfo()
        {
            int age = DateTime.Now.Year - BirthYear;
            return $"{FirstName} {LastName}, {age} лет";
        }
    }
}