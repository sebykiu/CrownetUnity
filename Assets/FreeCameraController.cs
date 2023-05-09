using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 100.0f;

    private void Update()
    {
        // Camera movement
        float moveHorizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveVertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float moveUpDown = 0;

        if (Input.GetKey(KeyCode.Q))
        {
            moveUpDown = -moveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E))
        {
            moveUpDown = moveSpeed * Time.deltaTime;
        }

        transform.Translate(moveHorizontal, moveUpDown, moveVertical);

        // Camera rotation
        float rotationHorizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float rotationVertical = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        transform.Rotate(0, rotationHorizontal, 0, Space.World);
        transform.Rotate(-rotationVertical, 0, 0, Space.Self);
    }
}