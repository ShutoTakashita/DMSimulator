using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    // カードの名前（スプライト名に対応）
    public string cardName;

    // 表示状態の列挙型
    public enum Outward
    {
        FaceUp,    // 表向き
        FaceDown,  // 裏向き
        Hidden     // 非表示
    }

    // 表示状態
    [SerializeField, ReadOnly] Outward currentOutward;

    // 上下左右のフラグ
    public enum Orientation
    {
        Vertical,   // 縦向き
        Horizontal,  // 横向き
        Inverted,   // 逆向き
        HorizontalInverted // 横向き逆
    }
    [SerializeField, ReadOnly] Orientation currentOrientation;

    // Imageコンポーネント (カードの画像を変更するため)
    [SerializeField, ReadOnly] Image cardImage;

    // 表向きのスプライト
    [SerializeField, ReadOnly] Sprite frontSprite;

    // 裏向きのスプライト (全カード共通)
    [SerializeField] Sprite backSprite;
    [SerializeField] ZoneManager zoneManager;

    public bool isHidden = true;
    GameObject blind;
    GameObject mark;
    public string deckFolder = "Deck1";

    // 初期化
    public void Instantiate()
    {
        // Imageコンポーネントの取得
        cardImage = GetComponent<Image>();

        // 表向きのスプライトをロード
        if (frontSprite == null && !string.IsNullOrEmpty(cardName))
        {
            frontSprite = Resources.Load<Sprite>($"{deckFolder}/{cardName}");
            //見つからなかったらResources/MyFolderから
            if (frontSprite == null)
            {
                frontSprite = Resources.Load<Sprite>($"MyFolder/{deckFolder}/{cardName}");
            }
        }


        // 裏向きのスプライトを一度だけロード
        if (backSprite == null)
        {
            backSprite = Resources.Load<Sprite>("CardBack");
        }

        //子のオブジェクトを取得
        blind = transform.Find("Blind").gameObject;
        mark = transform.Find("Mark").gameObject;
    }

    // カードの表示状態をセットする
    public void SetCard(Outward outward)
    {
        currentOutward = outward;
        Debug.Log($"Sprite Updated: {currentOutward} {cardName}");

        // 表示状態に応じてスプライトを切り替える
        switch (currentOutward)
        {
            case Outward.FaceUp:
                cardImage.sprite = frontSprite; // 事前にロードされたスプライトを設定
                cardImage.enabled = true; // 表示する
                SetHidden(false);
                break;
            case Outward.FaceDown:
                cardImage.sprite = backSprite; // 裏向きの画像
                cardImage.enabled = true; // 表示する
                SetHidden(false);
                break;
            case Outward.Hidden:
                cardImage.sprite = frontSprite; // 事前にロードされたスプライトを設定
                cardImage.enabled = true; // 表示する
                SetHidden(true);
                break;
        }
    }

    // カードの向きをセットする
    void SetOrientation(Orientation orientation)
    {
        currentOrientation = orientation;

        // 向きに応じて回転を設定
        switch (currentOrientation)
        {
            case Orientation.Vertical:
                transform.rotation = Quaternion.Euler(0, 0, 0); // 縦向き
                break;
            case Orientation.Horizontal:
                transform.rotation = Quaternion.Euler(0, 0, 90); // 横向き
                break;
            case Orientation.Inverted:
                transform.rotation = Quaternion.Euler(0, 0, 180); // 逆向き
                break;
            case Orientation.HorizontalInverted:
                transform.rotation = Quaternion.Euler(0, 0, 270); // 横向き逆
                break;
        }
    }
    private bool _isFieldCard = false; // バッキングフィールド

    public bool IsFieldCard
    {
        get { return _isFieldCard; }   // 値を取得する
        set
        {
            _isFieldCard = value;
            TapCard(isTapped);
        }  // 値をセットする
    }
    // カードの向きをタップ/アンタップする
    public bool isTapped = false;
    public void TapCard(bool t)
    {
        if (t)//タップの場合
        {
            if (this.transform.parent.gameObject.name == "ManaZone")//ManaZoneの場合
            {
                SetOrientation(Orientation.Horizontal);
            }
            else if (_isFieldCard && this.transform.parent.gameObject.name == "BattleZone") //バトルゾーンにあるフィールドカードの場合
            {
                SetOrientation(Orientation.HorizontalInverted);
            }
            else
            {
                SetOrientation(Orientation.Horizontal);
            }
            isTapped = true;
        }
        else
        {//アンタップ
            if (this.transform.parent.gameObject.name == "ManaZone")//ManaZoneの場合
            {
                SetOrientation(Orientation.Inverted);
            }
            else if (_isFieldCard && this.transform.parent.gameObject.name == "BattleZone") //バトルゾーンにあるフィールドカードの場合
            {
                SetOrientation(Orientation.Horizontal);
            }
            else
            {
                SetOrientation(Orientation.Vertical);
            }
            isTapped = false;
        }
    }

    // 外部スクリプトからカードの名前を設定し、スプライトをロードする
    public void SetCardName(string name)
    {
        cardName = name;
        frontSprite = Resources.Load<Sprite>($"{deckFolder}/{cardName}");
        //見つからなかったらResources/MyFolderから
        if (frontSprite == null)
        {
            frontSprite = Resources.Load<Sprite>($"MyFolder/{deckFolder}/{cardName}");
        }
        Debug.Log($"Set card name: {cardName}");

        if (frontSprite == null)
        {
            Debug.LogWarning($"Sprite '{cardName}' not found in Resources.");
        }
    }

    // 表示状態を取得する
    public Outward GetCardState()
    {
        return currentOutward;
    }

    // カードの向きを取得する
    public Orientation GetOrientation()
    {
        return currentOrientation;
    }

    public string GetCardName()
    {
        return cardName;
    }

    public void SetZoneManager(ZoneManager zm)
    {
        zoneManager = zm;
    }

    public ZoneManager GetZoneManager()
    {
        return zoneManager;
    }

    public Sprite GetCardSprite()
    {
        if (currentOutward == Outward.FaceDown)
        {
            return backSprite;
        }
        else
        {
            return frontSprite;
        }
    }

    private void SetHidden(bool hidden)
    {
        isHidden = hidden;
        blind.SetActive(hidden);
        mark.SetActive(hidden);
    }

    public bool GetHidden()
    {
        return isHidden;
    }

}
