using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CommandDatabaseExtension
{
    public static void Extend(CommandDatabase database){
        
    }

    public static CommandParameters ConvertDataToParameters(string[] data) => new CommandParameters(data);
}
