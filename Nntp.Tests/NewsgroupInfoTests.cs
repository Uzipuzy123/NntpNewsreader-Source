using Nntp.Core;
using Xunit;

namespace Nntp.Tests;

public class NewsgroupInfoTests
{
    [Fact]
    public void Parse_reads_name_last_first_and_flag()
    {
        var group = NewsgroupInfo.Parse("comp.lang.csharp 1500 1 y");

        Assert.Equal("comp.lang.csharp", group.Name);
        Assert.Equal(1500, group.Last);
        Assert.Equal(1, group.First);
        Assert.Equal('y', group.PostingFlag);
    }

    [Fact]
    public void Parse_handles_numbers_with_leading_zeros()
    {
        var group = NewsgroupInfo.Parse("ab.general 0000000817 0000000001 y");

        Assert.Equal(817, group.Last);
        Assert.Equal(1, group.First);
    }

    [Fact]
    public void PostingAllowed_is_true_when_flag_is_y()
    {
        var group = NewsgroupInfo.Parse("comp.lang.csharp 1500 1 y");

        Assert.True(group.PostingAllowed);
    }

    [Fact]
    public void PostingAllowed_is_false_when_group_is_moderated()
    {
        var group = NewsgroupInfo.Parse("ab.arnet 0 1 m");

        Assert.False(group.PostingAllowed);
    }
}
