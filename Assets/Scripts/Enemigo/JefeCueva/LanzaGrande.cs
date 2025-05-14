using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanzaGrande : MonoBehaviour
{
    public float speed = 20f;
    private bool lanzada = false;
    private bool direccionFijada = false;

    private Vector2 direccionFinal;
    private Transform jugador;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        jugador = GameObject.FindGameObjectWithTag("Player").transform;

        animator.Play("AparecerLanza");
    }

    void Update()
    {
        if (!direccionFijada && jugador != null)
        {
            // Solo apunta mientras no se ha fijado la dirección
            Vector2 dir = (jugador.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            direccionFinal = dir;
        }
    }

    // Llamado por evento al final de AparecerLanza
    public void FinalizarAparicion()
    {
        direccionFijada = true;
        lanzada = true;
        rb.velocity = direccionFinal * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (lanzada && (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Ground")))
        {
            rb.velocity = Vector2.zero;
            lanzada = false;
            animator.Play("DesaparecerLanza");
        }
    }


    // Llamado por evento al final de DesaparecerLanza
    public void DestruirLanza()
    {
        Destroy(gameObject);
    }
}
