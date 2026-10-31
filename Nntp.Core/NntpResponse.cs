namespace Nntp.Core;

// Every NNTP reply line looks like "215 list of newsgroups follows".
// Code is the 3-digit number, Text is whatever follows it.
public record NntpResponse(int Code, string Text)
{
    public static NntpResponse Parse(string line)
    {
        var spaceIndex = line.IndexOf(' ');
        if (spaceIndex < 0)
        {
            return new NntpResponse(int.Parse(line), string.Empty);
        }

        var code = int.Parse(line.Substring(0, spaceIndex));
        var text = line.Substring(spaceIndex + 1);
        return new NntpResponse(code, text);
    }
}
