using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

public class CommandManager : MonoBehaviour
{
    private const char SUB_COMMAND_IDENTIFIER = '.';
    public const string DB_CHARACTERS_BASE = "characters";
    public const string DB_CHARACTERS_SPRITE = "characters_sprite";
    public static CommandManager instance;
    //public static Coroutine processCoroutine = null;
    //public static bool isRunningProcess => processCoroutine != null;
    public CommandDatabase database;
    private Dictionary<string, CommandDatabase> subDatabases = new Dictionary<string, CommandDatabase>();

    private List<CommandProcess> activeProcess = new List<CommandProcess>();
    private CommandProcess topProcess => activeProcess.Last();

    private void Awake() {
        if(instance == null){
            instance = this;
            database = new CommandDatabase();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] extensionTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CommandDatabaseExtension))).ToArray();

            foreach(Type extension in extensionTypes){
                MethodInfo extendMethod = extension.GetMethod("Extend");
                extendMethod.Invoke(null, new object[]{database});
            }
        } else {
            DestroyImmediate(gameObject);
        }
    }

    public CoroutineWrapper Execute(string commandName, params string[] args){
        //Debug.Log("Command name : " + commandName);
        if(commandName.Contains(SUB_COMMAND_IDENTIFIER)){
            return ExecuteSubCommand(commandName, args);
        }

        Delegate command = database.GetCommand(commandName);

        if(command == null)
            return null;

        return StartProcess(commandName, command, args);
    }

    private CoroutineWrapper ExecuteSubCommand(string commandName, string[] args){
        string[] parts = commandName.Split(SUB_COMMAND_IDENTIFIER);
        string databaseName = string.Join(SUB_COMMAND_IDENTIFIER, parts.Take(parts.Length - 1));
        string subCommandName = parts.Last();

        if(subDatabases.ContainsKey(databaseName)){
            Delegate command = subDatabases[databaseName].GetCommand(subCommandName);
            if(command != null){
                return StartProcess(commandName, command, args);
            } else {
                Debug.LogError($"Command '{subCommandName}' not found in sub database '{databaseName}'");
            }
        }

        if(CharacterManager.instance.HasCharacter(databaseName)){
            List<string> newArgs = new List<string>();
            newArgs.Insert(0, databaseName);
            args = newArgs.ToArray();

            return ExecuteCharacterCommand(subCommandName, args);
        }

        Debug.LogError($"No sub database called '{databaseName}'");
        return null;
    }

    private CoroutineWrapper ExecuteCharacterCommand(string commandName, params string[] args){
        Delegate command = null;

        CommandDatabase db = subDatabases[DB_CHARACTERS_BASE];
        if(db.HasCommand(commandName)){
            command = db.GetCommand(commandName);
            return StartProcess(commandName, command, args);
        }

        CharacterConfigData characterConfigData = CharacterManager.instance.GetCharacterConfig(args[0]);
        if(characterConfigData.characterType == Character.CharacterTypes.Sprite){
            db = subDatabases[DB_CHARACTERS_SPRITE];
        }

        command = db.GetCommand(commandName);

        if(command != null)
            return StartProcess(commandName, command, args);

        Debug.LogError($"Command manager was unable to execute command '{commandName}' on character '{args[0]}'");
        return null;
    }

    public CoroutineWrapper StartProcess(string commandName, Delegate command, string[] args){
        System.Guid processID = System.Guid.NewGuid();
        CommandProcess cmd = new CommandProcess(processID, commandName, command, args, null);
        activeProcess.Add(cmd);

        Coroutine co = StartCoroutine(RunningProcess(cmd));

        cmd.runningProcess = new CoroutineWrapper(this, co);

        return cmd.runningProcess;
    }

    public void StopCurrentProcess(){
        if(topProcess != null)
            KillProcess(topProcess);
    }

    public void StopAllProcesses(){
        foreach(CommandProcess c in activeProcess){
            if(c.runningProcess != null && !c.runningProcess.isDone){
                c.runningProcess.Stop();
            }
            c.onTerminateAction?.Invoke();
        }

        activeProcess.Clear();
    }

    public IEnumerator RunningProcess(CommandProcess process){
        yield return WaitingForProcessToComplete(process.command, process.args);
        
        KillProcess(process);
    }

    public void KillProcess(CommandProcess cmd){
        activeProcess.Remove(cmd);

        if(cmd.runningProcess != null && !cmd.runningProcess.isDone){
            cmd.runningProcess.Stop();
        }

        cmd.onTerminateAction?.Invoke();
    }

    public IEnumerator WaitingForProcessToComplete(Delegate command, string[] args){
        //Debug.Log(command.Method);

        if(command is Action)
            command.DynamicInvoke();

        else if(command is Action<string>)
            command.DynamicInvoke(args[0]);

        else if(command is Action<string[]>)
            command.DynamicInvoke((object)args);

        else if(command is Func<IEnumerator>)
            yield return ((Func<IEnumerator>)command)();

        else if(command is Func<string, IEnumerator>)
            yield return ((Func<string, IEnumerator>)command)(args[0]);

        else if(command is Func<string[], IEnumerator>)
            yield return ((Func<string[], IEnumerator>)command)(args);
    }

    public void AddTerminationActionToCurrentProcess(UnityAction action){
        CommandProcess process = topProcess;

        if(process == null) return;

        process.onTerminateAction = new UnityEvent();
        process.onTerminateAction.AddListener(action);
    }

    public CommandDatabase CreateSubDatabase(string name){
        name = name.ToLower();
        if(subDatabases.TryGetValue(name, out CommandDatabase db)){
            return db;
        }

        CommandDatabase newDB = new CommandDatabase();
        subDatabases.Add(name, newDB);

        return newDB;
    }
}
