using UnityEngine;

public class GridManager : MonoBehaviour
{
    private PathFinding _pathFinding;
    MazeGenerator _mazeGenerator;

    public bool ExistsPath { get => _pathFinding != null; }
    public bool IsErasing { get => _mazeGenerator.IsErasing; }

    private void Start()
    {
        _mazeGenerator = GetComponent<MazeGenerator>();
    }

    public void GenerateMap(int width, int height)
    {
        // Alto y ancho deben ser impares
        width += width % 2 == 0 ? 1 : 0;
        height += height % 2 == 0 ? 1 : 0;

        if(_pathFinding == null) _pathFinding = new PathFinding(width, height);
        else _pathFinding.SetNewGrid(width, height);

        _mazeGenerator.GenerateMaze(_pathFinding.Grid);
    }


    public void PaintMap()
    {
        StartCoroutine(_mazeGenerator.IPaintPath());
    }

    public void EraseMap()
    {
        StartCoroutine(_mazeGenerator.IErasePath());
    }
}
