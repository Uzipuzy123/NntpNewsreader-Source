namespace Nntp.Core;

// The text of a GROUP reply: "count first last name", e.g. "817 1 817 ab.general".
public record GroupInfo(long Count, long First, long Last, string Name)
{
    public static GroupInfo Parse(string text)
    {
        throw new NotImplementedException();
    }
}
