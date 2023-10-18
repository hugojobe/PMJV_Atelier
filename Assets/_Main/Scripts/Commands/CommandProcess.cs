using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommandProcess
{
    public Guid ID;
    public string processName;
    public Delegate command;
    public string[] args;
    public UnityEvent onTerminateAction;
    public CoroutineWrapper runningProcess;

    public CommandProcess(Guid iD, string processName, Delegate command, string[] args, CoroutineWrapper runningProcess, UnityEvent onTerminateAction = null){
        ID = iD;
        this.processName = processName;
        this.command = command;
        this.args = args;
        this.onTerminateAction = onTerminateAction;
        this.runningProcess = runningProcess;
    }
}
