using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider difficulty, wideRatio, shiftSide, colorsNumber;

    public static int CurrentDifficulty { get; private set; }
    public static int CurrentRatio { get; private set; }
    public static int CurrentSide { get; private set; }
    public static int CurrentColors { get; private set; }

    private void OnEnable()
    {
        SetDifficulty();
        SetRatio();
        SetSide();
        SetColors();
    }

    public void SetDifficulty() => CurrentDifficulty = (int)difficulty.value;
    public void SetRatio() => CurrentRatio = (int)wideRatio.value;
    public void SetSide() => CurrentSide = (int)shiftSide.value;
    public void SetColors() => CurrentColors = (int)colorsNumber.value;
}
