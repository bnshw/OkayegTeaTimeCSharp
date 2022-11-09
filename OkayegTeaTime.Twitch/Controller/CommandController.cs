﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using HLE.Collections;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class CommandController
{
    public Command this[CommandType type] => GetCommand(type);

    public AfkCommand this[AfkType type] => GetAfkCommand(type);

    public Command[] Commands { get; }

    public AfkCommand[] AfkCommands { get; }

    private readonly string[] _afkCommandAliases;

    public CommandController()
    {
        Commands = JsonController.GetCommandList().Commands.OrderBy(c => c.Name).ForEach(c => c.Aliases = c.Aliases.Order().ToArray()).ToArray();
        AfkCommands = JsonController.GetCommandList().AfkCommands.OrderBy(c => c.Name).ForEach(c => c.Aliases = c.Aliases.Order().ToArray()).ToArray();
        _afkCommandAliases = AfkCommands.SelectMany(c => c.Aliases).ToArray();
    }

    public bool IsAfkCommand(string? prefix, string message)
    {
        return _afkCommandAliases.Any(alias =>
        {
            Regex pattern = PatternCreator.Create(alias, prefix);
            return pattern.IsMatch(message);
        });
    }

    private Command GetCommand(CommandType type)
    {
        string typeName = type.ToString();
        return Commands.First(c => c.Name == typeName);
    }

    private AfkCommand GetAfkCommand(AfkType type)
    {
        string typeName = type.ToString().ToLower();
        return AfkCommands.First(c => c.Name == typeName);
    }
}
