namespace Nntp.Core;

// The text of a GROUP reply: "count first last name", e.g. "817 1 817 ab.general".
public record GroupInfo(long Count, long First, long Last, string Name)
{
    public static GroupInfo Parse(string text)
    {
        var parts = text.Split(' ');
        var count = long.Parse(parts[0]);
        var first = long.Parse(parts[1]);
        var last = long.Parse(parts[2]);
        var name = parts[3];
        return new GroupInfo(count, first, last, name);
    }
}
