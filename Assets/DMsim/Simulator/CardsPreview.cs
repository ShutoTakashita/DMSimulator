using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CardsPreview : MonoBehaviour
{
    public Image previewImage; // カードのプレビューを表示するImageコンポーネント
    public GraphicRaycaster uiRaycaster; // UI Raycaster (Canvasにアタッチ)

    private Card hoveredCard; // マウスがホバーしているカード
    private bool isPreviewActive = false; // プレビューがアクティブかどうか

    GameObject blind;

    private void Start()
    {
        // プレビュー画像を初期状態で非アクティブにする
        previewImage.enabled = false;
        blind = transform.Find("Blind").gameObject;
    }

    private void Update()
    {
        // Spaceキーが押されているかどうかをチェック
        if (Input.GetKey(KeyCode.Space))
        {
            if (!isPreviewActive)
            {
                // プレビューをアクティブにする
                previewImage.enabled = true;
                isPreviewActive = true;
            }

            // マウスの位置に対してRaycastを実行
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            uiRaycaster.Raycast(pointerEventData, results);

            // カードにマウスがホバーしているかをチェック
            if (results.Count > 0)
            {
                GameObject hoveredObject = results[0].gameObject;
                Card card = hoveredObject.GetComponent<Card>();

                // ホバーしているオブジェクトがカードであればプレビューを更新
                if (card != null)
                {
                    hoveredCard = card;
                    UpdatePreviewImage(hoveredCard);
                }
            }
        }
        else
        {
            // Spaceキーが押されていない場合はプレビューを非アクティブにする
            if (isPreviewActive)
            {
                previewImage.enabled = false;
                isPreviewActive = false;
                blind.SetActive(false);
            }
        }
    }

    // プレビューのImageにカードの画像を設定する
    private void UpdatePreviewImage(Card card)
    {
        // カードのスプライトを取得し、プレビューImageに設定
        Sprite cardSprite = card.GetCardSprite(); // Cardクラス内にGetCardSprite()メソッドがあると仮定
        previewImage.sprite = cardSprite;
        if (card.GetCardState() == Card.Outward.Hidden){
            blind.SetActive(true);
        } else {
            blind.SetActive(false);
        }
    }
}
