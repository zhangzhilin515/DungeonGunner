using UnityEngine;
[DisallowMultipleComponent]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(Rigidbody2D))]
public class MovementToPosition : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private MovementToPositionEvent movementToPositionEvent;
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }
    private void OnEnable()
    {
        movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }
    private void OnDisable()
    {
        movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }
    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent,MovementToPositionArgs movementToPositionArgs)
    {
        MoveRigidBody(movementToPositionArgs.movePosition, movementToPositionArgs.currentPosition, movementToPositionArgs.moveSpeed);
    }
    private void MoveRigidBody(Vector3 movePosition,Vector3 currentPosition,float moveSpeed)
    {
        Vector2 unitVector = Vector3.Normalize(movePosition - currentPosition);
        rigidBody.MovePosition(rigidBody.position + (unitVector * moveSpeed * Time.fixedDeltaTime));
    }
}
