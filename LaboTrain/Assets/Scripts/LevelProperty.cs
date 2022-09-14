using UnityEngine;

public partial class GameManager
{
    [System.Serializable]
    private class LevelProperty
    {
        [SerializeField] private float _slimeSpeed;
        [SerializeField] private Transform _decorContenetor;

        [Header("Camera Properties")]
        [SerializeField] private Vector3 _camPosition;
        [SerializeField] private float _camSize;

        [Header("Grid")]
        [SerializeField] private int _mazeWidth; 
        [SerializeField] private int _mazeHeight; 

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
