using System;

namespace TodoApp.Exceptions
{
    /// <summary>
    /// Исключение для ошибок при работе с файлами (CSV)
    /// </summary>
    public class DataAccessException : TodoAppException
    {
        public string FilePath { get; }
        
        public DataAccessException(string message, string filePath) 
            : base(message)
        {
            FilePath = filePath;
        }
        
        public DataAccessException(string message, string filePath, Exception innerException) 
            : base(message, innerException)
        {
            FilePath = filePath;
        }
    }
}