namespace Nntp.Core;

public class NntpClient
{
    private readonly ILineConnection _connection;

    public NntpClient(ILineConnection connection)
    {
        _connection = connection;
    }

    // Sends LIST and reads one newsgroup per line until the line ".".
    public async Task<IReadOnlyList<NewsgroupInfo>> ListAsync()
    {
        await _connection.WriteLineAsync("LIST");

        var firstLine = await _connection.ReadLineAsync()
            ?? throw new IOException("The server closed the connection.");
        var response = NntpResponse.Parse(firstLine);
        if (response.Code != 215)
        {
            throw new NntpProtocolException(response);
        }

        var groups = new List<NewsgroupInfo>();
        while (true)
        {
            var line = await _connection.ReadLineAsync()
                ?? throw new IOException("The server closed the connection in the middle of the list.");

            if (line == ".")
            {
                break;
            }

            groups.Add(NewsgroupInfo.Parse(line));
        }

        return groups;
    }
}

public class NntpProtocolException(NntpResponse response)
    : Exception($"Server returned {response.Code}: {response.Text}")
{
    public NntpResponse Response { get; } = response;
}
