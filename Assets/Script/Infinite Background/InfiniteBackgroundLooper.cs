using System.Collections.Generic;
using UnityEngine;

public class InfiniteBackgroundLooper : MonoBehaviour
{
    [Header("Camera")]
    public Camera targetCamera;

    [Header("Background Tiles")]
    public Transform[] backgroundTiles;

    [Header("Settings")]
    public float recycleBuffer = 2f;
    public bool lockY = true;
    public float fixedY = 0f;

    private List<Transform> tiles = new List<Transform>();

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (backgroundTiles == null || backgroundTiles.Length == 0)
        {
            foreach (Transform child in transform)
            {
                tiles.Add(child);
            }
        }
        else
        {
            tiles.AddRange(backgroundTiles);
        }

        SortTilesByX();
    }

    private void LateUpdate()
    {
        if (targetCamera == null || tiles.Count < 2)
            return;

        SortTilesByX();

        float cameraHalfWidth = targetCamera.orthographicSize * targetCamera.aspect;
        float cameraLeft = targetCamera.transform.position.x - cameraHalfWidth;
        float cameraRight = targetCamera.transform.position.x + cameraHalfWidth;

        MoveLeftTileToRight(cameraLeft);
        MoveRightTileToLeft(cameraRight);

        if (lockY)
        {
            foreach (Transform tile in tiles)
            {
                Vector3 pos = tile.position;
                pos.y = fixedY;
                tile.position = pos;
            }
        }
    }

    private void MoveLeftTileToRight(float cameraLeft)
    {
        Transform leftMost = tiles[0];
        SpriteRenderer leftRenderer = leftMost.GetComponent<SpriteRenderer>();

        if (leftRenderer == null)
            return;

        if (leftRenderer.bounds.max.x < cameraLeft - recycleBuffer)
        {
            Transform rightMost = tiles[tiles.Count - 1];
            SpriteRenderer rightRenderer = rightMost.GetComponent<SpriteRenderer>();

            if (rightRenderer == null)
                return;

            float moveDistance = rightRenderer.bounds.max.x - leftRenderer.bounds.min.x;

            Vector3 newPos = leftMost.position;
            newPos.x += moveDistance;
            leftMost.position = newPos;

            tiles.RemoveAt(0);
            tiles.Add(leftMost);
        }
    }

    private void MoveRightTileToLeft(float cameraRight)
    {
        Transform rightMost = tiles[tiles.Count - 1];
        SpriteRenderer rightRenderer = rightMost.GetComponent<SpriteRenderer>();

        if (rightRenderer == null)
            return;

        if (rightRenderer.bounds.min.x > cameraRight + recycleBuffer)
        {
            Transform leftMost = tiles[0];
            SpriteRenderer leftRenderer = leftMost.GetComponent<SpriteRenderer>();

            if (leftRenderer == null)
                return;

            float moveDistance = rightRenderer.bounds.max.x - leftRenderer.bounds.min.x;

            Vector3 newPos = rightMost.position;
            newPos.x -= moveDistance;
            rightMost.position = newPos;

            tiles.RemoveAt(tiles.Count - 1);
            tiles.Insert(0, rightMost);
        }
    }

    private void SortTilesByX()
    {
        tiles.Sort((a, b) => a.position.x.CompareTo(b.position.x));
    }
}