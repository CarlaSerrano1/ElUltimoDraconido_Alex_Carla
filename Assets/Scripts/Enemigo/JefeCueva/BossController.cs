using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public enum BossState
    {
        Entrada = 0,
        Idle = 1,
        SacarBrazo = 2,
        LanzaGrande = 3,
        SacarLanza = 4,
        RomperLanza = 5,
        GuardarBrazo = 6,
        Muerte = 7
    }

    public BossState estadoActual = BossState.Entrada;
    public Animator animator;
    public GameObject lanzaGrandePrefab;
    public GameObject miniLanzaPrefab;
    public Transform lanzaSpawnPoint;
    public Transform[] miniLanzaSpawns;
    public Transform[] plataformas;
    public Transform[] posicionesEnIdle;     // Posiciones cuando el jefe está en idle
    public Transform[] posicionesEscondidas; // Posiciones fuera del mapa
    public float velocidadPlataformas = 3f;

    public int vida = 5;

    private bool vulnerable = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        CambiarEstado(BossState.Entrada);
    }

    void CambiarEstado(BossState nuevoEstado)
    {
        estadoActual = nuevoEstado;
        animator.SetInteger("estado", (int)estadoActual);

        if (estadoActual == BossState.Idle)
        {
            for (int i = 0; i < plataformas.Length; i++)
            {
                StartCoroutine(MoverPlataforma(plataformas[i], posicionesEnIdle[i].position));
            }
        }
        else
        {
            for (int i = 0; i < plataformas.Length; i++)
            {
                StartCoroutine(MoverPlataforma(plataformas[i], posicionesEscondidas[i].position));
            }
        }
    }


    // Llamado por evento al finalizar animación Entrada
    public void TerminarEntrada()
    {
        CambiarEstado(BossState.Idle);
        vulnerable = true;
        StartCoroutine(ElegirProximoAtaque());
    }

    IEnumerator ElegirProximoAtaque()
    {
        yield return new WaitForSeconds(5f);

        vulnerable = false;
        int ataque = Random.Range(0, 2);
        if (ataque == 0)
        {
            CambiarEstado(BossState.SacarBrazo);
        }
        else
        {
            CambiarEstado(BossState.SacarLanza);
        }
    }

    // Llamado por evento: cuando termina animación SacarBrazo
    public void LanzarLanzaGrande()
    {
        CambiarEstado(BossState.LanzaGrande);
        Instantiate(lanzaGrandePrefab, lanzaSpawnPoint.position, Quaternion.identity);
    }

    // Llamado por evento: cuando termina animación LanzaGrande
    public void GuardarBrazoLuegoDeLanzaGrande()
    {
        CambiarEstado(BossState.GuardarBrazo);
    }

    // Llamado por evento: cuando termina animación SacarLanza
    public void EmpezarRomperLanza()
    {
        CambiarEstado(BossState.RomperLanza);
        StartCoroutine(LanzarMiniLanzas());
    }

    IEnumerator LanzarMiniLanzas()
    {
        int cantidad = Random.Range(30, 40);

        for (int i = 0; i < cantidad; i++)
        {
            Transform spawn = miniLanzaSpawns[Random.Range(0, miniLanzaSpawns.Length)];
            Instantiate(miniLanzaPrefab, spawn.position, Quaternion.Euler(0, 0, 90f));
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);
        CambiarEstado(BossState.GuardarBrazo);
    }

    // Llamado por evento al terminar GuardarBrazo
    public void VolverAIdle()
    {
        CambiarEstado(BossState.Idle);
        vulnerable = true;
        StartCoroutine(ElegirProximoAtaque());
    }

    void OnTriggerEnter2D(Collider2D other)
{
    if (vulnerable && other.CompareTag("Fireball"))
    {
        vida--;

        if (vida <= 0)
        {
            // Asegura que el sprite está visible antes de morir
            GetComponent<SpriteRenderer>().enabled = true;
            vulnerable = false;
            CambiarEstado(BossState.Muerte);
            // Aquí puedes iniciar una animación de muerte si se necesita
        }
        else
        {
            StartCoroutine(DañoVisual());
        }
    }
}


    IEnumerator DañoVisual()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        for (int i = 0; i < 3; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sr.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator MoverPlataforma(Transform plataforma, Vector3 destino)
    {
        while (Vector3.Distance(plataforma.position, destino) > 0.05f)
        {
            plataforma.position = Vector3.MoveTowards(plataforma.position, destino, velocidadPlataformas * Time.deltaTime);
            yield return null;
        }

        plataforma.position = destino;
    }

}
