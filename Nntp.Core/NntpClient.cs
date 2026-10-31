namespace Nntp.Core;

public class NntpClient
{
    private readonly ILineConnection _connection;

    public NntpClient(ILineConnection connection)
    {
        _connection = connection;
    }

    // Sends LIST and reads one newsgroup per line until the line ".".
    public Task<IReadOnlyList<NewsgroupInfo>> ListAsync()
    {
        throw new NotImplementedException();
    }
}

public class NntpProtocolException(NntpResponse response)
    : Exception($"Server returned {response.Code}: {response.Text}")
{
    public NntpResponse Response { get; } = response;
}
