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

csharp
Copier le code
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
8. DialogueSystem.cs
Purpose: Manages NPC and player dialogues.
Details: Provides a structured way for players to interact with NPCs. Handles branching dialogue options, quest acceptance, and storytelling elements.
9. EventSystem.cs
Purpose: Coordinates in-game events and triggers.
Details: Manages timed events, player-triggered events, and world events that impact gameplay. This can include things like holiday events, world bosses, or special in-game celebrations.
10. GameManager.cs
Purpose: Central controller of the game’s main flow and state management.
Details: Initializes game systems, manages transitions between game states (e.g., main menu, in-game, pause), and handles high-level game logic.
11. GuildSystem.cs
Purpose: Manages player guilds and their related functionality.
Details: Allows players to create, join, and manage guilds. Handles guild ranks, permissions, chat, and member management.
12. HousingSystem.cs
Purpose: Provides player housing features.
Details: Allows players to own, customize, and decorate personal or guild housing. Manages furniture placement, storage, and housing permissions.
13. InstanceManager.cs
Purpose: Manages dungeon or instance creation and player assignment.
Details: Handles the creation of private or group instances for players, ensuring proper spawning, instance limits, and boss encounters.
14. InventorySystem.cs
Purpose: Manages player inventory, items, and equipment.
Details: Handles item pickup, storage, usage, and equipment management. Provides functionality for equipping/unequipping gear and organizing inventory slots.
15. LootSystem.cs
Purpose: Governs loot drops and distribution.
Details: Manages item drops from enemies, bosses, and world objects. Handles loot roll mechanics, loot tables, and distribution rules for parties or raids.
16. MarketSystem.cs
Purpose: Manages in-game marketplace transactions.
Details: Allows players to buy and sell items on a marketplace. Supports listing items, bidding, and completing transactions.
17. MatchmakingSystem.cs
Purpose: Facilitates player matchmaking for PvP and PvE activities.
Details: Matches players based on criteria such as skill level, experience, or gear for different game modes, such as arenas, dungeons, or battlegrounds.
18. MountSystem.cs
Purpose: Manages mount acquisition and usage.
Details: Allows players to collect, summon, and use mounts. Handles mount speed, abilities, and customization.
19. NetworkManager.cs
Purpose: Oversees the networking aspects of the game.
Details: Handles player connections, data synchronization, and network events to ensure seamless multiplayer experiences.
20. PartySystem.cs
Purpose: Manages party creation and management.
Details: Allows players to create or join parties, manage members, share loot, and enable party-based mechanics such as shared experience gain.
21. PersistenceSystem.cs
Purpose: Handles data persistence for players and the game world.
Details: Manages saving and loading player data, including inventory, progress, and achievements. Ensures data consistency across sessions.
22. PvPSystem.cs
Purpose: Manages player-versus-player interactions and combat.
Details: Handles PvP-specific rules, ranking, and combat mechanics, ensuring balanced and fair encounters between players.
23. QuestManager.cs
Purpose: Manages the game’s quest system.
Details: Provides functionality for quest creation, tracking, completion, and rewards. Supports main story quests, side quests, and daily tasks.
24. ReputationSystem.cs
Purpose: Manages player reputation with various factions.
Details: Tracks player actions and how they affect reputation with different in-game factions, unlocking rewards, quests, or penalties.
25. ServerLogic.cs
Purpose: Contains the logic executed on the server-side.
Details: Manages all server-related logic, ensuring that game events are handled correctly and consistently across all connected clients.
26. SkillSystem.cs
Purpose: Manages player skills and abilities.
Details: Handles skill acquisition, leveling, and application during gameplay. Includes support for passive and active skills.
27. TalentSystem.cs
Purpose: Provides a talent tree or progression system for players.
Details: Allows players to unlock talents as they level up, offering customization and specialization for their characters.
28. TradingSystem.cs
Purpose: Manages item trading between players.
Details: Allows players to trade items, gold, or other resources securely. Ensures fair trade practices and prevents exploitation.
29. TutorialSystem.cs
Purpose: Guides new players through the game mechanics.
Details: Provides interactive tutorials to help players understand core game features and controls.
30. UIManager.cs
Purpose: Manages the user interface elements.
Details: Handles displaying HUD elements, menus, dialogs, and other UI components. Ensures a smooth and responsive player experience.
31. VoiceChatSystem.cs
Purpose: Manages in-game voice communication.
Details: Enables voice chat functionality for players, allowing them to communicate with teammates or other players.
32. WeatherSystem.cs
Purpose: Controls dynamic weather effects in the game world.
Details: Manages weather changes such as rain, snow, fog, and other effects. These can influence gameplay, visibility, and world aesthetics.
33. WorldManager.cs
Purpose: Manages world settings and interactions.
Details: Oversees the game world’s state, including day/night cycles, environmental changes, and world events.
