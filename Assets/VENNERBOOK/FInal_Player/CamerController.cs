using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
public class CamerController : MonoBehaviour
{

    public Transform pivote;
    public float sensitivity = 0.1f;
    private Vector2 lookInput;
    private float rotHorizonal = 0f;
    private float rotVertical = 0f;

    public void OnLook(InputAction.CallbackContext context)
    {
        //Leer el movimiento de el ratón
        lookInput = context.ReadValue<Vector2>();

    }

    private void Update()
    {
        //Acumular rotación
        rotHorizonal += lookInput.x * sensitivity;
        rotVertical -= lookInput.y * sensitivity;

        //Limitar visión vertical
        //Clamp limita el valor de vetical a esos parametros
        rotVertical = math.clamp(rotVertical, -45f, 60f);

        //Quaternion sirve para calcular cualquier rotación
        //Pasa de grados a quaternion
        pivote.localRotation = Quaternion.Euler(rotVertical,rotHorizonal,0) ;

    }

}
