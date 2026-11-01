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
    public async Task ListAsync_stops_early_when_maxGroups_is_reached()
    {
        var connection = new FakeLineConnection(
            "215 list of newsgroups follows",
            "comp.lang.csharp 1500 1 y",
            "comp.lang.java 900 1 y",
            "comp.lang.python 700 1 y");
        // No "." on purpose: we should stop after 2 groups, before ever reaching the end.
        var client = new NntpClient(connection);

        var groups = await client.ListAsync(maxGroups: 2);

        Assert.Equal(2, groups.Count);
    }

    [Fact]
    public async Task ReadGreetingAsync_reads_the_first_line_from_the_server()
    {
        var connection = new FakeLineConnection("200 welcome to the server");
        var client = new NntpClient(connection);

        var greeting = await client.ReadGreetingAsync();

        Assert.Equal(200, greeting.Code);
        Assert.Equal("welcome to the server", greeting.Text);
    }

    [Fact]
    public async Task GroupAsync_sends_the_GROUP_command_with_the_name()
    {
        var connection = new FakeLineConnection("211 817 1 817 ab.general");
        var client = new NntpClient(connection);

        await client.GroupAsync("ab.general");

        Assert.Equal(new[] { "GROUP ab.general" }, connection.SentLines);
    }

    [Fact]
    public async Task GroupAsync_returns_the_group_details()
    {
        var connection = new FakeLineConnection("211 817 1 817 ab.general");
        var client = new NntpClient(connection);

        var group = await client.GroupAsync("ab.general");

        Assert.Equal(817, group.Count);
        Assert.Equal(1, group.First);
        Assert.Equal(817, group.Last);
        Assert.Equal("ab.general", group.Name);
    }

    [Fact]
    public async Task GroupAsync_throws_when_the_group_does_not_exist()
    {
        var connection = new FakeLineConnection("411 no such group");
        var client = new NntpClient(connection);

        await Assert.ThrowsAsync<NntpProtocolException>(() => client.GroupAsync("does.not.exist"));
    }

    [Fact]
    public async Task GetHeadlineAsync_sends_the_HEAD_command_with_the_number()
    {
        var connection = new FakeLineConnection(
            "221 817 <id@server> head follows",
            "Subject: Hello everyone",
            ".");
        var client = new NntpClient(connection);

        await client.GetHeadlineAsync(817);

        Assert.Equal(new[] { "HEAD 817" }, connection.SentLines);
    }

    [Fact]
    public async Task GetHeadlineAsync_returns_the_subject()
    {
        var connection = new FakeLineConnection(
            "221 817 <id@server> head follows",
            "From: someone@example.com",
            "Subject: Hello everyone",
            "Date: Sun, 1 Nov 2026 10:00:00 GMT",
            ".");
        var client = new NntpClient(connection);

        var headline = await client.GetHeadlineAsync(817);

        Assert.Equal("Hello everyone", headline);
    }

    [Fact]
    public async Task GetHeadlineAsync_reads_all_header_lines_so_the_next_call_works()
    {
        var connection = new FakeLineConnection(
            "221 817 <id@server> head follows",
            "Subject: First",
            "Date: Sun, 1 Nov 2026 10:00:00 GMT",
            ".",
            "221 816 <id2@server> head follows",
            "Subject: Second",
            ".");
        var client = new NntpClient(connection);

        await client.GetHeadlineAsync(817);
        var second = await client.GetHeadlineAsync(816);

        Assert.Equal("Second", second);
    }

    [Fact]
    public async Task GetHeadlineAsync_finds_the_subject_in_any_letter_case()
    {
        var connection = new FakeLineConnection(
            "221 817 <id@server> head follows",
            "SUBJECT: Hello everyone",
            ".");
        var client = new NntpClient(connection);

        var headline = await client.GetHeadlineAsync(817);

        Assert.Equal("Hello everyone", headline);
    }

    [Fact]
    public async Task GetHeadlineAsync_returns_null_when_the_article_is_gone()
    {
        var connection = new FakeLineConnection("423 no such article");
        var client = new NntpClient(connection);

        var headline = await client.GetHeadlineAsync(817);

        Assert.Null(headline);
    }

    [Fact]
    public async Task GetHeadlineAsync_throws_on_any_other_error()
    {
        var connection = new FakeLineConnection("412 no newsgroup selected");
        var client = new NntpClient(connection);

        await Assert.ThrowsAsync<NntpProtocolException>(() => client.GetHeadlineAsync(817));
    }

    [Fact]
    public async Task ListAsync_throws_when_the_server_answers_with_an_error()
    {
        var connection = new FakeLineConnection("502 permission denied");
        var client = new NntpClient(connection);

        await Assert.ThrowsAsync<NntpProtocolException>(() => client.ListAsync());
    }
}
