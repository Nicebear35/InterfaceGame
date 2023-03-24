using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    [SerializeField] private int _columns = 0;
    [SerializeField] private int _rows = 0;
    [SerializeField] private float _squareGap = 0.1f;
    [SerializeField] private GameObject _gridSquare;
    [SerializeField] private Vector2 _startPosition = new Vector2(0f, 0f);
    [SerializeField] private float _squareScale = 0.5f;
    [SerializeField] private float _everySquareOffset = 0f;

    public ShapeStorage ShapeStorage;

    private Vector2 _offset = new Vector2(0f, 0f);
    private List<GameObject> _gridSquares = new List<GameObject>();
    private LineIndicator _lineIndicator;

    private void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        CreateGrid();
    }

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquaresPositions();
    }

    private void SpawnGridSquares()
    {
        int squareIndex = 0;

        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                _gridSquares.Add(Instantiate(_gridSquare));

                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SquareIndex = squareIndex;
                _gridSquares[_gridSquares.Count - 1].transform.SetParent(transform);
                _gridSquares[_gridSquares.Count - 1].transform.localScale = new Vector3(_squareScale, _squareScale, _squareScale);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SetImage(_lineIndicator.GetGridSquareIndex(squareIndex) % 2 == 0);
                squareIndex++;
            }
        }
    }

    private void SetGridSquaresPositions()
    {
        int columnNumber = 0;
        int rowNumber = 0;
        Vector2 squareGapNumber = new Vector2(0f, 0f);
        bool isRowMoved = false;

        var squareRectangular = _gridSquares[0].GetComponent<RectTransform>();

        _offset.x = squareRectangular.rect.width * squareRectangular.transform.localScale.x + _everySquareOffset;
        _offset.y = squareRectangular.rect.height * squareRectangular.transform.localScale.y + _everySquareOffset;

        foreach (var gridSquare in _gridSquares)
        {
            if (columnNumber + 1 > _columns)
            {
                squareGapNumber.x = 0;
                columnNumber = 0;
                rowNumber++;
                isRowMoved = false;
            }

            var positionXOffset = _offset.x * columnNumber + (squareGapNumber.x * _squareGap);
            var positionYOffset = _offset.y * rowNumber + (squareGapNumber.y * _squareGap);

            if (columnNumber > 0 && columnNumber % 3 == 0)
            {
                squareGapNumber.x++;
                positionXOffset += _squareGap;
            }

            if (rowNumber > 0 && rowNumber % 3 == 0 && isRowMoved == false)
            {
                isRowMoved = true;
                squareGapNumber.y++;
                positionYOffset += _squareGap;
            }

            gridSquare.GetComponent<RectTransform>().anchoredPosition = new Vector2(_startPosition.x + positionXOffset, _startPosition.y - positionYOffset);
            gridSquare.GetComponent<RectTransform>().localPosition = new Vector3(_startPosition.x + positionXOffset, _startPosition.y - positionYOffset, 0f);

            columnNumber++;
        }
    }

    private void CheckIfShapeCanBePlaced()
    {
        var squareIndexes = new List<int>();

        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();

            if (gridSquare.IsSelected && gridSquare.IsSquareOccupied == false)
            {
                squareIndexes.Add(gridSquare.SquareIndex);
                gridSquare.IsSelected = false;
                //gridSquare.ActivateSquare();
            }
        }

        var currentSelectedShape = ShapeStorage.GetCurrentSelectedShape();

        if (currentSelectedShape == null)
        {
            return;
        }

        if (currentSelectedShape.TotalSquareNumber == squareIndexes.Count)
        {
            foreach (var squareIndex in squareIndexes)
            {
                _gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard();
            }

            var shapeLeft = 0;

            foreach (var shape in ShapeStorage.ShapeList)
            {
                if (shape.IsOnStartPosition() && shape.IsAnyShapeSquareActive())
                {
                    shapeLeft++;
                }
            }

            if (shapeLeft == 0)
            {
                GameEvents.RequestNewShapes();
            }
            else
            {
                GameEvents.SetShapeInactive();
            }

            CheckIfAnyLineIsCompleted();
        }
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }
    }

    private void CheckIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();

        foreach (var column in _lineIndicator.ColumnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        for (int row = 0; row < 9; row++)
        {
            List<int> data = new List<int>(9);

            for (int index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.LineData[row, index]);
            }

            lines.Add(data.ToArray());
        }

        for (int square = 0; square < 9; square++)
        {
            List<int> data = new List<int>(9);

            for (int index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.SquareData[square, index]);
            }

            lines.Add(data.ToArray());
        }

        var completedLines = CheckIfSquaresCompleted(lines);

        if (completedLines > 2)
        {
            //TODO: Play Animaton Bonus.
        }

        var totalScores = 10 * completedLines;
        GameEvents.AddScores(totalScores);

        CheckIfPlayerLost();
    }

    private int CheckIfSquaresCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();

        var linesCompleted = 0;

        foreach (var line in data)
        {
            var lineCompleted = true;

            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();

                if (comp.IsSquareOccupied == false)
                {
                    lineCompleted = false;
                }
            }

            if (lineCompleted)
            {
                completedLines.Add(line);
            }
        }

        foreach (var line in completedLines)
        {
            var completed = false;

            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();

                comp.DeactivateSquare();
                completed = true;
            }

            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }

            if (completed)
            {
                linesCompleted++;
            }
        }

        return linesCompleted;
    }

    private void CheckIfPlayerLost()
    {
        var validShapes = 0;

        for (int index = 0; index < ShapeStorage.ShapeList.Count; index++)
        {
            var isShapeActive = ShapeStorage.ShapeList[index].IsAnyShapeSquareActive();

            if (CheckIfShapeCanBePlacedOnGrid(ShapeStorage.ShapeList[index]) && isShapeActive)
            {
                ShapeStorage.ShapeList[index]?.ActivateShape();
                validShapes++;
            }
        }

        if(validShapes == 0)
        {
            //Game Over
            GameEvents.GameOver(false);
            //Debug.Log("GAME OVER!!!!!!");
        }
    }

    private bool CheckIfShapeCanBePlacedOnGrid(Shape currentShape)
    {
        var currentShapeData = currentShape.CurrentShapeData;
        var shapeColumns = currentShapeData.Columns;
        var shapeRows = currentShapeData.Rows;

        //All indexes of field up squares

        List<int> originalShapeFilledUpSquares = new List<int>();
        var squareIndex = 0;

        for (int rowIndex = 0; rowIndex < shapeRows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < shapeColumns; columnIndex++)
            {
                if (currentShapeData.Board[rowIndex].Column[columnIndex])
                {
                    originalShapeFilledUpSquares.Add(squareIndex);
                }

                squareIndex++;
            }
        }

        if (currentShape.TotalSquareNumber != originalShapeFilledUpSquares.Count)
        {
            Debug.LogError("Number of filled up squares is not the same as original shape have");
        }

        var squareList = GetAllSquaresCombination(shapeColumns, shapeRows);

        bool canBePlaced = false;

        foreach (var number in squareList)
        {
            bool shapeCanBePlacedOnBoard = true;

            foreach (var squareIndexToCheck in originalShapeFilledUpSquares)
            {
                var comp = _gridSquares[number[squareIndexToCheck]].GetComponent<GridSquare>();

                if (comp.IsSquareOccupied)
                {
                    shapeCanBePlacedOnBoard = false;
                }
            }

            if (shapeCanBePlacedOnBoard)
            {
                canBePlaced = true;
            }
        }

        return canBePlaced;
    }

    private List<int[]> GetAllSquaresCombination(int columns, int rows)
    {
        var squareList = new List<int[]>();
        var lastColumnIndex = 0;
        var lastRowIndex = 0;

        int safeIndex = 0;

        while (lastRowIndex + (rows - 1) < 9)
        {
            var rowData = new List<int>();

            for (int row = lastRowIndex; row < lastRowIndex + rows; row++)
            {
                for (int column = lastColumnIndex; column < lastColumnIndex + columns; column++)
                {
                    rowData.Add(_lineIndicator.LineData[row, column]);
                }
            }

            squareList.Add(rowData.ToArray());

            lastColumnIndex++;

            if (lastColumnIndex + (columns - 1) >= 9)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }

            safeIndex++;

            if(safeIndex > 100)
            {
                break;
            }
        }

        return squareList;
    }
}
