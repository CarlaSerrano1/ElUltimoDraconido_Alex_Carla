using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public float fuerza = 5f;
    public GameObject impactoPrefab;
    
    private Rigidbody2D rb;
    private Animator animator;
    private bool haImpactado = false;

public void Iniciar(Vector2 objetivo)
{
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();

    float gravedad = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
    Vector2 origen = transform.position;
    Vector2 destino = objetivo;

    float alturaMaxima = 4f; // Altura deseada del arco

    if (gravedad == 0)
    {
        Debug.LogWarning("¡Gravedad es cero! No se puede calcular parábola.");
        return;
    }

    // Calcular tiempos de subida y bajada
    float alturaRelativa = destino.y - origen.y;
    float alturaBajada = alturaMaxima - alturaRelativa;

    if (alturaBajada < 0) alturaBajada = 0; // Protege contra raíz negativa

    float tiempoSubida = Mathf.Sqrt(2 * alturaMaxima / gravedad);
    float tiempoBajada = Mathf.Sqrt(2 * alturaBajada / gravedad);
    float tiempoTotal = tiempoSubida + tiempoBajada;

    if (tiempoTotal <= 0)
    {
        Debug.LogWarning("Tiempo total de vuelo inválido.");
        return;
    }

    // Velocidades necesarias
    float velocidadX = (destino.x - origen.x) / tiempoTotal;
    float velocidadY = Mathf.Sqrt(2 * gravedad * alturaMaxima);

    Vector2 velocidadInicial = new Vector2(velocidadX, velocidadY);
    rb.velocity = velocidadInicial;

    // Rotar solo si la velocidad es válida
    if (velocidadInicial.sqrMagnitude > 0.0001f)
    {
        float angulo = Mathf.Atan2(velocidadInicial.y, velocidadInicial.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }
    else
    {
        Debug.LogWarning("Velocidad inicial demasiado baja o nula, no se puede rotar.");
    }

    Destroy(gameObject, 10f);
}




    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (haImpactado) return;
        
        haImpactado = true;
        rb.velocity = Vector2.zero; // Detener el movimiento
        rb.freezeRotation = true;
        rb.isKinematic = true; // Para que no siga colisionando
        animator.SetTrigger("Impacto"); // Activar la animación de impacto

        // Destruir el proyectil después de la animación
        Invoke("DestruirProyectil", 0.5f); // Ajusta el tiempo según la animación
    }

    private void DestruirProyectil()
    {
        Destroy(gameObject);
    }
}
