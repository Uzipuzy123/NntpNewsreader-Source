using Nntp.Core;

// A free public news server that needs no login.
const string host = "freenews.netfront.net";

Console.WriteLine($"Connecting to {host}...");
using var connection = await TcpLineConnection.ConnectAsync(host);
var client = new NntpClient(connection);

var greeting = await client.ReadGreetingAsync();
Console.WriteLine($"Server says: {greeting.Code} {greeting.Text}");

// The server has 100,000+ groups and sends them slowly, so we only show the first 20.
var groups = await client.ListAsync(maxGroups: 20);

Console.WriteLine($"First {groups.Count} newsgroups:");
foreach (var group in groups)
{
    Console.WriteLine($"  {group.Name}  (articles {group.First}-{group.Last}, posting allowed: {group.PostingAllowed})");
}
