using UnityEngine;

[ExecuteAlways]
public class RailSegment : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public BoxCollider2D railCollider;
    public SpriteRenderer railSpriteRenderer;
    public LineRenderer railLineRenderer;

    [SerializeField] private float length = 6f;
    [SerializeField] private float railThickness = 0.15f;

    public Vector3 StartWorldPosition
    {
        get
        {
            return startPoint != null ? startPoint.position : transform.position;
        }
    }

    public Vector3 EndWorldPosition
    {
        get
        {
            return endPoint != null ? endPoint.position : transform.position + Vector3.right * length;
        }
    }

    public float Length => length;

    private void Reset()
    {
        CacheMissingReferences();
        Setup(length);
    }

    private void OnValidate()
    {
        CacheMissingReferences();
        Setup(Mathf.Max(0.1f, length));
    }

    public void Setup(float newLength)
    {
        length = Mathf.Max(0.1f, newLength);
        CacheMissingReferences();

        if (startPoint != null)
        {
            startPoint.localPosition = Vector3.zero;
        }

        if (endPoint != null)
        {
            endPoint.localPosition = new Vector3(length, 0f, 0f);
        }

        if (railCollider != null)
        {
            railCollider.isTrigger = true;
            railCollider.offset = new Vector2(length * 0.5f, 0f);
            railCollider.size = new Vector2(length, railThickness);
        }

        if (railSpriteRenderer != null)
        {
            Transform spriteTransform = railSpriteRenderer.transform;
            spriteTransform.localPosition = new Vector3(length * 0.5f, 0f, 0f);
            spriteTransform.localRotation = Quaternion.identity;
            spriteTransform.localScale = new Vector3(length, railThickness, 1f);
        }

        if (railLineRenderer != null)
        {
            railLineRenderer.positionCount = 2;
            railLineRenderer.useWorldSpace = false;
            railLineRenderer.SetPosition(0, Vector3.zero);
            railLineRenderer.SetPosition(1, new Vector3(length, 0f, 0f));
        }
    }

    private void CacheMissingReferences()
    {
        if (railCollider == null)
        {
            railCollider = GetComponent<BoxCollider2D>();

            if (railCollider == null)
            {
                railCollider = GetComponentInChildren<BoxCollider2D>();
            }
        }

        if (railSpriteRenderer == null)
        {
            railSpriteRenderer = GetComponent<SpriteRenderer>();

            if (railSpriteRenderer == null)
            {
                railSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        if (railLineRenderer == null)
        {
            railLineRenderer = GetComponent<LineRenderer>();

            if (railLineRenderer == null)
            {
                railLineRenderer = GetComponentInChildren<LineRenderer>();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 start = StartWorldPosition;
        Vector3 end = EndWorldPosition;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(start, 0.15f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(end, 0.15f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, end);

        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Vector3 center = (start + end) * 0.5f;
        Gizmos.DrawCube(center, new Vector3(Mathf.Max(0.1f, length), railThickness, 0.05f));
    }
}
