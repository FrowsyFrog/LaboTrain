using System;
using UnityEngine;

public class Grid<T>
{
    private int _width;
    private int _height;
    private float _cellSize;
    private Vector3 _originPosition;
    private T[,] _gridArray;

    public float CellSize { get => _cellSize; }
    public int Width { get => _width; }
    public int Height { get => _height; }

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<int, int, T>  createGridObject)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _originPosition = originPosition;

        _gridArray = new T[width,height];

        for (int x = 0; x < _gridArray.GetLength(0); ++x)
        {
            for (int y = 0; y < _gridArray.GetLength(1); ++y)
            {
                _gridArray[x, y] = createGridObject(x, y);
            }
        }

    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
        y = Mathf.FloorToInt((worldPosition - _originPosition).y / _cellSize);
    }

    public T GetValue(int x, int y)
    {
        if (x < 0 || y < 0 || x >= _width || y >= _height) {
            return default(T);
        }
        return _gridArray[x, y];
    }
}
