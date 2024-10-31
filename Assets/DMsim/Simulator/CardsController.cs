using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine.EventSystems;  // 必須
using System.Collections.Generic;  // 必須
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public Camera uiCamera; // UIカメラ (Canvasに設定されているカメラ)

    public GraphicRaycaster uiRaycaster; // UI Raycaster

    Vector3 onMouseDownPosition;
    Card selectedCard; // 選択されたカードの参照

    bool isDragging = false;

    [SerializeField] private ZoneManager battleZone;
    [SerializeField] private ZoneManager manaZone;
    [SerializeField] private ZoneManager handZone;
    [SerializeField] private ZoneManager shieldZone;
    [SerializeField] private ZoneManager deckZone;
    [SerializeField] private ZoneManager graveZone;
    [SerializeField] private ZoneManager partnerZone;
    [SerializeField] private ZoneManager temporaryZone;

    float doubleClickTime = 0.3f;
    float lastClickTime = 0;
    // UltimateRadialMenu radialMenu = null;
    PieMenu radialMenu = null;

    ZoneManager activeZone = null;


    private void Start()
    {
    }

    private void Update()
    {
        Vector2 mousePosition = Vector2.zero;
        bool mouseButtonDown = false;
        bool mouseButtonUp = false;

        //右クリックを取得
        bool mouseButtonDownRight = false;
        bool mouseButtonUpRight = false;

#if ENABLE_INPUT_SYSTEM
        // Input Systemが有効な場合の処理
        Mouse mouse = InputSystem.GetDevice<Mouse>();
        if (mouse == null)
            return;

        mousePosition = mouse.position.ReadValue();
        mouseButtonDown = mouse.leftButton.wasPressedThisFrame;
        mouseButtonUp = mouse.leftButton.wasReleasedThisFrame;

        mouseButtonDownRight = mouse.rightButton.wasPressedThisFrame;
        mouseButtonUpRight = mouse.rightButton.wasReleasedThisFrame;
#else
        // Input Managerが有効な場合の処理
        mousePosition = Input.mousePosition;
        mouseButtonDown = Input.GetMouseButtonDown(0);
        mouseButtonUp = Input.GetMouseButtonUp(0);

        mouseButtonDownRight = Input.GetMouseButtonDown(1);
        mouseButtonUpRight = Input.GetMouseButtonUp(1);
#endif

        Vector3 worldPosition = uiCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, uiCamera.nearClipPlane));


        // マウスボタン(左)が押された場合
        if (mouseButtonDownRight)
        {
            // UI要素に対してRaycast
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = mousePosition;

            // Raycast結果を格納するリスト
            List<RaycastResult> results = new List<RaycastResult>();
            uiRaycaster.Raycast(pointerEventData, results);

            // UI要素にヒットしているか確認
            if (results.Count > 0)
            {
                // 最初のヒット結果を使う
                GameObject clickedObject = results[0].gameObject;
                onMouseDownPosition = clickedObject.transform.position;

                Debug.Log($"Clicked on UI element: {clickedObject.name}");

                // Cardをクリックした場合、カードの処理を続ける
                selectedCard = clickedObject.GetComponent<Card>();
                if (selectedCard != null)
                {
                    Debug.Log($"Selected card: {selectedCard.GetCardName()}");
                    // マウスの位置にPieメニューを表示
                    Vector3 screenPosition = Input.mousePosition;
                    PieMenuManager.ShowMenu("CardMenu", mousePosition);
                }
                else if (clickedObject.name.Substring(clickedObject.name.Length - 4) == "Zone")//ゾーンをクリックした場合
                {
                    Debug.Log($"Selected zone: {clickedObject.name}");
                    // マウスの位置にPieメニューを表示
                    Vector3 screenPosition = Input.mousePosition;
                    Debug.Log("ScreenPosiotn" + screenPosition);
                    if (clickedObject.name == "ManaZone")
                    {
                        PieMenuManager.ShowMenu("ManaZoneMenu", mousePosition);
                    }
                    else if (clickedObject.name == "BattleZone")
                    {
                        PieMenuManager.ShowMenu("BattleZoneMenu", mousePosition);

                    }
                    else if (clickedObject.name == "DeckZone")
                    {
                        PieMenuManager.ShowMenu("DeckZoneMenu", mousePosition);

                    }
                    else if (clickedObject.name == "GraveZone")
                    {
                        PieMenuManager.ShowMenu("GraveZoneMenu", mousePosition);

                    }
                    else if (clickedObject.name == "PartnerZone")
                    {
                        PieMenuManager.ShowMenu("PartnerZoneMenu", mousePosition);

                    }
                    else if (clickedObject.name == "HandZone")
                    {
                        PieMenuManager.ShowMenu("HandZoneMenu", mousePosition);

                    }
                    else if (clickedObject.name == "TemporaryZone")
                    {
                        PieMenuManager.ShowMenu("TemporaryZoneMenu", mousePosition);

                    }
                }
            }
            else
            {
                Debug.Log("No UI element clicked");
            }
        }


        // マウスボタンがはなされた場合
        if (mouseButtonUpRight)
        {
            // メニューが有効であれば無効にする
            // PieMenu.cs側に処理をうつした
        }



        // マウスボタン(右)が押された場合
        if (mouseButtonDown)
        {
            // UI要素に対してRaycast
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = mousePosition;

            // Raycast結果を格納するリスト
            List<RaycastResult> results = new List<RaycastResult>();
            uiRaycaster.Raycast(pointerEventData, results);

            // UI要素にヒットしているか確認
            if (results.Count > 0)
            {
                // 最初のヒット結果を使う
                GameObject clickedObject = results[0].gameObject;
                onMouseDownPosition = clickedObject.transform.position;

                Debug.Log($"Clicked on UI element: {clickedObject.name}");

                // Cardをクリックした場合、カードの処理を続ける
                selectedCard = clickedObject.GetComponent<Card>();
                if (selectedCard != null)
                {
                    Debug.Log($"Dragging card: {selectedCard.GetCardName()}");
                    // カードを持って移動
                    isDragging = true;
                    //持ったカードを最前面に
                    selectedCard.transform.SetAsLastSibling();
                }
            }
            else
            {
                Debug.Log("No UI element clicked");
            }
        }

        bool isOutScreen = false;

        if (isDragging && selectedCard != null)
        {
            // 親のRectTransformを取得
            RectTransform parentRectTransform = selectedCard.transform.parent.GetComponent<RectTransform>();
            Vector2 localPoint;

            // マウスのスクリーン座標を親のRectTransform内のローカル座標に変換
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRectTransform,
                mousePosition,
                uiCamera,
                out localPoint
            );

            // ローカル座標を使ってカードの位置を更新
            selectedCard.GetComponent<RectTransform>().anchoredPosition = localPoint;

            if (selectedCard.transform.position.x < -42 || selectedCard.transform.position.x > 42 || selectedCard.transform.position.y < -24 || selectedCard.transform.position.y > 24)
            {
                Debug.Log("Card out of screen");
                isOutScreen = true;
            }
        }

        // マウスボタンがはなされた場合もしくは画面外にカーソルが出た
        if (mouseButtonUp || isOutScreen)
        {
            if (!isDragging) return;
            isDragging = false;
            //離された場所に応じて処理を分岐
            // UI要素に対してRaycast
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = mousePosition;

            // Raycast結果を格納するリスト
            List<RaycastResult> results = new List<RaycastResult>();
            uiRaycaster.Raycast(pointerEventData, results);

            // UI要素にヒットしているか確認
            if (results.Count > 0)
            {
                Debug.Log($"Dropped on UI element: {results[0].gameObject.name}");
                // 最初のヒット結果はこのカードであるか別のゾーンである
                GameObject droppedObject = results[0].gameObject;
                if (droppedObject == selectedCard.gameObject)
                {
                    if (results.Count == 1) // 何もないところで落としたかもしれない
                    {
                        //元の位置に戻す
                        Debug.Log("Dropped on nothing");
                        selectedCard.transform.position = onMouseDownPosition;
                        return;
                    }
                    else
                    {
                        droppedObject = results[1].gameObject;
                    }
                }

                Debug.Log($"Dropped on UI element: {droppedObject.name}");

                // Cardの上でドロップした場合、カードの処理を続ける
                Card droppedCard = droppedObject.GetComponent<Card>();
                if (droppedCard != null)
                {
                    Debug.Log($"Dropped card: {droppedCard.GetCardName()}");

                    //Shiftを押していた場合
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (selectedCard.GetCardState() == Card.Outward.Hidden && selectedCard.GetZoneManager() == handZone) selectedCard.SetCard(Card.Outward.FaceUp);

                        Card.Outward outward = selectedCard.GetCardState();
                        ZoneManager zoneManager = selectedCard.GetZoneManager();
                        ZoneManager dropZoneManager = droppedCard.GetZoneManager();
                        zoneManager.MoveCardToZone(selectedCard, dropZoneManager);//Deckの場合はAddCardで最後尾に追加されるが、この後に最前面に直される
                        dropZoneManager.InsertCard(selectedCard, droppedCard, false);
                        dropZoneManager.PlaceAbove(selectedCard);
                        selectedCard.SetCard(outward);
                    }
                    else if (selectedCard.GetZoneManager() == droppedCard.GetZoneManager())
                    {
                        Debug.Log("Same zone");
                        //カードの順番を入れ替える
                        ZoneManager zoneManager = selectedCard.GetZoneManager();

                        Debug.Log(Input.mousePosition.x + " " + selectedCard.transform.position.x + " " + droppedCard.transform.position.x);
                        //Input.mousePositionがカードの左右どちらか
                        if (selectedCard.transform.position.x < droppedCard.transform.position.x)
                        {
                            zoneManager.InsertCard(selectedCard, droppedCard);
                            Debug.Log("Right side");
                        }
                        else
                        {
                            zoneManager.InsertCard(selectedCard, droppedCard, false);
                            Debug.Log("Left side");
                        }
                    }
                    else //別のゾーンのカードの上で落とした
                    {
                        Debug.Log("Different zone");
                        //カードを移動
                        ZoneManager zoneManager = selectedCard.GetZoneManager();
                        zoneManager.MoveCardToZone(selectedCard, droppedCard.GetZoneManager());
                    }
                }
                else if (droppedObject.name.Substring(droppedObject.name.Length - 4) == "Zone")//ゾーンの上で落とした場合
                {
                    if (droppedObject.name == selectedCard.GetZoneManager().gameObject.name)
                    {
                        Debug.Log("Same zone:" + droppedObject.name);
                        //大きく動いていない場合元の位置に戻す
                        Debug.Log(Vector3.Distance(onMouseDownPosition, selectedCard.transform.position));
                        if (Vector3.Distance(onMouseDownPosition, selectedCard.transform.position) < 20)
                        {
                            selectedCard.transform.position = onMouseDownPosition;
                        }
                        else if (selectedCard.transform.position.x < selectedCard.GetZoneManager().transform.position.x)
                        {
                            Debug.Log("Left side Dropped");
                            if (selectedCard.GetZoneManager().GetCard(0).GetCardName() != selectedCard.GetCardName())
                            {
                                selectedCard.GetZoneManager().InsertCard(selectedCard, selectedCard.GetZoneManager().GetCard(0));
                            }
                            selectedCard.GetZoneManager().ResetPlaceAbove(selectedCard);
                            selectedCard.GetZoneManager().ArrangeCards();
                        }
                        else
                        {
                            Debug.Log("Right side Dropped");
                            selectedCard.GetZoneManager().InsertCardAtLast(selectedCard);
                            selectedCard.GetZoneManager().ResetPlaceAbove(selectedCard);
                            selectedCard.GetZoneManager().ArrangeCards();
                        }

                    }
                    else
                    {
                        Card.Outward outward = selectedCard.GetCardState();
                        Debug.Log("Different zone:" + droppedObject.name);
                        //カードを移動
                        ZoneManager zoneManager = selectedCard.GetZoneManager();
                        ZoneManager targetZone = droppedObject.GetComponent<ZoneManager>();
                        zoneManager.MoveCardToZone(selectedCard, targetZone);
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            if (outward == Card.Outward.Hidden && zoneManager == handZone)
                                selectedCard.SetCard(Card.Outward.FaceUp);
                            else
                                selectedCard.SetCard(outward);
                        }
                    }
                }
                else
                {
                    Debug.Log("No UI element on drop");
                    //元の位置に戻す
                    selectedCard.transform.position = onMouseDownPosition;
                }
            }
            else // Countが0の場合(画面外におとした場合？)
            {
                Debug.Log("No UI element on drop");
                //元の位置に戻す
                selectedCard.transform.position = onMouseDownPosition;
            }

            if (Time.time - lastClickTime < doubleClickTime)
            {
                Debug.Log("Double click");
                selectedCard.TapCard(!selectedCard.isTapped);
                if (selectedCard.GetZoneManager() == manaZone && selectedCard.isTapped)
                {
                    Debug.Log("Mana zone tapped");
                    //リストの一番後ろに持ってくる
                    manaZone.InsertCardAtLast(selectedCard);
                }
            }
            lastClickTime = Time.time;
        }


        // ボタンを押している間、Pieメニューを表示する
        // 各ゾーンの対応するキーとメニュー名をリストにまとめる
        Dictionary<KeyCode, string> zoneKeyMappings = new Dictionary<KeyCode, string>()
    {
        { KeyCode.B, "BattleZoneMenu" },  // BattleZone
        { KeyCode.S, "ShieldZoneMenu" },  // ShieldZone
        { KeyCode.D, "DeckZoneMenu" },    // DeckZone
        { KeyCode.G, "GraveZoneMenu" },   // GraveZone
        { KeyCode.P, "PartnerZoneMenu" },   // SuperZone
        { KeyCode.M, "ManaZoneMenu" },    // ManaZone
        { KeyCode.H, "HandZoneMenu" },     // HandZone
        { KeyCode.T, "TemporaryZoneMenu" }     // HandZone
    };

        // 各ゾーンのオブジェクトのTransformをセット（ここでは例として宣言。実際は割り当て済みのものを使用）
        Dictionary<string, Transform> zoneTransforms = new Dictionary<string, Transform>()
    {
        { "BattleZoneMenu", battleZone.transform },
        { "ShieldZoneMenu", shieldZone.transform },
        { "DeckZoneMenu", deckZone.transform },
        { "GraveZoneMenu", graveZone.transform },
        { "PartnerZoneMenu", partnerZone.transform },
        { "ManaZoneMenu", manaZone.transform },
        { "HandZoneMenu", handZone.transform },
        { "TemporaryZoneMenu", temporaryZone.transform }
    };
        // Keyが押されたときの処理
        foreach (var zone in zoneKeyMappings)
        {
            if (Input.GetKeyDown(zone.Key))
            {
                // 対応するゾーンメニューを取得
                string menuName = zone.Value;
                radialMenu = PieMenuManager.GetMenu(menuName);

                if (radialMenu != null && zoneTransforms.ContainsKey(zone.Value))
                {
                    Debug.Log(zoneTransforms[zone.Value].position);
                    PieMenuManager.ShowMenu(menuName, zoneTransforms[zone.Value].position, true);
                }
                activeZone = null;
            }

            // Keyが離されたときの処理
            if (Input.GetKeyUp(zone.Key))
            {
                // メニューが有効であれば無効にする
                if (radialMenu != null && radialMenu.IsMenuActive())
                {
                    radialMenu.HideMenu();
                }
                radialMenu = null;
                activeZone = zoneTransforms[zone.Value].GetComponent<ZoneManager>();
                Debug.Log("Active zone:" + activeZone.gameObject.name);
            }
        }

        // 1～9,0のキーに対応させる
        for (int i = 0; i <= 9; i++)
        {
            KeyCode keyCode = (i == 0) ? KeyCode.Alpha0 : (KeyCode)((int)KeyCode.Alpha1 + (i - 1));

            // 該当のキーが押されたかチェック
            if (Input.GetKeyDown(keyCode) && radialMenu != null)
            {
                int buttonIndex = (i == 0) ? 9 : i - 1; // 1~9は0~8番目のボタン、0は9番目に対応

                if (radialMenu.menuItems.Count > buttonIndex)
                {
                    // PieMenuのボタンに関連付けられている UnityEvent を発火させる
                    radialMenu.ExecuteAction(buttonIndex);
                }
                else
                {
                    Debug.LogWarning($"Button {buttonIndex + 1} not found in the radial menu.");
                }
            }

            if (Input.GetKeyDown(keyCode) && activeZone != null)
            {
                if (activeZone == manaZone || activeZone == battleZone || activeZone == shieldZone)
                {
                    int buttonIndex = (i == 0) ? 9 : i - 1; // 1~9は0~8番目のボタン、0は9番目に対応
                    //対応するカードをタップ/アンタップ
                    activeZone.TapUntapCard(buttonIndex);
                }
                else if (activeZone == deckZone)
                {

                }
            }
        }
    }

    public void TapCard()
    {
        selectedCard.TapCard(true);
    }
    public void UntapCard()
    {
        selectedCard.TapCard(false);
    }

    public void PlaceAbove()
    {
        selectedCard.GetZoneManager().PlaceAbove(selectedCard);
    }

    public void PlaceBelow()
    {
        selectedCard.GetZoneManager().PlaceBelow(selectedCard);
    }

    public void FaceUp()
    {
        selectedCard.SetCard(Card.Outward.FaceUp);
    }

    public void FaceDown()
    {
        selectedCard.SetCard(Card.Outward.FaceDown);
    }

    public void Check()
    {
        if (selectedCard.GetCardState().Equals(Card.Outward.FaceDown))
        {
            selectedCard.SetCard(Card.Outward.Hidden);
        }
        else if (selectedCard.GetCardState().Equals(Card.Outward.Hidden))
        {
            selectedCard.SetCard(Card.Outward.FaceDown);
        }
    }

    public void ToggleIsField()
    {
        selectedCard.IsFieldCard = !selectedCard.IsFieldCard;
    }

}
