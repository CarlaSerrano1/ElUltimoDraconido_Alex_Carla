using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantamagoIA : MonoBehaviour
{
    public Animator animator;
    public GameObject proyectilPrefab;
    public Transform spawnPoint;
    public Transform jugador;

    public float distanciaAtaque = 5f;
    public float cooldownAtaque = 2f;

    private bool puedeAtacar = true;

    private void Update()
    {
        if (puedeAtacar && Vector2.Distance(transform.position, jugador.position) < distanciaAtaque)
        {
            Atacar();
        }
    }

    void Atacar()
    {
        animator.SetTrigger("Atacar");
        StartCoroutine(LanzarProyectil());
        puedeAtacar = false;
        Invoke(nameof(ReiniciarCooldown), cooldownAtaque);
    }

    void ReiniciarCooldown()
    {
        puedeAtacar = true;
    }

    IEnumerator LanzarProyectil()
    {
        // Espera a que termine la animación antes de lanzar el proyectil
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        GameObject proyectil = Instantiate(proyectilPrefab, spawnPoint.position, Quaternion.identity);
        proyectil.GetComponent<Proyectil>().Iniciar(jugador.position);
    }
}
