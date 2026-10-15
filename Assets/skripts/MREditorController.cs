using UnityEngine;

public class MREditorController : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float lookSpeed = 2.0f;

    private float _rotationX = 0f;
    private float _rotationY = 0f;

    private void Update()
    {
        // Огляд мишкою при затиснутій правій кнопці
        if (Input.GetMouseButton(1))
        {
            _rotationX += Input.GetAxis("Mouse X") * lookSpeed;
            _rotationY -= Input.GetAxis("Mouse Y") * lookSpeed;
            _rotationY = Mathf.Clamp(_rotationY, -80f, 80f);

            transform.localEulerAngles = new Vector3(_rotationY, _rotationX, 0);
        }

        // Пересування клавішами WASD
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        transform.Translate(new Vector3(moveX, 0, moveZ));
    }
}