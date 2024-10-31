using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    [SerializeField] float z;
    [SerializeField] Vector2 offset;
    // Update is called once per frame
    void Update()
    {
        //マウスに合わせてカーソルを動かす。UI/Image。ワールド座標・
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = z;
        // transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        // recttransformの場合
        GetComponent<RectTransform>().position = Camera.main.ScreenToWorldPoint(mousePos);
        // オフセットを加える
        GetComponent<RectTransform>().position += (Vector3)offset;
    }
}
