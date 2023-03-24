using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public GameObject SquareShapeImage;

    private Vector3 _shapeStartScale;
    private Vector2 _offset = new Vector2(0f, 800f);
    private RectTransform _transform;
    public ShapeData CurrentShapeData;
    private bool _isDragable;
    private bool _isShapeActive = true;

    private Canvas _canvas;
    private Vector3 _startPosition;

    private List<GameObject> _currentShape = new List<GameObject>();

    public Vector3 ShapeSelectedScale;
    public int TotalSquareNumber { get; set; }
    private void Awake()
    {
        _shapeStartScale = GetComponent<RectTransform>().localScale;
        _transform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _isDragable = true;
        _startPosition = transform.localPosition;
        _isShapeActive = true;
    }

    private void OnEnable()
    {
        GameEvents.MoveShapeToStartPosition += MoveShapeToStartPosition;
        GameEvents.SetShapeInactive += SetShapeInactive;
    }

    private void OnDisable()
    {
        GameEvents.MoveShapeToStartPosition -= MoveShapeToStartPosition;
        GameEvents.SetShapeInactive -= SetShapeInactive;
    }

    public bool IsOnStartPosition()
    {
        return transform.localPosition == _startPosition;
    }

    public bool IsAnyShapeSquareActive()
    {
        foreach (var square in _currentShape)
        {
            if (square.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    public void DeactivateShape()
    {
        if (_isShapeActive)
        {
            foreach (var square in _currentShape)
            {
                square?.GetComponent<ShapeSquare>().DeactivateShape();
            }
        }

        _isShapeActive = false;
    }

    private void SetShapeInactive()
    {
        if (IsOnStartPosition() == false && IsAnyShapeSquareActive())
        {
            foreach (var square in _currentShape)
            {
                square.gameObject.SetActive(false);
            }
        }
    }

    public void ActivateShape()
    {
        if (!_isShapeActive)
        {
            foreach (var square in _currentShape)
            {
                square?.GetComponent<ShapeSquare>().ActivateShape();

            }
        }

        _isShapeActive = true;
    }

    public void RequestNewShape(ShapeData shapeData)
    {
        transform.localPosition = _startPosition;
        CreateShape(shapeData);
    }

    public void CreateShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        TotalSquareNumber = GetNumberOfSquares(shapeData);

        while (_currentShape.Count <= TotalSquareNumber)
        {
            _currentShape.Add(Instantiate(SquareShapeImage, transform));
        }

        foreach (var square in _currentShape)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRectangular = SquareShapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRectangular.rect.width * squareRectangular.localScale.x, squareRectangular.rect.height * squareRectangular.localScale.y);

        int currentIndexInList = 0;

        for (int row = 0; row < shapeData.Rows; row++)
        {
            for (int column = 0; column < shapeData.Columns; column++)
            {
                if (shapeData.Board[row].Column[column])
                {
                    _currentShape[currentIndexInList].SetActive(true);
                    _currentShape[currentIndexInList].GetComponent<RectTransform>().localPosition =
                        new Vector2(GetXPositionForShapeSquare(shapeData, column, moveDistance),
                        GetYPositionForShapeSquare(shapeData, row, moveDistance));

                    currentIndexInList++;
                }
            }
        }
    }

    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        float shiftOnY = 0f;

        if (shapeData.Rows > 1)
        {
            if (shapeData.Rows % 2 != 0)
            {
                var middleSquareIndex = (shapeData.Rows - 1) / 2;
                var multiplier = (shapeData.Rows - 1) / 2;

                if (row < middleSquareIndex)
                {
                    shiftOnY = moveDistance.y * 1;
                    shiftOnY *= multiplier;
                }
                else if (row > middleSquareIndex)
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;
                }
            }
            else
            {
                var middleSquareIndex2 = (shapeData.Rows == 2) ? 1 : (shapeData.Rows / 2);
                var middleSquareIndex1 = (shapeData.Rows == 2) ? 0 : (shapeData.Rows - 2);
                var multiplier = (shapeData.Rows / 2);

                if (row == middleSquareIndex1 || row == middleSquareIndex2)
                {
                    if (row == middleSquareIndex2)
                    {
                        shiftOnY = (moveDistance.y / 2) * -1;
                    }

                    if (row == middleSquareIndex1)
                    {
                        shiftOnY = (moveDistance.y / 2);
                    }
                }

                if (row < middleSquareIndex1 && row < middleSquareIndex2)
                {
                    shiftOnY = moveDistance.y;
                    shiftOnY *= multiplier;
                }
                else if (row > middleSquareIndex1 && row > middleSquareIndex2)
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;
                }
            }
        }

        return shiftOnY;
    }

    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        float shiftOnX = 0f;

        if (shapeData.Columns > 1)
        {
            if (shapeData.Columns % 2 != 0)
            {
                var middleSquareIndex = (shapeData.Columns - 1) / 2;
                var multiplier = (shapeData.Columns - 1) / 2;

                if (column < middleSquareIndex)
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIndex)
                {
                    shiftOnX = moveDistance.x;
                    shiftOnX *= multiplier;
                }
            }
            else
            {
                var middleSquareIndex2 = (shapeData.Columns == 2) ? 1 : shapeData.Columns / 2;
                var middleSquareIndex1 = (shapeData.Columns == 2) ? 0 : shapeData.Columns - 1;
                var multiplier = shapeData.Columns / 2;

                if (column == middleSquareIndex1 || column == middleSquareIndex2)
                {
                    if (column == middleSquareIndex2)
                    {
                        shiftOnX = moveDistance.x / 2;
                    }
                    if (column == middleSquareIndex1)
                    {
                        shiftOnX = (moveDistance.x / 2) * -1;
                    }
                }

                if (column < middleSquareIndex1 && column < middleSquareIndex2)
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIndex1 && column > middleSquareIndex2)
                {
                    shiftOnX = moveDistance.x;
                    shiftOnX *= multiplier;
                }
            }
        }

        return shiftOnX;
    }
    private int GetNumberOfSquares(ShapeData shapeData)
    {
        int number = 0;

        foreach (var rowData in shapeData.Board)
        {
            foreach (var active in rowData.Column)
            {
                if (active)
                {
                    number++;
                }
            }
        }

        return number;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GetComponent<RectTransform>().localScale = _shapeStartScale;
        GameEvents.CheckIfShapeCanBePlaced();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchorMin = new Vector2(0f, 0f);
        _transform.anchorMax = new Vector2(0f, 0f);
        _transform.pivot = new Vector2(0f, 0f);

        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, eventData.position, Camera.main, out position);

        _transform.localPosition = position + _offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<RectTransform>().localScale = _shapeStartScale;
        GameEvents.CheckIfShapeCanBePlaced();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GetComponent<RectTransform>().localScale = ShapeSelectedScale;

        _transform.anchorMin = new Vector2(0f, 0f);
        _transform.anchorMax = new Vector2(0f, 0f);
        _transform.pivot = new Vector2(0f, 0f);

        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, eventData.position, Camera.main, out position);

        _transform.localPosition = position + _offset;
    }

    private void MoveShapeToStartPosition()
    {
        _transform.transform.localPosition = _startPosition;
    }

}