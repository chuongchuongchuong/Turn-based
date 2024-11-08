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

    float a1 = 0, b1 = 0;// vị trí của O1: góc trên bên trái
    int a2 = 0, b2 = 6;// vị trí của O2:góc trên bên phải
    int a3 = 6, b3 = 0;// vị trí của O3: góc dưới bên trái
    int a4 = 6, b4 = 6;// vị trí của O4: góc dưới bên phải
    int a = 3, b = 3;// vị trí của X ở giữa bản đồ

    Vector2[] pieces;
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

        pieces[1] = (a1, b1);


    }
    // Start is called before the first frame update
    void Start()
    {
        EndTurn();
    }

    public void SetTurn()
    {
        if (isSelecting) // lượt chọn quân đi
        {
            GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

            button[a1, b1].interactable = false;
            button[a2, b2].interactable = false;
            button[a3, b3].interactable = false;
            button[a4, b4].interactable = false;
            button[a, b].interactable = false;

            Findmn(clickedButton);

            if (m + 1 < 7) button[m + 1, n].interactable = true;
            if (m - 1 > -1) button[m - 1, n].interactable = true;
            if (n + 1 < 7) button[m, n + 1].interactable = true;
            if (n - 1 > -1) button[m, n - 1].interactable = true;

            isSelecting = false;
        }
        else // lượt di chuyển
        {
            GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

            piece[m, n].text = "";// xóa quân cờ ở ô này đi
            wall[m, n].enabled = true;// xuất hiện tường tại ô này
            if (m + 1 < 7) button[m + 1, n].interactable = false;  // tắt hết các nút di chuyển xung quanh
            if (m - 1 > -1) button[m - 1, n].interactable = false;
            if (n + 1 < 7) button[m, n + 1].interactable = false;
            if (n - 1 > -1) button[m, n - 1].interactable = false;

            Findmn(clickedButton);


            if (CheckGameOver(clickedButton, ref XorOWIin))
            {

            }
            else
            {
                piece[m, n].text = playerside;
                playerside = (playerside == "X") ? "O" : "X";// đổi phe

            }
        }



        EndTurn();// bật ảnh bật nút
    }

    void EndTurn() // bật hết các nút ở các vị trí X, O
    {
        button[a1, b1].interactable = true;
        button[a2, b2].interactable = true;
        button[a3, b3].interactable = true;
        button[a4, b4].interactable = true;
        button[a, b].interactable = true;

    }

    void TurnOffAllCurrentButton()// Hàm này để nút và tạo tường
    {
        piece[a, b].text = "";
        if (a + 1 < 3) button[a + 1, b].interactable = false;
        if (a - 1 > -1) button[a - 1, b].interactable = false;
        if (b + 1 < 3) button[a, b + 1].interactable = false;
        if (b - 1 > -1) button[a, b - 1].interactable = false;
    }

    private bool CheckGameOver(GameObject clickedButton, ref string XorOWin)// hàm này để check xem có bên nào thắng không, hay hòa
    {
        if (clickedButton.GetComponentInChildren<Text>().text == "") ;
        {

        }
        else
        {
            string XorO = clickedButton.GetComponentInChildren<Text>().text;
            if (XorO == "X")
            {
                XorOWin = "X";
                return true;
            }
            else if (XorO == "O")
            {
                if (playerside == "X")
                {
                    XorOWin = "X";
                    return true;
                }
                else
                {
                    XorOWin = "O";
                    return true;
                }    
            }
        }

    }

    void XWin()
    {

    }

    void OWin()
    {

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
}
