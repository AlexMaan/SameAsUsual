using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result : MonoBehaviour
{
    [SerializeField] private GameObject win, lose;

    public void ShowResult()
    {
        SetTitle(Field.WinState);
    }

    private void SetTitle(bool isWin)
    {
        win.SetActive(isWin);
        lose.SetActive(!isWin);
    }
}
