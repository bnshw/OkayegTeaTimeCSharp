﻿using System;
using System.Collections.Concurrent;
using HLE.Collections;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class CooldownController(CommandController commandController)
{
    private readonly ConcurrentDictionary<CooldownHash, DateTime> _cooldowns = new();
    private readonly ConcurrentDictionary<CooldownHash, DateTime> _afkCooldowns = new();
    private readonly CommandController _commandController = commandController;

    public void AddCooldown(long userId, CommandType type)
    {
#if DEBUG
        if (GlobalSettings.Settings.Users.Moderators.Contains(userId))
        {
            return;
        }
#endif

        CooldownHash cooldownHash = new(userId, type);
        TimeSpan cooldownTime = TimeSpan.FromMilliseconds(_commandController.GetCommand(type).Cooldown);
        DateTime cooldownUntil = DateTime.UtcNow + cooldownTime;
        _cooldowns.AddOrSet(cooldownHash, cooldownUntil);
    }

    public void AddAfkCooldown(long userId)
    {
#if DEBUG
        if (GlobalSettings.Settings.Users.Moderators.Contains(userId))
        {
            return;
        }
#endif

        CooldownHash cooldownHash = new(userId);
        TimeSpan cooldownTime = TimeSpan.FromMilliseconds(GlobalSettings.AfkCooldown);
        DateTime cooldownUntil = DateTime.UtcNow + cooldownTime;
        _afkCooldowns.AddOrSet(cooldownHash, cooldownUntil);
    }

    public bool IsOnCooldown(long userId, CommandType type)
    {
        CooldownHash cooldownHash = new(userId, type);
        return _cooldowns.TryGetValue(cooldownHash, out DateTime cooldownUntil) && cooldownUntil > DateTime.UtcNow;
    }

    public bool IsOnAfkCooldown(long userId)
    {
        CooldownHash cooldownHash = new(userId);
        return _afkCooldowns.TryGetValue(cooldownHash, out DateTime cooldownUntil) && cooldownUntil > DateTime.UtcNow;
    }
}
