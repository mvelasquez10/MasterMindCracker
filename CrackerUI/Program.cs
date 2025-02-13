using Engine;

using System.Diagnostics;

Stopwatch _stopwatch = new();

if (args.Length != 1)
{
    Console.WriteLine("Usage: ExpectedCode <base64-hash>");
    return;
}

var _cracker = new Cracker(args[0]);
var _format = $"[{DateTime.Now:HH:mm:ss}]";

_cracker.OnCrackerReport += (sender, e) =>
{
    if (e.Event == CrackerEventArgs.CrackerEvent.AttemptSucceeded)
    {
        _stopwatch.Stop();
        Console.WriteLine($"{_format} *** {e.Entry} - {_stopwatch.Elapsed} ***");
        _cracker.OnCrackerReport -= (sender, e) => { };
        Environment.Exit(0);
    }
    else
        Console.WriteLine($"{_format} {e.Entry}");
};

_stopwatch.Start();
_cracker.CreateAttempts();
_cracker.RunAttempts();