using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveZoneManager : ZoneManager
{
    // Start is called before the first frame update
    [SerializeField] private GameObject GraveOpenUnderlay;
    public bool isGraveOpen = false;
    public override void CustomZoneLogic()
    {
        // ゾーン固有の処理を記述
    }
    public override void ArrangeCards()
    {
        if (isGraveOpen)
        {
            ArrangeCardsWhenOpen();
            return;
        }
        else
        {
            base.ArrangeCards();
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    [SerializeField] int cardsPerRow = 10;
    [SerializeField] float horizontalSpacing = 100;
    [SerializeField] float verticalSpacing = 150;
    [SerializeField] Vector2 startPosition = new Vector2(-500, 200);
    [SerializeField] float scaleFactorOpen = 1.0f;
    public void OpenGrave()
    {
        isGraveOpen = true;
        GraveOpenUnderlay.SetActive(true);
        ArrangeCardsWhenOpen();

        foreach (Card card in cards)
        {
            card.SetCard(outward: Card.Outward.FaceUp);
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

    public void CloseGrave()
    {
        isGraveOpen = false;
        GraveOpenUnderlay.SetActive(false);
        ArrangeCards();
    }
}
