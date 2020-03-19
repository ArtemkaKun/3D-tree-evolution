using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float sensitivityVert = 5.0f;
    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;
    private float rotationX;
    private float rotationY;

    private static bool _canMove = false;
    
    void Update()
    {
        if (_canMove)
        {
            rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            rotationY += Input.GetAxis("Mouse X") * sensitivityVert;
            rotationX = Mathf.Clamp(rotationX, minimumVert, maximumVert);
            transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);

            var deltaX = Input.GetAxis("Horizontal") * 6f;
            var deltaZ = Input.GetAxis("Vertical") * 6f;
            transform.Translate(deltaX * Time.deltaTime, 0, deltaZ * Time.deltaTime);
        }
    }

    public static void SwitchMove()
    {
        _canMove = !_canMove;
    }
}
