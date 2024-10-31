using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaZoneManager : ZoneManager
{
    // Start is called before the first frame update
    [SerializeField] BattleZoneManager battleZone;
    [SerializeField] DeckZoneManager deckZone;
    [SerializeField] HandZoneManager handZone;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // public override void AddCard(Card card)
    // {
    //     base.AddCard(card);
    //     card.TapCard(false);
    //     if (Input.GetKey(KeyCode.LeftControl))
    //     {
    //         card.TapCard(true);
    //     }
    // }

    public void TapAllMana()
    {
        // 全てのマナをタップする処理
        foreach (Card card in cards)
        {
            card.TapCard(true);
        }
    }

    public void UntapAllMana()
    {
        // 全てのマナをアンタップする処理
        foreach (Card card in cards)
        {
            card.TapCard(false);
        }
    }

    public void TurnStart()
    {
        // 全てのマナをアンタップする処理
        UntapAllMana();
        battleZone.UntapAllCreatures();
        deckZone.MoveCardToZone(deckZone.GetCard(deckZone.GetCardCount() - 1), handZone);
    }

    public void TapNMana(int index)
    {
        // マナn枚をタップする処理
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].GetOrientation() == Card.Orientation.Inverted)
            {
                cards[i].TapCard(true);
                index--;
            }

            if (index == 0)
            {
                break;
            }
        }
    }


    public int GetMana()
    {
        // 現在のマナの量を取得する処理
        return 0;
    }

    public override void CustomZoneLogic()
    {

    }

}
