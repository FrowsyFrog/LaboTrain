using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class MazeGenerator : MonoBehaviour
{
    // Separación entre los caminos en el laberinto.
    // Se puede entender como el ancho de las paredes
    [SerializeField] private int _separation; 

    [Header("Map Tile")]
    [SerializeField] private Tilemap _tileMap; // Tilemap para los sprites del suelo
    [SerializeField] private Tilemap _fenceTileMap; //Tilemap para los sprites de las paredes

    // Los RuleTiles son sprites que cambian su diseño según cual sprite está a su alrededor
    [SerializeField] private RuleTile _pathTile; // Sprites de camino
    [SerializeField] private RuleTile _fenceTile; // Sprites de paredes
    [SerializeField] private RuleTile _decorTile; // Sprites de decoraciones en el suelo
    [SerializeField] private RuleTile _randomObsTile; // Sprites de decoraciones en las paredes

    [Header("Probabilities")]
    [Tooltip("Cuanto mayor sea el porcentaje, más complicado será para tiles en el camino aparecer")]
    [SerializeField] private float _minProbToAppear;
    [SerializeField] private float _maxProbForDecorTile;
    [SerializeField] private float _maxProbForRandomObs;

    [Header("Sounds")]
    [SerializeField] private List<AudioClip> _putFenceClips; // Sonido al ser colocado una pared

    private AudioSource _audioSource;
    private Grid<PathNode> _grid;
    private Stack<PathNode> _stackNodes;
    private bool _isErasingPart = false;
    private bool _isErasing = false;

    private int _width { get => _grid.Width; }
    private int _height { get => _grid.Height; }

    public bool IsErasing { get => _isErasing; }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void GenerateMaze(Grid<PathNode> grid)
    {
        _grid = grid;
        _stackNodes = new Stack<PathNode>();
        PathNode StartNode = _grid.GetValue(1, 1);
        StartNode.IsWalkable = true;
        _stackNodes.Push(StartNode); // Se le otorga un valor inicial
        // Generar laberinto mediante backtracking
        GeneratePath(1);
        // Se elimina dos esquinas del laberinto para la entrada y salida
        DeleteCorners();
    }

    private void GeneratePath(int visitated)
    {
        if (visitated < _width / _separation * _height / _separation && _stackNodes.Count > 0)
        {
            PathNode CurNode = _stackNodes.Peek();
            List<int> Neighbours = new List<int>(4);

            // Verificar Norte
            if (CurNode.Y - _separation > 0 && !_grid.GetValue(CurNode.X, CurNode.Y - _separation).IsWalkable)
                Neighbours.Add(0);

            // Verificar Este
            if (CurNode.X + _separation < _width && !_grid.GetValue(CurNode.X + _separation, CurNode.Y).IsWalkable)
                Neighbours.Add(1);

            // Verificar Sur
            if (CurNode.Y + _separation < _height && !_grid.GetValue(CurNode.X, CurNode.Y + _separation).IsWalkable)
                Neighbours.Add(2);

            //Verificar Oeste
            if (CurNode.X - _separation > 0 && !_grid.GetValue(CurNode.X - _separation, CurNode.Y).IsWalkable)
                Neighbours.Add(3);

            if (Neighbours.Count > 0)
            {
                int RandomNode = Neighbours[Random.Range(0, Neighbours.Count)];

                switch (RandomNode)
                {
                    case 0://Norte
                        for (int i = 0; i < _separation + 1; ++i)
                        {
                            _grid.GetValue(CurNode.X, CurNode.Y - i).IsWalkable = true;
                        }
                        _stackNodes.Push(_grid.GetValue(CurNode.X, CurNode.Y - _separation));
                        break;
                    case 1://Este
                        for (int i = 0; i < _separation + 1; ++i)
                        {
                            _grid.GetValue(CurNode.X + i, CurNode.Y).IsWalkable = true;
                        }
                        _stackNodes.Push(_grid.GetValue(CurNode.X + _separation, CurNode.Y));
                        break;
                    case 2://Sur
                        for (int i = 0; i < _separation + 1; ++i)
                        {
                            _grid.GetValue(CurNode.X, CurNode.Y + i).IsWalkable = true;
                        }
                        _stackNodes.Push(_grid.GetValue(CurNode.X, CurNode.Y + _separation));
                        break;
                    case 3://Oeste
                        for (int i = 0; i < _separation + 1; ++i)
                        {
                            _grid.GetValue(CurNode.X - i, CurNode.Y).IsWalkable = true;
                        }
                        _stackNodes.Push(_grid.GetValue(CurNode.X - _separation, CurNode.Y));
                        break;
                }
                visitated++;
            }
            else
            {
                _stackNodes.Pop();
            }
            if (visitated < _width / _separation * _height / _separation)
                GeneratePath(visitated);
        }
    }
 
    private void DeleteCorners()
    {
        _grid.GetValue(0, _height - 1).IsWalkable = 
        _grid.GetValue(0, _height - 2).IsWalkable = 
        _grid.GetValue(1, _height - 1).IsWalkable =
        _grid.GetValue(_width - 1, 1).IsWalkable = true;
    }

    // Corrutina para pintar el laberinto
    public IEnumerator IPaintPath()
    {
        while (_isErasingPart) yield return null;
        int counter = 0;
        for (int x = 0; x < _width; ++x)
        {
            for (int y = 0; y < _height; ++y)
            {
                // Verificar si el nodo es un camino o es una pared
                if (_grid.GetValue(x, y).IsWalkable)
                {
                    // Aleatoriamente colocar ningún sprite, un camino, o una decoración
                    float RandomTile = Random.Range(0f, 100f);
                    if (RandomTile > _minProbToAppear)
                    {
                        if (RandomTile < _maxProbForDecorTile)
                        {
                            _tileMap.SetTile(new Vector3Int(x, y), _decorTile);
                        }
                        else
                        {
                            _tileMap.SetTile(new Vector3Int(x, y), _pathTile);
                        }
                    }
                    continue;
                }

                // Cada 4 paredes colocadas, que se reproduzca un sonido
                if(++counter % 4 == 0) _audioSource.PlayOneShot(_putFenceClips[Random.Range(0, _putFenceClips.Count)]);
                // Colocar sprite de pared 
                _fenceTileMap.SetTile(new Vector3Int(x, y), Random.Range(0f, 100f) > _maxProbForRandomObs ? _fenceTile : _randomObsTile);
                // Esperar 0.01 segundo para la siguiente iteración
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }
        // Una vez terminada la colocación del camino y pared, empezar juego
        GameManager.Instance.StartLevel();
    }

    // Corrutina para borrar el laberinto
    public IEnumerator IErasePath()
    {
        // Indica cuando la tercera parte del laberinto ha sido borrada.
        // Esto para la generación del siguiente no demore tanto
        _isErasingPart = true;
        // Indica cuando todo el laberinto ha sido borrado
        _isErasing = true;

        // Ancho y alto del laberinto actual por si se actualiza en medio de la corrutina
        int CurWidth = _width;
        int CurHeight = _height;

        for (int x = 0; x < CurWidth; ++x)
        {
            for (int y = 0; y < CurHeight; ++y)
            {
                _tileMap.SetTile(new Vector3Int(x, y), null);
                _fenceTileMap.SetTile(new Vector3Int(x, y), null);
                // Esperar 0.01 segundo para la siguiente iteración
                if (x % 3 == 0) yield return new WaitForSecondsRealtime(0.01f);
            }
            if (x > CurWidth / 3) _isErasingPart = false;
        }
        _isErasing = false;
    }
}
