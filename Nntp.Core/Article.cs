namespace Nntp.Core;

// An article is header lines, then one empty line, then the body lines.
public record Article(IReadOnlyList<string> Header, IReadOnlyList<string> Body)
{
    public static Article Parse(IReadOnlyList<string> lines)
    {
        throw new NotImplementedException();
    }
}
