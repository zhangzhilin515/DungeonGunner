using System;
using UnityEngine;
[DisallowMultipleComponent]
public class MovementToPositionEvent : MonoBehaviour
{
    public event Action<MovementToPositionEvent, MovementToPositionArgs> OnMovementToPosition;
    public void CallMovementToPositionEvent(Vector3 movePosition,Vector3 currentPosition,Vector2 moveDirection,float moveSpeed,bool isRolling=false)
    {
        OnMovementToPosition?.Invoke(this, new MovementToPositionArgs()
        {
            movePosition = movePosition,
            currentPosition = currentPosition,
            moveDirection = moveDirection,
            moveSpeed = moveSpeed,
            isRolling = isRolling
        });
    }
}
public class MovementToPositionArgs : EventArgs
{
    public Vector3 movePosition;
    public Vector3 currentPosition;
    public Vector2 moveDirection;
    public float moveSpeed;
    public bool isRolling;
}
