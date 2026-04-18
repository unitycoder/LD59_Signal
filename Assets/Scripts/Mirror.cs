using UnityEngine;

public class Mirror : MonoBehaviour
{
    public Vector3 currentDirection = Vector3.up;
    int rotationIndex = 0;
    Vector3[] directions = new Vector3[] { Vector3.up, Vector3.right, Vector3.down, Vector3.left };

    void Awake()
    {
        currentDirection = directions[rotationIndex];
    }

    public void Rotate()
    {
        transform.Rotate(0, 0, -90);
        rotationIndex= ++rotationIndex % directions.Length;
        currentDirection = directions[rotationIndex];
    }

}
