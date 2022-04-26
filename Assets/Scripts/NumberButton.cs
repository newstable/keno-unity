using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberButton : MonoBehaviour
{
    public int id;

    public void setId(int _id)
    {
        id = _id;
    }

    public void OnClick()
    {
        GameObject gameManager = GameObject.Find("GameManager");
        gameManager.GetComponent<GameManager>().handleClickNumber(id);
    }
}
