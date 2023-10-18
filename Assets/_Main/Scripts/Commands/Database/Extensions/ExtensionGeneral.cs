using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using UnityEngine;

public class ExtensionGeneral : CommandDatabaseExtension
{
    new public static void Extend(CommandDatabase database){
        database.AddCommand("wait", new Func<string, IEnumerator>(Wait));

        database.AddCommand("showdialog", new Func<IEnumerator>(ShowDialog));
        database.AddCommand("hidedialog", new Func<IEnumerator>(HideDialog));
    }

    private static IEnumerator Wait(string data){
        //Debug.Log("Waiting for time : " + data);
        if(float.TryParse(data, NumberStyles.Any, CultureInfo.InvariantCulture, out float waitTime))
            yield return new WaitForSecondsRealtime(waitTime);
    }

    private static IEnumerator ShowDialog(){
        yield return DialogueSystem.instance.dialogContainer.Show();
    }

    private static IEnumerator HideDialog(){
        yield return DialogueSystem.instance.dialogContainer.Hide();
    }
}
