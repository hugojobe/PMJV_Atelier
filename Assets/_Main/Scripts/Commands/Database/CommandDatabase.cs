using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class CommandDatabase
{
    public Dictionary<string, Delegate> database = new Dictionary<string, Delegate>();

    public bool HasCommand(string commandName) => database.ContainsKey(commandName.ToLower());

    public void AddCommand(string commandName, Delegate command){
        if(!database.ContainsKey(commandName)){
            database.Add(commandName, command);
        } else {
            Debug.LogError($"Commad already exists in the database '{commandName}'");
        }
    }

    public Delegate GetCommand(string commandName){
        commandName = commandName.ToLower();
        if(!database.ContainsKey(commandName)){
            Debug.LogError($"Commad '{commandName}' does not exists in the database '{commandName}'");
            return null;
        }

        return database[commandName];
    }
}

public class CommandParameters{
    private const char PARAMETER_IDENTIFIER = '-';

    private Dictionary<string, string> parameters = new Dictionary<string, string>();

    private List<string> unlabeledParameters = new List<string>();

    public CommandParameters(string[] parameterArray){
        for(int i= 0; i < parameterArray.Length; i++){
            if(parameterArray[i].StartsWith(PARAMETER_IDENTIFIER) && !float.TryParse(parameterArray[i], out _)){
                string pName = parameterArray[i];
                string pValue = "";

                if(i + 1 < parameterArray.Length && !parameterArray[i + 1].StartsWith(PARAMETER_IDENTIFIER)){
                    pValue = parameterArray[i + 1];
                    i++;
                }

                parameters.Add(pName, pValue);
            } else
                unlabeledParameters.Add(parameterArray[i]);
        }
    }

    public bool TryGetValue<T>(string parameterName, out T value, T defaultValue = default(T)) => TryGetValue(new string[] {parameterName}, out value, defaultValue);

    public bool TryGetValue<T>(string[] parameterNames, out T value, T defaultValue = default(T)){
        foreach(string parameterName in parameterNames){
            if(parameters.TryGetValue(parameterName, out string parameterValue)){
                if(TryCastParameter(parameterValue, out value)){
                    return true;
                }
            }
        }

        foreach(string parameterName in unlabeledParameters){
            if(TryCastParameter(parameterName, out value)){
                unlabeledParameters.Remove(parameterName);
                return true;
            }
        }
        value = defaultValue;
        return false;
    }

    private bool TryCastParameter<T>(string parameterValue, out T value){
        if(typeof(T) == typeof(bool)){
            if(bool.TryParse(parameterValue, out bool boolValue)){
                value = (T)(object)boolValue;
                return true;
            }
        }
        else if(typeof(T) == typeof(int)){
            if(int.TryParse(parameterValue, out int intValue)){
                value = (T)(object)intValue;
                return true;
            }
        }
        else if(typeof(T) == typeof(float)){
            if(float.TryParse(parameterValue, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatValue)){
                value = (T)(object)floatValue;
                return true;
            }
        }
        else if(typeof(T) == typeof(string)){
            value = (T)(object)parameterValue;
            return true;
        }

        value = default(T);
        return false;
    }
}
