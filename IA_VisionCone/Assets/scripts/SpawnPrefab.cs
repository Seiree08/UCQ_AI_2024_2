using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefab : MonoBehaviour
{
    public GameObject objectToTeleport; // Referencia al objeto que deseas teletransportar
    public Vector3 teleportPosition;    // Posici�n a la cual se teleportar� el objeto

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Verificar que el objeto a teletransportar no sea nulo
            if (objectToTeleport != null)
            {
                // Activar el objeto si est� desactivado
                if (!objectToTeleport.activeSelf)
                {
                    objectToTeleport.SetActive(true);
                }

                // Teletransportar el objeto a la posici�n especificada
                objectToTeleport.transform.position = teleportPosition;
            }
        }
    }
}