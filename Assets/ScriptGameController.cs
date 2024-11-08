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
    private Text[,] player;

    int a1 = 0, b1 = 0;// vị trí của O1: góc trên bên trái
    int a2 = 0, b2 = 6;// vị trí của O2:góc trên bên phải
    int a3 = 6, b3 = 0;// vị trí của O3: góc dưới bên trái
    int a4 = 6, b4 = 6;// vị trí của O4: góc dưới bên phải
    int a = 3, b = 3;// vị trí của X ở giữa bản đồ


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

        player = new Text[7, 7];
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
            {
                player[i, j] = button[i, j].transform.Find("Text").GetComponent<Text>();
            }

        foreach (Button btn in button)
        {
            btn.onClick.AddListener(SetTurn);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        EndTurn();
    }

    public void SetTurn()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        TurnOffAllCurrentButton();

        if (clickedButton != null)  // vong lặp này để xác định lại vị trí a,b mới
        {
            Button clickedButtonComponent = clickedButton.GetComponent<Button>();

            if (clickedButtonComponent != null)
            {
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                    {
                        if (button[i, j] == clickedButtonComponent)
                        {
                            a = i; b = j;
                        }
                    }

                Debug.Log($"{a}   {b}");
            }
            else
            {
                Debug.LogError("Clicked object does not have a Button component.");
            }
        }
        else
        {
            Debug.LogError("No button was clicked.");
        }

        EndTurn();// bật ảnh bật nút
    }

    void EndTurn() // bật hết các nút ở các vị trí X, O
    {
        button[a1,b1].interactable = true;
        button[a2,b2].interactable = true;
        button[a3,b3].interactable = true;
        button[a4,b4].interactable = true;
        button[a,b].interactable = true;
        
    }

    void TurnOffAllCurrentButton()// Hàm này để nút và tạo tường
    {
        player[a, b].text = "";
        if (a + 1 < 3) button[a + 1, b].interactable = false;
        if (a - 1 > -1) button[a - 1, b].interactable = false;
        if (b + 1 < 3) button[a, b + 1].interactable = false;
        if (b - 1 > -1) button[a, b - 1].interactable = false;
    }

}
