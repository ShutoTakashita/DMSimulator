using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDebug : MonoBehaviour
{
    // デバッグ対象のCardコンポーネントへの参照
    public Card card;

    // UIのボタンやテキストフィールドへの参照
    public Button faceUpButton;
    public Button faceDownButton;
    public Button hiddenButton;
    public Button rotateButton;

    void Start()
    {
        // 各ボタンにイベントを追加
        faceUpButton.onClick.AddListener(SetFaceUp);
        faceDownButton.onClick.AddListener(SetFaceDown);
        hiddenButton.onClick.AddListener(SetHidden);
        rotateButton.onClick.AddListener(ToggleOrientation);
    }

    // 表向きにする処理
    public void SetFaceUp()
    {
        card.SetCard(Card.Outward.FaceUp);
    }

    // 裏向きにする処理
    public void SetFaceDown()
    {
        card.SetCard(Card.Outward.FaceDown);
    }

    // 非表示にする処理
    public void SetHidden()
    {
        card.SetCard(Card.Outward.Hidden);
    }

    // カードの向きを切り替える処理
    public void ToggleOrientation()
    {
        card.TapCard(!card.isTapped);
    }

    // カード名を設定する処理
    public void SetCardName(string name)
    {
        card.SetCardName(name);
        // ついでに表示状態を更新してカードを反映
        card.SetCard(Card.Outward.FaceUp);
    }
}
