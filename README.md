# Photon-Fusion-Third-Person-Networked-Game-Base
This project is a third-person multiplayer game using Unity with Photon Fusion for networking. The game features a third-person controller setup with player movement, camera management using Cinemachine, and interaction with various game elements.
![image](https://github.com/user-attachments/assets/42641b03-0cca-49d5-8d52-72e6ede680bc)
Overview
This project is a third-person multiplayer game using Unity with Photon Fusion for networking. The game features a third-person controller setup with player movement, camera management using Cinemachine, and interaction with various game elements. The setup is designed to handle both local and remote player instances, ensuring smooth synchronization and control over networked gameplay. This project integrates advanced networking techniques with Unity's physics and animation systems.

Technologies Used

Unity: Version 2022.3.47f1
Photon Fusion: For managing networking, synchronization, and real-time multiplayer interactions.
Cinemachine: For camera control, providing a dynamic and smooth third-person camera experience.
Input System: Uses Unity's new Input System for capturing player input, ensuring better flexibility and control.
C# Scripting: Custom scripts manage player behavior, camera management, and network interactions.
Project Structure
Scripts:
ThirdPersonController.cs: Manages the player's movement, animations, and interactions. Handles both local and networked player logic.
StarterAssetsInputs.cs: Captures player input and provides values for movement, camera control, jumping, and sprinting.
PlayerSpawnerM.cs: Handles the spawning of player objects within the networked environment, ensuring each player has a unique instance with proper authority and control.
Prefabs:
PlayerPrefab: Contains the player model, animations, character controller, and relevant scripts.
Main Camera: Manages the player's camera with Cinemachine FreeLook for smooth third-person camera controls.
Scenes:
SampleScene: The main scene where all gameplay takes place, containing environmental elements and player spawn points.
Features


1. Player Movement & Control
Third-Person Controller: A character controller that supports smooth movement, jumping, and sprinting.
Cinemachine Integration: The camera follows and adjusts based on the player's movements, providing an immersive third-person view.
Network Synchronization: Utilizes Photon Fusion to manage player positions, actions, and animations across the network, ensuring all players see each other's actions in real time.


2. Networking with Photon Fusion
Network Object Management: Spawns and manages player objects across different clients using Fusion's NetworkObject system.
Input Authority: Ensures only the local player can control their character, while remote players remain in sync.
Player Spawning: Uses PlayerSpawnerM to handle the instantiation of player objects, maintaining correct ownership and camera setup.


3. Camera Management
Local Player Camera Activation: Activates and deactivates cameras based on whether the player object has input authority, ensuring only the local player's camera is active.
Cinemachine Brain: Manages multiple cameras in the scene, automatically blending between them for smooth transitions.


4. Input Handling
Uses Unity's Input System to capture player actions, providing better control and flexibility.
Supports different input methods, such as keyboard/mouse or gamepad.
Setup & Installation


1. Prerequisites
Unity: Install Unity 2022.3.47f1 or later.
Photon Fusion: Set up a Photon account and download Photon Fusion from the Asset Store.
2. Cloning the Repository

git clone https://github.com/MaelikR/Photon-Fusion-Third-Person-Networked-Game-Base.git


3. Configuring Photon Fusion
Create a Photon Fusion app at the Photon Dashboard.
Copy your App ID and paste it into Unity: Photon > Fusion > App Settings.
Adjust the Network Runner component on your player prefab to match your Fusion configuration.


4. Importing Cinemachine
Download and import Cinemachine from the Unity Package Manager.


5. Running the Project
Open the Unity project.
Ensure your PlayerPrefab is assigned in the PlayerSpawnerM script.
Play the scene to start testing locally.
To test multiplayer functionality, build the project and run multiple instances.
Usage


1. Local Testing
Press Play in Unity to start the game.
You can control the player using WASD or the arrow keys. Camera control is managed by moving the mouse.


2. Multiplayer Testing
Run multiple instances of the built game.
Each instance represents a different player in the multiplayer environment.
The project uses Photon Fusion to synchronize player movements, actions, and animations across the network.

Troubleshooting

Camera Issues
Ensure the CinemachineBrain and FreeLookCamera are enabled for the local player.
Remote players should have these components disabled to avoid conflicting controls.

Input Not Working
Verify the StarterAssetsInputs component is attached to your player prefab.
Make sure HasInputAuthority is correctly checked in the ThirdPersonController.cs script.

Player Movement Not Syncing
Confirm that the NetworkObject component is properly initialized on your player prefab.
Check that the Runner is active and managing the game’s network state correctly.

Future Improvements
Enhanced UI: Add UI elements to indicate player health, stamina, and other important information.
Combat System: Implement an attack/defense system for player interactions.
Multiplayer Lobby: Create a pre-game lobby where players can join, select characters, and start matches together.

Contributing
Contributions are welcome! Please fork the repository and submit a pull request with your improvements or fixes.
(Under Dev)
