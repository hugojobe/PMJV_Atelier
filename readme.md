# Visual Novel in Unity Engine
Here is the code for a project I developed for an exam during my studies. We were a team of four students but I was the only programmer.
> [!WARNING]
> The code present here is not commented as I was the only programmer and it was not intended to be exposed.
> The production time being very short, forgive me any optimization errors that may have crept in here.</br>
> I would like to point out that although I used tutorials to write the code, I understand what I wrote here.

**MAIN CODE STRUCTURE**
![Rapport Prog PMJV](https://github.com/hugojobe/PMJV_Atelier/assets/41127485/4537335c-7f9d-4a06-8833-ebde3caf6960)

**SCRIPTS**
|![#f03c15](https://placehold.co/15x15/f5cbcc/f5cbcc.png) Main Systems|The scripts below are essential for the correct execution of the game.|
|:------------------------|:------------------------------|
|[VNManager](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/VNSystem/VNManager.cs)|This script is responsible for the main logic of the visual novel. It communicates, among other things, with the backup system, the dialog system and the configuration files. It can start a new dialogue, load save files, or make the player lose or win.|
|[DialogueSystem](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/DialogueSystem.cs)|This script is not central to the dialogue system but is nevertheless essential. Indeed, it reacts to player inputs, changes the color of the dialogue depending on the character speaking, and sends the dialogues to be executed to the Conversation Manager.|
|[ConversationManager](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/ConversationManager.cs)|This script executes the lines of dialogue to be spoken. It retrieves lines from dialog files, interprets them using the LogicalLineManager, TagManager, and CommandManager scripts. If nothing results, it simply displays the dialog.|

|![#C9DAF8](https://placehold.co/15x15/C9DAF8/C9DAF8.png) Interpreters|The scripts below are used to interpret the lines of dialogue in search of possible commands, variables, conditions, etc.|
|:------------------------|:------------------------------|
