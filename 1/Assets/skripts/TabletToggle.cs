using UnityEngine;
using UnityEngine.InputSystem;

public class TabletToggle : MonoBehaviour
{
    [Header("Об'єкт планшета")]
    public GameObject spatialTablet;

    [Header("Дистанція від очей (метри)")]
    public float distance = 1.2f;

    void Update()
    {
        bool tabPressed = Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame;
        bool mPressed = Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame;

        if (tabPressed || mPressed)
        {
            ToggleTablet();
        }
    }

    public void ToggleTablet()
    {
        if (spatialTablet == null) return;

        bool isOpening = !spatialTablet.activeSelf;
        spatialTablet.SetActive(isOpening);
        
        if (isOpening)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 forwardDirection = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
                
                spatialTablet.transform.position = cam.transform.position + (forwardDirection * distance) + new Vector3(0, -0.1f, 0);
                
                spatialTablet.transform.rotation = Quaternion.LookRotation(forwardDirection);
            }
        }
    }
}