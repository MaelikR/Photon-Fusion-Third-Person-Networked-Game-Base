using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
{
    [Header("Texts")]
    public Text questLog;
    public Text stateText;
    public Text notificationText;

    [Header("Progress and Notifications")]
    public Slider progressBar;
    public GameObject notificationPanel;

    [Header("Inventory")]
    public GameObject inventoryUI;
    public GameObject inventorySlotPrefab;

    [Header("UI Settings")]
    public Canvas uiCanvas;
    public Color darkModeColor = Color.black;
    public Color lightTextColor = Color.white;
    public float notificationDuration = 2.0f;

    public InventorySystem inventorySystem;

    public override void Spawned()
    {
        if (!HasStateAuthority)
        {
            DisableLocalUI();
            return;
        }

        InitializeUI();

        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            foreach (Quest quest in questManager.GetAllQuests())
            {
                RPC_UpdateQuestLog($"Existing Quest: {quest.questName} ({quest.currentProgress}/{quest.goal})");
            }
        }
        else
        {
            Debug.LogError("QuestManager not found in the scene.");
        }
    }

    private void InitializeUI()
    {
        ApplyDarkMode();
        if (questLog == null || inventoryUI == null)
        {
            Debug.LogError("UI components are not fully assigned. Please check the inspector.");
        }
    }

    private void ApplyDarkMode()
    {
        Image canvasImage = uiCanvas.GetComponent<Image>();
        if (canvasImage != null)
        {
            canvasImage.color = darkModeColor;
        }

        foreach (var textElement in uiCanvas.GetComponentsInChildren<Text>())
        {
            textElement.color = lightTextColor;
        }
    }

    public void UpdateInventoryUI()
    {
        if (inventoryUI == null || inventorySlotPrefab == null)
        {
            Debug.LogError("Inventory UI or Slot Prefab is not assigned.");
            return;
        }

        foreach (Transform child in inventoryUI.transform)
        {
            Destroy(child.gameObject);
        }

        if (inventorySystem == null)
        {
            Debug.LogError("Inventory System is not assigned.");
            return;
        }

        foreach (Item item in inventorySystem.inventory)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryUI.transform);
            Text slotText = slot.GetComponentInChildren<Text>();
            if (slotText != null)
            {
                slotText.text = item.itemName;
            }
        }
    }

    public void ShowNotification(string message)
    {
        if (notificationPanel != null && notificationText != null)
        {
            notificationPanel.SetActive(true);
            notificationText.text = message;
            Invoke(nameof(HideNotification), notificationDuration);
        }
        else
        {
            Debug.LogError("Notification Panel or Text is not assigned.");
        }
    }

    private void HideNotification()
    {
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
    }

    public void UpdateStateText(string state)
    {
        if (stateText != null)
        {
            stateText.text = $"Current State: {state}";
        }
    }

    public void UpdateProgressBar(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = progress;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateQuestLog(string newQuest)
    {
        if (questLog != null)
        {
            questLog.text += "\n" + newQuest;
            Debug.Log($"Quest Log Updated: {newQuest}");
        }
        else
        {
            Debug.LogError("QuestLog Text component is not assigned.");
        }
    }

    private void DisableLocalUI()
    {
        if (uiCanvas != null)
        {
            uiCanvas.gameObject.SetActive(false);
        }
    }
}
