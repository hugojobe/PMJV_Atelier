using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class VNGameSave
{
    public static VNGameSave activeFile = null;

    public const string FILE_TYPE = ".eyesave";
    public const string SCREENSHOT_FILE_TYPE = ".jpg";
    public const bool ENCRYPT_FILES = true;

    public string filePath => $"{FilePaths.gameSaves}{slotNumber}{FILE_TYPE}";
    public string screenshotPath => $"{FilePaths.gameSaves}{slotNumber}{SCREENSHOT_FILE_TYPE}";

    public int playerMoney = 7;
    public int playerOx = 10;
    public bool helpedAutostop = false;

    public int slotNumber = 1;

    public bool newGame = true;
    public string[] activeConversation;
    public HistoryState activeState;
    public HistoryState[] historyLogs;
    public VNVariablesData[] variables;

    public string timestamp;

    public static VNGameSave Load(string filePath, bool activateOnLoad = false) {
        VNGameSave save = FileManager.Load<VNGameSave>(filePath, ENCRYPT_FILES);

        if(activateOnLoad){
            activeFile = save;
            save.Activate();
        }

        return save;
    }

    public void Save() {
        activeFile.newGame = false;

        activeState = HistoryState.Capture();
        historyLogs = HistoryManager.instance.history.ToArray();
        activeConversation = GetConversationData();
        variables = GetVariableData();

        timestamp = DateTime.Now.ToString("dd/mm/yy HH:MM:ss");

        string saveJSON = JsonUtility.ToJson(this);
        FileManager.Save(filePath, saveJSON, ENCRYPT_FILES);

        ScreenshotMaster.CaptureScreenshot(VNManager.instance.mainCamera, Screen.width, Screen.height, 0.25f, screenshotPath);
    }

    public void Activate() {
        if(activeState != null)
            activeState.Load();

        HistoryManager.instance.history = historyLogs.ToList();
        HistoryManager.instance.logManager.Clear();
        HistoryManager.instance.logManager.Rebuild();

        SetVariableData();
        SetConversationData();

        DialogueSystem.instance.continuePrompt.Hide();
    }

    private string[] GetConversationData() {
        List<string> retData = new List<string>();

        var conversations = DialogueSystem.instance.conversationManager.GetConversationQueue();

        for(int i = 0; i < conversations.Length; i++) {
            var conversation = conversations[i];
            string data = "";

            if (conversation.file != string.Empty) {
                var compressedData = new VNConversationDataCompressed();
                compressedData.fileName = conversation.file;
                compressedData.progress = conversation.GetProgress();
                compressedData.startIndex = conversation.fileStartIndex;
                compressedData.endIndex = conversation.fileEndIndex;
                data = JsonUtility.ToJson(compressedData);
            } else {
                var fullData = new VNConversationData();
                fullData.conversation = conversation.GetLines();
                fullData.progress = conversation.GetProgress();
                data = JsonUtility.ToJson(fullData);
            }

            retData.Add(data);
        }

        return retData.ToArray();
    }

    private void SetConversationData() {
        for(int i = 0; i < activeConversation.Length; i++) {
            try {
                string data = activeConversation[i];
                Conversation conversation = null;

                var fullData = JsonUtility.FromJson<VNConversationData>(data);
                if(fullData != null && fullData.conversation != null && fullData.conversation.Count > 0) {
                    conversation = new Conversation(fullData.conversation, fullData.progress);
                }
                else {
                    var compressedData = JsonUtility.FromJson<VNConversationDataCompressed>(data);
                    if(compressedData != null && compressedData.fileName != string.Empty) {
                        TextAsset file = Resources.Load<TextAsset>(compressedData.fileName);
                        
                        int count = compressedData.endIndex - compressedData.startIndex;
                        
                        List<string> lines = FileManager.ReadTextAsset(file).Skip(compressedData.startIndex).Take(count + 1).ToList();
                        
                        conversation = new Conversation(lines, compressedData.progress, compressedData.fileName, compressedData.startIndex, compressedData.endIndex);
                        
                    } else {
                        Debug.LogError($"Unknown conversation format! Unable to reload conversation from VNGameSave using data '{data}'");
                    }
                }

                if(conversation != null && conversation.GetLines().Count > 0) {
                    if(i == 0)
                        DialogueSystem.instance.conversationManager.StartConversation(conversation);
                    else
                        DialogueSystem.instance.conversationManager.Enqueue(conversation);
                }

            } catch (System.Exception e) {
                Debug.LogError($"Encountered error while extracting saved conversation data! {e}");
                continue;
            }
        }
    }

    public VNVariablesData[] GetVariableData() {
        List<VNVariablesData> retData = new List<VNVariablesData>();

        foreach(var database in VariableStore.databases.Values) {
            foreach(var variable in database.variables) {
                VNVariablesData variablesData = new VNVariablesData();
                variablesData.name = $"{database.name}.{variable.Key}";
                string val = $"{variable.Value.Get()}";
                variablesData.value = val;
                variablesData.type = (val == string.Empty)? "System.String" : variable.Value.Get().GetType().ToString();
                retData.Add(variablesData);
            }
        }

        return retData.ToArray();
    }

    public void SetVariableData() {
        foreach(var variable in variables) {
            string value = variable.value;

            switch(variable.type) {
                case "System.Boolean":
                    if(bool.TryParse(value, out bool bout)){
                        VariableStore.TrySetValue(variable.name, bout);
                        continue;
                    }
                    break;
                case "System.Int32":
                    if(int.TryParse(value, out int iout)){
                        VariableStore.TrySetValue(variable.name, iout);
                        continue;
                    }
                    break;
                case "System.Single":
                    if(float.TryParse(value, out float fout)){
                        VariableStore.TrySetValue(variable.name, fout);
                        continue;
                    }
                    break;
                default:
                    VariableStore.TrySetValue(variable.name, value);
                    continue;
            }

            Debug.Log($"Could not interpret variable type '{variable.type}' for variable '{variable.name}'");
        }
    }
}
