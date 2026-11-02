namespace Nntp.Core;

// An article is header lines, then one empty line, then the body lines.
public record Article(IReadOnlyList<string> Header, IReadOnlyList<string> Body)
{
    public static Article Parse(IReadOnlyList<string> lines)
    {
        var header = new List<string>();
        var body = new List<string>();
        var inBody = false;

        foreach (var line in lines)
        {
            if (inBody)
            {
                body.Add(line);
            }
            else if (line == "")
            {
                inBody = true;
            }
            else
            {
                header.Add(line);
            }
        }

        return new Article(header, body);
    }
}
