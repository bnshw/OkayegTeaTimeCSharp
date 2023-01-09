﻿using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Web;
using HLE;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Math)]
public readonly unsafe ref struct MathCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public MathCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, GetMathResult(ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..]));
        }
    }

    private static string GetMathResult(string expression)
    {
        HttpGet request = new($"https://api.mathjs.org/v4/?expr={HttpUtility.UrlEncode(expression)}");
        return request.Result ?? PredefinedMessages.ApiError;
    }
}
