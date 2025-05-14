using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Room camera
    [SerializeField] private float speed;
    private float currentPosX;
    private Vector3 velocity = Vector3.zero;

    // Follow player
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float verticalFollowSpeed;
    [SerializeField] private float cameraOffsetY = 0f; // Ajuste de altura
    [SerializeField] private float cameraZoom = -10f;  // Ajuste de distancia (profundidad)

    private float lookAhead;

    private void Update()
    {
        // Seguimiento horizontal y vertical del jugador con offset y zoom
        float targetX = player.position.x + lookAhead;
        float targetY = Mathf.Lerp(transform.position.y, player.position.y + cameraOffsetY, Time.deltaTime * verticalFollowSpeed);

        transform.position = new Vector3(targetX, targetY, cameraZoom);

        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);
    }

    public void MoveToNewRoom(Transform _newRoom)
    {
        currentPosX = _newRoom.position.x;
    }

    public void SetCameraOffset(float offsetY)
    {
        cameraOffsetY = offsetY;
    }

    public void SetCameraZoom(float zoom)
    {
        cameraZoom = zoom;
    }
}


