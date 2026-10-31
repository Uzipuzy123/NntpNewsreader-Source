namespace Nntp.Core;

// One line of the LIST reply: "name last first flag", e.g. "comp.lang.csharp 1500 1 y".
public record NewsgroupInfo(string Name, long Last, long First, char PostingFlag)
{
    public bool PostingAllowed => throw new NotImplementedException();

    public static NewsgroupInfo Parse(string line)
    {
        throw new NotImplementedException();
    }
}
