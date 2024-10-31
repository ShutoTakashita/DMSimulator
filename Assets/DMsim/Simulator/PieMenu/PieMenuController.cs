using UnityEngine;

public class PieMenuController : MonoBehaviour
{
    void Update()
    {
        string menuName = "Menu1"; // 表示したいパイメニューの名前

        // 右クリックを押したときにメニューを表示
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = Input.mousePosition;
            PieMenuManager.ShowMenu(menuName, mousePosition);
        }
    }
}
