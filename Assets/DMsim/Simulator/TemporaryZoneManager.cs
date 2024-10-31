using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TemporaryZoneManager : ZoneManager
{
    [SerializeField] ZoneManager deckZone;
    // Start is called before the first frame update
    public bool isShownAllPlayers = true;

    float startPositionX = 0;
    float hiddenPositionX = 100;

    [SerializeField] protected TMP_Text textOfState;
    void Start()
    {
        startPositionX = transform.position.x;
        TogglePosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePosition();
        }
    }

    void TogglePosition()
    {
        if (transform.position.x == startPositionX)
        {
            transform.position = new Vector3(hiddenPositionX, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(startPositionX, transform.position.y, transform.position.z);
        }
    }

    public override void CustomZoneLogic()
    {
        // ゾーン固有の処理を記述
    }

    public override void AddCard(Card card)
    {
        base.AddCard(card);
        if (isShownAllPlayers)
        {
            card.SetCard(Card.Outward.FaceUp);
        }
        else
        {
            card.SetCard(Card.Outward.Hidden);
        }
    }

    public override void AddCards(List<Card> newCards)
    {
        base.AddCards(newCards);
        foreach (Card card in newCards)
        {
            if (isShownAllPlayers)
            {
                card.SetCard(Card.Outward.FaceUp);
            }
            else
            {
                card.SetCard(Card.Outward.Hidden);
            }
        }
    }

    public void ToggleShownAllPlayers()
    {
        isShownAllPlayers = !isShownAllPlayers;
        foreach (Card card in cards)
        {
            if (isShownAllPlayers)
            {
                card.SetCard(Card.Outward.FaceUp);
            }
            else
            {
                card.SetCard(Card.Outward.Hidden);
            }
        }

        if (isShownAllPlayers)
        {
            textOfState.SetText("Show");
            textOfState.color = Color.yellow;
        }
        else
        {
            textOfState.SetText("Hide");
            textOfState.color = Color.white;
        }
    }

    public void CloseZone()
    {
        foreach (Card card in cards)
        {
            card.SetCard(Card.Outward.FaceDown);
        }

        MoveCardsToZone(cards, deckZone);
        TogglePosition();
    }

    public void CloseZoneShuffle()
    {
        ShuffleCards();

        foreach (Card card in cards)
        {
            card.SetCard(Card.Outward.FaceDown);
        }


        MoveCardsToZone(cards, deckZone);
        TogglePosition();
    }

}
