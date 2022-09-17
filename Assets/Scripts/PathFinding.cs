using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    public static PathFinding Instance { get; private set; }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private Grid<PathNode> _grid;
    private List<PathNode> _openList; // Nodos en cola para revisar
    private List<PathNode> _closedList; // Nodos que ya han sido revisados

    public Grid<PathNode> Grid { get => _grid; }

    public PathFinding(int width, int height)
    {
        Instance = this;
        SetNewGrid(width, height);
    }

    public void SetNewGrid(int width, int height)
    {
        _grid = new Grid<PathNode>(width, height, 1f, Vector3.zero, (int x, int y) => new PathNode(x, y));
    }

    // Devuelve la lista de nodos que conformar el camino del inicio al final según el tamaño del escenario de juego
    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        _grid.GetXY(startWorldPosition, out int startX, out int startY);
        _grid.GetXY(endWorldPosition, out int endX, out int endY);
        List<PathNode> Path = FindPath(startX, startY, endX, endY);
        if (Path == null) return null;
        else
        {
            List<Vector3> VectorPath = new List<Vector3>();
            foreach(PathNode pathNode in Path)
            {
                VectorPath.Add(new Vector3(pathNode.X, pathNode.Y) * _grid.CellSize + Vector3.one * _grid.CellSize * .5f);
            }
            return VectorPath;
        }
    }

    // Devuelve la lista de nodos que conformar el camino del inicio al final en el grid
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode StartNode = _grid.GetValue(startX, startY);
        PathNode EndNode = _grid.GetValue(endX, endY);

        _openList = new List<PathNode>() { StartNode};
        _closedList = new List<PathNode>();

        // Establecer todos los nodos en el grid con valores por defecto
        for(int x = 0;  x < _grid.Width; ++x)
        {
            for(int y = 0; y < _grid.Height; ++y)
            {
                PathNode Node = _grid.GetValue(x, y);
                Node.GCost = int.MaxValue;
                Node.CalculateFCost();
                Node.Parent = null;
            }
        }

        // Establecer valor del primer nodo
        StartNode.GCost = 0;
        StartNode.HCost = CalculateDistanceCost(StartNode, EndNode);
        StartNode.CalculateFCost();

        // Mientras hayan nodos a revisar
        while(_openList.Count > 0)
        {
            PathNode CurrentNode = GetLowestFCostNode(_openList);
            // Si el nodo actual es el alcanzado, retornar el camino hasta llegar
            if(CurrentNode == EndNode) return CalculatePath(EndNode);

            // Cambiar el nodo de lista al ya ser revisado
            _openList.Remove(CurrentNode);
            _closedList.Add(CurrentNode);

            //Revisar todos los nodos vecinos
            foreach(PathNode neighbourNode in GetNeighbourList(CurrentNode)){
                // Revisar si el nodo vecino se encuentra en la closedList, lo que significa que ya la hemos revisado
                if (_closedList.Contains(neighbourNode)) continue;
                // Si el nodo no es un camino, añadirlo a la closedList y pasar al siguiente vecino
                if (!neighbourNode.IsWalkable)
                {
                    _closedList.Add(neighbourNode);
                    continue;
                }

                // Si el costo del nodo actual de llegada (G Cost) es menor al que actualmente tiene el vecino
                // Ese será el siguiente nodo
                int tentativeGCost = CurrentNode.GCost + CalculateDistanceCost(CurrentNode, neighbourNode);
                if(tentativeGCost < neighbourNode.GCost)
                {
                    neighbourNode.Parent = CurrentNode; // Se coloca de padre al actual para saber de cual viene
                    neighbourNode.GCost = tentativeGCost;
                    neighbourNode.HCost = CalculateDistanceCost(neighbourNode, EndNode);
                    neighbourNode.CalculateFCost();

                    if(!_openList.Contains(neighbourNode)) _openList.Add(neighbourNode);
                }
            }

        }
        // Sin nodos en la openList, por lo que no logramos encontrar el camino
        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> NeighbourList = new List<PathNode>();

        //Izquierda
        if (currentNode.X - 1 >= 0) NeighbourList.Add(GetNode(currentNode.X - 1, currentNode.Y));
        //Derecha
        if (currentNode.X + 1 < _grid.Width) NeighbourList.Add(GetNode(currentNode.X + 1, currentNode.Y));
        //Abajo
        if (currentNode.Y - 1 >= 0) NeighbourList.Add(GetNode(currentNode.X, currentNode.Y - 1));
        //Arriba
        if(currentNode.Y + 1 < _grid.Height) NeighbourList.Add(GetNode(currentNode.X, currentNode.Y + 1));

        return NeighbourList;
    }

    private PathNode GetNode(int x, int y)
    {
        return _grid.GetValue(x, y);
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> Path = new List<PathNode>();
        Path.Add(endNode);
        PathNode CurrentNode = endNode;
        while(CurrentNode.Parent != null)
        {
            Path.Add(CurrentNode.Parent);
            CurrentNode = CurrentNode.Parent;
        }
        Path.Reverse();
        return Path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int XDistance = Mathf.Abs(a.X - b.X);
        int YDistance = Mathf.Abs(a.Y - b.Y);
        int RemainingDistance = Mathf.Abs(XDistance - YDistance);
        //Retornamos el valor a recorrer de manera diagonal más el de recorrer recto
        return MOVE_DIAGONAL_COST * Mathf.Min(XDistance, YDistance) + MOVE_STRAIGHT_COST * RemainingDistance;
    }

    // Obtener nodo de menor F Cost
    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode LowestFCostNode = pathNodeList[0];
        for(int i = 1; i < pathNodeList.Count; ++i)
        {
            if(pathNodeList[i].FCost < LowestFCostNode.FCost)
                LowestFCostNode = pathNodeList[i];
        }
        return LowestFCostNode;
    }
}
