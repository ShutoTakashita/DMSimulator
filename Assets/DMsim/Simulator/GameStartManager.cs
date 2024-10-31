using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartManager : ZoneManager
{
    // カードのプレハブをインスペクタから設定
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] GameSetting gameSetting;

    // デッキのフォルダ名を指定（Resourcesフォルダ内）
     private string deckFolder = "Deck1";
    private Sprite partner;

    // カードの親オブジェクト（ゾーンなど）
    [SerializeField] private Transform cardParent;

    // 他のゾーン (バトルゾーンやシールドゾーンなど)
    [SerializeField] private ZoneManager battleZone;
    [SerializeField] private ZoneManager manaZone;
    [SerializeField] private ZoneManager handZone;
    [SerializeField] private ZoneManager shieldZone;
    [SerializeField] private ZoneManager deckZone;
    [SerializeField] private ZoneManager superZone;

    //enum デュエパか普通か
    public enum GameMode
    {
        DuelParty,
        Original,
        Advance
    }
     GameMode gameMode = GameMode.DuelParty;

    private int firstHandCardNum = 5;
    private int firstShieldCardNum = 5;

    // デッキのカードを保持するリスト
    // private List<Card> deckCards = new List<Card>();

    // Start is called before the first frame update
    void Start()
    {
        gameMode = (GameMode)((int) gameSetting.gameMode);
        deckFolder = gameSetting.deckFolder;
        partner = gameSetting.partner;

        LoadDeckAndCreateCards();
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // ゲーム開始時に各ゾーンに指定した枚数のカードを追加
    void StartGame()
    {
        // デッキのカードをシャッフル
        ShuffleCards();

        if (gameMode == GameMode.DuelParty)
        {
            //パートナーをsuperZoneに移動
            Card partnerCard = cards.Find(card => card.GetCardName() == partner.name);
            MoveCardToZone(partnerCard, superZone);
            firstHandCardNum = 6;
            firstShieldCardNum = 6;
        }
        else if (gameMode == GameMode.Original)
        {
            firstHandCardNum = 5;
            firstShieldCardNum = 5;
        }
        else if (gameMode == GameMode.Advance)
        {
            firstHandCardNum = 5;
            firstShieldCardNum = 5;
        }

        // 例: 最初に6枚のカードをシールドゾーンに移動
        List<Card> shieldCards = cards.GetRange(0, firstShieldCardNum);
        MoveCardsToZone(shieldCards, shieldZone);
        GetCardOrder();
        Debug.Log($"Moved {firstShieldCardNum} cards to shield zone." + "remain cards: " + cards.Count);

        // 6枚カードをハンドゾーンに移動
        List<Card> manaCards = cards.GetRange(0, firstHandCardNum);
        MoveCardsToZone(manaCards, handZone);
        GetCardOrder();
        Debug.Log($"Moved {firstHandCardNum} cards to hand zone." + "remain cards: " + cards.Count);

        // 残りのカードを山札に移動
        List<Card> deckCards = cards.GetRange(0, cards.Count);
        MoveCardsToZone(deckCards, deckZone);
        GetCardOrder();
        Debug.Log("Moved Remaining cards to deck zone." + "remain cards: " + cards.Count);
    }

    public override void CustomZoneLogic()
    {
        // ゾーン固有の処理を記述
    }

    // デッキをロードしてカードを作成するメソッド
    void LoadDeckAndCreateCards()
    {
        // Resources/{Deck} フォルダから全てのスプライトをロード
        Sprite[] deckSprites = Resources.LoadAll<Sprite>($"{deckFolder}");
        // Debug.Log($"Loaded {deckSprites.Length} sprites from {deckFolder}.");
        // みつからなかったら、Resources/MyFolder から探す
        if (deckSprites.Length == 0)
        {
            deckSprites = Resources.LoadAll<Sprite>($"MyFolder/{deckFolder}");
            // Debug.Log($"Loaded {deckSprites.Length} sprites from MyFolder/{deckFolder}.");
        }
        // 各スプライトごとにカードプレハブを作成
        foreach (Sprite sprite in deckSprites)
        {
            // カードプレハブを作成
            GameObject newCard = Instantiate(cardPrefab, cardParent);

            // Cardコンポーネントを取得
            Card cardComponent = newCard.GetComponent<Card>();

            // スプライト名をカード名として設定
            if (cardComponent != null)
            {
                cardComponent.deckFolder = deckFolder;
                cardComponent.Instantiate();
                cardComponent.SetCardName(sprite.name);
                // カードを表向きにしてセット（スプライトを適用）
                cardComponent.SetCard(Card.Outward.FaceUp);
            }
            else
            {
                Debug.LogWarning("Card component not found on the instantiated prefab.");
            }

            // デッキのカードリストに追加
            cards.Add(cardComponent);
            isCardAbove.Add(false);
        }
    }
}
