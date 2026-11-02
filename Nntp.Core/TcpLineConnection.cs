using System.Net.Sockets;
using System.Text;

namespace Nntp.Core;

// The real connection: opens a TCP "phone line" to a news server and lets us
// send and receive one line of text at a time (NNTP is a line-based protocol).
public class TcpLineConnection : ILineConnection
{
    private readonly TcpClient _tcpClient;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;

    private TcpLineConnection(TcpClient tcpClient, StreamReader reader, StreamWriter writer)
    {
        _tcpClient = tcpClient;
        _reader = reader;
        _writer = writer;
    }

    // Port 119 is the standard NNTP port.
    public static async Task<TcpLineConnection> ConnectAsync(string host, int port = 119)
    {
        var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(host, port);

        var stream = tcpClient.GetStream();
        // NNTP lines end in "\r\n". AutoFlush sends each line right away instead of buffering it.
        // Latin-1 keeps letters like æ, ø and é, which ASCII would turn into "?".
        var reader = new StreamReader(stream, Encoding.Latin1);
        var writer = new StreamWriter(stream, Encoding.Latin1) { AutoFlush = true, NewLine = "\r\n" };

        return new TcpLineConnection(tcpClient, reader, writer);
    }

    public Task<string?> ReadLineAsync()
    {
        return _reader.ReadLineAsync();
    }

    public Task WriteLineAsync(string line)
    {
        return _writer.WriteLineAsync(line);
    }

    // Hang up: close the reader, the writer and the connection itself.
    public void Dispose()
    {
        _reader.Dispose();
        _writer.Dispose();
        _tcpClient.Dispose();
    }
}
