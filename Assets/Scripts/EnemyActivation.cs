using UnityEngine;

public class EnemyActivation : MonoBehaviour
{
    private bool isActive = false; // Estado del enemigo
    private Collider2D enemyCollider;
    private Rigidbody2D rb;
    private Animator anim;

    private void Awake()
    {
        // Desactivar la lógica del enemigo al inicio
        enemyCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        ActivateEnemy(false);
    }

    // Se llama cuando el enemigo entra en la vista de la cámara
    private void OnBecameVisible()
    {
        if (!isActive)
        {
            ActivateEnemy(true);
        }
    }

    private void ActivateEnemy(bool state)
    {
        isActive = state;
        enemyCollider.enabled = state;
        if (rb != null) rb.simulated = state; // Evita que el Rigidbody afecte el rendimiento
        if (anim != null) anim.enabled = state; // Detiene la animación si está fuera de pantalla
    }
}

