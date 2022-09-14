using UnityEngine;

public class PlayerMovement : EntityMovement
{
    private Rigidbody2D _rb;
    private CircleCollider2D _circleCollider;
    private Vector2 _movement;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (_isDead) return;
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");
        _movement = _movement.normalized;

        if (_movement.magnitude > 0)
        {
            _animator.SetBool("isWalking", true);
            if(_movement.x != 0) LookLeft(_movement.x < 0);
        }
        else
        {
            _animator.SetBool("isWalking", false);
        }
    }

    private void FixedUpdate()
    {
        if (!_canMove) return;
        _rb.MovePosition(_rb.position + _movement * _moveSpeed * Time.fixedDeltaTime);
    }

    public override void ReviveEntity(bool leftOrientation, Vector2 position)
    {
        base.ReviveEntity(leftOrientation, position);
        EnablePlayerCollider(false);
    }

    public void EnablePlayerCollider(bool enable)
    {
        _circleCollider.enabled = enable;
    }
}
