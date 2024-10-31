using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckZoneManager : ZoneManager
{
    [SerializeField] private GameObject deckOpenUnderlay;
    public bool isDeckOpen = false;
    // Start is called before the first frame update
    bool hiddenImages = false;

    [SerializeField] HandZoneManager handZone;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Tab) && isDeckOpen)
        // {
        //     ToggleImages();
        // }
    }

    public override void ArrangeCards()
    {
        if (isDeckOpen)
        {
            ArrangeCardsWhenOpen();
            return;
        }
        else
        {
            base.ArrangeCards();
        }
    }

    public override void CustomZoneLogic()
    {
        // ゾーン固有の処理を記述
    }

    public override void AddCard(Card card)
    {
        base.AddCard(card);
        InsertCard(card, GetCard(0));
        card.SetCard(Card.Outward.FaceDown);
    }

    public override void AddCards(List<Card> newCards)
    {
        foreach (Card card in newCards)
        {
            if (cards.Contains(card)) return;

            isCardAbove.Add(false);
            cards.Add(card);
            card.transform.SetParent(transform); // ゾーンの子オブジェクトに設定
            card.SetZoneManager(this); // カードの所属ゾーンを設定
            // card.SetOrientation(Card.Orientation.Vertical); // カードの向きを設定(マナゾーンの場合はManaZoneManagerで上書き)
            card.TapCard(false); // カードを縦向きに設定

            if (card != GetCard(0))
            {
                InsertCard(card, GetCard(0));
            }

            card.SetCard(Card.Outward.FaceDown);
        }
        ArrangeCards(); // カードの並びを更新
        if (text != null) text.SetText(cards.Count.ToString()); // ゾーンにあるカードの枚数を表示
    }

    [SerializeField] int cardsPerRow = 10;
    [SerializeField] float horizontalSpacing = 100;
    [SerializeField] float verticalSpacing = 150;
    [SerializeField] Vector2 startPosition = new Vector2(-500, 200);
    [SerializeField] float scaleFactorOpen = 1.0f;
    public void OpenDeck()
    {
        isDeckOpen = true;
        deckOpenUnderlay.SetActive(true);
        ArrangeCardsWhenOpen();

        foreach (Card card in cards)
        {
            card.SetCard(Card.Outward.Hidden);
        }
    }

    public void ArrangeCardsWhenOpen()
    {
        if (cards.Count == 0) return; // カードがない場合は終了

        // 新しいパラメータ
        float currentScaleFactor = Mathf.Clamp(scaleFactor, minScaleFactor, scaleFactor);

        // カードの大きさを調整
        currentScaleFactor = scaleFactorOpen;

        // 各カードの位置とスケールを設定
        for (int i = 0; i < cards.Count; i++)
        {
            // 行と列を計算
            int row = i / cardsPerRow; // 何番目の行か
            int col = i % cardsPerRow; // 行内での位置

            // カードの位置を計算
            Vector2 cardPosition;
            cardPosition = startPosition + new Vector2(col * horizontalSpacing, -row * verticalSpacing) * currentScaleFactor;

            // カードの位置とスケールを設定
            cards[i].transform.localPosition = cardPosition;
            cards[i].transform.localScale = new Vector3(currentScaleFactor, currentScaleFactor, 1);
            cards[i].transform.SetAsLastSibling(); // カードを最前面に表示
        }
    }

    public void CloseDeck()
    {
        isDeckOpen = false;
        deckOpenUnderlay.SetActive(false);
        ShuffleCards();
        ArrangeCards();

        foreach (Card card in cards)
        {
            card.SetCard(Card.Outward.FaceDown);
        }
    }

    // public void ToggleImages()
    // {
    //     if (hiddenImages)
    //     {
    //         hiddenImages = false;
    //         foreach (Card card in cards)
    //         {
    //             card.SetCard(outward: Card.Outward.FaceUp);
    //         }
    //     }
    //     else
    //     {
    //         hiddenImages = true;
    //         foreach (Card card in cards)
    //         {
    //             card.SetCard(outward: Card.Outward.Hidden);
    //         }
    //     }
    // }

    public void Draw(){
        if (cards.Count == 0) return;
        Card card = GetCard(cards.Count - 1);
        MoveCardToZone(card, handZone);
    }

}
