using UnityEngine;

[CreateAssetMenu]
[System.Serializable]

public class ShapeData : ScriptableObject
{
    [System.Serializable]
    public class Row
    {
        [SerializeField] private bool[] column;
        [SerializeField] private int _size = 0;

        public bool[] Column => column; 

        public Row() { }

        public Row(int size)
        {
            CreateRow(size);
        }

        public void CreateRow(int size)
        {
            _size = size;
            column = new bool[_size];
            ClearRow();
        }

        public void ClearRow()
        {
            for (int i = 0; i < _size; i++)
            {
                column[i] = false;
            }
        }
    }

    public int Columns = 0;
    public int Rows = 0;
    public Row[] Board;

    public void Clear()
    {
        for (int i = 0; i < Rows; i++)
        {
            Board[i].ClearRow();
        }
    }

    public void CreateNewBoard()
    {
        Board = new Row[Rows];

        for (int i = 0; i < Rows; i++)
        {
            Board[i] = new Row(Columns);
        }
    }
}
