using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Models
{
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Текст задачи обязателен")]
        [MaxLength(500, ErrorMessage = "Текст задачи не может быть длиннее 500 символов")]
        public string Text { get; set; } = string.Empty;

        public TodoStatus Status { get; set; }

        public DateTime LastUpdate { get; set; }

        // Внешний ключ
        public Guid ProfileId { get; set; }

        // Навигационное свойство
        [ForeignKey("ProfileId")]
        public virtual Profile Profile { get; set; } = null!;

        // Не сохраняется в БД
        [NotMapped]
        public string ShortText => Text.Length > 30 
            ? Text.Replace("\n", " ").Substring(0, 30) + "..." 
            : Text;

        public TodoItem()
        {
            Status = TodoStatus.NotStarted;
            LastUpdate = DateTime.Now;
        }

        public TodoItem(string text) : this()
        {
            Text = text;
        }

        public void UpdateText(string newText)
        {
            if (string.IsNullOrWhiteSpace(newText))
                throw new ArgumentException("Текст задачи не может быть пустым");
            
            Text = newText;
            LastUpdate = DateTime.Now;
        }

        public void SetStatus(TodoStatus status)
        {
            Status = status;
            LastUpdate = DateTime.Now;
        }

        public string GetShortInfo()
        {
            return ShortText;
        }

        public string GetFullInfo()
        {
            return $"ID: {Id}\nТекст: {Text}\nСтатус: {Status}\nПоследнее изменение: {LastUpdate:yyyy-MM-dd HH:mm:ss}";
        }
    }
}