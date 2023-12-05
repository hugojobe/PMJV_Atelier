using System.Collections.Generic;

public class Conversation
{
    private List<string> lines = new List<string>();
    private int progress = 0;

    public string file {get; private set;}
    public int fileStartIndex {get; private set;}
    public int fileEndIndex {get; private set;}

    public Conversation(List<string> lines, int progress = 0, string file = "", int fileStartIndex = -1, int fileEndIndex = -1) {
        this.lines = lines;
        this.progress = progress;
        this.file = file;
        
        if(fileStartIndex == -1)
            fileStartIndex = 0;
        if(fileEndIndex == -1)
            fileEndIndex = lines.Count-1;

        this.fileStartIndex = fileStartIndex;
        this.fileEndIndex = fileEndIndex;
    }

    public int GetProgress() => progress;
    public void SetProgress(int value) => progress = value;
    public void IncrementProgress() => progress++;
    public int Count => lines.Count;
    public List<string> GetLines() => lines;
    public string CurrentLine() => lines[progress];
    public bool HasReachedEnd() => progress >= lines.Count;
}
