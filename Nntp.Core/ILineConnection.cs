namespace Nntp.Core;

// Something that can send and receive one line of text at a time.
// The real implementation talks over TCP; tests use a fake that returns prepared lines.
public interface ILineConnection : IDisposable
{
    Task<string?> ReadLineAsync();
    Task WriteLineAsync(string line);
}
