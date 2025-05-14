using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredMovingPlatform : MonoBehaviour
{
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool startRight = true;

    private Vector3 leftPoint;
    private Vector3 rightPoint;
    private Vector3 targetPosition;
    private bool movingRight;
    private Vector3 previousPosition;
    private bool playerOnPlatform = false;

    private void Start()
    {
        // Definir extremos del recorrido
        leftPoint = transform.position - Vector3.right * (moveDistance / 2f);
        rightPoint = transform.position + Vector3.right * (moveDistance / 2f);

        // Posicionar la plataforma en uno de los extremos
        transform.position = startRight ? leftPoint : rightPoint;

        movingRight = startRight;
        targetPosition = movingRight ? rightPoint : leftPoint;

        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (playerOnPlatform)
        {
            // Mover la plataforma hacia el objetivo
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);

            // Si llegó al destino, cambiar de dirección
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                movingRight = !movingRight;
                targetPosition = movingRight ? rightPoint : leftPoint;
            }
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
            playerOnPlatform = true;

            // Mover al jugador con la plataforma
            Vector3 platformDelta = transform.position - previousPosition;
            collision.transform.position += platformDelta;
        }
    }
}

