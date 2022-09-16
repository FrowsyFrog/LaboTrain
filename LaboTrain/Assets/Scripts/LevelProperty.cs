using UnityEngine;

public partial class GameManager
{
    [System.Serializable]
    private class LevelProperty
    {
        [SerializeField] private float _slimeSpeed; // Velocidad del enemigo
        [SerializeField] private Transform _decorContenetor; // Contenedor de las decoraciones

        [Header("Camera Properties")]
        [SerializeField] private Vector3 _camPosition; // Posición de la cámar
        [SerializeField] private float _camSize; // Zoom de la cámara

        [Header("Grid")]
        [SerializeField] private int _mazeWidth; // Ancho del laberinto
        [SerializeField] private int _mazeHeight; // Alto del laberinto

        public float SlimeSpeed { get => _slimeSpeed; }

        public Transform DecorContenetor { get => _decorContenetor; }

        public void GetMazeSizes(out int width, out int height)
        {
            width = _mazeWidth;
            height = _mazeHeight;
        }

        public Vector3 CamPosition { get => _camPosition; }
        public float CamSize { get=> _camSize; }
    }
}
