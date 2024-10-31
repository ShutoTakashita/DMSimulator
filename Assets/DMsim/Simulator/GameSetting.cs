using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    //enum デュエパか普通か
    public enum GameMode
    {
        DuelParty,
        Original,
        Advance
    }
    public GameMode gameMode = GameMode.DuelParty;

    // デッキのフォルダ名を指定（Resourcesフォルダ内）
    public string deckFolder = "Deck1";
    public Sprite partner;

}
