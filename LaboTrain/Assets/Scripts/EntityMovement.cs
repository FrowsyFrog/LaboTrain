using UnityEngine;

// Clase padre de la que hereda el movimiento del enemigo y jugador
public class EntityMovement : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected SpriteRenderer _entityRenderer;
    [SerializeField] protected float _moveSpeed;

    protected bool _canMove;
    protected bool _isDead;
    public bool CanMove { set => _canMove = value; }
    public float MoveSpeed { set => _moveSpeed = value; }

    // Reflejar sprite de la entidad hacia la izquierda
    public void LookLeft(bool left)
    {
        _entityRenderer.flipX = left;
    }
    public virtual void KillEntity()
    {
        _canMove = false;
        _isDead = true;
        _animator.SetBool("kill", true);
    }

    public virtual void ReviveEntity(bool leftOrientation, Vector2 position)
    {
        _animator.SetBool("kill", false);
        _isDead = false;
        transform.position = position;
        LookLeft(leftOrientation);
    }
}
