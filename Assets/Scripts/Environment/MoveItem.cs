using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MoveItem : MonoBehaviour
{
    [SerializeField] private SoundEffectSO moveSoundEffect;

    [HideInInspector] public BoxCollider2D boxCollider2D;
    private Rigidbody2D rigidBody2D;
    private InstantiatedRoom instantiatedRoom;
    private Vector3 previousPosition;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        instantiatedRoom = GetComponentInParent<InstantiatedRoom>();

        instantiatedRoom.moveableItemsList.Add(this);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        UpdateObstacles();
    }
    private void UpdateObstacles()
    {
        ConfineItemToRoomBounds();

        instantiatedRoom.UpdateMoveableObstacles();

        previousPosition = transform.position;

        if (Mathf.Abs(rigidBody2D.velocity.x) > 0.001f || Mathf.Abs(rigidBody2D.velocity.y) > 0.001f)
        {
            // Play moving sound every 10 frames
            if (moveSoundEffect != null && Time.frameCount % 10 == 0)
            {
                SoundEffectManager.Instance.PlaySoundEffect(moveSoundEffect);
            }
        }
    }
    private void ConfineItemToRoomBounds()
    {
        Bounds itemBounds = boxCollider2D.bounds;
        Bounds roomBounds = instantiatedRoom.roomColliderBounds;

        if (itemBounds.min.x <= roomBounds.min.x ||
            itemBounds.max.x >= roomBounds.max.x ||
            itemBounds.min.y <= roomBounds.min.y ||
            itemBounds.max.y >= roomBounds.max.y)
        {
            transform.position = previousPosition;
        }
    }
}