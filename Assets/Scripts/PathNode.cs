public class PathNode
{
    private int _x;
    private int _y;
    private int _gCost; // Costo de llegada desde el nodo inicial
    private int _hCost; // Coste heurístico para alcanzar el nodo final
    private int _fCost; // Suma del gCost y hCost. 

    private bool _isWalkable;

    private PathNode _parent;

    public int X { get => _x; }
    public int Y { get => _y; }
    public int GCost { get => _gCost; set => _gCost = value; }
    public int HCost { get => _hCost; set => _hCost = value; }
    public int FCost { get => _fCost;}
    public bool IsWalkable { get => _isWalkable; set => _isWalkable = value; }
    public PathNode Parent { get => _parent; set => _parent = value; }

    public PathNode(int x, int y)
    {
        _x = x;
        _y = y;
        _isWalkable = false;
    }

    public void CalculateFCost()
    {
        _fCost = _gCost + _hCost;
    }
}
