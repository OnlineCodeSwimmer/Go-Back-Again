using System.Collections.Generic;
using UnityEngine;

public class InfiniteIrregularRailManager : MonoBehaviour
{
    public Transform vehicle;
    public RailSegment railPrefab;
    public float preloadDistance = 80f;
    public float recycleDistance = 50f;

    public Vector2 lengthRange = new Vector2(4f, 8f);
    public Vector2 gapRange = new Vector2(2f, 5f);
    public Vector2 heightRange = new Vector2(-2f, 4f);
    public float maxHeightStep = 2.5f;
    public BoxCollider2D failZoneCollider;
    public float failZoneY = -6f;
    public float failZoneHeight = 1.5f;
    public float failZonePadding = 20f;

    public bool useDebugPattern = true;
    public float[] debugHeights = { 0f, 0f, 1.2f, 2.4f, 0f, 3f, -0.8f, 1.5f };
    public float[] debugLengths = { 6f, 5f, 5f, 6f, 7f, 5f, 6f, 5f };
    public float[] debugGaps = { 2f, 3f, 3f, 4f, 3f, 5f, 3f, 4f };

    private readonly Queue<RailSegment> activeSegments = new Queue<RailSegment>();
    private readonly Queue<RailSegment> pool = new Queue<RailSegment>();

    private float nextSpawnX;
    private float lastHeight;
    private int spawnIndex;
    private bool hasSpawnedFirstSegment;

    private void Start()
    {
        if (vehicle == null)
        {
            Debug.LogError($"{nameof(InfiniteIrregularRailManager)} needs a Vehicle transform.", this);
            enabled = false;
            return;
        }

        if (railPrefab == null)
        {
            Debug.LogError($"{nameof(InfiniteIrregularRailManager)} needs a RailSegment prefab.", this);
            enabled = false;
            return;
        }

        nextSpawnX = vehicle.position.x;
        lastHeight = vehicle.position.y;

        SpawnUntilPreloaded();
        UpdateFailZone();
    }

    private void Update()
    {
        if (vehicle == null)
        {
            return;
        }

        SpawnUntilPreloaded();
        RecycleOldSegments();
        UpdateFailZone();
    }

    private void SpawnUntilPreloaded()
    {
        float targetX = vehicle.position.x + preloadDistance;

        while (nextSpawnX < targetX)
        {
            SpawnSegment();
        }
    }

    private void SpawnSegment()
    {
        RailSegment segment = GetFromPool();
        SegmentData data = GetNextSegmentData();

        if (!hasSpawnedFirstSegment)
        {
            nextSpawnX = vehicle.position.x - 1f;
        }
        else
        {
            nextSpawnX += data.gap;
        }

        segment.transform.position = new Vector3(nextSpawnX, data.height, 0f);
        segment.Setup(data.length);
        segment.gameObject.SetActive(true);

        activeSegments.Enqueue(segment);

        nextSpawnX = segment.EndWorldPosition.x;
        lastHeight = data.height;
        spawnIndex++;
        hasSpawnedFirstSegment = true;
    }

    private SegmentData GetNextSegmentData()
    {
        float length = PickDebugOrRandom(debugLengths, lengthRange, spawnIndex);
        float gap = PickDebugOrRandom(debugGaps, gapRange, spawnIndex);
        float height;

        if (useDebugPattern && debugHeights != null && debugHeights.Length > 0)
        {
            height = debugHeights[spawnIndex % debugHeights.Length];
        }
        else
        {
            height = Mathf.Clamp(
                lastHeight + Random.Range(-maxHeightStep, maxHeightStep),
                heightRange.x,
                heightRange.y
            );
        }

        return new SegmentData(length, gap, height);
    }

    private float PickDebugOrRandom(float[] debugValues, Vector2 range, int index)
    {
        if (useDebugPattern && debugValues != null && debugValues.Length > 0)
        {
            return Mathf.Max(0f, debugValues[index % debugValues.Length]);
        }

        return Random.Range(range.x, range.y);
    }

    private RailSegment GetFromPool()
    {
        RailSegment segment;

        if (pool.Count > 0)
        {
            segment = pool.Dequeue();
            segment.transform.SetParent(transform, true);
            return segment;
        }

        segment = Instantiate(railPrefab, transform);
        return segment;
    }

    private void RecycleOldSegments()
    {
        float recycleX = vehicle.position.x - recycleDistance;

        while (activeSegments.Count > 0 && activeSegments.Peek().EndWorldPosition.x < recycleX)
        {
            RailSegment oldSegment = activeSegments.Dequeue();
            oldSegment.gameObject.SetActive(false);
            oldSegment.transform.SetParent(transform, true);
            pool.Enqueue(oldSegment);
        }
    }

    private void UpdateFailZone()
    {
        if (failZoneCollider == null)
        {
            return;
        }

        float leftX = vehicle.position.x - recycleDistance - failZonePadding;
        float rightX = Mathf.Max(nextSpawnX, vehicle.position.x + preloadDistance) + failZonePadding;
        float width = Mathf.Max(1f, rightX - leftX);
        float centerX = (leftX + rightX) * 0.5f;

        Transform failZoneTransform = failZoneCollider.transform;
        failZoneTransform.position = new Vector3(centerX, failZoneY, failZoneTransform.position.z);
        failZoneTransform.localScale = Vector3.one;
        failZoneCollider.isTrigger = true;
        failZoneCollider.offset = Vector2.zero;
        failZoneCollider.size = new Vector2(width, failZoneHeight);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.25f);
        Vector3 center = transform.position;
        center.y = (heightRange.x + heightRange.y) * 0.5f;
        Gizmos.DrawWireCube(center, new Vector3(20f, heightRange.y - heightRange.x, 0.1f));
    }

    private readonly struct SegmentData
    {
        public readonly float length;
        public readonly float gap;
        public readonly float height;

        public SegmentData(float length, float gap, float height)
        {
            this.length = length;
            this.gap = gap;
            this.height = height;
        }
    }
}
