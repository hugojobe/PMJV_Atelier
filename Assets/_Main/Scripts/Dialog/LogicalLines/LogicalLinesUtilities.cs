using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public static class LogicalLinesUtilities
{
    public static class Encapsulation{
        public struct EncapsulatedData{
            public List<string> lines;
            public int startingIndex;
            public int endingIndex;
        }

        private const char ENCAPSULATION_START = '{';
        private const char ENCAPSULATION_END = '}';

        public static EncapsulatedData RipEncapsulationData(Conversation conversation, int startingIndex, bool ripHeaderAndEncapsulators = false){
            int encapsulationDepth = 0;

            EncapsulatedData data = new EncapsulatedData{ lines = new List<string>(), startingIndex = startingIndex, endingIndex = 0};

            for(int i = startingIndex; i <conversation.Count; i++){
                string line = conversation.GetLines()[i];
                if(ripHeaderAndEncapsulators || (encapsulationDepth > 0 && !IsEncapsulationEnd(line)))
                    data.lines.Add(line);

                if(IsEncapsulationStart(line)){
                    encapsulationDepth++;
                    continue;
                }

                if(IsEncapsulationEnd(line)){
                    encapsulationDepth--;
                    if(encapsulationDepth == 0){
                        data.endingIndex = i;
                        break;
                    }
                }
            }

            return data;
        }

        public static bool IsEncapsulationStart(string line) => line.Trim().StartsWith(ENCAPSULATION_START);
        public static bool IsEncapsulationEnd(string line) => line.Trim().StartsWith(ENCAPSULATION_END);
    }

    public static class Expressions{
        public static HashSet<string> OPERATORS = new HashSet<string>(){"-", "-=", "+", "+=", "*", "*=", "/", "/=", "="};

        public static readonly string REGEX_ARITHMETIC = @"([-+*/=]=?)";
        public static readonly string REGEX_OPERATOR_LINE = @"^\$\w+\s*(=|\+=|\-=|\*=|\/=)\s*";

        public static object CalculateValue(string[] expressionParts){
            List<string> operandsString = new List<string>();
            List<string> operatorStrings = new List<string>();
            List<object> operands = new List<object>();

            for(int i = 0; i < expressionParts.Length; i++){
                string part = expressionParts[i].Trim();

                if(part == string.Empty)
                    continue;

                if(OPERATORS.Contains(part))
                    operatorStrings.Add(part);
                else
                    operandsString.Add(part);
            }

            foreach(string operandString in operandsString){
                operands.Add(ExtractValue(operandString));
            }

            CalculateValue_DivisionAndMultiplication(operatorStrings, operands);
            CalculateValue_AdditionAndSubtraction(operatorStrings, operands);

            return operands[0];
        }

        private static void CalculateValue_DivisionAndMultiplication(List<string> operatorStrings, List<object> operands){
            for(int i = 0; i < operatorStrings.Count; i++){
                string operatorString = operatorStrings[i];

                if(operatorString == "*" || operatorString == "/"){
                    double leftOperand = Convert.ToDouble(operands[i]);
                    double rightOperand = Convert.ToDouble(operands[i + 1]);

                    if(operatorString == "*")
                        operands[i] = leftOperand * rightOperand;
                    else{
                        if(rightOperand == 0){
                            Debug.LogError("Cannot divide by 0 !");
                            return;
                        }

                        operands[i] = leftOperand / rightOperand;
                    }
                }

                operands.RemoveAt(i + 1);
                operatorStrings.RemoveAt(i);
                i--;
            }
        }

        private static void CalculateValue_AdditionAndSubtraction(List<string> operatorStrings, List<object> operands){
            for(int i = 0; i < operatorStrings.Count; i++){
                string operatorString = operatorStrings[i];

                if(operatorString == "+" || operatorString == "-"){
                    double leftOperand = Convert.ToDouble(operands[i]);
                    double rightOperand =Convert.ToDouble(operands[i + 1]);

                    if(operatorString == "+")
                        operands[i] = leftOperand + rightOperand;
                    else
                        operands[i] = leftOperand - rightOperand;

                    operands.RemoveAt(i);
                    operatorStrings.RemoveAt(i);
                    i--;
                }
            }
        }

        private static object ExtractValue(string value){
            bool negate = false;
            if(value.StartsWith('!')){
                negate = true;
                value = value.Substring(1);    
            }

            if(value.StartsWith(VariableStore.VARIABLE_ID)){
                string variableName = value.TrimStart(VariableStore.VARIABLE_ID);
                if(!VariableStore.HasVariable(variableName)){
                    Debug.LogError($"The variable '{variableName}' does not exists !");
                    return null;
                }

                VariableStore.TryGetValue(variableName, out object val);

                if(val is bool boolval && negate)
                    return !boolval;

                return val;
            }
            else if(value.StartsWith('\"') && value.EndsWith('\"')){
                value = TagManagers.Inject(value, injectTags:true, injectVariables: true);
                return value.Trim('"');
            }
            else{
                if(int.TryParse(value, out int intvVal))
                    return intvVal;
                else if(float.TryParse(value, out float floatVal))
                    return floatVal;
                else if(bool.TryParse(value, out bool boolVal))
                    return negate? !boolVal : boolVal;
                else
                    value = TagManagers.Inject(value, injectTags:true, injectVariables: true);
                    return value;
            }
        }
    }

    public static class Conditions {
        public static readonly string REGEX_CONDITIONAL_OPERATORS = @"(==|!=|<=|>=|&&|\|\|)";

        public static bool EvaluateCondition(string condition) {
            condition = TagManagers.Inject(condition, injectTags: true, injectVariables: true);

            string[] parts = Regex.Split(condition, REGEX_CONDITIONAL_OPERATORS).Select(p => p.Trim()).ToArray();

            for(int i = 0; i < parts.Length; i++) {
                if(parts[i].StartsWith("\"") && parts[i].EndsWith("\""))
                    parts[i] = parts[i].Substring(1, parts[i].Length - 2);
            }

            if(parts.Length == 1) {
                if(bool.TryParse(parts[0], out bool result))
                    return result;
                else {
                    Debug.LogError($"Could not parse condition {condition}");
                    return false;
                }
            } else if(parts.Length == 3) {
                return EvaluateExpression(parts[0], parts[1], parts[2]);
            } else {
                Debug.LogError($"Unsupported condition format: {condition}");
                return false;
            }
            
        }

        private delegate bool OperatorFunc<T>(T left, T right);
        private static Dictionary<string,  OperatorFunc<bool>> boolOperators = new Dictionary<string, OperatorFunc<bool>>() {
            { "&&", (left, right) => left && right },
            { "||", (left, right) => left || right },
            { "==", (left, right) => left == right },
            { "!=", (left , right) => left != right }
        };
        private static Dictionary<string,  OperatorFunc<float>> floatOperators = new Dictionary<string, OperatorFunc<float>>() {
            { "==", (left, right) => left == right },
            { "!=", (left, right) => left != right },
            { ">", (left, right) => left > right },
            { ">=", (left , right) => left >= right },
            { "<", (left, right) => left < right },
            { "<=", (left , right) => left <= right }
        };
        private static Dictionary<string,  OperatorFunc<int>> intOperators = new Dictionary<string, OperatorFunc<int>>() {
            { "==", (left, right) => left == right },
            { "!=", (left, right) => left != right },
            { ">", (left, right) => left > right },
            { ">=", (left , right) => left >= right },
            { "<", (left, right) => left < right },
            { "<=", (left , right) => left <= right }
        };

        private static bool EvaluateExpression(string left, string op, string right) {
            if(bool.TryParse(left, out bool leftBool) && bool.TryParse(right, out bool rightBool))
                return boolOperators[op](leftBool, rightBool);  
            
            if(float.TryParse(left, out float leftFloat) && float.TryParse(right, out float rightFloat))
                return floatOperators[op](leftFloat, rightFloat);

            if(int.TryParse(left, out int leftInt) && int.TryParse(right, out  int rightInt))
                return intOperators[op](leftInt, rightInt);

            switch(op) {
                case("=="): return left == right;
                case("!="): return left != right;
                default: throw new InvalidOperationException($"Unsupported operation: {op}");
            }
        }
    }
}
