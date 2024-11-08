using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ScriptGameController : MonoBehaviour
{
    private GameObject[] buttonObject;
    private Button[,] button;
    private Text[,] pieceName;// quân cờ
    private Image[,] wall;

    int t;// t là biến để chọn ra quân cờ nào đang được chọn

    Vector2Int[] piece;
    int a, b;// a, b 2 vị trí tọa độ vừa được click

    bool isSelecting = true;// biến để quyết định xem là đang chọn quân hay đang chọn cách di chuyển
    string playerside = "X";// lượt đầu tiên bao giờ cũng là X đi
    string XorOWIin;


    private void Awake()
    {
        buttonObject = GameObject.FindGameObjectsWithTag("button");

        button = new Button[7, 7];
        int z = 0;
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
            {
                button[i, j] = buttonObject[z].GetComponent<Button>();
                z++;
            }

        pieceName = new Text[7, 7];
        wall = new Image[7, 7];
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
            {
                pieceName[i, j] = button[i, j].transform.Find("Text").GetComponent<Text>();
                wall[i, j] = button[i, j].transform.Find("wall").GetComponent<Image>();
            }

        foreach (Button btn in button) // gắn hàm SetTurn vào Onclick của các button
        {
            btn.onClick.AddListener(SetTurn);
        }

        piece = new Vector2Int[5];





    }
    // Start is called before the first frame update
    void Start()
    {
        piece[0] = new Vector2Int(0, 0); // vị trí của O1: góc trên bên trái
        piece[1] = new Vector2Int(0, 6); // vị trí của O2:góc trên bên phải
        piece[2] = new Vector2Int(6, 0); // vị trí của O3: góc dưới bên trái
        piece[3] = new Vector2Int(6, 6); // vị trí của O4: góc dưới bên phải
        piece[4] = new Vector2Int(3, 3); // vị trí của X ở giữa bản đồ

        a = 1; b = 2;
        TurnOnPieceButton();
    }

    public void SetTurn()
    {
        if (isSelecting) // lượt chọn quân đi
        {
            GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
            Debug.Log(clickedButton.name);
            for (int i = 0; i < 5; i++) // tắt hết các nút đi
            {
                button[piece[i].x, piece[i].y].interactable = false;
            }

            Findmn(clickedButton, ref a, ref b);
            Debug.Log($"{a} {b}");

            if (a + 1 < 7) button[a + 1, b].interactable = true; // bật các nút xung quanh quân đó để di chuyển
            if (a - 1 > -1) button[a - 1, b].interactable = true;
            if (b + 1 < 7) button[a, b + 1].interactable = true;
            if (b - 1 > -1) button[a, b - 1].interactable = true;


            for (int i = 0; i < 5; i++)
            {
                if (piece[i].x == a && piece[i].y == b)
                {
                    t = i;
                    break;
                }
            }

            isSelecting = false;
            return;
        }
        else // lượt di chuyển
        {
            GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

            pieceName[a, b].text = "";// xóa quân cờ ở ô này đi
            button[a, b].enabled = false; // vô hiệu hóa luôn nút này
            wall[a, b].enabled = true;// xuất hiện tường tại ô này

            if (a + 1 < 7) button[a + 1, b].interactable = false;  // tắt hết các nút di chuyển xung quanh
            if (a - 1 > -1) button[a - 1, b].interactable = false;
            if (b + 1 < 7) button[a, b + 1].interactable = false;
            if (b - 1 > -1) button[a, b - 1].interactable = false;

            Findmn(clickedButton, ref a, ref b);
            piece[t] = new Vector2Int(a, b);// chuyển vị trí quân đang điều khiển

            if (CheckGameOver(clickedButton, ref XorOWIin))
            {
                switch (XorOWIin) // nếu gameover thì sẽ gameover theo kiểu nào
                {
                    case "X": // kiểu 1: X thắng
                        XWin(); break;
                    case "O": // kiểu 2: O thắng
                        OWin(); break;
                    case "Draw": // Kiểu 3: Hòa
                        Draw(); break;
                }
            }
            else
            {
                pieceName[a, b].text = playerside;
                playerside = (playerside == "X") ? "O" : "X";// đổi phe
                TurnOnPieceButton();// bật nút của vị trí các quân
                isSelecting = true;
            }
        }

    }

    void TurnOnPieceButton() // bật nút của vị trí các quân
    {
        for (int i = 0; i < 5; i++) // bật nút của vị trí các quân
        {
            button[piece[i].x, piece[i].y].interactable = true;
        }
    }

    private bool CheckGameOver(GameObject clickedButton, ref string XorOWin)// hàm này để check xem có bên nào thắng không, hay hòa
    {
        if (clickedButton.GetComponentInChildren<Text>().text == "")// nếu như vị trí di chuyển là ô trống
        {
            if (CheckAllPiecesCanMove()) return false; // nếu vẫn có thể di chuyển thì game tiêp tục
            else
            {
                if (playerside == "X") // nếu ko còn có thể di chuyển mà đang trong lượt X, thì X thắng
                {
                    XorOWin = "X";
                    return true;
                }
                else //nếu ko còn có thể di chuyển mà đang trong lượt O, thì hòa
                {
                    XorOWin = "Draw";
                    return true;
                }
            }
        }
        else// nếu vị trí di chuyển tới không phải ô trống
        {
            string XorO = clickedButton.GetComponentInChildren<Text>().text;
            if (XorO == "X") // nếu vị trí di chuyển tới là X
            {
                XorOWin = "X";
                return true;
            }
            else if (XorO == "O")// nếu vị trí di chuyển tới là O
            {
                if (playerside == "X")//Nếu quân cờ di chuyển là X
                {
                    XorOWin = "X";
                    return true;
                }
                else // Nếu quân cờ di chuyển là O
                {
                    XorOWin = "O";
                    return true;
                }
            }
        }

        return false;

    }


    void Findmn(GameObject clickedButton, ref int m, ref int n)// hàm tìm ra tọa độ của nút vừa bấm
    {
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
            {
                if (button[i, j] == clickedButton.GetComponent<Button>())
                {
                    m = i; n = j;
                }
            }
    }

    //Hàm này để check xem tất cả các quân còn khả năng đi không
    bool CheckAllPiecesCanMove()
    {
        for (int i = 0; i < 5; i++)
        {
            if (Check1pieceCanMove(button, piece[i].x, piece[i].y))
                return true;
        }

        return false;
    }

    // hàm này để check xem 4 ô xung quanh có đi được nữa không.
    //// truyền vào đây 1 dãy nút, m,n là vị trí của nút này trong tọa độ
    bool Check1pieceCanMove(Button[,] button, int a, int b)
    {
        if (a + 1 < 7 && button[a + 1, b].enabled) return true;

        if (b + 1 < 7 && button[a, b + 1].enabled) return true;

        if (a - 1 > -1 && button[a - 1, b].enabled) return true;

        if (b - 1 > -1 && button[a, b - 1].enabled) return true;

        return false;
    }

    void XWin()
    {

    }

    void OWin()
    {

    }

    void Draw()
    {

    }
}
