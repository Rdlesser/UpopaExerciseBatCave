using System;
using System.Collections;
using System.Collections.Generic;
using BatCave;
using Infra.Gameplay;
using UnityEngine;

public class RepeatingBackground : MonoBehaviour
{
    /// <summary>
    /// A reusable cave screen 
    /// </summary>

    private float caveLength = 5f;

    public float CaveLength => caveLength;

    private Vector2 initialPosition;

    private void Awake()
    {
        initialPosition = transform.position;
        GameManager.OnGameReset += OnGameReset;
    }

    // Start is called before the first frame update
    void Start()
    {
        Transform childTransform = transform.GetChild(0);
        if (childTransform != null)
        {
            caveLength = childTransform.localScale.x;
        }
    }

    /// <summary>
    /// Reposition the background from its position offscreen, behind the player,
    /// to its new position in front of the player, off-camera
    /// </summary>
    public void RepositionBackground()
    {
        // This is how far to the right we will move our background object, in this case,
        // twice its length. This will position it directly to the right of the currently visible background object.
        Vector2 groundOffSet = new Vector2(caveLength * 2f, 0);
        
        // Move this object from it's position offscreen, behind the player,
        // to the new position off-camera in front of the player.
        var transform1 = transform;
        transform1.position = (Vector2) transform1.position + groundOffSet;

        RandomizeChildrenPolygonPoints();
    }

    private void OnGameReset()
    {
        transform.position = initialPosition;
        RandomizeChildrenPolygonPoints();
    }

    /// <summary>
    /// Randomize the cave polygon collider of the children
    /// </summary>
    private void RandomizeChildrenPolygonPoints()
    {
        foreach (Transform child in transform)
        {
            PolygonColliderWithMesh polygonCollider = child.GetComponent<PolygonColliderWithMesh>();
            if (polygonCollider != null)
            {
                polygonCollider.RandomizePolygonPoints();
            }
        }
    }
}
