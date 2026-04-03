using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TodoApp.Commands;
using TodoApp.Exceptions;
using TodoApp.Models;

namespace TodoApp.Services
{
    public static class CommandParser
    {
        private static Dictionary<string, Func<string[], ICommand>> _commandHandlers;

        static CommandParser()
        {
            InitializeHandlers();
        }

        private static void InitializeHandlers()
        {
            _commandHandlers = new Dictionary<string, Func<string[], ICommand>>
            {
                ["help"] = args => new HelpCommand(),
                ["profile"] = args => ParseProfileCommand(args),
                ["add"] = args => ParseAddCommand(args),
                ["view"] = args => ParseViewCommand(args),
                ["read"] = args => ParseReadCommand(args),
                ["status"] = args => ParseStatusCommand(args),
                ["update"] = args => ParseUpdateCommand(args),
                ["delete"] = args => ParseDeleteCommand(args),
                ["undo"] = args => new UndoCommand(),
                ["redo"] = args => new RedoCommand(),
                ["linq"] = args => new LinqDemoCommand(),
                ["error"] = args => new ErrorDemoCommand(),
                ["load"] = args => ParseLoadCommand(args),
            };
        }

        public static ICommand Parse(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
            {
                throw new InvalidArgumentException("Пустая команда. Введите 'help' для справки.");
            }

            var parts = SplitCommand(inputString);
            if (parts.Length == 0)
            {
                throw new InvalidArgumentException("Пустая команда. Введите 'help' для справки.");
            }

            string command = parts[0].ToLower();
            var args = parts.Skip(1).ToArray();

            if (_commandHandlers.ContainsKey(command))
            {
                try
                {
                    return _commandHandlers[command](args);
                }
                catch (Exception ex)
                {
                    throw new InvalidCommandException(command, $"Ошибка выполнения: {ex.Message}");
                }
            }

            throw new InvalidCommandException(command);
        }

        private static ICommand ParseProfileCommand(string[] args)
        {
            bool logout = args.Any(a => a == "-o" || a == "--out");
            return new ProfileCommand(logout);
        }

        private static ICommand ParseAddCommand(string[] args)
        {
            bool isMultiline = args.Any(a => a == "-m" || a == "--multiline");

            if (isMultiline)
            {
                return new AddCommand("", true);
            }

            string text = string.Join(" ", args);
            text = text.Trim('"');

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidArgumentException("text", text, "Текст задачи не может быть пустым");
            }

            return new AddCommand(text, false);
        }

        private static ICommand ParseViewCommand(string[] args)
        {
            bool showIndex = args.Any(a => a == "-i" || a == "--index");
            bool showStatus = args.Any(a => a == "-s" || a == "--status");
            bool showDate = args.Any(a => a == "-d" || a == "--update-date");
            bool showAll = args.Any(a => a == "-a" || a == "--all");

            if (showAll)
                return new ViewCommand(true, true, true);

            return new ViewCommand(showIndex, showStatus, showDate);
        }

        private static ICommand ParseReadCommand(string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidArgumentException("index", "не указан", "Требуется указать индекс задачи");
            }

            if (!int.TryParse(args[0], out int index))
            {
                throw new InvalidArgumentException("index", args[0], "Индекс должен быть целым числом");
            }

            if (index < 0)
            {
                throw new InvalidArgumentException("index", index.ToString(), "Индекс не может быть отрицательным");
            }

            return new ReadCommand(index);
        }

        private static ICommand ParseStatusCommand(string[] args)
        {
            if (args.Length < 2)
            {
                throw new InvalidArgumentException("status", string.Join(" ", args), "Требуется: status <индекс> <статус>");
            }

            if (!int.TryParse(args[0], out int index))
            {
                throw new InvalidArgumentException("index", args[0], "Индекс должен быть целым числом");
            }

            if (index < 0)
            {
                throw new InvalidArgumentException("index", index.ToString(), "Индекс не может быть отрицательным");
            }

            string statusStr = args[1].ToLower();
            if (!Enum.TryParse<TodoStatus>(statusStr, ignoreCase: true, out var status))
            {
                throw new InvalidArgumentException("status", statusStr, "Доступные статусы: NotStarted, InProgress, Completed, Postponed, Failed");
            }

            return new StatusCommand(index, status);
        }

        private static ICommand ParseUpdateCommand(string[] args)
        {
            if (args.Length < 2)
            {
                throw new InvalidArgumentException("update", string.Join(" ", args), "Требуется: update <индекс> \"новый текст\"");
            }

            if (!int.TryParse(args[0], out int index))
            {
                throw new InvalidArgumentException("index", args[0], "Индекс должен быть целым числом");
            }

            if (index < 0)
            {
                throw new InvalidArgumentException("index", index.ToString(), "Индекс не может быть отрицательным");
            }

            string newText = string.Join(" ", args.Skip(1)).Trim('"');
            if (string.IsNullOrWhiteSpace(newText))
            {
                throw new InvalidArgumentException("text", newText, "Текст задачи не может быть пустым");
            }

            return new UpdateCommand(index, newText);
        }

        private static ICommand ParseDeleteCommand(string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidArgumentException("index", "не указан", "Требуется указать индекс задачи для удаления");
            }

            if (!int.TryParse(args[0], out int index))
            {
                throw new InvalidArgumentException("index", args[0], "Индекс должен быть целым числом");
            }

            if (index < 0)
            {
                throw new InvalidArgumentException("index", index.ToString(), "Индекс не может быть отрицательным");
            }

            return new DeleteCommand(index);
        }

        private static ICommand ParseLoadCommand(string[] args)
        {
            if (args.Length < 2)
            {
                throw new InvalidArgumentException("load", string.Join(" ", args), 
                    "Требуется: load <количество_скачиваний> <размер_скачиваний>");
            }
            
            if (!int.TryParse(args[0], out int downloadsCount))
            {
                throw new InvalidArgumentException("downloadsCount", args[0], 
                    "Количество скачиваний должно быть целым положительным числом");
            }
            
            if (!int.TryParse(args[1], out int downloadSize))
            {
                throw new InvalidArgumentException("downloadSize", args[1], 
                    "Размер скачиваний должен быть целым положительным числом");
            }
            
            if (downloadsCount <= 0)
            {
                throw new InvalidArgumentException("downloadsCount", downloadsCount.ToString(), 
                    "Количество скачиваний должно быть больше 0");
            }
            
            if (downloadSize <= 0)
            {
                throw new InvalidArgumentException("downloadSize", downloadSize.ToString(), 
                    "Размер скачиваний должен быть больше 0");
            }
            
            return new LoadCommand(downloadsCount, downloadSize);
        }

        private static string[] SplitCommand(string input)
        {
            var result = new List<string>();
            var regex = new Regex(@"[^\s""]+|""([^""]*)""");
            var matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                if (match.Groups[1].Success)
                {
                    result.Add(match.Groups[1].Value);
                }
                else
                {
                    result.Add(match.Value);
                }
            }

            return result.ToArray();
        }
    }
}