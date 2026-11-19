using System.Net.Sockets;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

public class Movimiento : MonoBehaviour
{

    public float velocity = 5f;
    public float rotation = 177;

    public Transform camara;
    private Rigidbody rb;
    private Vector3 movimientoXZ;

    private Animator animator;
    private bool isRunning;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

        float horizontal = Input.GetAxisRaw("Horizontal"); //Para A <--> D
        float vertical = Input.GetAxisRaw("Vertical"); //Para A <--> D
        //Con Raw pasamos de 0 a 1 sin valores decimales, el movimiento es más instantaneo.

        //Hay que tener cuidado porque estas dos toman la rotación de la cámara también
        Vector3 adelanteCamara = camara.forward; //Nos dice a done está viendo la camara
                                                    //Si la rotamos para abajo (0,Y,1) en Y un bajor a donde a punte la camara
                                                    //Por lo tanto el movimiento de W y S seá hacia la camara y el
                                                        //personaje flotará
            //Nos da (0,0,1) en general
        Vector3 derechaCamara = camara.right; //Represetna hacia donde está la derecha de la cámara
        //Tomamos el transform de la camara para modificarlo
            //Nos da (0,0,1) en general


        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        adelanteCamara.y = 0;
        derechaCamara.y = 0;

        Vector3 direccion = (derechaCamara * horizontal + adelanteCamara * vertical);

        if(direccion.sqrMagnitude > 0.0001f) direccion.Normalize(); //Para que no vaya más rapido en movimiento horizontal

        //Movimiento del jugador
        movimientoXZ = direccion * velocity; //* Time.deltaTime); Ya no es necesaria

        //Mover jugador
        //transform.position += movimientoXZ;

        //ANIMACION
        if(vertical != 0) isRunning = true;
        else isRunning = false;

        UpdateAnimation();

        Debug.Log($"Valor Horizontal: {horizontal} Valor Vertical: {vertical} IsRunnig: {isRunning}");



    }

    void FixedUpdate()
    {
        // Aplicar movimiento mediante Rigidbody
        rb.linearVelocity = new Vector3(movimientoXZ.x, 
        rb.linearVelocity.y, //Recuperar Gravedad
        movimientoXZ.z);
    }

    public void UpdateAnimation()
    {

        animator.SetBool("isRunning", isRunning);

    }
}
