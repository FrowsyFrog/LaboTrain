using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Panels")]
    [SerializeField] private Animator _nextLevelPanel;
    [SerializeField] private Animator _enemyEndPanel;
    [SerializeField] private Animator _playerEndPanel;
    [SerializeField] private Animator _menuAnim;

    [Header("Entities")]
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private EnemyMovement _enemyHandler;
    [SerializeField] private Transform _chestContenetor;

    [Header("Texts")]
    [SerializeField] private TMPro.TextMeshProUGUI _curLevel;
    [SerializeField] private TMPro.TextMeshProUGUI _shadowCurLevel;
    [SerializeField] private TMPro.TextMeshProUGUI _levelCompleted;
    [SerializeField] private TMPro.TextMeshProUGUI _shadowLevelCompleted;
    [SerializeField] private TMPro.TextMeshProUGUI _screenMode;
    [SerializeField] private TMPro.TextMeshProUGUI _shadowScreenMode;

    [Header("References")]
    [SerializeField] private GameObject _localResetLevel;
    [SerializeField] private MouseController _mouseController;

    [Header("Properties")]
    [SerializeField] private float _dissapearRate;
    [SerializeField] private float _timeToEditCamera;
    [SerializeField] private List<LevelProperty> _levelProperties;

    private Camera _camera;
    private GridManager _gridManager;
    private AudioManager _audioManager;
    private Animator _chestAnim;
    private bool _inGame = false;
    private bool _levelEnded = false;
    private bool _editingCamera = false;
    private bool _reshowingObjects = false;
    private bool _isFullscreen;
    private int _currentLevel = 0;
    private Resolution _windowedResolution;
    private Resolution _fullscreenResolution;

    private void Awake()
    {
        Instance = this;
    }

    // Colocar la configuración inicial del juego. Además obtiene referencias para diferentes variables
    void Start()
    {
        Application.targetFrameRate = 60;
        _camera = Camera.main;
        _gridManager = GetComponentInChildren<GridManager>();
        _audioManager = GetComponent<AudioManager>();
        _chestAnim = _chestContenetor.GetComponentInChildren<Animator>();
        _isFullscreen = Screen.fullScreen;

        Resolution[] Resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        _windowedResolution = Resolutions[Resolutions.Length - 2];
        _fullscreenResolution = Resolutions[Resolutions.Length - 1];
        SetScreen();
    }

    // Intercambiar resolución entre pantalla completa y modo ventana
    public void TriggerWindows()
    {
        _isFullscreen = !_isFullscreen;
        SetScreen();
    }

    void SetScreen()
    {
        Resolution NewResolution = _isFullscreen ? _fullscreenResolution : _windowedResolution;
        Screen.SetResolution(NewResolution.width, NewResolution.height, _isFullscreen);
        _screenMode.text = _shadowScreenMode.text = _isFullscreen ? "Pantalla\nCompleta" : "Modo\nVentana";
    }

    // Una vez el jugador colisiona con el cofre, esta función es llamada
    public void EndWinPlayer()
    {
        if (_levelEnded) return;
        _levelEnded = true;
        _localResetLevel.gameObject.SetActive(false);
        _mouseController.MouseHover(false);
        _levelCompleted.text = _shadowLevelCompleted.text =
            $"NIVEL {_currentLevel}\n" +
            $"COMPLETADO!";

        _player.CanMove = false;
        _chestAnim.SetBool("open", true);
        _enemyHandler.KillEntity();

        if(_currentLevel < _levelProperties.Count)
        {
            _nextLevelPanel.gameObject.SetActive(true);
            _nextLevelPanel.SetBool("hide", false);
            _audioManager.PlayLevelWin();
        }
        else
        {
            _playerEndPanel.gameObject.SetActive(true);
            _playerEndPanel.SetBool("hide", false);
            _audioManager.PlayGameEnd();
        }

    }

    // Una vez el enemigo haya llegado a su último nodo, esta función es llamdada, indicando que el jugador ha perdido
    public void EndWinEnemy()
    {
        if (_levelEnded) return;
        _localResetLevel.gameObject.SetActive(false);
        _levelEnded = true;
        _enemyEndPanel.gameObject.SetActive(true);
        _enemyEndPanel.SetBool("hide", false);
        _enemyHandler.CanMove = false;
        _player.KillEntity();
        _audioManager.PlayLevelLose();
    }

    // Se llama al momento que se presiona el botón de ir al menú
    public void ResetGoMenu()
    {
        if (_reshowingObjects) return;
        _localResetLevel.gameObject.SetActive(false);
        _enemyEndPanel.SetBool("hide", true);
        _playerEndPanel.SetBool("hide", true);

        _inGame = false;
        _menuAnim.SetBool("hide", false);
        _player.gameObject.SetActive(false);
        _chestContenetor.gameObject.SetActive(false);
        _enemyHandler.gameObject.SetActive(false);
        _shadowCurLevel.gameObject.SetActive(false);
        _gridManager.EraseMap();
        StartCoroutine(IShowDecorObjects(_currentLevel - 1));
        _currentLevel = 0;
    }

    // Activa diferentes objetos para el inicio del juego
    public void StartGame()
    {
        if (_inGame) return;
        _inGame = true;
        _menuAnim.SetBool("hide", true);
        _player.gameObject.SetActive(true);
        _enemyHandler.gameObject.SetActive(true);
        _chestContenetor.gameObject.SetActive(true);
        _player.CanMove = _enemyHandler.CanMove = false;
        _shadowCurLevel.gameObject.SetActive(true);
        NextLevel();
    }

    public void ResetLevel()
    {
        _player.CanMove = _enemyHandler.CanMove = false;
        _enemyEndPanel.SetBool("hide", true);
        _currentLevel--;
        _localResetLevel.gameObject.SetActive(false);
        NextLevel();
    }

    public void StartLevel()
    {
        _player.CanMove = _enemyHandler.CanMove = true;
        _localResetLevel.SetActive(true);
        _player.EnablePlayerCollider(true);
    }

    // Función llamada al empezar el juego o al resetar un nivel
    public void NextLevel()
    {
        if (_editingCamera) return;

        // Si es que existe un camino, significa que no es la primera vez que el jugador ha creado un mapa,
        // por lo que debe borrarse el antiguo mapa y resetear diferentes variables
        if (_gridManager.ExistsPath) {
            _chestAnim.SetBool("open", false);
            _nextLevelPanel.SetBool("hide", true);;
            if(!_gridManager.IsErasing) _gridManager.EraseMap();
            _levelEnded = false;
        }

        //Cambiar las propiedades de la cámara y la decoración del mundo
        if(_levelProperties[_currentLevel].DecorContenetor != null)
        {
            StartCoroutine(IHideDecorObjects(_currentLevel));
            StartCoroutine(IEditCamera());
        }

        _levelProperties[_currentLevel].GetMazeSizes(out int Width, out int Height);

        // Generar y pintar el mapa en la escena
        _gridManager.GenerateMap(Width, Height);
        _gridManager.PaintMap();

        // Colocar el jugador y enemigo en sus respectivas posiciones. Además, si estaban muertos, reviven.
        _player.ReviveEntity(false, new Vector2(1, Height - 2.5f));
        _enemyHandler.ReviveEntity(true, new Vector2(Width - 2, 1));

        _chestContenetor.position = new Vector2(Width - 1, 1);

        _enemyHandler.MoveSpeed = _levelProperties[_currentLevel].SlimeSpeed;
        _enemyHandler.SetTargetPosition(new Vector2(0, Height - 1));

        _currentLevel++;

        _curLevel.text = _shadowCurLevel.text = $"Nivel: {_currentLevel} - {_levelProperties.Count}";
    }

    // Corrutina para ocultar progresivamente objetos que están en el espacio del laberinto
    IEnumerator IHideDecorObjects(int decorIndex)
    {
        foreach(Transform decoration in _levelProperties[decorIndex].DecorContenetor)
        {
            decoration.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(_dissapearRate);
        }
        _levelProperties[decorIndex].DecorContenetor.gameObject.SetActive(false);
    }

    // Corrutina para mostrar progresivamente objetos que fueron ocultados
    IEnumerator IShowDecorObjects(int decorIndex)
    {
        _reshowingObjects = true;
        int objectCounter = 0;
        while(decorIndex >= 0)
        {
            _levelProperties[decorIndex].DecorContenetor.gameObject.SetActive(true);
            foreach (Transform decoration in _levelProperties[decorIndex].DecorContenetor)
            {
                decoration.gameObject.SetActive(true);
                if(++objectCounter % 3 == 0) yield return new WaitForSecondsRealtime(_dissapearRate);
            }
            decorIndex--;
        }
        _reshowingObjects = false;
    }

    // Corrutina para editar el zoom y posición de la cámara tras el paso de niveles
    IEnumerator IEditCamera()
    {
        _editingCamera = true;
        Vector3 PrevPosition = _camera.transform.position;
        Vector3 TargetPosition = _levelProperties[_currentLevel].CamPosition;

        if(TargetPosition != PrevPosition)
        {
            float PrevSize = _camera.orthographicSize;
            float TargetSize = _levelProperties[_currentLevel].CamSize;
            float T = 0;
            while (T < _timeToEditCamera)
            {
                _camera.transform.position = Vector3.Lerp(PrevPosition, TargetPosition, T / _timeToEditCamera);
                _camera.orthographicSize = Mathf.Lerp(PrevSize, TargetSize, T / _timeToEditCamera);
                T += Time.deltaTime;
                yield return null;
            }
            _camera.transform.position = TargetPosition;
            _camera.orthographicSize = TargetSize;
        }
        _editingCamera = false;
    }
}
