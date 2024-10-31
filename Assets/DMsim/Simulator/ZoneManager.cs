using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public abstract class ZoneManager : MonoBehaviour
{
    // ゾーンにあるカードのリスト
    [SerializeField] protected List<Card> cards = new List<Card>();

    // 並べる位置のベース
    [SerializeField] protected Vector2 centerPosition = new Vector2(0, 0);
    [SerializeField] protected float cardSpacing = 1f; // カード間のスペース
    [SerializeField] protected float cardSpacingVert = 0f; // カード間のスペース
    [SerializeField] protected float scaleFactor = 1.0f; // 通常時のカードの大きさ
    [SerializeField] protected float minScaleFactor = 0.5f; // 枚数が多いときの最小縮小率
    [SerializeField] protected int maxCardsBeforeScaling = 5; // この枚数を超えるとスケールダウン

    [SerializeField] protected List<bool> isCardAbove = new List<bool>();
    [SerializeField] protected Vector2 cardAboveOffset = new Vector2(1.5f, 1.5f); // カードが上に重なるときのオフセット

    [SerializeField] protected TMP_Text text; // ゾーンに表示するテキスト

    // カードの並びをリフレッシュする（共通ロジック）
    [ContextMenu("ArrangeCards")]
    public virtual void ArrangeCards()
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

            cards[i].transform.localPosition = cardPosition;
            cards[i].transform.localScale = new Vector3(currentScaleFactor, currentScaleFactor, 1);
            cards[i].transform.SetAsLastSibling();//カードを最前面に表示
        }
    }

    // ゾーンにカードを追加する（共通ロジック）
    public virtual void AddCard(Card card)
    {
        if (cards.Contains(card)) return;

        isCardAbove.Add(false);
        cards.Add(card);
        card.transform.SetParent(transform); // ゾーンの子オブジェクトに設定
        card.SetZoneManager(this); // カードの所属ゾーンを設定
        ArrangeCards(); // カードの並びを更新
        card.TapCard(false);
        card.SetCard(Card.Outward.FaceUp);

        if (text != null) text.SetText(cards.Count.ToString()); // ゾーンにあるカードの枚数を表示

        //Ctrlキーを押しながらカードを追加した場合、カードをタップインにする
        if (Input.GetKey(KeyCode.LeftControl))
        {
            card.TapCard(true);
        }

        //手札かデッキとして追加された場合、hiddenをtrueに、それ以外はfalseに
        if (this.gameObject.name == "HandZone") card.SetCard(Card.Outward.Hidden);
        //シールドの場合
        if (this.gameObject.name == "ShieldZone") card.SetCard(Card.Outward.FaceDown);
    }

    // カードを複数枚まとめて追加する
    public virtual void AddCards(List<Card> newCards)
    {
        foreach (Card card in newCards)
        {
            if (cards.Contains(card)) return;

            isCardAbove.Add(false);
            cards.Add(card);
            card.transform.SetParent(transform); // ゾーンの子オブジェクトに設定
            card.SetZoneManager(this); // カードの所属ゾーンを設定
            card.TapCard(false); // カードの向きを設定(マナゾーンの場合はManaZoneManagerで上書き)
            card.SetCard(Card.Outward.FaceUp);
            // card.SetHidden(this.gameObject.name == "HandZone");
            if (this.gameObject.name == "HandZone") card.SetCard(Card.Outward.Hidden); //手札の場合は隠して表示
            if (this.gameObject.name == "ShieldZone") card.SetCard(Card.Outward.FaceDown); // シールドの場合は裏向き
        }
        ArrangeCards(); // カードの並びを更新

        if (text != null) text.SetText(cards.Count.ToString()); // ゾーンにあるカードの枚数を表示
    }

    // ゾーンからカードを削除する（共通ロジック）
    public virtual void RemoveCard(Card card)
    {
        if (!cards.Contains(card)) return;

        //このカードが何かの下にあるとき、かつ自分自身の下にもないときにはそのカードのAboveフラグを下ろす
        if (cards.IndexOf(card) < cards.Count - 1)
        {
            if (isCardAbove[cards.IndexOf(card) + 1] == true && isCardAbove[cards.IndexOf(card)] == false)
            {
                isCardAbove[cards.IndexOf(card) + 1] = false;
            }
        }

        isCardAbove.RemoveAt(cards.IndexOf(card));
        cards.Remove(card);

        ArrangeCards(); // カードの並びを更新

        if (text != null) text.SetText(cards.Count.ToString()); // ゾーンにあるカードの枚数を表示
    }

    // カードを複数枚まとめて削除する
    public virtual void RemoveCards(List<Card> removeCards)
    {
        foreach (Card card in removeCards)
        {
            if (!cards.Contains(card)) return;

            isCardAbove.RemoveAt(cards.IndexOf(card));
            cards.Remove(card);
        }
        ArrangeCards(); // カードの並びを更新

        if (text != null) text.SetText(cards.Count.ToString()); // ゾーンにあるカードの枚数を表示
    }

    // カードの順番をシャッフルする
    public virtual void ShuffleCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Card temp = cards[i];
            int randomIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }

        Debug.Log(this.gameObject.name + " : Shuffled cards.");
        ArrangeCards(); // シャッフル後に並びを更新
    }

    // カードを挿入する(同じゾーン)
    public virtual void InsertCard(Card card, Card targetCard, bool onLeft = true)
    {
        // リストにない場合は終了
        if (!cards.Contains(targetCard)) return;
        if (!cards.Contains(card)) return;
        if (card == targetCard) return; // 同じ場合でも終了


        // 元のリストからカードを削除（重複を防ぐ）
        bool wasAbove = isCardAbove[cards.IndexOf(card)]; // isCardAbove の状態を保存
        isCardAbove.RemoveAt(cards.IndexOf(card)); // isCardAboveの対応する位置も削除
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

        if (onLeft)
        {
            // 新しい位置にカードを挿入
            int targetIndex = cards.IndexOf(targetCard);
            cards.Insert(targetIndex, card);
            isCardAbove.Insert(targetIndex, wasAbove); // 同じ位置に挿入
        }
        else
        {
            // 新しい位置にカードを挿入
            int targetIndex = cards.IndexOf(targetCard) + 1;
            cards.Insert(targetIndex, card);
            isCardAbove.Insert(targetIndex, wasAbove); // 同じ位置に挿入
        }

        // カードの並びを更新
        ArrangeCards();
    }

    public virtual void InsertCardAtLast(Card card)
    {
        if (!cards.Contains(card)) return;
        if (card == cards[cards.Count - 1]) return;
        InsertCard(card, cards[cards.Count - 1], false);
    }


    public virtual void PlaceAbove(Card card)
    {
        if (!cards.Contains(card)) return;
        if (cards.IndexOf(card) <= 0) return;

        isCardAbove[cards.IndexOf(card)] = true;

        ArrangeCards();
    }

    public virtual void ResetPlaceAbove(Card card)
    {
        if (!cards.Contains(card)) return;
        if (cards.IndexOf(card) <= 0) return;

        isCardAbove[cards.IndexOf(card)] = false;

        ArrangeCards();
    }

    public virtual void PlaceBelow(Card card)
    {
        if (!cards.Contains(card)) return;
        if (cards.IndexOf(card) <= 0) return;

        // if (isCardAbove[cards.IndexOf(card) - 1] == true)//一つ前のカードが上に載っているカードの場合
        // {
        //     //Aboveフラグが立っていない直前のカードをさがす
        //     for (int i = cards.IndexOf(card) - 1; i >= 0; i--)
        //     {
        //         if (isCardAbove[i] == false)
        //         {
        //             //見つけたカードにAboveフラグを立てる
        //             isCardAbove[i] = true;

        //             //順番を入れ替える
        //             InsertCard(card, cards[i], true);
        //             break;
        //         }
        //     }
        // }
        // else
        // {
        //     //リストの前のカードにAboveフラグを立てる
        //     isCardAbove[cards.IndexOf(card) - 1] = true;

        //     //順番を入れ替える
        //     InsertCard(card, cards[cards.IndexOf(card) - 1], true);
        // }


        InsertCard(card, cards[cards.IndexOf(card) - 1]);
        isCardAbove[cards.IndexOf(card) + 1] = true;


        ArrangeCards();
    }

    //カードが重なっているかを確認する
    public virtual bool CheckPile(Card card)
    {
        if (!cards.Contains(card)) return false;

        if (isCardAbove[cards.IndexOf(card)] == true)
        {
            return true;
        }

        if (cards.IndexOf(card) < cards.Count - 1)
        {
            if (isCardAbove[cards.IndexOf(card) + 1] == true)
            {
                return true;
            }
        }

        return false;
    }

    //カードを取得する
    public virtual Card GetCard(int index)
    {
        if (index < 0 || index >= cards.Count) return null;
        return cards[index];
    }

    public virtual int GetCardCount()
    {
        return cards.Count;
    }

    // 他のゾーンにカードを移動させる（共通ロジック）
    public virtual void MoveCardToZone(Card card, ZoneManager targetZone)
    {
        RemoveCard(card); // このゾーンからカードを削除
        targetZone.AddCard(card); // 目的のゾーンにカードを追加
        Debug.Log($"Moved card {card.GetCardName()} from {this.name} to {targetZone.name}.");
    }

    // 複数枚のカードを移動させる
    public virtual void MoveCardsToZone(List<Card> cardsToMove, ZoneManager targetZone)
    {
        // foreach (Card card in cardsToMove)
        // {
        //     MoveCardToZone(card, targetZone);
        // }
        List<Card> _moveCards = cardsToMove.GetRange(0, cardsToMove.Count);
        Debug.Log($"Removed {_moveCards.Count} cards from {this.name} to {targetZone.name}.");
        RemoveCards(_moveCards); // このゾーンからカードを削除
        Debug.Log($"Added {_moveCards.Count} cards from {this.name} to {targetZone.name}.");
        targetZone.AddCards(_moveCards); // 目的のゾーンにカードを追加
    }

    // カードの順番を保存してその順番で並べる（共通ロジック）
    public virtual void SetCardOrder(List<Card> cardOrder)
    {
        cards = new List<Card>(cardOrder);
        ArrangeCards(); // 新しい順番で並べ直す
    }

    // ゾーン内のカードの順番を取得する（共通ロジック）
    [ContextMenu("GetCardOrder")]
    public virtual List<Card> GetCardOrder()
    {
        // ゾーン内のカードの名前をすべて返す
        string cardNames = "";
        foreach (Card card in cards)
        {
            cardNames += "cards[" + cards.IndexOf(card) + "] : " + card.GetCardName() + ", ";
        }
        Debug.Log(cardNames);
        return new List<Card>(cards);
    }

    public virtual void TapUntapCard(int index)
    {
        if (index < 0 || index >= cards.Count) return;
        cards[index].TapCard(!cards[index].isTapped);

    }

    // ゾーンに固有の動作があれば子クラスで実装
    public abstract void CustomZoneLogic();
}
