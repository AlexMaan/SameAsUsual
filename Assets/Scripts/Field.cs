using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Field : MonoBehaviour
{
    public static bool WinState { get; private set; }
    public static Field _Field { get; private set; }
    private List<TilePoint> groupedPoints = new List<TilePoint>();
    private Queue<TilePoint> samePoints = new Queue<TilePoint>();
    private List<TilePoint> visiblePoints = new List<TilePoint>();
    List<TilePoint> resultPoints = new List<TilePoint>();
    private delegate void CheckMethod(TilePoint point, TilePoint check);

    private void Awake() => _Field = this;

    #region Select

    public void SelectTileGroup(TilePoint startPoint)
    {
        if (startPoint.isSelected) return;
        if (groupedPoints.Count > 0) {
            GroupSetSelected(false);
            groupedPoints.Clear();
        }
        samePoints.Enqueue(startPoint);
        do {
            CheckAdjacentPoint(samePoints.Dequeue(), true, ColorCheck);
        } while (samePoints.Count > 0);
        if (groupedPoints.Count > 1) {
            GroupSetSelected(true);
            Score._Score.ViewPlus(groupedPoints.Count);
        }
    }

    private void GroupSetSelected(bool isSelected)
    {
        foreach (var point in groupedPoints) {
            point.isSelected = isSelected;
            point.relatedTile.SetSelected(isSelected);
        }
    }

    #endregion

    #region Pick

    public void PickTileGroup()
    {
        Score._Score.ViewPlus(0, true);
        Score._Score.AddScore(groupedPoints.Count);

        foreach (var point in groupedPoints) {
            point.color = -1;
            point.relatedTile.Visibility(false);
        }
        GroupSetSelected(false);

        FallTiles(groupedPoints.Min(x => x.x), groupedPoints.Max(x => x.x));
        StartCoroutine(WaitFalling());
        CheckResult();
    }

    private void FallTiles(int minX, int maxX)
    {
        for (int x = minX; x <= maxX; x++) {
            int emptyCount = 0;
            bool lastEmpty = false;
            for (int y = 0; y < FieldGenerator._FieldGenerator.YCount; y++) {
                if (FieldGenerator._FieldGenerator.PointsMatrix[x, y].color == -1) {
                    emptyCount++;
                    lastEmpty = true;
                }
                else if (lastEmpty) {
                    SwapTile(FieldGenerator._FieldGenerator.PointsMatrix[x, y],
                             FieldGenerator._FieldGenerator.PointsMatrix[x, y - emptyCount], 0);
                }
            }
        }
    }

    private IEnumerator WaitFalling()
    {
        //temp
        yield return new WaitForSeconds(0.2f);
        ShiftTiles();
    }

    private void ShiftTiles()
    {
        int emptyCount = 0;
        bool lastEmpty = false;
        int side = Settings.CurrentSide == 0 ? -1 : 1;
        foreach (var point in SideDefine()) {
            if (point.color == -1) {
                emptyCount++;
                lastEmpty = true;
            }
            else if (lastEmpty) {
                for (int y = 0; y < FieldGenerator._FieldGenerator.YCount; y++) {
                    if (FieldGenerator._FieldGenerator.PointsMatrix[point.x, y].color != -1) {
                        SwapTile(FieldGenerator._FieldGenerator.PointsMatrix[point.x, y],
                            FieldGenerator._FieldGenerator.PointsMatrix[point.x + (emptyCount * side), y], 1);
                    }
                    else break;
                }
            }
        }
    }


    private void SwapTile(TilePoint orig, TilePoint target, int direction)
    {
        TilePoint holdTile = new TilePoint(orig);
        orig.color = target.color;
        orig.relatedTile = target.relatedTile;
        target.color = holdTile.color;
        target.relatedTile = holdTile.relatedTile;

        int distance = 0;
        if (direction == 0) distance = Mathf.Abs(orig.y - target.y);
        if (direction == 1) distance = Mathf.Abs(orig.x - target.x);
        target.relatedTile.MoveTile(target, orig, direction, distance);
    }

    private List<TilePoint> SideDefine()
    {
        List<TilePoint> row = new List<TilePoint>();
        if (Settings.CurrentSide == 0) {
            for (int x = 0; x < FieldGenerator._FieldGenerator.XCount; x++) {
                row.Add(FieldGenerator._FieldGenerator.PointsMatrix[x, 0]);
            }
        }
        else {
            for (int x = FieldGenerator._FieldGenerator.XCount - 1; x >= 0; x--) {
                row.Add(FieldGenerator._FieldGenerator.PointsMatrix[x, 0]);
            }
        }
        return row;
    }

    #endregion

    #region Check

    private void CheckAdjacentPoint(TilePoint point, bool group, CheckMethod check)
    {
        if (group) groupedPoints.Add(point);
        if (point.x > 0) check(point, FieldGenerator._FieldGenerator.PointsMatrix[point.x - 1, point.y]);
        if (point.y > 0) check(point, FieldGenerator._FieldGenerator.PointsMatrix[point.x, point.y - 1]);
        if (point.x < FieldGenerator._FieldGenerator.PointsMatrix.GetLength(0) - 1)
            check(point, FieldGenerator._FieldGenerator.PointsMatrix[point.x + 1, point.y]);
        if (point.y < FieldGenerator._FieldGenerator.PointsMatrix.GetLength(1) - 1)
            check(point, FieldGenerator._FieldGenerator.PointsMatrix[point.x, point.y + 1]);
    }

    private void ColorCheck(TilePoint point, TilePoint check)
    {
        if (check.color == point.color && !groupedPoints.Contains(check)) samePoints.Enqueue(check);
    }

    private void ColorCheckResult(TilePoint point, TilePoint check)
    {
        if (check.color == point.color) resultPoints.Add(check);
    }

    private void CheckResult()
    {
        visiblePoints.Clear();
        foreach (var point in FieldGenerator._FieldGenerator.PointsMatrix) {
            if (point.color != -1) visiblePoints.Add(point);
        }
        if (visiblePoints.Count == 0) {
            WinState = true;
            GameFlow._GameFlow.ChangeStage(Stage.Result);
        }
        else {
            resultPoints.Clear();
            foreach (var point in visiblePoints) {
                CheckAdjacentPoint(point, false, ColorCheckResult);
            }
            if (resultPoints.Count == 0) {
                WinState = false;
                GameFlow._GameFlow.ChangeStage(Stage.Result);
            }
        }
    }

    #endregion
}
