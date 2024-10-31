using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HandZoneManager : ZoneManager
{
    // Start is called before the first frame update
    public bool isShownAllPlayers = false;
    [SerializeField] protected TMP_Text textOfState;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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


    public override void ArrangeCards()
    {
        if (cards.Count > 20)
        {
            ArrangeCardsOver20();
            return;
        }
        else
        {
            base.ArrangeCards();
        }
    }

    public float rowSpaceing = 20;

    public void ArrangeCardsOver20()
    {
        if (cards.Count == 0) return; // カードがない場合は終了

        // 新しいパラメータ

        // カードの大きさを調整
        float currentScaleFactor = minScaleFactor;

        // 各カードの位置とスケールを設定
        for (int i = 0; i < cards.Count; i++)
        {
            // 行と列を計算
            int row = i / 20; // 何番目の行か
            int col = i % 20; // 行内での位置

            int rowNumber = (cards.Count - 1) / 20 + 1; // 行数

            // カードの位置を計算
            Vector2 cardPosition;
            cardPosition = centerPosition + new Vector2(col * cardSpacing, col * cardSpacingVert) * currentScaleFactor + (20 - 1) * new Vector2(-cardSpacing / 2, -cardSpacingVert / 2) * currentScaleFactor;
            // cardPosition = centerPosition + new Vector2(j * cardSpacing, j * cardSpacingVert) * currentScaleFactor + (nonAboveCardsCount - 1) * new Vector2(-cardSpacing / 2, -cardSpacingVert / 2) * currentScaleFactor;

            //rowに応じて調整　上下にずらす
            cardPosition += new Vector2(0, (-row + (rowNumber - 1f) / 2f) * rowSpaceing * currentScaleFactor);

            // カードの位置とスケールを設定
            cards[i].transform.localPosition = cardPosition;
            cards[i].transform.localScale = new Vector3(currentScaleFactor, currentScaleFactor, 1);
            cards[i].transform.SetAsLastSibling(); // カードを最前面に表示
        }
    }

    public override void CustomZoneLogic()
    {
        // ゾーン固有の処理を記述
    }

    public void OpenCloseHand()
    {
        isShownAllPlayers = !isShownAllPlayers;
        foreach (Card card in cards)
        {
            if (isShownAllPlayers)
            {//隠れている場合、開示
                card.SetCard(Card.Outward.FaceUp);
                textOfState.color = Color.yellow;
            }
            else
            {//隠れていない場合、隠す
                card.SetCard(Card.Outward.Hidden);
                textOfState.color = Color.white;
            }
        }
    }
}
