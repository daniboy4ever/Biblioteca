using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{

    private CharacterController controller;

    //public float speed = 3f;

    private Vector2 moveInput;

    public float gravity = -9.81f;
    private float verticalVelocity;
    public Transform modelTransform;
    public Transform cameraPivot;

    private Animator animator;
    private bool isWalking;
    private bool isRunning;
    public float walkspeed = 3f;
    public float runSpeed = 7f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //Accedemos al documento de actions de nuestro CharacterControler
        moveInput = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        float actualSpeed = (isRunning&&isWalking)? runSpeed : walkspeed;

        //Darle gravedad
        if(controller.isGrounded && verticalVelocity<0)
        {
            verticalVelocity = -2f;
        }
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 forward = cameraPivot.forward;
        Vector3 right = cameraPivot.right;

        //Para que no camine hacia arriba o abajo
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = (forward * moveInput.y + right * moveInput.x);

        if (desiredMoveDirection.magnitude > 0.1f)
        {
            quaternion targetRotation =  Quaternion.LookRotation(desiredMoveDirection);
            modelTransform.rotation =  Quaternion.Slerp(modelTransform.rotation, targetRotation, 17f*Time.deltaTime);
        }
        Vector3 velocity = desiredMoveDirection * actualSpeed;
        velocity.y = verticalVelocity;

        //Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(velocity *Time.deltaTime);

        Animations();
    }

    
    public void Animations()
    {
        isWalking = moveInput.magnitude > 0.1f;

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning && isWalking);
    }

    public void onRunning(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }


    


}
