using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    // Enum for game modes
    public enum GameMode
    {
        DuelParty,
        Original,
        Advance
    }

    // Game mode selection
    public GameMode gameMode = GameMode.DuelParty;

    // Class to store deck-folder and partner pairs
    [System.Serializable]
    public class DeckSetting
    {
        public string deckFolder;
        public Sprite partner;
    }

    // List to store multiple deck-folder and partner pairs
    public List<DeckSetting> deckSettings = new List<DeckSetting>();

    // Currently selected deck folder and partner
    public string deckFolder;
    public Sprite partner;

    void Awake()
    {
        // Ensure there is at least one deck setting
        if (deckSettings != null && deckSettings.Count > 0)
        {
            // Set the first pair as the active setting
            deckFolder = deckSettings[0].deckFolder;
            partner = deckSettings[0].partner;
        }
        else
        {
            Debug.LogWarning("No deck settings found. Please add at least one deck setting to the list.");
        }
    }
}
