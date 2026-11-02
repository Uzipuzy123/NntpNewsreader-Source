using Nntp.Core;
using Xunit;

namespace Nntp.Tests;

public class ArticleTests
{
    [Fact]
    public void Parse_splits_header_and_body_at_the_empty_line()
    {
        var article = Article.Parse(new[]
        {
            "From: someone@example.com",
            "Subject: Hello",
            "",
            "First line of text.",
            "Second line of text."
        });

        Assert.Equal(new[] { "From: someone@example.com", "Subject: Hello" }, article.Header);
        Assert.Equal(new[] { "First line of text.", "Second line of text." }, article.Body);
    }

    [Fact]
    public void Parse_keeps_later_empty_lines_in_the_body()
    {
        var article = Article.Parse(new[]
        {
            "Subject: Hello",
            "",
            "Paragraph one.",
            "",
            "Paragraph two."
        });

        Assert.Equal(new[] { "Paragraph one.", "", "Paragraph two." }, article.Body);
    }

    [Fact]
    public void Parse_gives_an_empty_body_when_there_is_no_empty_line()
    {
        var article = Article.Parse(new[] { "Subject: Hello" });

        Assert.Equal(new[] { "Subject: Hello" }, article.Header);
        Assert.Empty(article.Body);
    }
}
