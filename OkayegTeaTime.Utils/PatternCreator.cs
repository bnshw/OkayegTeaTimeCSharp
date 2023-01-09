﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Files;

namespace OkayegTeaTime.Utils;

public static class PatternCreator
{
    private static readonly IDictionary<string, Regex> _cachedPatterns = new ConcurrentDictionary<string, Regex>();

    private const string _patternEnding = @"(\s|$)";

    public static unsafe Regex Create(string alias, string? prefix, [StringSyntax(StringSyntaxAttribute.Regex)] string? addition = null)
    {
        StringBuilder builder = stackalloc char[512];
        builder.Append('^');

        Span<string?> patternItems = new[]
        {
            prefix,
            alias,
            AppSettings.Suffix
        };
        bool isEmpty = string.IsNullOrEmpty(prefix);
        byte isEmptyAsByte = Unsafe.As<bool, byte>(ref isEmpty);
        builder.Append(patternItems[isEmptyAsByte], patternItems[++isEmptyAsByte]);
        builder.Append(addition, _patternEnding);
        string patternKey = builder.ToString();

        if (_cachedPatterns.TryGetValue(patternKey, out Regex? cachedPattern))
        {
            return cachedPattern;
        }

        Regex compiledRegex = new(patternKey, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
        _cachedPatterns.Add(patternKey, compiledRegex);
        return compiledRegex;
    }
}
