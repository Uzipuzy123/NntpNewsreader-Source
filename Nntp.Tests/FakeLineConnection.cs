using Nntp.Core;

namespace Nntp.Tests;

// A stand-in for a real server connection: you give it the lines the "server"
// should answer with, and it records every line we send to it.
public class FakeLineConnection : ILineConnection
{
    private readonly Queue<string> _serverLines;

    public List<string> SentLines { get; } = new();

    public FakeLineConnection(params string[] serverLines)
    {
        _serverLines = new Queue<string>(serverLines);
    }

    public Task<string?> ReadLineAsync()
    {
        return Task.FromResult(_serverLines.Count > 0 ? _serverLines.Dequeue() : null);
    }

    public Task WriteLineAsync(string line)
    {
        SentLines.Add(line);
        return Task.CompletedTask;
    }

    public void Dispose() { }
}
