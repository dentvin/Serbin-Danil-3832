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
                ["load"] = args => new LoadCommand(
                    ParseInt(args, 0, "downloadsCount"),
                    ParseInt(args, 1, "downloadSize")),
                ["sync"] = args => ParseSyncCommand(args),
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

            int index = ParseInt(args, 0, "index");
            return new ReadCommand(index);
        }

        private static ICommand ParseStatusCommand(string[] args)
        {
            if (args.Length < 2)
            {
                throw new InvalidArgumentException("status", string.Join(" ", args), "Требуется: status <индекс> <статус>");
            }

            int index = ParseInt(args, 0, "index");

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

            int index = ParseInt(args, 0, "index");

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

            int index = ParseInt(args, 0, "index");
            return new DeleteCommand(index);
        }

        private static int ParseInt(string[] args, int position, string argumentName)
        {
            if (args.Length <= position)
            {
                throw new InvalidArgumentException(argumentName, "не указан", $"Требуется указать аргумент '{argumentName}'");
            }

            if (!int.TryParse(args[position], out int value))
            {
                throw new InvalidArgumentException(argumentName, args[position], "Значение должно быть целым числом");
            }

            if (value < 0)
            {
                throw new InvalidArgumentException(argumentName, value.ToString(), "Значение не может быть отрицательным");
            }

            return value;
        }

        private static ICommand ParseSyncCommand(string[] args)
        {
            bool pull = args.Contains("--pull");
            bool push = args.Contains("--push");

            if (!pull && !push)
            {
                throw new InvalidArgumentException("sync", string.Join(" ", args), "Требуется указать --pull или --push");
            }

            if (pull && push)
            {
                throw new InvalidArgumentException("sync", string.Join(" ", args), "Нельзя использовать --pull и --push одновременно");
            }

            return new SyncCommand(pull, push);
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