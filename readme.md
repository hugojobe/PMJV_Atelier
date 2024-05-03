# Visual Novel in Unity Engine
Here is the code for a project I developed for an exam during my studies. We were a team of four students but I was the only programmer.
> [!WARNING]
> The code present here is not commented as I was the only programmer and it was not intended to be exposed.
> The production time being very short, forgive me any optimization errors that may have crept in here.</br>
> I would like to point out that although I used tutorials to write the code, I understand what I wrote here.

> [!CAUTION]
> Due to the size of the project, only part of the code is explained below. Feel free to explore or contact me for more information.

The dialog system works by using custom plain text dialog files that will be interpreted by the code.
<details>
<summary>You can find an example file here</summary>
  
```
CreateCharacter(Autostoppeur -e false -i true)
Move(Soeur -x 0.75 -y 0 -i true)
Move(Autostoppeur -x 0.25 -y 0 -i true)

PlayMusic(Autostop -ch 0 -l true)

ShowAll()
SetBackground(-img Desert)
Wait(1)
ShowDialog()

narrator "Quelqu'un lui fait déjà signe de se stopper..."

Show(Soeur)
<soeur> "Vraiment… Je viens à peine de partir et je dois déjà m'arrêter…{a} Bon voyons voir ce qu’il veut."
<soeur> "Qu’est-ce que vous voulez ?"
Hide(Soeur), Show(Autostoppeur)
Autostoppeur "Merci de t’être arrêtée petite...{c}S’il te plaît Est-ce que tu peux me déposer dans l’hôpital de <color=#F771D9>Sky City</color>, je dois y être au plus vite et mon véhicule est en panne.""
Hide(Autostoppeur), Show(Soeur)
<soeur> "Et merde... c’est un grand détoure par rapport à la ou je dois aller."
Hide(Soeur), Show(Autostoppeur)
Autostoppeur "S’il vous plait je dois vraiment y aller, ma fille est mourante et je dois absolument la rejoindre à l’hôpital."
Hide(Autostoppeur), Show(Soeur)
<soeur> "<i>Qu’est ce qui me dit que c’est vrai ce qu’il raconte, il pourrait très bien être aussi bizarre que les crack head du coin ou pire... En plus je risque de perdre de l’<ox></i>"

choice "Que faire ?"
{
	-Accepter
		$playerMoney += 25
		$playerOx -= 2
		$helpedAutostop = true
		<soeur> "Bon d’accord, je vais vous déposer là-bas. Juste faites pas n’importe quoi si vous tentez quoi que ce soit ça finira mal..."
		Hide(Soeur), Show(Autostoppeur)
		Autostoppeur "Merci, vraiment vous me sauvez, tenez voici un peu d’argent pour compenser le voyage."
		Hide(Autostoppeur)
		narrator "Vous gagnez 25 <money>"
		narrator "Vous perdez 2 <ox>"
		HideDialog()
		wait(2)
		SetBackground(SkyCity)
		ShowDialog()
		Show(Soeur)
		<soeur> "Voilà on est arrivé. Vous avez intérêt à parler de moi à votre fille, ça lui fera une bonne histoire."
		Hide(Soeur), Hide(Autostoppeur)

		HideDialog()
		Wait(1)
		ClearBackground()
		Wait(1)
		HideAll()

		Load(Marchand)
	-Refuser
		$helpedAutostop = false
		<soeur> "Qu’est ce qui me prouve que tu dois aller là-bas ? Tu pourrais très bien me poignarder par derrière ou pire. {a}Donc non démerdez vous pour vos problème"
		Hide(Soeur), Show(Autostoppeur)
		SetSprite(Autostoppeur -spr 2)
		Autostoppeur "Va te faire foutre, je te souhaite que du malheur."
		Hide(Autostoppeur), Show(Soeur)
		<soeur> "M’en fou démerdez vous dans vos problèmes."
		Hide(Soeur), Hide(Autostoppeur)

		HideDialog()
		Wait(1)
		ClearBackground()
		Wait(1)
		HideAll()

		SetSprite(Autostoppeur -spr 1)

		Load(Marchand)
		
}
```

</details>

## MAIN CODE STRUCTURE
![Rapport Prog PMJV](https://github.com/hugojobe/PMJV_Atelier/assets/41127485/4537335c-7f9d-4a06-8833-ebde3caf6960)

## SCRIPTS
### Main Systems
|![#f03c15](https://placehold.co/15x15/f5cbcc/f5cbcc.png) Main Systems|The scripts below are essential for the correct execution of the game.|
|:------------------------|:------------------------------|
|[VNManager](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/VNSystem/VNManager.cs)|This script is responsible for the main logic of the visual novel. It communicates, among other things, with the backup system, the dialog system and the configuration files. It can start a new dialogue, load save files, or make the player lose or win.|
|[DialogueSystem](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/DialogueSystem.cs)|This script is not central to the dialogue system but is nevertheless essential. Indeed, it reacts to player inputs, changes the color of the dialogue depending on the character speaking, and sends the dialogues to be executed to the Conversation Manager.|
|[ConversationManager](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/ConversationManager.cs)|This script executes the lines of dialogue to be spoken. It retrieves lines from dialog files, interprets them using the LogicalLineManager, TagManager, and CommandManager scripts. If nothing results, it simply displays the dialog.|

### Interpreters
|![#C9DAF8](https://placehold.co/15x15/C9DAF8/C9DAF8.png) Interpreters|The scripts below are used to interpret the lines of dialogue in search of possible commands, variables, conditions, etc.|
|:------------------------|:------------------------------|
|[LogicalLineManager](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/LogicalLines/LogicalLineManager.cs)|Logical lines are special lines of dialogue that can pause the dialogue system, or skip lines (in the case of choices or QTEs for example), but also execute conditions. They are all used with braces like an “if”. There are several types which will be described below.|
|[TagManager](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/TagManagers.cs)|Tags are small pieces of code that can be integrated into a dialog to replace it with a variable. For example, the line “You have <money> coins.” will be displayed as “You have 15 coins.” These lines can communicate with backup files, and the Variable Store.|
|[CommandManager](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Commands/CommandManager.cs)|This script is used to locate and execute commands within dialog files (e.g. hiding characters, changing dialog, changing sprite, etc.). For convenience, the commands have been divided into categories (“Extensions”) which are described below.|

### Interpreters
|![#C9DAF8](https://placehold.co/15x15/EAD1DB/EAD1DB.png) Logical Lines||
|:------------------------|:------------------------------|
|[ILogicalLine](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/LogicalLines/ILogicalLine.cs)|Parent class of all logical lines. It does not contain any code but only a few variables used to identify the logical lines.|
|[LL_Choice](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/LogicalLines/Types/LL_Choice.cs)|This line is used to trigger a choice during a dialog and to display the corresponding buttons. The rest of the dialogue is determined by the player's choice.|
|[LL_QTE](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/LogicalLines/Types/LL_QTE.cs)|This line is used to trigger a QTE whose result defines the rest of the dialogue to be executed.|
|[LL_Conditions](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/LogicalLines/Types/LL_Condition.cs)|These lines serve as “if/else”. They can do almost everything an “if” can do (compare bool, string, float, int values). These lines can communicate with the VariableStore or TagManager to test the values.|
|[LL_Operator](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/LogicalLines/Types/LL_Operator.cs)|These lines are used to perform operations on variables (addition, subtraction, multiplication, division, changing the values of bool, string).|

### Commands
|![#C9DAF8](https://placehold.co/15x15/EAD1DB/EAD1DB.png) Commands||
|:------------------------|:------------------------------|
|[CommandDatabase](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Commands/Database/CommandDatabase.cs)|This script contains all available commands.|
|[ExtensionGeneral](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Commands/Database/Extensions/ExtensionGeneral.cs)|This script contains “uncategorized” commands that are used in several contexts (wait, hide dialog, change dialog file, etc.).|
|[ExtensionAudio](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Commands/Database/Extensions/ExtensionAudio.cs)|This script contains audio commands (play sfx, music, etc.).|
|[ExtensionCharacters](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Commands/Database/Extensions/ExtensionCharacters.cs)|This script contains character-specific commands (show, hide, move, etc.).|
|[ExtensionGraphicPanel](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Commands/Database/Extensions/ExtensionGraphicPanels.cs)|This script contains commands specific to backgrounds (change background, clear background).|

### Game Save
|![#D9E9D3](https://placehold.co/15x15/D9E9D3/D9E9D3.png) Game Save||
|:------------------------|:------------------------------|
|[VNGameSave](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/VNSystem/VNGameSave.cs)|This script contains all the data to be saved. It is serialized into JSON when saved, before being encrypted using an XOR algorithm, making it unreadable and immutable by players. It is also capable of decrypting save files and loading them when requested by the player.|

### Database
|![#C9DAF8](https://placehold.co/15x15/FFF2CC/FFF2CC.png) Database||
|:------------------------|:------------------------------|
|[VNDatabaseLinkSetup](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/VNSystem/VNDatabaseLinksSetup.cs)|This script is used to link Variable Store variables to keywords so that they can be accessed in dialog files. To call a variable in a dialog file, use “$” in front of the defined keyword. The whole will then be replaced by the value of this variable.|
|[VariableStore](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/Dialog/LogicalLines/VariableStore.cs)|This script contains all the game variables. This is where they are stored when a dialog file needs to create a new one for example.|

### Configs
|![#C9DAF8](https://placehold.co/15x15/FCE5CD/FCE5CD.png) Logical Lines||
|:------------------------|:------------------------------|
|[DialogueConfig](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/ScriptableObjects/DialogConfig.cs)|This scriptableObject contains variables that can be modified by the user in order to personalize their experience.|
|[CharacterConfig](https://github.com/hugojobe/PMJV_Atelier/blob/main/Assets/_Main/Scripts/ScriptableObjects/CharacterConfig.cs)|This scriptableObject contains all character-related data (dialog color, name, sprites, etc.).|
