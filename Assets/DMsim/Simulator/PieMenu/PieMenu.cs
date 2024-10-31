using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PieMenu : MonoBehaviour
{
    public string menuName;  // パイメニューのユニークな名前

    public List<PieMenuItem> menuItems = new List<PieMenuItem>();
    public GameObject piePiecePrefab;  // 円弧型ボタンのプレハブ
    public float radius = 100f;        // パイメニューの半径

    private List<GameObject> piePieces = new List<GameObject>();
    private bool isMenuActive = false;
    private Vector3 menuPosition;

    void Awake()
    {
        // PieMenuManagerに自身を登録
        if (!string.IsNullOrEmpty(menuName))
        {
            PieMenuManager manager = FindObjectOfType<PieMenuManager>();
            if (manager != null)
            {
                manager.RegisterPieMenu(menuName, this);
            }
            else
            {
                Debug.LogError("PieMenuManagerがシーン内に存在しません。");
            }
        }
        else
        {
            Debug.LogError("PieMenuの名前が設定されていません。ユニークな名前を設定してください。");
        }
    }

    void Start()
    {
        CreatePiePieces();
        HideMenu();
    }


    void Update()
    {
        if (isMenuActive)
        {
            UpdateSelection(Input.mousePosition);

            // 右クリックを離したタイミングでアクションを実行
            if (Input.GetMouseButtonUp(1))
            {
                ExecuteAction();
                HideMenu();
            }
        }
    }

    // ピースを生成する
    private void CreatePiePieces()
    {
        // 既存のピースを削除
        foreach (var piece in piePieces)
        {
            Destroy(piece);
        }
        piePieces.Clear();

        // メニューアイテムに応じてピースを生成
        float angleStep = 360f / menuItems.Count;
        for (int i = 0; i < menuItems.Count; i++)
        {
            GameObject piece = Instantiate(piePiecePrefab, transform);
            piePieces.Add(piece);

            // ピースの設定
            PiePiece piePieceScript = piece.GetComponent<PiePiece>();
            piePieceScript.SetValues(menuItems[i], i, angleStep, radius);

            // ピースの回転
            piece.transform.rotation = Quaternion.Euler(0, 0, -angleStep * i + angleStep * menuItems.Count / 2);
        }
    }

    // ピースを更新する（インスペクタで変更があった場合）
    private void UpdatePiePieces()
    {
        if (piePieces.Count != menuItems.Count)
        {
            CreatePiePieces();
        }
        else
        {
            float angleStep = 360f / menuItems.Count;
            for (int i = 0; i < menuItems.Count; i++)
            {
                PiePiece piePieceScript = piePieces[i].GetComponent<PiePiece>();
                piePieceScript.SetValues(menuItems[i], i, angleStep, radius);
                piePieces[i].transform.rotation = Quaternion.Euler(0, 0, -angleStep * i);
            }
        }
    }

    // メニューの表示
    public void ShowMenuInstance(Vector3 position, bool useLocal = false)
    {

        isMenuActive = true;
        menuPosition = new Vector3(position.x - 1902 / 2, position.y - 1080 / 2, 0);
        Debug.Log("menuPosition" + menuPosition.x + " " + menuPosition.y);
        // transform.position = position;
        this.GetComponent<RectTransform>().localPosition = menuPosition;

        if (useLocal)
        {
            RectTransform parentRt = this.transform.parent.GetComponent<RectTransform>();
            Debug.Log("parentRt.localScale" + parentRt.localScale.x + " " + parentRt.localScale.y);
            Debug.Log("position" + position.x + " " + position.y);
            this.GetComponent<RectTransform>().localPosition = new Vector3(position.x / parentRt.localScale.x, position.y / parentRt.localScale.y, 0);
        }

        // ピースを表示
        foreach (var piece in piePieces)
        {
            piece.SetActive(true);
        }
    }

    // メニューの非表示
    public void HideMenu()
    {
        isMenuActive = false;
        foreach (var piece in piePieces)
        {
            piece.SetActive(false);
        }
    }

    // メニューの表示状態を取得
    public bool IsMenuActive()
    {
        return isMenuActive;
    }

    // 選択の更新
    private void UpdateSelection(Vector3 mousePosition)
    {
        if (!isMenuActive) return;

        mousePosition = new Vector3(mousePosition.x - 1902 / 2, mousePosition.y - 1080 / 2, 0);

        Vector2 direction = mousePosition - menuPosition;
        // 角度を計算（上を0度、時計回りに増加）
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;
        float anglePerItem = 360f / menuItems.Count;
        int selectedIndex = Mathf.FloorToInt(angle / anglePerItem) % menuItems.Count;
        for (int i = 0; i < piePieces.Count; i++)
        {
            PiePiece piePieceScript = piePieces[i].GetComponent<PiePiece>();
            piePieceScript.SetHighlighted(i == selectedIndex);
        }
    }

    // アクションの実行
    private void ExecuteAction()
    {
        if (!isMenuActive) return;

        foreach (var piece in piePieces)
        {
            PiePiece piePieceScript = piece.GetComponent<PiePiece>();
            if (piePieceScript.isHighlighted)
            {
                piePieceScript.menuItem.action.Invoke();
                break;
            }
        }
    }

    public void ExecuteAction(int index)
    {
        if (!isMenuActive) return;

        piePieces[index].GetComponent<PiePiece>().menuItem.action.Invoke();
    }
}
