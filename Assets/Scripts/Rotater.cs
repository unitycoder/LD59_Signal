using UnityEngine;

public class Rotater : MonoBehaviour
{
    public float speed = 10f;
    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
