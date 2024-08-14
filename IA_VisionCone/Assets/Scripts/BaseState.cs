using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState : MonoBehaviour
{
    public String Name = "BaseState";
    // Necesita "conocer" (tener una referencia o forma de contactar) a la m?quina de estados que es su due?a.
    public BaseFSM FSMRef;

    public BaseState()
    {
        Name = "BaseState";
    }

    public BaseState(string inName, BaseFSM inBaseFSM)
    {
        Name = inName;
        FSMRef = inBaseFSM;
    }

    public virtual void InitializeState(BaseFSM inBaseFSM)
    {
        FSMRef = inBaseFSM;
    }

    // Start is called before the first frame update
    public virtual void OnEnter()
    {
        // Vamos a poner inicializaciones, pedir memoria, recursos, etc.

        Debug.Log("OnEnter del estado: " + Name);
    }

    // Update is called once per frame
    public virtual void OnUpdate()
    {
        Debug.Log("OnUpdate del estado: " + Name);
    }

    public virtual void OnExit()
    {
        // Vamos a liberar memoria, quitar recursos, ocultar cosas que ya no sean necesarias, etc.
        Debug.Log("OnExit del estado: " + Name);
    }
}