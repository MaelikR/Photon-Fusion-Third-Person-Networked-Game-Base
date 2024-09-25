Project Overview
This repository is a comprehensive base for an online multiplayer game with multiple systems such as combat, crafting, networking, and more. Each script represents a different aspect of the game, ensuring modularity and ease of scalability. This README provides detailed explanations for each script file, highlighting its role and functionality within the game.

Script Descriptions
1. AbilityManager.cs
Purpose: Manages player abilities, including activation, cooldowns, and effects.
Details: Handles different abilities that players can use, tracks cooldowns, and ensures abilities are applied correctly based on player input and game state.
AbilityManager.cs
Purpose
The AbilityManager.cs script is responsible for managing the abilities available to the player, displaying them in the game's UI, and providing functionality for activating and switching abilities during gameplay.

How It Works
Singleton Pattern:

The script uses a Singleton pattern (Instance) to ensure that there's only one instance of AbilityManager throughout the game. This allows other scripts to easily access and interact with the AbilityManager.
Ability Slots and Available Abilities:

abilitySlots: An array that holds references to the UI slots where abilities will be displayed. Each slot is represented by an AbilitySlot class that manages individual abilities.
availableAbilities: A list that contains all abilities that the player can use. Each ability is represented by an Ability class.
Awake Method:

The Awake() method ensures the AbilityManager follows the Singleton pattern. If another instance exists, it will be destroyed to maintain a single active instance in the game.
Start Method:

The Start() method is called when the script is first activated. It calls AssignAbilities() to initialize the ability slots with the abilities available to the player.
AssignAbilities Method:

The AssignAbilities() method loops through the abilitySlots and availableAbilities lists and assigns each ability to a corresponding UI slot. It also sets the activationKey for each ability, allowing the player to use keyboard shortcuts to activate abilities.
ReassignAbilities Method:

The ReassignAbilities() method is used to update the ability slots, ensuring they match the availableAbilities list. It either assigns an ability to the slot or clears the slot if no ability is available.
How To Use This Script
Setting Up the Ability Manager:

Attach the AbilityManager script to a GameObject in your scene (e.g., an empty GameObject named "AbilityManager").
Make sure to assign the abilitySlots array with the UI slots representing the player's abilities in the Unity Inspector. These slots should use the AbilitySlot component.
Adding Abilities:

Populate the availableAbilities list with the abilities you want the player to have access to. Each ability should be an instance of the Ability class, containing information like the name, description, activation key, cooldown, and effect.
Updating Abilities:

Call ReassignAbilities() whenever you want to update the abilities available to the player. For example, you may want to update abilities when the player levels up, learns a new skill, or changes their equipment.
Practical Example of Usage
Imagine a player progressing through your game and unlocking a new fireball ability:

You would add this fireball ability to the availableAbilities list and call ReassignAbilities() to ensure it appears in the next available UI slot.

Ability newFireball = new Ability() { name = "Fireball", keyCode = KeyCode.F, cooldown = 5f };
AbilityManager.Instance.availableAbilities.Add(newFireball);
AbilityManager.Instance.ReassignAbilities();
The AbilityManager would then assign the new fireball ability to an available slot, and the player can activate it by pressing the assigned key.

Additional Notes
Ensure you have a correctly implemented Ability and AbilitySlot class that interacts with this script. These classes should handle individual ability details and the functionality for displaying and activating abilities.
The AbilityManager can be further customized to include features such as drag-and-drop ability assignment, cooldown timers, and visual feedback when an ability is activated or on cooldown.



3. ChatSystem.cs
Purpose: Manages the in-game chat functionality.
Details: Allows players to communicate with each other. Includes support for different chat channels such as global, team, and private messages.
ChatSystem.cs
Purpose
The ChatSystem.cs script provides a simple but effective communication system for players in a multiplayer game. It allows players to send messages either globally (visible to all players) or to a specific party (visible only to party members) using the Photon Fusion networking system.

How It Works
NetworkBehaviour Inheritance:

The ChatSystem script inherits from NetworkBehaviour, allowing it to leverage Photon Fusion's networking capabilities for multiplayer communication.
Remote Procedure Calls (RPCs):

This script uses Fusion's Rpc attribute to implement Remote Procedure Calls, which enable communication between clients and the server across the network.
There are two primary RPC methods:
RPC_SendMessageToGlobal: Sends a chat message to all players in the game.
RPC_SendMessageToParty: Sends a chat message only to players within the same party.
RPC Method Details:

Both RPC_SendMessageToGlobal and RPC_SendMessageToParty are marked with [Rpc(RpcSources.InputAuthority, RpcTargets.All)], meaning they can be called by the client with input authority, and their results are sent to all clients connected to the game.
These RPCs use UnityEngine.Debug.Log to print the chat message to the Unity console. In a complete implementation, these methods would be expanded to display messages in an in-game chat UI.
SendMessage Methods:

SendMessageToGlobal(string playerName, string message): Calls the RPC_SendMessageToGlobal method if the local player has input authority.
SendMessageToParty(string playerName, string message): Calls the RPC_SendMessageToParty method if the local player has input authority.
These methods ensure that only the player with authority over this object can send messages, maintaining proper network communication control.
How To Use This Script
Setting Up the Chat System:

Attach the ChatSystem script to a GameObject in your scene, such as a "GameManager" or "NetworkManager" GameObject.
Ensure that the object to which this script is attached is also a NetworkObject so that it can interact with Fusion's networking functionalities.
Sending Messages:

To send a message to the global chat, call SendMessageToGlobal(playerName, message) with the player's name and the message content.
To send a message to the party chat, call SendMessageToParty(playerName, message) with the player's name and the message content.
For example:

ChatSystem chatSystem = FindObjectOfType<ChatSystem>();
chatSystem.SendMessageToGlobal("Player1", "Hello everyone!");
chatSystem.SendMessageToParty("Player1", "Let's gather at the town square.");
Network Authority:

Ensure that the GameObject with the ChatSystem script has appropriate network authority. Only the player who has input authority over this object can send messages using the SendMessageToGlobal and SendMessageToParty methods.
Practical Example of Usage
Imagine two players in a multiplayer game:

Player 1 wants to send a global message to all players, so they call SendMessageToGlobal("Player1", "Welcome to the game!"). This triggers the RPC_SendMessageToGlobal method and sends the message to every connected client, who will then display it in their chat window.
Player 2, who is part of Player 1's party, wants to send a message only to their party. They call SendMessageToParty("Player2", "Let's move to the next objective!"). This message is only sent to party members, not to all players.
Additional Notes
The current implementation only logs messages to the Unity console (UnityEngine.Debug.Log). To fully integrate this into an in-game chat system, additional code is required to update the game's UI and display messages to the player.
This script is flexible and can be further expanded to support more chat channels, such as guild chat, whispers, or zone-specific messages.
The use of RpcSources.InputAuthority and RpcTargets.All ensures that messages are only sent by authorized clients and received by all connected clients, maintaining synchronization across the network.


5. CombatSystem.cs
Purpose: Governs the combat mechanics between players and enemies.
Details: Handles attack calculations, damage application, health reduction, and other combat-related actions like hit detection, critical hits, and combo management.
CombatSystem.cs
Purpose
The CombatSystem.cs script is responsible for handling the combat mechanics within a multiplayer game using the Photon Fusion networking system. It manages player attacks, skills, cooldowns, and interactions between the player and enemies, ensuring a seamless and synchronized experience in a networked environment.

How It Works
NetworkBehaviour Inheritance:

The CombatSystem script inherits from NetworkBehaviour, allowing it to utilize Photon Fusion’s networking capabilities, ensuring synchronized combat across all players.
CharacterStats Integration:

The script interacts with two CharacterStats objects:
playerStats: Represents the player's statistics, such as health, mana, and other combat-related attributes.
enemyStats: Represents the enemy’s statistics, which the player will engage with during combat.
Skill Management:

The script maintains a List<Skill> availableSkills, representing all skills that the player can use during combat. Each Skill object contains the skill's name, damage, mana cost, cooldown, and remaining cooldown time.
Network Synchronization:

The script utilizes Fusion’s Rpc system to synchronize combat actions across the network. This ensures that all players see the same combat interactions, regardless of who initiates them.
Core Functionalities
FixedUpdateNetwork:

This method overrides Fusion's FixedUpdateNetwork and manages the cooldown of each skill.
Every frame, it reduces the cooldownRemaining for each skill by Runner.DeltaTime, which is a Fusion-provided time value for network synchronization.
RPC_Attack Method:

The RPC_Attack method is invoked as an Rpc using [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]. This allows a player with input authority to initiate an attack, but the execution is handled by the state authority, ensuring consistent combat outcomes across the network.
When called, the method:
Finds the skill by its name from the availableSkills list.
Checks if the skill is ready to be used (i.e., its cooldown has expired and the player has enough mana).
Deducts the required mana and resets the skill’s cooldown.
Applies damage to the enemyStats and logs the attack action.
If the skill is not ready or the player lacks sufficient mana, it logs an appropriate message.
Update Method:

This method is responsible for detecting player input:
When the player presses the "1" key (KeyCode.Alpha1), the RPC_Attack method is called with the skill name "Fireball", but only if the player has input authority over this object.
Skill Class
The Skill class represents an individual skill that a player can use in combat:

Attributes:
skillName: The name of the skill.
damage: The amount of damage the skill deals.
manaCost: The mana cost required to use the skill.
cooldown: The time required before the skill can be used again.
cooldownRemaining: The remaining time before the skill is ready to be used again, marked as [Networked] to synchronize it across all players in the network.
How To Use This Script
Setting Up the Combat System:

Attach the CombatSystem script to a player GameObject in your scene.
Ensure that the player GameObject has the CharacterStats component attached and appropriately configured for both playerStats and enemyStats.
Defining Skills:

You can define the skills available to the player by populating the availableSkills list. Each skill must be defined with a name, damage, mana cost, and cooldown.
Example of adding a skill:


Skill fireball = new Skill { skillName = "Fireball", damage = 30, manaCost = 10, cooldown = 5f };
combatSystem.availableSkills.Add(fireball);
Triggering Skills:

The script currently listens for the KeyCode.Alpha1 key press to trigger the "Fireball" skill. You can expand this by adding more key bindings to trigger other skills.
Networking Authority:

Ensure that the GameObject with this script is a NetworkObject and that the player has appropriate input authority to trigger the RPC_Attack method.
Practical Example of Usage
In a typical gameplay scenario:

Player 1 decides to use a "Fireball" attack. When they press the "1" key, RPC_Attack("Fireball") is called. This attack deducts mana from playerStats and applies damage to enemyStats. The attack is synchronized across all players, ensuring everyone sees the same result.
The cooldown for the "Fireball" skill is updated, preventing Player 1 from using it again until the cooldown expires.
Additional Notes
The script currently supports only one attack skill using the "1" key. You can expand this functionality by adding more key checks and calls to RPC_Attack for additional skills.
The current implementation is designed for PvE (Player vs. Environment) but can be extended to support PvP (Player vs. Player) by replacing enemyStats with other players' CharacterStats.
The cooldown management and mana cost checks ensure that players cannot spam skills, maintaining balanced gameplay.



7. CraftingSystem.cs
Purpose: Provides crafting functionality for players to create items.
Details: Allows players to combine resources into new items based on predefined recipes. Manages inventory, crafting success/failure rates, and resource consumption.
CraftingSystem.cs
The CraftingSystem.cs script manages crafting mechanics in the game. It allows players to craft items using predefined recipes and ensures that players have the required materials in their inventory before crafting an item.

Key Functionalities:
AddRecipe(CraftingRecipe recipe): Adds a new crafting recipe to the available recipes list.
CraftItem(string recipeName, InventorySystem inventory): Crafts an item by checking if the player has the necessary materials and deducts them from the inventory.
RPC_NotifyCraftingSuccess(string itemName): Displays a message when an item is successfully crafted.
Usage: Attach this script to a game object to manage crafting mechanics in a networked environment. The script ensures that crafting actions are synchronized across the network.



8. DialogueSystem.cs
Purpose: Manages NPC and player dialogues.
Details: Provides a structured way for players to interact with NPCs. Handles branching dialogue options, quest acceptance, and storytelling elements.
DialogueSystem.cs
The DialogueSystem.cs script handles interactive dialogues between the player and NPCs or other characters. It allows for dialogue lines and choices to be displayed in a structured manner.

Key Functionalities:

StartDialogue(Dialogue dialogue): Initiates a dialogue with a character, displaying lines one by one.
DisplayNextLine(Dialogue dialogue): Displays the next line in the dialogue sequence.
DisplayPlayerChoices(Dialogue dialogue): Presents dialogue choices to the player.
EndDialogue(): Ends the dialogue interaction.
Usage: Attach this script to manage conversations in your game. It provides a queue-based system to display dialogue lines and choices, allowing for interactive storytelling.



9. EventSystem.cs
Purpose: Coordinates in-game events and triggers.
Details: Manages timed events, player-triggered events, and world events that impact gameplay. This can include things like holiday events, world bosses, or special in-game celebrations.
EventSystem.cs
The EventSystem.cs script controls events that occur within the game world. It manages the initiation, handling, and termination of events, ensuring that they are synchronized across the network.

Key Functionalities:

StartEvent(string newEvent): Begins a new event and handles its logic.
EndEvent(): Ends the ongoing event.
HandleEvent(): A coroutine that manages the event’s duration and actions.
Usage: This script is essential for controlling game events, making sure they are executed properly and visible to all players in a multiplayer environment.


10. GameManager.cs
Purpose: Central controller of the game’s main flow and state management.
Details: Initializes game systems, manages transitions between game states (e.g., main menu, in-game, pause), and handles high-level game logic.
GameManager.cs
The GameManager.cs script serves as the central manager for handling network initialization and player management using Photon Fusion. It manages player spawning, game startup, and network-related callbacks.

Key Functionalities:

Start(): Initializes and starts the network game session.
OnPlayerJoined(NetworkRunner runner, PlayerRef player): Spawns a player when they join the game.
OnPlayerLeft(NetworkRunner runner, PlayerRef player): Despawns a player when they leave the game.
Usage: Attach this script to a GameObject to handle network initialization and player management. It allows automatic player spawning and integrates with Fusion’s callback system for smooth multiplayer experiences.



11. GuildSystem.cs
Purpose: Manages player guilds and their related functionality.
Details: Allows players to create, join, and manage guilds. Handles guild ranks, permissions, chat, and member management.
GuildSystem.cs
The GuildSystem.cs script manages the creation and management of player guilds in the game.

Key Functionalities:

CreateGuild(string guildName, string founderName): Creates a new guild with the specified name and founder.
AddMemberToGuild(Guild guild, string playerName): Adds a player to an existing guild.
RPC_NotifyGuildUpdate(string message): Notifies all players about guild-related updates.
Usage: This script facilitates guild formation and management, allowing players to join, leave, or create guilds within a networked environment.



12. HousingSystem.cs
Purpose: Provides player housing features.
Details: Allows players to own, customize, and decorate personal or guild housing. Manages furniture placement, storage, and housing permissions.
HousingSystem.cs
The HousingSystem.cs script handles the purchase, placement, and customization of player housing.

Key Functionalities:

BuyHouse(Vector3 position): Allows the player to buy and place a house at a specified position.
CustomizeHouse(string customization): Allows customization of the house.
RPC_NotifyHouseBought() and RPC_NotifyHouseCustomized(string customization): Notify all players about house-related actions.
Usage: Use this script to enable a housing system for players, where they can own, place, and customize their own homes in a multiplayer environment.



13. InstanceManager.cs
Purpose: Manages dungeon or instance creation and player assignment.
Details: Handles the creation of private or group instances for players, ensuring proper spawning, instance limits, and boss encounters.
InstanceManager.cs
The InstanceManager.cs script manages game instances, which are separate, isolated environments that players can join.

Key Functionalities:

CreateInstance(string instanceName): Creates a new instance with the specified name.
JoinInstance(string instanceName, string playerName): Allows a player to join an existing instance.
RPC_NotifyInstanceUpdate(string message): Notifies all players about instance-related updates.
Usage: Use this script to manage instances in your game, allowing for the creation and joining of unique, separate game environments.



14. InventorySystem.cs
Purpose: Manages player inventory, items, and equipment.
Details: Handles item pickup, storage, usage, and equipment management. Provides functionality for equipping/unequipping gear and organizing inventory slots.
InventorySystem.cs
The InventorySystem.cs script handles the player's inventory, allowing items to be added, removed, or tracked.

Key Functionalities:

AddItem(Item newItem): Adds a new item to the inventory.
RemoveItem(Item itemToRemove): Removes a specified item from the inventory.
RPC_NotifyItemAdded(string itemName): Notifies all players when an item is added to the inventory.
Usage: This script manages player inventory, enabling item collection, tracking, and removal in a networked setting.



15. LootSystem.cs
Purpose: Governs loot drops and distribution.
Details: Manages item drops from enemies, bosses, and world objects. Handles loot roll mechanics, loot tables, and distribution rules for parties or raids.
LootSystem.cs
The LootSystem.cs script handles loot drops in the game, determining what items players can collect from defeated enemies or found in the world.

Key Functionalities:

DropLoot(): Randomly determines whether loot drops based on a predefined chance.
RPC_NotifyLootDrop(string itemName): Notifies all players when loot is dropped.
Usage: Use this script to manage loot drops from enemies or other sources, making the process interactive and visible to all players.



16. MarketSystem.cs
Purpose: Manages in-game marketplace transactions.
Details: Allows players to buy and sell items on a marketplace. Supports listing items, bidding, and completing transactions.
LootSystem.cs
The LootSystem.cs script handles loot drops in the game, determining what items players can collect from defeated enemies or found in the world.

Key Functionalities:

DropLoot(): Randomly determines whether loot drops based on a predefined chance.
RPC_NotifyLootDrop(string itemName): Notifies all players when loot is dropped.
Usage: Use this script to manage loot drops from enemies or other sources, making the process interactive and visible to all players.



17. MatchmakingSystem.cs
Purpose: Facilitates player matchmaking for PvP and PvE activities.
Details: Matches players based on criteria such as skill level, experience, or gear for different game modes, such as arenas, dungeons, or battlegrounds.
MatchmakingSystem.cs
The MatchmakingSystem.cs script handles the logic for queuing players for raid events and managing raid initiation.

Key Functionalities:

QueueForRaid(GameObject player): Adds a player to the raid queue. When the number of players reaches the required threshold (playersNeededForRaid), the StartRaid method is called.
StartRaid(): Starts a raid with all queued players and logs their participation.
RPC_NotifyRaidMatch(): Sends a notification to all players when matchmaking is successful and the raid is starting.
Usage: Use this script to manage player queuing and initiate raid battles when enough players are ready.



18. MountSystem.cs
Purpose: Manages mount acquisition and usage.
Details: Allows players to collect, summon, and use mounts. Handles mount speed, abilities, and customization.
MountSystem.cs
The MountSystem.cs script manages the summoning and dismissing of player mounts in the game.

Key Functionalities:

SummonMount(Vector3 position): Spawns a mount at the specified position if the player doesn't already have one summoned. If the player already has a mount, it will dismiss the current mount instead.
Dismount(): Dismisses the current mount if one is active.
RPC_NotifyMountSummon() and RPC_NotifyMountDismount(): Notify all players when a mount is summoned or dismissed.
Usage: Integrate this script for a mount system where players can summon and dismiss mounts in a multiplayer environment.



19. NetworkManager.cs
Purpose: Oversees the networking aspects of the game.
Details: Handles player connections, data synchronization, and network events to ensure seamless multiplayer experiences.
NetworkManager.cs
The NetworkManager.cs script is responsible for managing network sessions, including starting games, joining sessions, creating rooms, and leaving rooms.

Key Functionalities:

StartGame(GameMode mode): Initializes the NetworkRunner and starts the game in the specified mode (e.g., Host, Client).
JoinSession(): Allows a player to join an existing game session.
CreateRoom(string roomName): Creates a new game room for players to join.
LeaveRoom(): Ends the current network session and shuts down the NetworkRunner.
Usage: This script manages networking and game session handling, allowing players to start, join, and leave multiplayer games.



20. PartySystem.cs
Purpose: Manages party creation and management.
Details: Allows players to create or join parties, manage members, share loot, and enable party-based mechanics such as shared experience gain.
PartySystem.cs
The PartySystem.cs script handles party management, allowing players to form and manage parties with other players.

Key Functionalities:

AddPlayerToParty(string playerName): Adds a player to the party if there’s space available.
RemovePlayerFromParty(string playerName): Removes a player from the party.
RPC_NotifyPartyUpdate(string message): Notifies all players about changes in the party (e.g., when someone joins or leaves).
Usage: This script provides a way for players to form parties, enabling group play in a networked setting.



21. PersistenceSystem.cs
Purpose: Handles data persistence for players and the game world.
Details: Manages saving and loading player data, including inventory, progress, and achievements. Ensures data consistency across sessions.
PersistenceSystem.cs
The PersistenceSystem.cs script handles saving and loading player data, ensuring game progress is maintained between sessions.

Key Functionalities:

SaveData(): Saves the player's character stats to a JSON file in the persistent data path.
LoadData(): Loads the character stats from a saved JSON file.
Usage: Integrate this script for a save/load system, allowing players to retain their progress across gaming sessions.



22. PvPSystem.cs
Purpose: Manages player-versus-player interactions and combat.
Details: Handles PvP-specific rules, ranking, and combat mechanics, ensuring balanced and fair encounters between players.

Here are detailed summaries of the provided scripts for inclusion in your README file:

MatchmakingSystem.cs
The MatchmakingSystem.cs script handles the logic for queuing players for raid events and managing raid initiation.

Key Functionalities:

QueueForRaid(GameObject player): Adds a player to the raid queue. When the number of players reaches the required threshold (playersNeededForRaid), the StartRaid method is called.
StartRaid(): Starts a raid with all queued players and logs their participation.
RPC_NotifyRaidMatch(): Sends a notification to all players when matchmaking is successful and the raid is starting.
Usage: Use this script to manage player queuing and initiate raid battles when enough players are ready.

MountSystem.cs
The MountSystem.cs script manages the summoning and dismissing of player mounts in the game.

Key Functionalities:

SummonMount(Vector3 position): Spawns a mount at the specified position if the player doesn't already have one summoned. If the player already has a mount, it will dismiss the current mount instead.
Dismount(): Dismisses the current mount if one is active.
RPC_NotifyMountSummon() and RPC_NotifyMountDismount(): Notify all players when a mount is summoned or dismissed.
Usage: Integrate this script for a mount system where players can summon and dismiss mounts in a multiplayer environment.

NetworkManager.cs
The NetworkManager.cs script is responsible for managing network sessions, including starting games, joining sessions, creating rooms, and leaving rooms.

Key Functionalities:

StartGame(GameMode mode): Initializes the NetworkRunner and starts the game in the specified mode (e.g., Host, Client).
JoinSession(): Allows a player to join an existing game session.
CreateRoom(string roomName): Creates a new game room for players to join.
LeaveRoom(): Ends the current network session and shuts down the NetworkRunner.
Usage: This script manages networking and game session handling, allowing players to start, join, and leave multiplayer games.

PartySystem.cs
The PartySystem.cs script handles party management, allowing players to form and manage parties with other players.

Key Functionalities:

AddPlayerToParty(string playerName): Adds a player to the party if there’s space available.
RemovePlayerFromParty(string playerName): Removes a player from the party.
RPC_NotifyPartyUpdate(string message): Notifies all players about changes in the party (e.g., when someone joins or leaves).
Usage: This script provides a way for players to form parties, enabling group play in a networked setting.

PersistenceSystem.cs
The PersistenceSystem.cs script handles saving and loading player data, ensuring game progress is maintained between sessions.

Key Functionalities:

SaveData(): Saves the player's character stats to a JSON file in the persistent data path.
LoadData(): Loads the character stats from a saved JSON file.
Usage: Integrate this script for a save/load system, allowing players to retain their progress across gaming sessions.

PvPSystem.cs
The PvPSystem.cs script manages Player vs. Player (PvP) interactions, allowing players to engage in combat within designated PvP zones.

Key Functionalities:

EnterPvPZone() and ExitPvPZone(): Marks the player as being in or out of a PvP zone and sends a notification.
AttackPlayer(GameObject targetPlayer, int damage): Allows a player to attack another player within a PvP zone, dealing damage.
Usage: Use this script to handle PvP mechanics, enabling players to engage in combat within specific areas.



23. QuestManager.cs
Purpose: Manages the game’s quest system.
Details: Provides functionality for quest creation, tracking, completion, and rewards. Supports main story quests, side quests, and daily tasks.
QuestManager.cs
The QuestManager.cs script manages the quest system, tracking player quests and their progress.

Key Functionalities:

AddQuest(Quest newQuest): Adds a new quest to the player’s quest log.
UpdateQuestProgress(Quest quest, int progress): Updates a quest’s progress, marking it as completed when the goal is reached.
RPC_UpdateQuestLog(string questUpdate): Sends quest updates to all players.
Usage: Integrate this script for a comprehensive quest system, tracking and updating player quests in real-time.



24. ReputationSystem.cs
Purpose: Manages player reputation with various factions.
Details: Tracks player actions and how they affect reputation with different in-game factions, unlocking rewards, quests, or penalties.
ReputationSystem.cs
The ReputationSystem.cs script manages player reputation with various factions in the game.

Key Functionalities:

ModifyReputation(string factionName, int reputationChange): Modifies the player’s reputation with a specified faction.
CheckReputation(string factionName): Checks the player’s current reputation with a faction.
RPC_NotifyReputationChange(string factionName, int newReputation): Sends reputation change notifications to all players.
Usage: Use this script to implement a faction reputation system, allowing players to gain or lose reputation based on their actions.



25. ServerLogic.cs
Purpose: Contains the logic executed on the server-side.
Details: Manages all server-related logic, ensuring that game events are handled correctly and consistently across all connected clients.
ServerLogic.cs
The ServerLogic.cs script handles server-side validation of player actions.

Key Functionalities:

ValidatePlayerAction(string actionType, GameObject player): Validates different player actions such as "Attack" or "Move."
Usage: This script ensures that all player actions are validated by the server, maintaining game integrity and preventing cheating.



26. SkillSystem.cs
Purpose: Manages player skills and abilities.
Details: Handles skill acquisition, leveling, and application during gameplay. Includes support for passive and active skills.
SkillSystem.cs
The SkillSystem.cs script manages player skills, handling skill usage, cooldowns, and unlocking skills.

Key Functionalities:

RPC_UseSkill(string skillName): Executes a skill action, deducting mana and applying cooldown.
UnlockSkill(string skillName): Unlocks a new skill for the player.
Usage: Use this script to manage the player’s skill set, enabling skill usage and unlocking within the game.



27. TalentSystem.cs
Purpose: Provides a talent tree or progression system for players.
Details: Allows players to unlock talents as they level up, offering customization and specialization for their characters.
TalentSystem.cs
The TalentSystem.cs script manages the player’s talents, allowing them to unlock and manage talent points.

Key Functionalities:

UnlockTalent(string talentName): Unlocks a talent if the player has enough points.
AddTalentPoints(string talentName, int points): Adds talent points to a specified talent.
Usage: Use this script to implement a talent system, enabling players to develop their character's abilities further.



28. TradingSystem.cs
Purpose: Manages item trading between players.
Details: Allows players to trade items, gold, or other resources securely. Ensures fair trade practices and prevents exploitation.
TradingSystem.cs
The TradingSystem.cs script manages item trading between players.

Key Functionalities:

OfferItemForTrade(string playerName, Item item): Adds an item to the trade offer for a specified player.
AcceptTrade(): Finalizes the trade between two players.
RPC_UpdateTradeStatus(string playerName, string itemName) and RPC_ConfirmTrade(): Notify players about trade status.
Usage: Integrate this script to allow item trading between players in a multiplayer environment.



29. TutorialSystem.cs
Purpose: Guides new players through the game mechanics.
Details: Provides interactive tutorials to help players understand core game features and controls.
TutorialSystem.cs
The TutorialSystem.cs script guides the player through a tutorial, teaching basic game mechanics.

Key Functionalities:

DisplayNextStep(): Displays the next step in the tutorial.
EndTutorial(): Ends the tutorial once all steps are completed.
Usage: Use this script to guide new players through a tutorial on game controls and mechanics.



30. UIManager.cs
Purpose: Manages the user interface elements.
Details: Handles displaying HUD elements, menus, dialogs, and other UI components. Ensures a smooth and responsive player experience.
UIManager.cs
The UIManager.cs script manages the game’s user interface, updating health bars, mana bars, and the quest log.

Key Functionalities:

UpdateHealthBar() and UpdateManaBar(): Update the player’s health and mana UI elements.
RPC_UpdateQuestLog(string newQuest): Updates the quest log UI for all players.
Usage: This script controls the player’s in-game UI, ensuring that health, mana, and quests are displayed correctly.



31. VoiceChatSystem.cs
Purpose: Manages in-game voice communication.
Details: Enables voice chat functionality for players, allowing them to communicate with teammates or other players.
(under dev)


32. WeatherSystem.cs
Purpose: Controls dynamic weather effects in the game world.
Details: Manages weather changes such as rain, snow, fog, and other effects. These can influence gameplay, visibility, and world aesthetics.
WeatherSystem.cs
The WeatherSystem.cs script controls the day/night cycle and weather effects.

Key Functionalities:

ChangeWeather(string weatherType): Changes the weather to rain, snow, or clear.
RPC_NotifyDayCycleChange(bool isDay): Notifies players about day/night changes.
Usage: Use this script to implement dynamic weather and day/night cycles in your game world.



33. WorldManager.cs
Purpose: Manages world settings and interactions.
Details: Oversees the game world’s state, including day/night cycles, environmental changes, and world events.
WorldManager.cs
The WorldManager.cs script manages different zones in the game world and handles scene transitions.

Key Functionalities:

LoadZone(string zoneName): Loads a new game zone.
UnloadZone(string zoneName): Unloads an existing zone.
TravelToZone(string newZone): Facilitates player travel between zones.
Usage: Use this script to manage world zones, allowing for dynamic scene transitions and exploration.

(Under development base game Framework)

MRenDev GameStudio
