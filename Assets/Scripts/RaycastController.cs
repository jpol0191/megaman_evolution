﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour {

    public LayerMask collisionMask;

    public const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    [HideInInspector]
    public float horizontalRaycastSpacing;
    [HideInInspector]
    public float verticalRaycastSpacing;

    public BoxCollider2D col;
    public  RaycastOrigins raycastOrigins;

    // Use this for initialization
    public virtual void Start() {
        col = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void CalculateRaySpacing() {
        Bounds bounds = col.bounds;
        bounds.Expand(skinWidth * -2);

        // Number of rays will never be lower than 2
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaycastSpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaycastSpacing = bounds.size.x / (verticalRayCount - 1);
    }

    //Update theraycast origins  
    public void UpdateRaycastOrigins() {
        Bounds bounds = col.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    // Making a namespace to hold the origins of the raycastz
    public struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;

    }
}