using System;
using System.Collections;
using UnityEngine;

public class CommandDatabaseExtensionExamples : CommandDatabaseExtension
{
    new public static void Extend(CommandDatabase database){
        // COMMANDES
        database.AddCommand("print", new Action(PrintDefaultMessage));
        database.AddCommand("print_1p", new Action<string>(PrintUserMessage));
        database.AddCommand("print_mp", new Action<string[]>(PrintLines));

        // LAMBDA
        database.AddCommand("lambda", new Action(() => {Debug.Log("Printing a default message from lambda");}));
        database.AddCommand("lambda_1p", new Action<string>((arg) => {Debug.Log($"User lambda message : '{arg}'");}));
        
        //COROUTINES
        database.AddCommand("process", new Func<IEnumerator>(SimpleProcessCoroutine));
    }

    public static void PrintDefaultMessage(){
        Debug.Log("Printing a default message");
    }

    public static void PrintUserMessage(string message){
        Debug.Log($"User message : '{message}'");
    }

    public static void PrintLines(string[] lines){
        int i = 0;
        foreach (string line in lines)
        {   
            Debug.Log($"[{i++}] : '{line}'");
        }
    }

    public static IEnumerator SimpleProcessCoroutine(){
        for (int i = 0; i <= 5; i++)
        {
            Debug.Log("Process running... " + i);
            yield return new WaitForSeconds(1);
        }
    }
}
