    using System;
    using UnityEngine;
    using System.Collections.Generic;
    using Fusion;
    using System.Collections;
    using UnityEditor;


namespace Fusion
{
    [RequireComponent(typeof(FusionBootstrap))]
    [AddComponentMenu("Fusion/Fusion Bootstrap Debug GUI")]
    [ScriptHelp(BackColor = ScriptHeaderBackColor.Steel)]
    public class FusionBootstrapDebugGUI : Fusion.Behaviour
    {
        public Canvas gameCanvas;
        public bool EnableHotkeys;
        public GUISkin BaseSkin;
        private bool showCreditsMenu = false;
        private bool isConnecting = false;
        private float connectionProgress = 0.0f;


        private FusionBootstrap _networkDebugStart;
        private string _clientCount;
        private bool _isMultiplePeerMode;
        private Dictionary<FusionBootstrap.Stage, string> _nicifiedStageNames;

        protected bool showOptionsMenu = false;
        private Resolution[] availableResolutions;
        private int selectedResolutionIndex;
        private string[] qualityLevels;
        private int selectedQualityIndex;
        private bool isFullscreen;
        private float masterVolume = 1.0f;
        //public ThirdPersonController playerController;
        public Material skyboxMaterial;
        public Light sunLight;
        public bool enableFog = true;

        // Reference to the menu's AudioListener
        public AudioListener menuAudioListener;

        // Ajoutez ces deux variables pour le défilement
        private Vector2 scrollPosition = Vector2.zero;
        private Vector2 scrollPositionQuality = Vector2.zero;

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            _networkDebugStart = EnsureNetworkDebugStartExists();
            _clientCount = _networkDebugStart.AutoClients.ToString();
            BaseSkin = GetAsset<GUISkin>("e59b35dfeb4b6f54e9b2791b2a40a510");
        }
#endif

        protected virtual void OnValidate()
        {
            ValidateClientCount();
        }

        private IEnumerator WaitForPlayerSpawn()
        {
            // Wait until the ThirdPersonController and its AudioListener are instantiated
            ThirdPersonController playerController = null;
            AudioListener playerAudioListener = null;

            while (playerController == null || playerAudioListener == null)
            {
                ThirdPersonController foundManager = FindFirstObjectByType<ThirdPersonController>();
                if (playerController != null)
                {
                    playerAudioListener = playerController.GetComponent<AudioListener>();
                }

                yield return null; // Wait for the next frame
            }

            

            // Disable the menu AudioListener
            if (menuAudioListener != null)
            {
                menuAudioListener.enabled = false;
            }

            // Ensure all other AudioListeners are disabled except the player's
            AudioListener[] allAudioListeners = UnityEngine.Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None);


            foreach (var listener in allAudioListeners)
            {
                if (listener != playerAudioListener)
                {
                    //listener.enabled = false;
                }
            }
        }

        private void ActivateLightingSettings()
        {
            var bootstrap = EnsureNetworkDebugStartExists();

            if (bootstrap != null)
            {
                if (RenderSettings.skybox == null && bootstrap.skyboxMaterial != null)
                {
                    RenderSettings.skybox = bootstrap.skyboxMaterial;
                }

                if (bootstrap.sunLight != null)
                {
                    RenderSettings.sun = bootstrap.sunLight;
                }

                RenderSettings.fog = enableFog;

                if (enableFog)
                {
                    RenderSettings.fogColor = new Color(0.529f, 0.847f, 1f);
                    RenderSettings.fogMode = FogMode.Exponential;
                    RenderSettings.fogDensity = 0.002f;
                }

                RenderSettings.fog = bootstrap.enableFog;
            }
        }

        protected void ValidateClientCount()
        {
            if (_clientCount == null)
            {
                _clientCount = "1";
            }
            else
            {
                _clientCount = System.Text.RegularExpressions.Regex.Replace(_clientCount, "[^0-9]", "");
            }
        }

        protected int GetClientCount()
        {
            try
            {
                return Convert.ToInt32(_clientCount);
            }
            catch
            {
                return 0;
            }
        }

        protected void Awake()
        {
            _nicifiedStageNames = ConvertEnumToNicifiedNameLookup<FusionBootstrap.Stage>("Fusion Status: ");
            _networkDebugStart = EnsureNetworkDebugStartExists();
            _clientCount = _networkDebugStart.AutoClients.ToString();
            ValidateClientCount();
        }

        private void Update()
        {
            var nds = EnsureNetworkDebugStartExists();
            if (!nds.ShouldShowGUI)
            {
                return;
            }

            var currentstage = nds.CurrentStage;

            if (currentstage == FusionBootstrap.Stage.AllConnected)
            {
                ActivateLightingSettings();

                if (gameCanvas != null && gameCanvas.enabled)
                {
                    gameCanvas.enabled = false;
                }

                // Terminer la progression de la connexion
                isConnecting = false;
                connectionProgress = 1.0f;
            }
            else if (isConnecting)
            {
                // Met à jour la barre de progression
                connectionProgress += Time.deltaTime * 0.1f;
                if (connectionProgress > 1.0f)
                {
                    connectionProgress = 1.0f;
                }
            }

            if (currentstage != FusionBootstrap.Stage.Disconnected)
            {
                return;
            }

            if (EnableHotkeys && Input.GetKeyDown(KeyCode.P))
            {
                nds.StartSharedClient();
            }
        }


        protected void Start()
        {
            // Start the coroutine to check for player spawning
            StartCoroutine(WaitForPlayerSpawn());

             //Disable menuAudioListener if the player already exists
            var existingPlayer = UnityEngine.Object.FindFirstObjectByType<ThirdPersonController>();

            if (existingPlayer != null && existingPlayer.GetComponent<AudioListener>() != null)
            {
                menuAudioListener.enabled = false;
            }

            // Other initialization code
            availableResolutions = Screen.resolutions;
            selectedResolutionIndex = Array.FindIndex(availableResolutions, r => r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height);

            qualityLevels = QualitySettings.names;
            selectedQualityIndex = QualitySettings.GetQualityLevel();

            isFullscreen = Screen.fullScreen;
            masterVolume = AudioListener.volume;
            _isMultiplePeerMode = NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple;
        }


        protected FusionBootstrap EnsureNetworkDebugStartExists()
        {
            if (_networkDebugStart)
            {
                if (_networkDebugStart.gameObject == gameObject)
                    return _networkDebugStart;
            }

            if (TryGetBehaviour<FusionBootstrap>(out var found))
            {
                _networkDebugStart = found;
                return found;
            }

            _networkDebugStart = AddBehaviour<FusionBootstrap>();
            return _networkDebugStart;
        }

        protected virtual void OnGUI()
        {
            var nds = EnsureNetworkDebugStartExists();
            if (!nds.ShouldShowGUI)
            {
                return;
            }

            var currentstage = nds.CurrentStage;
            if (nds.AutoHideGUI && currentstage == FusionBootstrap.Stage.AllConnected)
            {
                return;
            }

            var holdskin = GUI.skin;
            GUI.skin = FusionScalableIMGUI.GetScaledSkin(BaseSkin, out var height, out var width, out var padding, out var margin, out var leftBoxMargin);

            // Ajuste la zone d'affichage
            float areaWidth = Screen.width * 0.9f;
            float areaHeight = showOptionsMenu || showCreditsMenu ? Screen.height * 0.75f : Screen.height * 0.4f;
            float centerX = (Screen.width - areaWidth) / 2;
            float centerY = (Screen.height - areaHeight) / 2 + 50;

            GUILayout.BeginArea(new Rect(centerX, centerY, areaWidth, areaHeight), GUI.skin.box);
            {
                GUILayout.BeginVertical(GUI.skin.window);
                {
                    if (isConnecting)
                    {
                        // Barre de chargement pendant la connexion
                        GUILayout.Label("Connecting to the World...");
                        GUILayout.Space(20);
                        Rect progressBarRect = GUILayoutUtility.GetRect(200, 20);
                        //EditorGUI.ProgressBar(progressBarRect, connectionProgress, "Loading...");
                    }
                    else if (currentstage == FusionBootstrap.Stage.Disconnected && !showOptionsMenu && !showCreditsMenu)
                    {
                        if (GUILayout.Button("Connect to the World", GUILayout.Height(height)))
                        {
                            ActivateLightingSettings();

                            if (gameCanvas != null)
                            {
                                gameCanvas.enabled = false;
                            }

                            nds.StartSharedClient();

                            // Commence la progression de la connexion
                            isConnecting = true;
                            connectionProgress = 0.0f;
                        }

                        if (GUILayout.Button("Options", GUILayout.Height(height)))
                        {
                            showOptionsMenu = true;
                        }

                        if (GUILayout.Button("Credits", GUILayout.Height(height)))
                        {
                            showCreditsMenu = true;
                        }

                        if (GUILayout.Button("Quit", GUILayout.Height(height)))
                        {
                            QuitGame();
                        }
                    }
                    else if (showOptionsMenu)
                    {
                        DrawOptionsMenu(height);

                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Back", GUILayout.Height(height), GUILayout.Width(125)))
                        {
                            showOptionsMenu = false;
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                    else if (showCreditsMenu)
                    {
                        DrawCreditsMenu(height);

                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Back", GUILayout.Height(height), GUILayout.Width(125)))
                        {
                            showCreditsMenu = false;
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();

            GUI.skin = holdskin;
        }

        private void DrawCreditsMenu(float height)
        {
            // Définir la taille de la fenêtre en pourcentage de la taille de l'écran
            int windowWidth = (int)(Screen.width * 0.7f);
            int windowHeight = (int)(Screen.height * 0.75f);

            // Centre la fenêtre sur l'écran
            GUILayout.BeginArea(new Rect((Screen.width - windowWidth) / 2, (Screen.height - windowHeight) / 2, windowWidth, windowHeight), GUI.skin.box);

            // Style du texte
            GUIStyle creditsStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.Clamp(windowWidth / 30, 14, 24),
                alignment = TextAnchor.UpperLeft,
                wordWrap = true // Permettre le retour à la ligne
            };

            GUILayout.Label("Crédits", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 20 }, GUILayout.Height(40));

            GUILayout.Space(10);

            // Afficher le texte des crédits
            GUILayout.Label("Développeur : M.RenDev x 4o \nGraphisme : M.RenDev\nMusique : M.RenDev\nSpécial remerciement : à ma famille ...", creditsStyle);

            GUILayout.EndArea();
        }


        private void DrawOptionsMenu(float height)
        {
            // Définir la taille de la fenêtre en pourcentage de la taille de l'écran
            int windowWidth = (int)(Screen.width * 0.7f); // 70% de la largeur de l'écran
            int windowHeight = (int)(Screen.height * 0.75f); // 75% de la hauteur de l'écran

            // Centre la fenêtre sur l'écran
            GUILayout.BeginArea(new Rect((Screen.width - windowWidth) / 2, (Screen.height - windowHeight) / 2, windowWidth, windowHeight), GUI.skin.box);

            // Style du texte
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.Clamp(windowWidth / 25, 18, 28), // Ajuste la taille de la police en fonction de la largeur de la fenêtre
                alignment = TextAnchor.MiddleCenter
            };

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.Clamp(windowWidth / 30, 14, 20), // Ajuste la taille de la police
                alignment = TextAnchor.MiddleLeft // Alignement à gauche
            };

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.Clamp(windowWidth / 30, 12, 20) // Taille de la police pour les boutons
            };

            GUILayout.Label("Options", headerStyle, GUILayout.Height(30));

            GUILayout.Space(10);

            // Fullscreen toggle aligné
            GUILayout.BeginHorizontal();
            GUILayout.Label("Fullscreen", labelStyle, GUILayout.Width(windowWidth * 0.3f)); // 30% de la largeur de la fenêtre
            isFullscreen = GUILayout.Toggle(isFullscreen, "", GUILayout.Width(20));
            Screen.fullScreen = isFullscreen;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Choix de la résolution avec défilement
            GUILayout.BeginHorizontal();
            GUILayout.Label("Resolution", labelStyle, GUILayout.Width(windowWidth * 0.3f));
            GUILayout.BeginVertical();

            // Ajouter un espace de défilement pour la sélection de résolution
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(windowWidth * 0.6f), GUILayout.Height(120));
            string[] resolutionOptions = Array.ConvertAll(availableResolutions, r => r.width + " x " + r.height);
            selectedResolutionIndex = GUILayout.SelectionGrid(selectedResolutionIndex, resolutionOptions, 1);
            GUILayout.EndScrollView();

            if (selectedResolutionIndex >= 0 && selectedResolutionIndex < availableResolutions.Length)
            {
                Screen.SetResolution(availableResolutions[selectedResolutionIndex].width, availableResolutions[selectedResolutionIndex].height, isFullscreen);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Choix de la qualité graphique avec un espace de défilement
            GUILayout.BeginHorizontal();
            GUILayout.Label("Graphics Quality", labelStyle, GUILayout.Width(windowWidth * 0.3f));
            GUILayout.BeginVertical();

            // Ajouter un espace de défilement pour la sélection de qualité graphique
            scrollPositionQuality = GUILayout.BeginScrollView(scrollPositionQuality, GUILayout.Width(windowWidth * 0.6f), GUILayout.Height(60));
            selectedQualityIndex = GUILayout.SelectionGrid(selectedQualityIndex, qualityLevels, 1);
            QualitySettings.SetQualityLevel(selectedQualityIndex);
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Curseur de volume
            GUILayout.BeginHorizontal();
            GUILayout.Label("Volume", labelStyle, GUILayout.Width(windowWidth * 0.3f));
            masterVolume = GUILayout.HorizontalSlider(masterVolume, 0f, 1f, GUILayout.Width(windowWidth * 0.6f));
            AudioListener.volume = masterVolume;
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            // Bouton Back centré
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //if (GUILayout.Button("Back", buttonStyle, GUILayout.Height(40), GUILayout.Width(100)))
            //{
                //showOptionsMenu = false;
            //}
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }


        private void QuitGame()
        {
            Debug.Log("Quitting the Game...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private Dictionary<T, string> ConvertEnumToNicifiedNameLookup<T>(string prefix = null) where T : Enum
        {
            Dictionary<T, string> nicifiedNames = new Dictionary<T, string>();
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                string name = value.ToString();
                nicifiedNames.Add((T)value, prefix + name);
            }
            return nicifiedNames;
        }

#if UNITY_EDITOR
        public static T GetAsset<T>(string Guid) where T : UnityEngine.Object
        {
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(Guid);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            else
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            }
        }
#endif
    }
}
