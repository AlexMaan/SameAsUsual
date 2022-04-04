using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class TilePoint
{
    public int x, y;
    public Vector2 position;
    public int color;
    public bool isSelected;
    public Tile relatedTile;

    public TilePoint(int x, int y, Vector2 position, int color)
    {
        this.x = x;
        this.y = y;
        this.position = position;
        this.color = color;
    }

    public TilePoint(TilePoint tilePoint)
    {
        x = tilePoint.x;
        y = tilePoint.y;
        color = tilePoint.color;
        isSelected = tilePoint.isSelected;
        relatedTile = tilePoint.relatedTile;
    }
}

public class FieldGenerator : MonoBehaviour
{
    [SerializeField] private GameObject field;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private float tileSize, fieldSizeX, fieldSizeY;
    [SerializeField] private int tilesCountMultiplier;
    [SerializeField] private List<Color> colorsDepo;
    private List<int> orderedColors = new List<int>();
    private Stack<Tile> pooledTiles = new Stack<Tile>();
    private Tile placedTile;

    public int XCount { get; private set; }
    public int YCount { get; private set; }
    public List<Color> ColorsDepo => colorsDepo;
    public static FieldGenerator _FieldGenerator { get; private set; }
    public TilePoint[,] PointsMatrix { get; private set; }
    public List<Tile> PlacedTiles { get; } = new List<Tile>();
    public float ScaleK { get; set; }

    private void Awake() => _FieldGenerator = this;

    public void GenerateField()
    {
        FillMatrix();
        PlaceTiles();
    }

    private void FillMatrix()
    {
        YCount = (Settings.CurrentDifficulty + 1) * tilesCountMultiplier;
        XCount = YCount * Settings.CurrentRatio;
        ScaleK = Mathf.Min(fieldSizeX / (XCount * tileSize), fieldSizeY / (YCount * tileSize), 1);
        float xShift = (tileSize * XCount * ScaleK / 2) - (tileSize * ScaleK / 2);
        float yShift = (tileSize * YCount * ScaleK / 2) - (tileSize * ScaleK / 2);
        PointsMatrix = new TilePoint[XCount, YCount];

        SelectColors();
        int counter = 0;

        for (int x = 0; x < XCount; x++) {
            for (int y = 0; y < YCount; y++) {
                PointsMatrix[x, y] =
                    new TilePoint(x, y, new Vector2(tileSize * x * ScaleK - xShift, tileSize * y * ScaleK - yShift), orderedColors[counter]);
                counter++;
            }
        }
    }

    private void SelectColors()
    {
        orderedColors.Clear();
        List<int> selectedColorIndexes = new List<int>();
        List<Color> pickFrom = colorsDepo.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < Settings.CurrentColors; i++) {
            selectedColorIndexes.Add(colorsDepo.IndexOf(pickFrom[i]));
        }
        int singleColorCount = Mathf.CeilToInt(XCount * YCount / (float)Settings.CurrentColors);
        for (int i = 0; i < selectedColorIndexes.Count; i++) {
            for (int c = 0; c < singleColorCount; c++) {
                orderedColors.Add(selectedColorIndexes[i]);
            }
        }
        orderedColors = orderedColors.OrderBy(x => Random.value).ToList();
    }

    private void PlaceTiles()
    {
        foreach (var point in PointsMatrix) {
            if (pooledTiles.Count > 0) {
                placedTile = pooledTiles.Pop();
                placedTile.Visibility(true);
            }
            else placedTile = Instantiate(tilePrefab, field.transform).GetComponent<Tile>();

            PointsMatrix[point.x, point.y].relatedTile = placedTile;
            placedTile.PlaceTile(point, true, ScaleK);
            PlacedTiles.Add(placedTile);
        }
    }

    public void PoolField()
    {
        foreach (var tile in PlacedTiles) {
            pooledTiles.Push(tile);
            tile.Visibility(false);
        }
        PlacedTiles.Clear();
    }
}
