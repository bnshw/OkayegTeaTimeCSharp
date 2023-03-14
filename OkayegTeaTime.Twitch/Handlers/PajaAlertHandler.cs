﻿using System.Text.RegularExpressions;
using HLE.Emojis;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class PajaAlertHandler : PajaHandler
{
    protected override Regex Pattern { get; } = Utils.Pattern.PajaAlert;

    protected override string Message => $"/me pajaStare {Emoji.RotatingLight} OBACHT";

    public PajaAlertHandler(TwitchBot twitchBot) : base(twitchBot)
    {
    }

    public override void Handle(ChatMessage chatMessage)
    {
        if (chatMessage.ChannelId != _pajaChannelId || chatMessage.UserId != _pajaAlertUserId || !Pattern.IsMatch(chatMessage.Message))
        {
            return;
        }

        _twitchBot.Send(_pajaAlertChannel, Message, false, false, false);
        _twitchBot.Send(AppSettings.OfflineChatChannel, $"{AppSettings.DefaultEmote} {Emoji.RotatingLight}", false, false, false);
    }
}
