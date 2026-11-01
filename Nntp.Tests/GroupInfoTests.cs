using Nntp.Core;
using Xunit;

namespace Nntp.Tests;

public class GroupInfoTests
{
    [Fact]
    public void Parse_reads_count_first_last_and_name()
    {
        var group = GroupInfo.Parse("817 1 817 ab.general");

        Assert.Equal(817, group.Count);
        Assert.Equal(1, group.First);
        Assert.Equal(817, group.Last);
        Assert.Equal("ab.general", group.Name);
    }

    [Fact]
    public void Parse_handles_an_empty_group()
    {
        var group = GroupInfo.Parse("0 1 0 ab.jobs");

        Assert.Equal(0, group.Count);
        Assert.Equal(0, group.Last);
    }

    [Fact]
    public void Parse_ignores_extra_text_after_the_name()
    {
        var group = GroupInfo.Parse("817 1 817 ab.general group selected");

        Assert.Equal("ab.general", group.Name);
    }
}
