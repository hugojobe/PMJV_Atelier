using System.Collections.Generic;

public class Conversation
{
    private List<string> lines = new List<string>();
    private int progress = 0;

    public Conversation(List<string> lines, int progress = 0){
        this.lines = lines;
        this.progress = progress;
    }

    public int GetProgress() => progress;
    public void SetProgress(int value) => progress = value;
    public void IncrementProgress() => progress++;
    public int Count => lines.Count;
    public List<string> GetLines() => lines;
    public string CurrentLine() => lines[progress];
    public bool HasReachedEnd() => progress >= lines.Count;
}
