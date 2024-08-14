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
        bool mantenerwPresionadow = Input.GetKey("w");
        bool mantenerwPresionadoa = Input.GetKey("a");
        bool mantenerwPresionados = Input.GetKey("s");
        bool mantenerwPresionadod = Input.GetKey("d");
        bool mantenershiftPresionado = Input.GetKey("left shift");
        bool mantenerctrlPresionado = Input.GetKey("c");
        bool saltarunaVez = Input.GetKey("space");
       
        if (mantenerwPresionadow)
        {
            animator.SetBool("IsWalking", true);
        }
        
        if (!mantenerwPresionadow)
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

        if(mantenerctrlPresionado)
        {
            animator.SetBool("Crouch", true);
        }

        if (!mantenerctrlPresionado)
        {
            animator.SetBool("Crouch", false);
        }
       
        if (saltarunaVez)
        {
            animator.SetBool("IsJumping", true);
        }

        if (!saltarunaVez)
        {
            animator.SetBool("IsJumping", false);
        }
    }
}
