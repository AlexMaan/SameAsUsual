using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject view;
    [SerializeField] private GameObject selectMatte;
    [SerializeField] private AnimationCurve fallCurve, shiftCurve;
    [SerializeField] private float fallTime, shiftTime;
    private AnimationCurve pickedCurve;

    public TilePoint RelatedPoint { get; private set; }

    private void OnEnable() => SetSelected(false);

    private void OnMouseDown() => FieldInput._FieldInput.CheckTile(this);

    public void PlaceTile(TilePoint point, bool sized = true, float sizeK = 1)
    {
        RelatedPoint = point;
        transform.position = point.position;
        if (sized) transform.localScale = Vector3.one * sizeK;
        spriteRenderer.color = FieldGenerator._FieldGenerator.ColorsDepo[point.color];
    }

    public void Visibility(bool show) => gameObject.SetActive(show);

    public void SetSelected(bool isSelect) => selectMatte.SetActive(isSelect);

    public void MoveTile(TilePoint target, TilePoint orig, int direction, int distance)
    {
        PlaceTile(target, false);
        StartCoroutine(Moving(target.position, orig.position, direction, distance));
    }

    private IEnumerator Moving(Vector2 targetPos, Vector2 origPos, int direction, int distance)
    {
        float timeStamp = 0;
        pickedCurve = direction == 0 ? fallCurve : shiftCurve;
        float time = direction == 0 ? fallTime : shiftTime;
        while (timeStamp < time * distance) {
            view.transform.position =
                Vector2.Lerp(origPos, targetPos, pickedCurve.Evaluate(Mathf.Min(1, timeStamp / (time * distance))));
            timeStamp += Time.deltaTime;
            yield return null;
        }
        view.transform.localPosition = Vector3.zero;
    }
}
