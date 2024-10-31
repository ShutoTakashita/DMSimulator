using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PieMenuItem
{
    public UnityEvent action;      // 実行するアクション
    public string itemName;        // 表示するテキスト
    public float textScale = 4.5f;   // テキストのスケール
    public float textOffset = 0f;  // テキストのオフセット（半径方向）
    public Sprite icon;            // 表示するアイコン
    public float iconScale = 4f;   // アイコンのスケール
}
