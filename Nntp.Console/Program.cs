using Nntp.Core;

// A free public news server that needs no login.
const string host = "freenews.netfront.net";

Console.WriteLine($"Connecting to {host}...");

// The server has 100,000+ groups and sends them slowly, so we only show the first 20.
// It keeps sending the rest anyway, so we hang up here and dial again below.
using (var listConnection = await TcpLineConnection.ConnectAsync(host))
{
    var listClient = new NntpClient(listConnection);

    var greeting = await listClient.ReadGreetingAsync();
    Console.WriteLine($"Server says: {greeting.Code} {greeting.Text}");

    var groups = await listClient.ListAsync(maxGroups: 20);

    Console.WriteLine($"First {groups.Count} newsgroups:");
    foreach (var group in groups)
    {
        Console.WriteLine($"  {group.Name}  (articles {group.First}-{group.Last}, posting allowed: {group.PostingAllowed})");
    }
}

Console.Write("Type a group name: ");
var groupName = Console.ReadLine() ?? string.Empty;

using var connection = await TcpLineConnection.ConnectAsync(host);
var client = new NntpClient(connection);
await client.ReadGreetingAsync();

GroupInfo selected;
try
{
    selected = await client.GroupAsync(groupName);
}
catch (NntpProtocolException ex)
{
    Console.WriteLine($"Could not open the group. {ex.Message}");
    return;
}

if (selected.Count == 0)
{
    Console.WriteLine("This group has no articles.");
    return;
}

// One HEAD per article is slow, so we only fetch the newest 20, newest first.
var oldest = Math.Max(selected.First, selected.Last - 19);

Console.WriteLine($"Newest headlines in {selected.Name}:");
for (var number = selected.Last; number >= oldest; number--)
{
    var headline = await client.GetHeadlineAsync(number);

    // null means the article is gone, so we skip it.
    if (headline != null)
    {
        Console.WriteLine($"  {number}: {headline}");
    }
}
