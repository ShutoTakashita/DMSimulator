using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleZoneManager : ZoneManager
{
    // Start is called before the first frame update
    [SerializeField] private ZoneManager deckZone;

    [SerializeField] protected List<bool> isCardLink = new List<bool>();
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("ArrangeCardsBattle")]
    public override void ArrangeCards()
    {
        if (cards.Count == 0) return; // カードがない場合は終了
        isCardAbove[0] = false;//最初のカードは上に載せない

        float currentScaleFactor = Mathf.Clamp(scaleFactor, minScaleFactor, scaleFactor);

        //上に載っていないカードのカウント
        int nonAboveCardsCount = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            if (!isCardAbove[i])
            {
                nonAboveCardsCount++;
            }
        }

        if (nonAboveCardsCount > maxCardsBeforeScaling)
        {
            currentScaleFactor = Mathf.Clamp(scaleFactor * ((float)maxCardsBeforeScaling / nonAboveCardsCount), minScaleFactor, scaleFactor);
        }

        float preCardAbove = 0;

        // 各カードの位置とスケールを設定
        for (int i = 0; i < cards.Count; i++)
        {
            float j = i - preCardAbove; //上に載っているカードの数を引いた数

            Vector2 cardPosition;
            if (isCardAbove[i])
            {
                cardPosition = new Vector2(cards[i - 1].transform.localPosition.x, cards[i - 1].transform.localPosition.y) + cardAboveOffset * currentScaleFactor;
                preCardAbove++;
            }
            else
            {
                cardPosition = centerPosition + new Vector2(j * cardSpacing, j * cardSpacingVert) * currentScaleFactor + (nonAboveCardsCount - 1) * new Vector2(-cardSpacing / 2, -cardSpacingVert / 2) * currentScaleFactor;
            }

            if (isCardLink[i])
            {
                //カードがリンクされている場合、横のカードと並べる

                //リンクしているカードの最も前を取得
                int linkCardFirstIndex = i;
                if (i > 0)
                {
                    while (linkCardFirstIndex > 0 && isCardLink[linkCardFirstIndex - 1])
                    {
                        linkCardFirstIndex--;
                    }
                }
                Card linkFirstCard = cards[linkCardFirstIndex];

                //リンクしているカードの最も後ろを取得
                int linkLastCardIndex = i;
                if (i < cards.Count - 1)//iが最後のカードでない場合
                {
                    while (linkLastCardIndex < cards.Count - 1 && isCardLink[linkLastCardIndex + 1])
                    {
                        linkLastCardIndex++;
                    }
                }
                Card linkLastCard = cards[linkLastCardIndex];

                //リンクしているカードの数を取得
                int linkCardCount = linkLastCardIndex - linkCardFirstIndex + 1;

                Debug.Log("linkCardCount:" + linkCardCount);

                //リンクしているカードの中心位置を計算
                Vector2 linkCenterPosition;
                float centerIndex = (linkCardFirstIndex + linkLastCardIndex) / 2 - preCardAbove;
                linkCenterPosition = centerPosition + new Vector2(centerIndex * cardSpacing, centerIndex * cardSpacingVert) * currentScaleFactor + (nonAboveCardsCount - 1) * new Vector2(-cardSpacing / 2, -cardSpacingVert / 2) * currentScaleFactor;

                //リンクしているカードの中心位置とリンクしているカードの中心位置の差を計算
                //インデックスの差を計算
                float indexDiff = j - centerIndex;
                //タップしているとき
                if (cards[i].isTapped)
                {
                    cardPosition = linkCenterPosition + new Vector2(cardSpacingVert, cardSpacing - 3) * currentScaleFactor * indexDiff;
                }
                else
                {
                    cardPosition = linkCenterPosition + new Vector2(cardSpacing - 3, cardSpacingVert) * currentScaleFactor * indexDiff;
                }
            }

            cards[i].transform.localPosition = cardPosition;
            cards[i].transform.localScale = new Vector3(currentScaleFactor, currentScaleFactor, 1);
            cards[i].transform.SetAsLastSibling();//カードを最前面に表示
        }
    }

    public override void AddCard(Card card)
    {
        isCardLink.Add(false);
        base.AddCard(card);
    }

    public override void AddCards(List<Card> addCards)
    {
        foreach (Card card in addCards)
        {
            isCardLink.Add(false);
        }
        base.AddCards(addCards);
    }

    public override void RemoveCard(Card card)
    {
        isCardLink.RemoveAt(cards.IndexOf(card));
        base.RemoveCard(card);
        card.IsFieldCard = false;
    }

    public override void RemoveCards(List<Card> removeCards)
    {
        foreach (Card card in removeCards)
        {
            card.IsFieldCard = false;
            isCardLink.RemoveAt(cards.IndexOf(card));
        }
        base.RemoveCards(removeCards);
    }

    public override void InsertCard(Card card, Card targetCard, bool onLeft = true)
    {
        // リストにない場合は終了
        if (!cards.Contains(targetCard)) return;
        if (!cards.Contains(card)) return;
        if (card == targetCard) return; // 同じ場合でも終了


        // 元のリストからカードを削除（重複を防ぐ）
        bool wasAbove = isCardAbove[cards.IndexOf(card)]; // isCardAbove の状態を保存
        isCardAbove.RemoveAt(cards.IndexOf(card)); // isCardAboveの対応する位置も削除

        bool wasLink = isCardLink[cards.IndexOf(card)]; // isCardLink の状態を保存
        isCardLink.RemoveAt(cards.IndexOf(card)); // isCardLinkの対応する位置も削除

        cards.Remove(card);

        //重なっている場合の処理
        if (CheckPile(targetCard))
        {
            if (onLeft) // 一番下の左に挿入
            {
                //重なっている一番下のカードを探す
                for (int i = cards.IndexOf(targetCard); i >= 0; i--)
                {
                    if (isCardAbove[i] == false)
                    {
                        targetCard = cards[i];
                        break;
                    }
                }
            }
            else //一番上の右に挿入
            {
                //重なっている一番上のカードを探す
                for (int i = cards.IndexOf(targetCard); i < cards.Count; i++)
                {
                    if (isCardAbove[i] == true)
                    {
                        targetCard = cards[i];
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        // ターゲットがリンクしている場合
        if (isCardLink[cards.IndexOf(targetCard)])
        {
            if (onLeft)
            {//一番前の左に挿入
                for (int i = cards.IndexOf(targetCard); i >= 0; i--)
                {
                    if (isCardLink[i] == false)
                    {
                        targetCard = cards[i];
                        break;
                    }
                }
            }
            else
            {//一番後ろの右に挿入
                for (int i = cards.IndexOf(targetCard); i < cards.Count; i++)
                {
                    if (isCardLink[i] == false)
                    {
                        targetCard = cards[i];
                        break;
                    }
                }
            }
        }

        if (onLeft)
        {
            // 新しい位置にカードを挿入
            int targetIndex = cards.IndexOf(targetCard);
            cards.Insert(targetIndex, card);
            isCardAbove.Insert(targetIndex, wasAbove); // 同じ位置に挿入
            isCardLink.Insert(targetIndex, wasLink); // 同じ位置に挿入
        }
        else
        {
            // 新しい位置にカードを挿入
            int targetIndex = cards.IndexOf(targetCard) + 1;
            cards.Insert(targetIndex, card);
            isCardAbove.Insert(targetIndex, wasAbove); // 同じ位置に挿入
            isCardLink.Insert(targetIndex, wasLink); // 同じ位置に挿入
        }

        // カードの並びを更新
        ArrangeCards();
    }

    public override void CustomZoneLogic()
    {
        // ゾーン固有の処理を記述
    }

    public void UntapAllCreatures()
    {
        // 全てのクリーチャーをアンタップする処理
        foreach (Card card in cards)
        {
            if (!card.IsFieldCard)
                card.TapCard(false);
        }
    }

    public void SealCreature()
    {
        Card card = deckZone.GetCard(0);
        deckZone.MoveCardToZone(card, this);
        card.SetCard(Card.Outward.FaceDown);
    }

}
