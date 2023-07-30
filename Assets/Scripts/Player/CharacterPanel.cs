using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    private static CharacterPanel instance;
    public GameObject inventoryTab;
    public GameObject suspectsTab;
    public GameObject spellsTab;
    public GameObject lastPanel;
    public GameObject panel;
    public Image statsButton;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        instance.lastPanel = instance.inventoryTab;
    }

    public static void ShowInventoryTab()
    {
        instance.panel.SetActive(true);
        // instance.spellsTab.SetActive(false);
        instance.suspectsTab.SetActive(false);
        instance.inventoryTab.SetActive(true);
        instance.lastPanel = instance.inventoryTab;
    }

    public static void ShowSpellsTab()
    {
        instance.panel.SetActive(true);
        // instance.spellsTab.SetActive(true);
        instance.suspectsTab.SetActive(false);
        instance.inventoryTab.SetActive(false);
    }

    public static void ShowSuspectsTab()
    {
        instance.panel.SetActive(true);
        // instance.spellsTab.SetActive(false);
        instance.suspectsTab.SetActive(true);
        instance.inventoryTab.SetActive(false);
        instance.lastPanel = instance.suspectsTab;
    }

    public static void ShowLastTab()
    {
        instance.panel.SetActive(true);
        instance.lastPanel.SetActive(true);
    }

    public static void HideCharacterPanel()
    {
        instance.panel.SetActive(false);
    }
}
