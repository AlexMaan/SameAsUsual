using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private Text scoreCount;
    [SerializeField] private Text scorePlus;
    [SerializeField] private Text scoreResult;
    private int currentScore;

    public static Score _Score { get; private set; }

    private void OnEnable() => _Score = this;

    public void ResetScore()
    {
        currentScore = 0;
        ViewPlus(0, true);
        AddScore();
    }

    public void AddScore(int tilesCount = 0)
    {
        CalculateScore(tilesCount);
        scoreCount.text = scoreResult.text = currentScore.ToString();
    }

    private void CalculateScore(int tilesCount) =>
        currentScore += ScoreFormula(tilesCount);

    public void ViewPlus(int tilesCount, bool hide = false)
    {
        if (hide) scorePlus.text = "";
        else scorePlus.text = "+" + ScoreFormula(tilesCount);
    }

    //temp
    private int ScoreFormula(int tilesCount)
    {
        return Mathf.RoundToInt(Mathf.Min(tilesCount, 20) * (((float)tilesCount / 5) + 1) * ((Settings.CurrentColors * 0.1f) + 1)) / 2;
    }
}
