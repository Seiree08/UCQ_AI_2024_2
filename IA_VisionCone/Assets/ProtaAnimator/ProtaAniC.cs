using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProtaAniC : MonoBehaviour
{
    //> <
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Log(animator);
    }

    // Update is called once per frame
    void Update()
    {
        bool mantenerwPresionado = Input.GetKey("w");
        bool mantenershiftPresionado = Input.GetKey("left shift");
       
        if (mantenerwPresionado)
        {
            animator.SetBool("IsWalking", true);
        }
        
        if (!mantenerwPresionado)
        {
            animator.SetBool("IsWalking", false);
        }



        if (mantenershiftPresionado)
        {
            animator.SetBool("IsRunning", true);
        }

        if (!mantenershiftPresionado)
        {
            animator.SetBool("IsRunning", false);
        }
    }
}
