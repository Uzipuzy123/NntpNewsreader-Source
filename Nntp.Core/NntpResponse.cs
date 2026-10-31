namespace Nntp.Core;

// Every NNTP reply line looks like "215 list of newsgroups follows".
// Code is the 3-digit number, Text is whatever follows it.
public record NntpResponse(int Code, string Text)
{
    public static NntpResponse Parse(string line)
    {
        throw new NotImplementedException();
    }
}
