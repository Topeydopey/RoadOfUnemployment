using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed = 5f;
    public Rigidbody2D body;

    [Header("Input")]
    public InputActionReference move;      // “Move” action (Vector 2)

    [Header("Visuals")]
    public Animator anim;                  // Player Animator
    public SpriteRenderer sr;              // Player SpriteRenderer (for flip)

    /* ─────────────────────── private ─────────────────────── */
    Vector2 _moveDir;
    float _lastFacingX = 1f;            // remember last non-zero X (1 = right, -1 = left)

    /* ─────────────────────── lifecycle ───────────────────── */
    void OnEnable() => move.action.Enable();
    void OnDisable() => move.action.Disable();

    void Update()
    {
        /* -- read input -- */
        _moveDir = move.action.ReadValue<Vector2>();

        /* -- if NOT in Hurt, drive Speed param -- */
        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Hurt"))
        {
            anim.SetFloat("Speed", _moveDir.sqrMagnitude);
        }

        /* -- flip sprite -- */
        if (Mathf.Abs(_moveDir.x) > 0.01f)
            _lastFacingX = Mathf.Sign(_moveDir.x);

        sr.flipX = _lastFacingX < 0f;

        /* -- move rigidbody -- */
        body.linearVelocity = _moveDir * MoveSpeed;
    }
}
