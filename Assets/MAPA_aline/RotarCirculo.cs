using UnityEngine;

public class RotarCirculo : MonoBehaviour
{
    public float velocidad = 60f;
    void Update()
    {
        transform.Rotate(0, 0, velocidad * Time.deltaTime);
    }
}