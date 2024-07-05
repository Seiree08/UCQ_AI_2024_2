using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefab : MonoBehaviour
{
    public GameObject objectToTeleport; // Referencia al objeto que deseas teletransportar
    public Vector3 teleportPosition;    // Posición a la cual se teleportará el objeto

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Verificar que el objeto a teletransportar no sea nulo
            if (objectToTeleport != null)
            {
                // Activar el objeto si está desactivado
                if (!objectToTeleport.activeSelf)
                {
                    objectToTeleport.SetActive(true);
                }

                // Teletransportar el objeto a la posición especificada
                objectToTeleport.transform.position = teleportPosition;
            }
        }
    }
}