using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class VehicleRailRunner : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpVelocity = 7f;
    public float gravityScaleInAir = 3f;
    public float contactRadius = 0.12f;
    public float failY = -6f;
    public bool useFailYFallback = false;
    public LayerMask railLayer;
    public Transform railContactPoint;
    public bool inputEnabled = true;

    private Rigidbody2D rb;
    private bool grounded;
    private bool failed;

    public bool Grounded => grounded;
    public bool Failed => failed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        Time.timeScale = 1f;

        if (railContactPoint == null)
        {
            Debug.LogError($"{nameof(VehicleRailRunner)} needs a RailContactPoint assigned.", this);
        }
    }

    private void Update()
    {
        if (failed)
        {
            return;
        }

        if (inputEnabled && grounded && WasJumpPressed())
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (failed)
        {
            rb.gravityScale = gravityScaleInAir;
            return;
        }

        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        Collider2D rail = CheckRailContact();

        if (rail != null && rb.velocity.y <= 0.1f)
        {
            grounded = true;
            rb.gravityScale = 0f;
            SnapToRail(rail);
        }
        else
        {
            grounded = false;
            rb.gravityScale = gravityScaleInAir;
        }

        if (useFailYFallback && transform.position.y < failY)
        {
            FailAndFall();
        }
    }

    public void Jump()
    {
        if (failed)
        {
            return;
        }

        grounded = false;
        rb.gravityScale = gravityScaleInAir;
        rb.velocity = new Vector2(moveSpeed, jumpVelocity);
    }

    public Collider2D CheckRailContact()
    {
        if (railContactPoint == null)
        {
            return null;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(railContactPoint.position, contactRadius, railLayer);
        Collider2D bestRail = null;
        float bestTopY = float.NegativeInfinity;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];

            if (hit == null || hit.GetComponentInParent<RailSegment>() == null)
            {
                continue;
            }

            float topY = hit.bounds.max.y;

            if (topY > bestTopY)
            {
                bestTopY = topY;
                bestRail = hit;
            }
        }

        return bestRail;
    }

    public void SnapToRail(Collider2D railCollider)
    {
        if (railCollider == null || railContactPoint == null)
        {
            return;
        }

        float railTopY = railCollider.bounds.max.y;
        float contactOffsetY = railContactPoint.position.y - transform.position.y;

        Vector3 position = transform.position;
        position.y = railTopY - contactOffsetY;
        transform.position = position;

        rb.velocity = new Vector2(moveSpeed, 0f);
    }

    public void FailAndFall()
    {
        GameOver("Game Over: Vehicle missed the rail.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FailZone"))
        {
            GameOver("Game Over: Vehicle touched FailZone.");
        }
    }

    private void GameOver(string message)
    {
        if (failed)
        {
            return;
        }

        failed = true;
        inputEnabled = false;
        grounded = false;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.simulated = false;
        Time.timeScale = 0f;

        Debug.Log(message);
    }

    private bool WasJumpPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Space);
#else
        return false;
#endif
    }

    private void OnDrawGizmosSelected()
    {
        if (railContactPoint == null)
        {
            return;
        }

        Gizmos.color = grounded ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(railContactPoint.position, contactRadius);
    }
}
