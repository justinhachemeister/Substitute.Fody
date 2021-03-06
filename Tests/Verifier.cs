﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

using JetBrains.Annotations;

using NUnit.Framework;

public static class Verifier
{
    public static void Verify([NotNull] string beforeAssemblyPath, [NotNull] string afterAssemblyPath)
    {
        var before = Validate(beforeAssemblyPath);
        var after = Validate(afterAssemblyPath);
        var message = $"Failed processing {Path.GetFileName(afterAssemblyPath)}\r\n{after}";
        Assert.AreEqual(TrimLineNumbers(before), TrimLineNumbers(after), message);
    }

    [NotNull]
    private static string Validate([NotNull] string assemblyPath2)
    {
        var exePath = GetPathToPEVerify();
        if (!File.Exists(exePath))
        {
            return string.Empty;
        }

        var process = Process.Start(new ProcessStartInfo(exePath, "\"" + assemblyPath2 + "\"")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });

        // ReSharper disable once PossibleNullReferenceException
        process.WaitForExit(10000);
        return process.StandardOutput.ReadToEnd().Trim().Replace(assemblyPath2, "");
    }

    [NotNull]
    private static string GetPathToPEVerify()
    {
        var exePath = Environment.ExpandEnvironmentVariables(@"%programfiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\PEVerify.exe");

        if (!File.Exists(exePath))
        {
            exePath = Environment.ExpandEnvironmentVariables(@"%programfiles(x86)%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\PEVerify.exe");
        }
        return exePath;
    }

    [NotNull]
    private static string TrimLineNumbers([NotNull] string foo)
    {
        return Regex.Replace(foo, @"0x.*]", "");
    }
}