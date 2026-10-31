using Nntp.Core;
using Xunit;

namespace Nntp.Tests;

public class NntpResponseTests
{
    [Fact]
    public void Parse_splits_code_and_text()
    {
        var response = NntpResponse.Parse("215 list of newsgroups follows");

        Assert.Equal(215, response.Code);
        Assert.Equal("list of newsgroups follows", response.Text);
    }

    [Fact]
    public void Parse_handles_code_with_no_text()
    {
        var response = NntpResponse.Parse("205");

        Assert.Equal(205, response.Code);
        Assert.Equal(string.Empty, response.Text);
    }
}
