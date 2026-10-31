namespace Nntp.Core;

// One line of the LIST reply: "name last first flag", e.g. "comp.lang.csharp 1500 1 y".
public record NewsgroupInfo(string Name, long Last, long First, char PostingFlag)
{
    public bool PostingAllowed => PostingFlag == 'y';

    public static NewsgroupInfo Parse(string line)
    {
        var parts = line.Split(' ');

        var name = parts[0];
        var last = long.Parse(parts[1]);
        var first = long.Parse(parts[2]);
        var flag = parts[3][0];

        return new NewsgroupInfo(name, last, first, flag);
    }
}
