using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stage { Menu, Game, Result }

public class GameFlow : MonoBehaviour
{
    [SerializeField] private FieldGenerator fieldGenerator;
    [SerializeField] private Result resultControl;
    [SerializeField] private GameObject menu, result, game, field;

    public static Stage CurrentStage { get; private set; }
    public static GameFlow _GameFlow { get; private set; }

    private void Awake()
    {
        _GameFlow = this;
        StartMenu();
    }

    public void ChangeStage(int stage)
    {
        ChangeStage((Stage)stage);
    }

    public void ChangeStage(Stage stage)
    {
        if (CurrentStage == stage) return;
        switch (stage) {
            case Stage.Menu:StartMenu(); break;
            case Stage.Game:StartGame(); break;
            case Stage.Result: StartResult(); break;
            default: break;
        }
    }

    private void StartMenu()
    {
        CurrentStage = Stage.Menu;
        fieldGenerator.PoolField();
        game.SetActive(false);
        field.SetActive(false);
        result.SetActive(false);
        menu.SetActive(true);
    }

    private void StartGame()
    {
        CurrentStage = Stage.Game;
        fieldGenerator.PoolField();
        fieldGenerator.GenerateField();
        FieldInput._FieldInput.BlockInput(false);
        Score._Score.ResetScore();
        field.SetActive(true);
        game.SetActive(true);
        result.SetActive(false);
        menu.SetActive(false);
    }

    private void StartResult()
    {
        CurrentStage = Stage.Result;
        FieldInput._FieldInput.BlockInput(true);
        resultControl.ShowResult();
        result.SetActive(true);
    }
}
