namespace Nntp.Core;

public class NntpClient
{
    private readonly ILineConnection _connection;

    public NntpClient(ILineConnection connection)
    {
        _connection = connection;
    }

    // The server says hello ("200 welcome...") as soon as we connect, before we send anything.
    public async Task<NntpResponse> ReadGreetingAsync()
    {
        var line = await _connection.ReadLineAsync()
            ?? throw new IOException("The server closed the connection before saying hello.");
        return NntpResponse.Parse(line);
    }

    // Sends GROUP to select a newsgroup. The server answers 211, or 411 if it doesn't exist.
    public async Task<GroupInfo> GroupAsync(string name)
    {
        await _connection.WriteLineAsync($"GROUP {name}");

        var line = await _connection.ReadLineAsync()
            ?? throw new IOException("The server closed the connection.");
        var response = NntpResponse.Parse(line);
        if (response.Code != 211)
        {
            throw new NntpProtocolException(response);
        }

        return GroupInfo.Parse(response.Text);
    }

    // Sends HEAD and returns the article's Subject, or null if the article is gone (423).
    public async Task<string?> GetHeadlineAsync(long articleNumber)
    {
        await _connection.WriteLineAsync($"HEAD {articleNumber}");

        var firstLine = await _connection.ReadLineAsync()
            ?? throw new IOException("The server closed the connection.");
        var response = NntpResponse.Parse(firstLine);
        if (response.Code == 423)
        {
            return null;
        }
        if (response.Code != 221)
        {
            throw new NntpProtocolException(response);
        }

        // Keep reading to the "." even after finding the subject, otherwise
        // the leftover lines would be mistaken for the answer to the next command.
        string? subject = null;
        while (true)
        {
            var line = await _connection.ReadLineAsync()
                ?? throw new IOException("The server closed the connection in the middle of the header.");

            if (line == ".")
            {
                break;
            }

            if (line.StartsWith("Subject:", StringComparison.OrdinalIgnoreCase))
            {
                subject = line.Substring("Subject:".Length).Trim();
            }
        }

        return subject;
    }

    // Sends LIST and reads one newsgroup per line until the line ".".
    // maxGroups lets the caller stop early, because some servers have 100,000+ groups.
    public async Task<IReadOnlyList<NewsgroupInfo>> ListAsync(int? maxGroups = null)
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
        // Keep going while there is no limit, or while we have fewer groups than the limit.
        while (maxGroups is null || groups.Count < maxGroups)
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
