using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PiePiece : MonoBehaviour
{
    public PieMenuItem menuItem;
    public bool isHighlighted = false;

    public Image fillImage;      // 円弧の画像
    public TMP_Text labelText;       // ラベルテキスト?
    public Image iconImage;      // アイコン画像

    Color normalColor = new Color(0f, 0f, 0f, 0.5f);
    Color highlightedColor = new Color(1f, 1f, 1f, 0.8f);

    Vector3 defaultScale;

    public void SetValues(PieMenuItem item, int index, float angleStep, float radius)
    {
        menuItem = item;

        // テキストの設定
        if (labelText != null)
        {
            labelText.SetText(item.itemName);
            labelText.transform.localScale = Vector3.one * item.textScale / this.GetComponent<RectTransform>().localScale.x;
            defaultScale = labelText.transform.localScale;

            // テキストのオフセット設定
            Vector3 textOffset = labelText.transform.localPosition;
            // textOffset.y = item.textOffset;
            // (0, -60)を角度の半分分回転
            textOffset.y = -70 * Mathf.Cos(Mathf.Deg2Rad * angleStep / 2);
            textOffset.x = -70 * Mathf.Sin(Mathf.Deg2Rad * angleStep / 2);

            labelText.transform.localPosition = textOffset;

            // テキストの回転
            labelText.transform.localRotation = Quaternion.Euler(0, 0, angleStep * index + 180);
        }

        // アイコンの設定
        if (iconImage != null)
        {
            iconImage.sprite = item.icon;
            iconImage.transform.localScale = Vector3.one * item.iconScale;

            // アイコンの回転
            iconImage.transform.localRotation = Quaternion.Euler(0, 0, angleStep * index + 180);

            // アイコンのオフセット設定
            Vector3 iconOffset = iconImage.transform.localPosition;

            iconOffset.y = -40 * Mathf.Cos(Mathf.Deg2Rad * angleStep / 2);
            iconOffset.x = -40 * Mathf.Sin(Mathf.Deg2Rad * angleStep / 2);

            iconImage.transform.localPosition = iconOffset;

        }

        if (item.icon == null)
        {
            iconImage.enabled = false;
        }

        // 円弧の角度を設定
        if (fillImage != null)
        {
            fillImage.fillAmount = 1f / (360f / angleStep);
        }

        // ピースの位置を設定
        transform.localPosition = Vector3.zero;

        transform.localScale = Vector3.one * radius;
    }

    public void SetHighlighted(bool highlighted)
    {
        isHighlighted = highlighted;
        if (fillImage != null)
        {
            fillImage.color = highlighted ? highlightedColor : normalColor;
        }
        if (labelText != null)
        {
            // labelText.color = highlighted ? highlightedColor : normalColor;
            labelText.transform.localScale = highlighted ? defaultScale * 2f : defaultScale;
        }
    }
}
