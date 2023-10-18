using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILogicalLine
{
        string keyword {get;}
        bool Matches(DialogLine line);
        IEnumerator Execute(DialogLine line);
}
