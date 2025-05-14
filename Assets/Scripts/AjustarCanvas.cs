using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class AjustarCanvas : MonoBehaviour
{
    private Camera camara;

    private void Start()
    {
        camara = Camera.main;
        AjustarAlTamañoDeCamara();
    }

    private void AjustarAlTamañoDeCamara()
    {
        if (camara == null)
        {
            Debug.LogWarning("No se encontró la cámara principal.");
            return;
        }

        if (!camara.orthographic)
        {
            Debug.LogWarning("Este script solo funciona con cámaras ortográficas.");
            return;
        }

        RectTransform rectTransform = GetComponent<RectTransform>();

        // Calcular el tamaño visible de la cámara en unidades del mundo
        float altura = 2f * camara.orthographicSize;
        float anchura = altura * camara.aspect;

        rectTransform.sizeDelta = new Vector2(anchura, altura);
        rectTransform.position = camara.transform.position;
    }
}

