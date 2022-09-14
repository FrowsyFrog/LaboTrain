using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : EntityMovement
{
    [SerializeField] private float _enemyRadius;
    [SerializeField] private Vector3 _targetOffset;

    private List<Vector3> _pathVectorList = null;
    private int _currentPathIndex;

    private void Update()
    {
        if (!_canMove) return;
        HandleMovement();
    }

    private void HandleMovement()
    {
        if(_pathVectorList != null)
        {
            Vector3 TargetPosition = _pathVectorList[_currentPathIndex] + _targetOffset;
            if(Vector3.Distance(transform.position, TargetPosition) > _enemyRadius)
            {
                Vector3 MoveDir = (TargetPosition - transform.position).normalized;

                _animator.SetBool("isWalking", true);
                if (MoveDir.x != 0) LookLeft(MoveDir.x < 0);

                transform.position = transform.position + MoveDir * _moveSpeed * Time.deltaTime;
            }
            else
            {
                _currentPathIndex++;
                if(_currentPathIndex >= _pathVectorList.Count)
                {
                    _pathVectorList = null;
                    _animator.SetBool("isWalking", false);
                    //End path
                    GameManager.Instance.EndWinEnemy();
                }
            }
        }
        else
        {
            _animator.SetBool("isWalking", false);
        }
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        _currentPathIndex = 0;
        _pathVectorList = PathFinding.Instance.FindPath(transform.position, targetPosition);
        if(_pathVectorList != null && _pathVectorList.Count > 1)
        {
            _pathVectorList.RemoveAt(0);
        }
    }

    public override void KillEntity()
    {
        base.KillEntity();
        _pathVectorList = null;
    }

    public override void ReviveEntity(bool leftOrientation, Vector2 position)
    {
        base.ReviveEntity(leftOrientation, position);
        _animator.SetBool("isWalking", false);

    }
}
