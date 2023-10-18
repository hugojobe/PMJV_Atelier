using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CommandData
{
    public List<Command> commands;
    public const char COMMANDSPLITTER_ID = ',';
    public const char ARGUMENTSCONTAINER_ID = '(';
    public const string WAITCOMMAND_ID = "[wait]";

    public struct Command{
        public string name;
        public string[] arguments;
        public bool waitForCompletion;
    }

    public CommandData(string rawCommand){
        commands = RipCommands(rawCommand);
    }

    public List<Command> RipCommands(string rawCommand){
        string[] data = rawCommand.Split(COMMANDSPLITTER_ID, System.StringSplitOptions.RemoveEmptyEntries);
        List<Command> result = new List<Command>();

        foreach(string cmd in data){
            Command command = new Command();
            int index = cmd.IndexOf(ARGUMENTSCONTAINER_ID);
            command.name = cmd.Substring(0, index).Trim();

            if(command.name.ToLower().StartsWith(WAITCOMMAND_ID)){
                command.name = command.name.Substring(WAITCOMMAND_ID.Length).Trim();
                command.waitForCompletion = true;
            } else
                command.waitForCompletion = false;

            command.arguments = GetArgs(cmd.Substring(index + 1, cmd.Length - index - 2));
            result.Add(command);
        }

        return result;
    }

    public string[] GetArgs(string args){
        List<string> argList = new List<string>();
        StringBuilder currentArg = new StringBuilder();
        bool inQuotes = false;

        for(int i = 0; i < args.Length; i++){
            if(args[i] == '"'){
                inQuotes = !inQuotes;
                continue;
            }

            if(!inQuotes && args[i] == ' '){
                argList.Add(currentArg.ToString());
                currentArg.Clear();
                continue;
            }

            currentArg.Append(args[i]);
        }

        if(currentArg.Length > 0){
            argList.Add(currentArg.ToString());
        }

        return argList.ToArray();
    }
}
