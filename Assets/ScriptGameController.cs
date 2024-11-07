using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ScriptGameController : MonoBehaviour
{
    private GameObject[] buttonObject;
    private Button[,] button;
    private Text[,] player;

    int a = 1, b = 1;


    private void Awake()
    {
        buttonObject = GameObject.FindGameObjectsWithTag("button");

        button = new Button[3, 3];
        int z = 0;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                button[i, j] = buttonObject[z].GetComponent<Button>();
                z++;
            }

        player = new Text[3, 3];
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
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
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        TurnOffAllCurrentButton();// tắt ảnh, tắt nút

        if (clickedButton != null)
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

        //GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

        /*for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                if (button[i, j] == clickedButton.GetComponent<Button>())
                {
                    a = i; b = j;
                }
            }*/

        
        EndTurn();// bật ảnh bật nút
    }

    void EndTurn() // sau khi xác định được a,b thì hàm này để bật ảnh, bật nút
    {
        player[a, b].text = "X";
        if (a + 1 < 3) button[a + 1, b].interactable = true;
        if (a - 1 > -1) button[a - 1, b].interactable = true;
        if (b + 1 < 3) button[a, b + 1].interactable = true;
        if (b - 1 > -1) button[a, b - 1].interactable = true;
    }

    void TurnOffAllCurrentButton()// Hàm này để tắt ảnh, nút trước khi di chuyển
    {
        player[a, b].text = "";
        if (a + 1 < 3) button[a + 1, b].interactable = false;
        if (a - 1 > -1) button[a - 1, b].interactable = false;
        if (b + 1 < 3) button[a, b + 1].interactable = false;
        if (b - 1 > -1) button[a, b - 1].interactable = false;
    }

}
