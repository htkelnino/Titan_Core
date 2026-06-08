using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private BoxCollider2D mapBounds;

    private Transform player1;
    private Transform player2;

    private Camera cam;
    public void SetTargets(Transform player1, Transform player2)
    {
        this.player1 = player1;
        this.player2 = player2;
    }
    private void Awake()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        //Theo 2 player
        if (player1 == null || player2 == null)
            return;
        Vector3 center = (player1.position + player2.position) / 2f;


        //Chống tràn viền map
        Bounds bounds = mapBounds.bounds;

        float minX = bounds.min.x;
        float maxX = bounds.max.x;

        float minY = bounds.min.y;
        float maxY = bounds.max.y;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float x = Mathf.Clamp(
           center.x,
           minX + halfWidth,
           maxX - halfWidth
       );

        float y = Mathf.Clamp(
            center.y,
            minY + halfHeight,
            maxY - halfHeight
        );

        transform.position = new Vector3(
            x,
            y,
            transform.position.z
        );
    }
}