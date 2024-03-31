using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementEvent))]
public class Movement : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private MovementEvent movementEvent;
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        movementEvent = GetComponent<MovementEvent>();
    }
    private void OnEnable()
    {
        movementEvent.OnMovement += MovementEvent_OnMovement;
    }
    private void OnDisable()
    {
        movementEvent.OnMovement -= MovementEvent_OnMovement;
    }

    private void MovementEvent_OnMovement(MovementEvent movementEvent,MovementArgs movementArgs)
    {
        MoveRigidBody(movementArgs.moveDirection, movementArgs.moveSpeed);
    }
    private void MoveRigidBody(Vector2 moveDirection,float moveSpeed)
    {
        rigidBody.velocity = moveDirection * moveSpeed;
    }
}
