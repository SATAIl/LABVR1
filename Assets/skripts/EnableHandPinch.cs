using UnityEngine;
using UnityEngine.InputSystem;

public class EnableHandPinch : MonoBehaviour
{
    [SerializeField] private InputActionReference pinch;

    private void OnEnable()
    {
        if (pinch != null)
            pinch.action.Enable();
    }

    private void OnDisable()
    {
        if (pinch != null)
            pinch.action.Disable();
    }
}