Project Overview
This repository serves as a comprehensive foundation for an online multiplayer game featuring various systems like combat, crafting, networking, and more. Each script in this repository encapsulates a different aspect of the game, ensuring modularity and easy scalability. This README provides in-depth explanations for each script file, highlighting its purpose, functionality, and role within the game.

![image](https://github.com/user-attachments/assets/ad304d07-319a-4064-9d09-03b79e999f3c)

Script Descriptions
1. AbilityManager.cs
Purpose: Manages player abilities, including their activation, cooldowns, and effects.

Details: The AbilityManager script is responsible for handling different player abilities, tracking their cooldowns, and ensuring they are applied correctly based on player input and the current game state.

How It Works:

Singleton Pattern: Uses a Singleton pattern to ensure a single instance of AbilityManager throughout the game, making it easily accessible to other scripts.
Ability Slots and Available Abilities: Manages an array of UI slots (abilitySlots) for displaying abilities and a list (availableAbilities) of all abilities available to the player.
Initialization and Assignment: The AssignAbilities() method initializes ability slots with the abilities available to the player, setting activation keys for keyboard shortcuts.
Updating Abilities: The ReassignAbilities() method updates the ability slots to match the player's current abilities.
Usage: Attach the AbilityManager script to a GameObject (e.g., an empty GameObject named "AbilityManager") and assign the abilitySlots array with the UI slots representing player abilities. Populate the availableAbilities list with the abilities the player can access.

Practical Example: When a player unlocks a new ability (e.g., a fireball), you add it to the availableAbilities list and call ReassignAbilities() to update the UI.

2. ChatSystem.cs
Purpose: Manages in-game chat functionality, allowing player communication.

Details: The ChatSystem script enables players to communicate through different channels, such as global or party chats, using Photon Fusion's networking system.

How It Works:

Networking: Inherits from NetworkBehaviour to utilize Photon Fusion's networking capabilities.
Remote Procedure Calls (RPCs): Utilizes RPCs for sending messages to all players or specific party members.
Usage: Attach the script to a GameObject (e.g., "GameManager") and ensure it’s a NetworkObject to interact with Fusion's networking functionalities.

3. TalentSystem.cs
Purpose:
Provides a talent tree or progression system for players.

Details:

Allows players to unlock talents as they level up, offering customization and specialization for their characters.
Manages the allocation of talent points, talent trees, and the effects they grant to the player.
Usage:
Integrate this script to create a talent system, allowing players to develop their characters' abilities and progress over time.

4. CombatSystem.cs
Purpose: Handles combat mechanics between players and enemies.

Details: The CombatSystem manages player attacks, skills, cooldowns, and interactions between players and enemies within a networked environment.

How It Works:

Skill Management: Maintains a list of available skills, tracking their cooldowns and mana requirements.
Network Synchronization: Uses Photon Fusion's RPCs to synchronize combat actions across the network.
Usage: Attach the script to a player GameObject, configure playerStats and enemyStats, and define skills within the availableSkills list.

5. TradingSystem.cs
Purpose:
Manages item trading between players.

Details:

Enables secure trading of items, gold, or other resources between players.
Manages trade offers, confirmations, and the completion of trade transactions to ensure fair exchanges.
Usage:
Use this script to allow trading between players in your multiplayer game environment, ensuring all transactions are secure and properly validated.

6. CraftingSystem.cs
Purpose: Provides crafting functionality for creating items.

Details: Allows players to craft items using predefined recipes and ensures they have the required materials in their inventory before crafting.

Usage: Attach this script to manage crafting mechanics and synchronize crafting actions across the network.

7. DialogueSystem.cs
Purpose: Manages dialogues between NPCs and players.

Details: Provides structured dialogues with NPCs, including branching dialogue options, quest acceptance, and storytelling.

Usage: Attach this script to manage conversations in your game, utilizing it to display dialogue lines and choices interactively.

8. EventSystem.cs
Purpose: Coordinates in-game events and triggers.

Details: Manages events such as timed occurrences, player-triggered events, and world events that impact gameplay. Examples include holiday events, world bosses, or special celebrations.

Usage: This script controls game events, ensuring they are executed correctly and are visible to all players in a multiplayer environment.

9. GameManager.cs
Purpose: Acts as the central controller for the game’s flow and state management.

Details: The GameManager initializes game systems, manages transitions between game states, and handles high-level game logic.

Usage: Attach this script to a GameObject to manage network initialization, player management, and game session handling.

10. GuildSystem.cs
Purpose: Manages player guilds and related functionalities.

Details: Allows players to create, join, and manage guilds, handling ranks, permissions, chat, and member management.

Usage: Utilize this script to facilitate guild creation and management, enabling players to participate in guild activities within a networked environment.

11. HousingSystem.cs
Purpose: Provides player housing features.

Details: Enables players to own, customize, and decorate personal or guild housing, including furniture placement and storage.

Usage: Integrate this script for player housing, where players can own and personalize their homes in a multiplayer setting.

12. InstanceManager.cs
Purpose: Manages dungeon or instance creation and player assignment.

Details: Handles the creation of private or group instances for players, including proper spawning, instance limits, and boss encounters.

Usage: Use this script to create and manage instances, allowing players to enter unique environments separate from the main game world.
13. InventorySystem.cs
Purpose: Manages the player’s inventory, items, and equipment.

Details: Handles item pickup, storage, usage, and equipment management, enabling players to organize their inventory effectively.

Usage: Integrate this script for managing player inventory in a networked setting.

14. LootSystem.cs
Purpose: Governs loot drops and distribution.

Details: Manages item drops from enemies, bosses, and world objects, including loot roll mechanics, loot tables, and distribution rules for parties or raids.

Usage: This script handles loot drops, making the process interactive and visible to all players in a multiplayer environment.

15. MarketSystem.cs
Purpose: Manages in-game marketplace transactions.

Details: Allows players to buy and sell items, supporting listing, bidding, and completing transactions.

Usage: Integrate this script to provide a functional marketplace where players can engage in trading activities.

16. MatchmakingSystem.cs
Purpose: Facilitates player matchmaking for PvP and PvE activities.

Details: Matches players based on criteria such as skill level, experience, or gear for different game modes like arenas, dungeons, or battlegrounds.

Usage: Use this script to manage player queuing and initiate events like raid battles when enough players are ready.

17. MountSystem.cs
Purpose: Manages mount acquisition and usage.

Details: Allows players to collect, summon, and use mounts, handling speed, abilities, and customization.

Usage: Integrate this script to enable a mount system where players can summon and dismiss mounts.

18. NetworkManager.cs
Purpose: Oversees the networking aspects of the game.

Details: Handles player connections, data synchronization, and network events, ensuring a seamless multiplayer experience.

Usage: This script manages networking and game session handling, allowing players to start, join, and leave multiplayer games.

19. PartySystem.cs
Purpose: Manages party creation and functionality.

Details: Allows players to form or join parties, share loot, and enable party-based mechanics like shared experience gain.

Usage: Use this script to facilitate group play and enable party management in your multiplayer game.

20. PersistenceSystem.cs
Purpose: Handles data persistence for players and the game world.

Details: Manages saving and loading player data, including inventory, progress, and achievements, ensuring consistency across sessions.

Usage: Use this script to maintain player progress and ensure game data is saved and loaded correctly.

21. PvPSystem.cs
Purpose: Manages player-versus-player interactions and combat.

Details: Handles PvP-specific rules, ranking, and combat mechanics, ensuring fair encounters between players.

Usage: Utilize this script to enable PvP mechanics, allowing players to engage in combat within designated PvP zones.

22. QuestManager.cs
Purpose: Manages the game’s quest system.

Details: Provides functionality for quest creation, tracking, completion, and rewards, supporting main story quests, side quests, and daily tasks.

Usage: Use this script to implement a quest system, tracking player quests and their progress in real-time.

23. ReputationSystem.cs
Purpose: Manages player reputation with various factions.

Details: Tracks player actions and their impact on faction reputation, unlocking rewards, quests, or penalties.

Usage: Integrate this script to manage a faction reputation system, allowing players to gain or lose reputation based on their actions.

24. ServerLogic.cs
Purpose: Contains the logic executed on the server-side.

Details: Manages all server-related logic, ensuring consistent handling of game events across all connected clients.

Usage: Use this script to validate player actions on the server, maintaining game integrity and preventing cheating.

25. SkillSystem.cs
Purpose: Manages player skills and abilities.

Details: Handles skill acquisition, leveling, cooldowns, and skill usage during gameplay, supporting both passive and active skills.

Usage: Attach this script to manage the player's skill set, enabling skill usage and progression.

26. TalentSystem.cs
Purpose: Provides a talent tree or progression system for players.

Details: Allows players to unlock talents as they level up, offering customization and specialization for their characters.

Usage: Utilize this script to implement a talent system, enabling players to enhance their character's abilities.

27. TutorialSystem.cs
Purpose: Guides new players through the game mechanics.

Details: Provides interactive tutorials to help players understand core game features and controls.

Usage: Use this script to guide new players through a tutorial, ensuring they understand basic game mechanics.

28. UIManager.cs
Purpose: Manages the user interface elements.

Details: Handles the display of HUD elements, menus, dialogs, and other UI components to ensure a smooth player experience.

Usage: Use this script to control the player’s in-game UI, updating health, mana, and quest displays accurately.

29. VoiceChatSystem.cs
Purpose: Manages in-game voice communication.

Details: Enables voice chat functionality for players, allowing them to communicate with teammates or other players.

Note: This feature is still under development.

30. WeatherSystem.cs
Purpose: Controls dynamic weather effects in the game world.

Details: Manages weather changes such as rain, snow, fog, and other effects, which can influence gameplay and aesthetics.

Usage: Integrate this script to implement dynamic weather and day/night cycles in your game world.

31. WorldManager.cs
Purpose: Manages world settings and interactions.

Details: Oversees the game world’s state, including day/night cycles, environmental changes, and world events.

Usage: Use this script to manage world zones, allowing for dynamic scene transitions and exploration.

By following the descriptions and usage guidelines, you'll be able to integrate these scripts seamlessly into your online multiplayer game, leveraging a solid foundation for combat, crafting, networking, and other essential game mechanics.

(Under development base game Framework)

MRenDev GameStudio
