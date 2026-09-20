using UnityEngine;
using UnityEngine.InputSystem;

public class HandSimulatorAnimator : MonoBehaviour
{
    public Transform wristRoot; // Коренева кисть (R_Wrist)

    void Start()
    {
        if (wristRoot == null) wristRoot = transform;
    }

    void Update()
    {
        // Щипок: Пробіл, G або ЛКМ
        bool pinchInput = (Keyboard.current != null && (Keyboard.current.spaceKey.isPressed || Keyboard.current.gKey.isPressed)) ||
                          (Mouse.current != null && Mouse.current.leftButton.isPressed);

        if (pinchInput)
        {
            // Стискаємо долоню (імітація щипка)
            wristRoot.localScale = new Vector3(0.85f, 0.85f, 0.75f);
        }
        else
        {
            // Розслаблена кисть
            wristRoot.localScale = Vector3.one;
        }
    }
}