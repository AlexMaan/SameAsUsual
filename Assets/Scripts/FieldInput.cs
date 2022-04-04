using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldInput : MonoBehaviour
{
    [SerializeField] private Field field;
    [SerializeField] private Toggle pickToggle;
    private bool quickPick;

    public bool ISPaused { get; private set; }
    public static FieldInput _FieldInput;

    private void Awake() => _FieldInput = this;

    public void CheckTile(Tile tile, bool quick = true)
    {
        if (ISPaused) return;
        if (!tile.RelatedPoint.isSelected) {
            field.SelectTileGroup(tile.RelatedPoint);            
        }
        else field.PickTileGroup();
        if (quickPick && quick) Quick(tile);
    }

    private void Quick(Tile tile) => CheckTile(tile, false);

    public void QuickPick() => quickPick = pickToggle.isOn;

    public void BlockInput(bool block) => ISPaused = block;

    public IEnumerator PauseInput(float time)
    {
        ISPaused = true;
        yield return new WaitForSeconds(time);
        ISPaused = false;
    }
}
