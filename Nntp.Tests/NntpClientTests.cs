using Nntp.Core;
using Xunit;

namespace Nntp.Tests;

public class NntpClientTests
{
    [Fact]
    public async Task ListAsync_sends_the_LIST_command()
    {
        var connection = new FakeLineConnection(
            "215 list of newsgroups follows",
            ".");
        var client = new NntpClient(connection);

        await client.ListAsync();

        Assert.Equal(new[] { "LIST" }, connection.SentLines);
    }

    [Fact]
    public async Task ListAsync_reads_every_group_until_the_dot()
    {
        var connection = new FakeLineConnection(
            "215 list of newsgroups follows",
            "comp.lang.csharp 1500 1 y",
            "comp.lang.java 900 1 y",
            ".");
        var client = new NntpClient(connection);

        var groups = await client.ListAsync();

        Assert.Equal(2, groups.Count);
        Assert.Equal("comp.lang.csharp", groups[0].Name);
        Assert.Equal("comp.lang.java", groups[1].Name);
    }

    [Fact]
    public async Task ListAsync_returns_an_empty_list_when_the_server_has_no_groups()
    {
        var connection = new FakeLineConnection(
            "215 list of newsgroups follows",
            ".");
        var client = new NntpClient(connection);

        var groups = await client.ListAsync();

        Assert.Empty(groups);
    }

    [Fact]
    public async Task ListAsync_throws_when_the_server_answers_with_an_error()
    {
        var connection = new FakeLineConnection("502 permission denied");
        var client = new NntpClient(connection);

        await Assert.ThrowsAsync<NntpProtocolException>(() => client.ListAsync());
    }
}
