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
    private Text[,] piece;// quân cờ
    private Image[,] wall;

    int[] a, b;
    int t;// t là biến để chọn ra quân cờ nào đang được chọn

    Vector2Int[] pieces;
    int m, n;

    bool isSelecting = true;// biến để quyết định xem là đang chọn quân hay đang chọn cách di chuyển
    string playerside = "X";
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

        piece = new Text[7, 7];
        wall = new Image[7, 7];
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
            {
                piece[i, j] = button[i, j].transform.Find("Text").GetComponent<Text>();
                wall[i, j] = button[i, j].transform.Find("wall").GetComponent<Image>();
            }

        foreach (Button btn in button) // gắn hàm SetTurn vào Onclick của các button
        {
            btn.onClick.AddListener(SetTurn);
        }

        pieces = new Vector2Int[5];
        a = new int[5]; b = new int[5];





    }
    // Start is called before the first frame update
    void Start()
    {
        a[0] = 0; b[0] = 0; // vị trí của O1: góc trên bên trái
        a[1] = 0; b[1] = 6; // vị trí của O2:góc trên bên phải
        a[2] = 6; b[2] = 0; // vị trí của O3: góc dưới bên trái
        a[3] = 6; b[3] = 6; // vị trí của O4: góc dưới bên phải
        a[4] = 3; b[4] = 3; // vị trí của X ở giữa bản đồ

        for (int i = 0; i < 5; i++)
        {
            pieces[i] = new Vector2Int(a[i], b[i]);
        }

        EndTurn();
    }

    public void SetTurn()
    {
        if (isSelecting) // lượt chọn quân đi
        {
            GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
            for (int i = 0; i < 5; i++) // tắt hết các nút đi
            {
                button[a[i], b[i]].interactable = false;
            }

            Findmn(clickedButton);

            if (m + 1 < 7) button[m + 1, n].interactable = true; // bật các nút xung quanh quân đó để di chuyển
            if (m - 1 > -1) button[m - 1, n].interactable = true;
            if (n + 1 < 7) button[m, n + 1].interactable = true;
            if (n - 1 > -1) button[m, n - 1].interactable = true;

            for (int i = 0; i < 5; i++)
            {
                if (pieces[i] == new Vector2Int(m, n))
                {
                    t = i;
                    break;
                }
            }

            isSelecting = false;
        }
        else // lượt di chuyển
        {
            GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

            piece[m, n].text = "";// xóa quân cờ ở ô này đi
            button[m, n].enabled = false; // vô hiệu hóa luôn nút này
            wall[m, n].enabled = true;// xuất hiện tường tại ô này

            if (m + 1 < 7) button[m + 1, n].interactable = false;  // tắt hết các nút di chuyển xung quanh
            if (m - 1 > -1) button[m - 1, n].interactable = false;
            if (n + 1 < 7) button[m, n + 1].interactable = false;
            if (n - 1 > -1) button[m, n - 1].interactable = false;

            Findmn(clickedButton);
            pieces[t] = new Vector2Int(m, n);// chuyển vị trí quân đang điều khiển

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
                piece[m, n].text = playerside;
                playerside = (playerside == "X") ? "O" : "X";// đổi phe
            }
        }



        EndTurn();// bật nút của vị trí các quân
    }

    void EndTurn() // bật nút của vị trí các quân
    {
        for (int i = 0; i < 5; i++) // bật nút của vị trí các quân
        {
            button[a[i], b[i]].interactable = true;
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

    }


    void Findmn(GameObject clickedButton)// hàm tìm ra tọa độ của nút vừa bấm
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
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
            if (Check1pieceCanMove(button[pieces[i].x, pieces[i].y]))
                return true;
        }

        return false;
    }

    // hàm này để check xem 4 ô xung quanh có đi được nữa không
    bool Check1pieceCanMove(Button button)
    {

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
