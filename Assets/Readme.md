Project Setup Guide
1. Initializing the Project
Open Unity Hub:

Create a new project using your preferred version of Unity (preferably Unity 2022.3.x or later if you're using Fusion networking features).
Select Project Template:

Choose the "3D Core" template as this project heavily relies on 3D models, networking, and mechanics.
2. Import Required Assets
Import Essential Packages:

Go to Window > Package Manager and install the following packages:
Cinemachine: For advanced camera control.
Input System: For handling modern input management.
Photon Fusion: This is the networking solution we're using. You can download it from the Unity Asset Store or Photon’s website.
Download and Import Models/Assets:
![image](https://github.com/user-attachments/assets/8cd1b3af-0c06-4547-b52f-ce62b6717fe1)

If you have any 3D models, animations, or prefabs needed for characters, environments, or other elements, import them into the Assets/Models folder.
3. Folder Structure Setup
Create the following folder structure in the Assets directory:

- Scripts
- Prefabs
- Models
- Materials
- Textures
- Animations
- Scenes
- UI
- Audio
This organization will help keep the project clean and manageable.
4. Setting Up Photon Fusion
Register Your Photon App:

Visit the Photon Dashboard and create a new application to get your App ID.
Integrate Photon Fusion:

Import the Photon Fusion package into Unity.
In the Photon folder, find PhotonAppSettings and paste your App ID.
5. Creating Core Scenes
Create Main Scenes:
Create a new scene called MainMenu and another called GameScene (or any name that suits your game) in the Assets/Scenes folder.
Set MainMenu as the default scene that loads when the project starts.
6. Implementing Networking
Setting Up Network Runner:

Create an empty GameObject called NetworkManager in your scene and attach the NetworkManager.cs script.
Assign the NetworkRunner component to manage multiplayer sessions.
Spawning Players:

Create a PlayerPrefab GameObject with the necessary components (CharacterController, Animator, etc.).
Make sure your player prefab has network components such as NetworkObject attached.
7. Configuring Camera and Input
Cinemachine FreeLook Camera:

Create a new Cinemachine FreeLook camera in your scene.
Attach it to the player using the ThirdPersonController.cs script for seamless movement and control.
Setting Up Input System:

Replace the default Input settings with Unity’s new Input System for better control. Make sure your PlayerInput component is configured correctly with actions like Move, Jump, Attack, etc.
8. UI Setup
Health and Mana Bars:

Create a new Canvas and add Slider elements for health and mana bars.
Reference these elements in your UIManager.cs script to update the player stats in real-time.
Inventory and Quest Log:

Create a simple UI layout for inventory and quest logs. This can be done using Text and Image elements and will be managed by the UIManager.cs.
9. Adding Game Mechanics and Systems
Assign Scripts to GameObjects:

Attach the respective scripts to relevant GameObjects. For example:
PlayerPrefab gets ThirdPersonController.cs, InventorySystem.cs, SkillSystem.cs.
NetworkManager GameObject receives GameManager.cs and NetworkRunner.
Set Up Managers:

Attach and configure managers like CombatSystem.cs, QuestManager.cs, and WeatherSystem.cs to manage gameplay elements.
10. Final Configurations
Lighting and Skybox:

Configure lighting settings in the Lighting tab, and ensure your Sun (Directional Light) is correctly referenced in WeatherSystem.cs.
Assign a suitable Skybox in the environment settings.
Build Settings:

Go to File > Build Settings and add both MainMenu and GameScene to the build list.
Set the MainMenu as the first scene to ensure the game starts correctly.
Testing Multiplayer:

Launch multiple instances of your game by building and running the project or by using multiple Unity editors. Test joining and leaving sessions, character controls, and gameplay mechanics.
Troubleshooting Tips
Networking Issues:

Ensure Photon App ID is correctly set up, and check network permissions in Unity if you face connection problems.
Input System Errors:

Make sure your Input Actions asset is assigned to the PlayerInput component, and mappings are correctly set.
Script Errors:

Always verify that your components are properly attached to GameObjects. Missing scripts or unassigned references can cause runtime errors.

License
This project is licensed under the MIT License - see the LICENSE file for details.

Contact
For any inquiries or suggestions, please feel free to reach out via GitHub Issues or directly to the project maintainer.
