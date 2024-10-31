using UnityEngine;
using System.Collections.Generic;

public class PieMenuManager : MonoBehaviour
{
    private static PieMenuManager instance;

    // パイメニューを名前で管理する辞書
    private Dictionary<string, PieMenu> pieMenus = new Dictionary<string, PieMenu>();

    void Awake()
    {
        // シングルトンパターンの実装
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 必要に応じて削除
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // パイメニューを登録する
    public void RegisterPieMenu(string name, PieMenu pieMenu)
    {
        if (!pieMenus.ContainsKey(name))
        {
            pieMenus.Add(name, pieMenu);
        }
        else
        {
            Debug.LogError("同じ名前のPieMenuが既に登録されています: " + name);
        }
    }

    // パイメニューを表示する
    public static void ShowMenu(string name, Vector3 position, bool useLocal = false)
    {
        if (instance == null)
        {
            Debug.LogError("PieMenuManagerのインスタンスが存在しません。シーンにPieMenuManagerを追加してください。");
            return;
        }

        if (instance.pieMenus.ContainsKey(name))
        {
            instance.pieMenus[name].ShowMenuInstance(position, useLocal);
        }
        else
        {
            Debug.LogError("指定された名前のPieMenuが見つかりません: " + name);
        }
    }

    //パイメニューへの参照を取得する
    public static PieMenu GetMenu(string name)
    {
        if (instance == null)
        {
            Debug.LogError("PieMenuManagerのインスタンスが存在しません。シーンにPieMenuManagerを追加してください。");
            return null;
        }

        if (instance.pieMenus.ContainsKey(name))
        {
            return instance.pieMenus[name];
        }
        else
        {
            Debug.LogWarning("指定された名前のPieMenuが見つかりません: " + name);
            return null;
        }
    }

    // パイメニューを非表示にする
    public static void HideMenu(string name)
    {
        if (instance == null)
        {
            Debug.LogError("PieMenuManagerのインスタンスが存在しません。シーンにPieMenuManagerを追加してください。");
            return;
        }

        if (instance.pieMenus.ContainsKey(name))
        {
            instance.pieMenus[name].HideMenu();
        }
        else
        {
            Debug.LogError("指定された名前のPieMenuが見つかりません: " + name);
        }
    }

    // パイメニューの表示状態を取得する
    public static bool IsMenuActive(string name)
    {
        if (instance == null)
        {
            Debug.LogError("PieMenuManagerのインスタンスが存在しません。シーンにPieMenuManagerを追加してください。");
            return false;
        }

        if (instance.pieMenus.ContainsKey(name))
        {
            return instance.pieMenus[name].IsMenuActive();
        }
        else
        {
            Debug.LogError("指定された名前のPieMenuが見つかりません: " + name);
            return false;
        }
    }
}
