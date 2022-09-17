using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseController : MonoBehaviour
{
    [SerializeField] private List<Sprite> _mouseSprites;

    private Transform _mouseTransform;
    private Image _image;

    void Start()
    {
        Cursor.visible = false;
        _mouseTransform = transform;
        _image = GetComponent<Image>();
    }

    void Update()
    {
        _mouseTransform.position = Input.mousePosition;
    }

    // Cambiar sprite del mouse cuando está encima de un botón
    public void MouseHover(bool hover)
    {
        _image.sprite = _mouseSprites[hover ? 1 : 0];
    }
}
