using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

using static LogicalLinesUtilities.Expressions;

public class LL_Operator : ILogicalLine
{
    public string keyword => throw new System.NotImplementedException();

    public IEnumerator Execute(DialogLine line)
    {
        string trimmedLine = line.rawData.Trim();
        string[] parts = Regex.Split(trimmedLine, REGEX_ARITHMETIC);

        if(parts.Length < 3){
            Debug.LogError($"Invalid command: {trimmedLine}");
            yield break;
        }

        string variable = parts[0].Trim().TrimStart(VariableStore.VARIABLE_ID);
        string op = parts[1].Trim();
        string[] remainingParts = new string[parts.Length - 2];
        Array.Copy(parts, 2, remainingParts, 0, parts.Length - 2);

        object value = CalculateValue(remainingParts);

        if(value == null)
            yield break;

        ProcessOperator(variable, op, value);
    }

    private void ProcessOperator(string variable, string op, object value){
        if(VariableStore.TryGetValue(variable, out object currentValue)){
            ProcessOperatorOnVariable(variable, op, value, currentValue);
        } else if(op == "=") {
            VariableStore.CreateVariable(variable, value);
        }
    }

    private void ProcessOperatorOnVariable(string variable, string op, object value, object currentValue){
        switch(op){
            case "=":
                VariableStore.TrySetValue(variable, value);
                break;
            case "+=":
                VariableStore.TrySetValue(variable, ConcatenateOrAdd(currentValue, value));
                break;
            case "-=":
                VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) - Convert.ToDouble(value));
                break;
            case "*=":
                VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) * Convert.ToDouble(value));
                break;
            case "/=":
                VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) / Convert.ToDouble(value));
                break;
            default:
                Debug.LogError($"Invalid operator '{op}'");
                break;
        }
    }

    private object ConcatenateOrAdd(object currentValue, object value){
        if(currentValue is string)
            return currentValue.ToString() + value;
        
        return Convert.ToDouble(currentValue) + Convert.ToDouble(value);
    }

    public bool Matches(DialogLine line)
    {
        Match match = Regex.Match(line.rawData.Trim(), REGEX_OPERATOR_LINE);
        return match.Success;
    }
}
