using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool startRight = true;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingRight;
    private Vector3 previousPosition;

    private void Start()
    {
        startPosition = transform.position;
        movingRight = startRight;
        targetPosition = startPosition + Vector3.right * moveDistance;
        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // Mover plataforma
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            movingRight = !movingRight;
            targetPosition = startPosition + (movingRight ? Vector3.right : Vector3.left) * moveDistance;
        }
    }

    private void LateUpdate()
    {
        previousPosition = transform.position;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Calcular el movimiento de la plataforma
            Vector3 platformDelta = transform.position - previousPosition;

            // Mover al jugador junto con la plataforma (sin cambiar su jerarquía)
            collision.transform.position += platformDelta;
        }
    }
}
