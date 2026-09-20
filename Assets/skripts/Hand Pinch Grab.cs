using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HandPinchGrab : MonoBehaviour
{
    [Header("Hand")]
    [SerializeField] private Transform indexTip;
    [SerializeField] private Transform thumbTip;

    [Header("XRI")]
    [SerializeField] private XRDirectInteractor directInteractor;

    [Header("Pinch Attach")]
    [SerializeField] private Transform pinchAttach;

    [Header("Pinch Settings")]
    [SerializeField] private float pinchDistance = 0.035f;
    [SerializeField] private float releaseDistance = 0.055f;

    [Header("Grab Search")]
    [SerializeField] private float grabRadius = 0.12f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabbedObject;
    private bool isPinching;

    private void Update()
    {
        if (indexTip == null || thumbTip == null)
            return;

        // Позиція між великим та вказівним пальцями
        Vector3 pinchPosition =
            (indexTip.position + thumbTip.position) * 0.5f;

        // Переміщуємо точку захоплення
        if (pinchAttach != null)
        {
            pinchAttach.position = pinchPosition;
        }

        // Відстань між великим та вказівним пальцями
        float distance = Vector3.Distance(
            indexTip.position,
            thumbTip.position
        );

        // PINCH
        if (!isPinching && distance <= pinchDistance)
        {
            isPinching = true;
            TryGrab();
        }

        // RELEASE
        if (isPinching && distance >= releaseDistance)
        {
            Release();
        }
    }

    private void TryGrab()
    {
        if (directInteractor == null)
            return;

        if (grabbedObject != null)
            return;

        Vector3 pinchPosition =
            (indexTip.position + thumbTip.position) * 0.5f;

        // Шукаємо колайдери біля пальців
        Collider[] colliders = Physics.OverlapSphere(
            pinchPosition,
            grabRadius
        );

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable closestGrab = null;
        float closestDistance = float.MaxValue;

        foreach (Collider col in colliders)
        {
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab =
                col.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

            if (grab == null)
                continue;

            float distance =
                Vector3.Distance(
                    pinchPosition,
                    grab.transform.position
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestGrab = grab;
            }
        }

        // Нічого не знайдено
        if (closestGrab == null)
            return;

        grabbedObject = closestGrab;

        // Ручне захоплення через XRI
        directInteractor.StartManualInteraction(
            (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)grabbedObject
        );
    }

    private void Release()
    {
        if (grabbedObject != null &&
            directInteractor != null)
        {
            directInteractor.EndManualInteraction();
        }

        grabbedObject = null;
        isPinching = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (indexTip == null || thumbTip == null)
            return;

        Vector3 pinchPosition =
            (indexTip.position + thumbTip.position) * 0.5f;

        Gizmos.DrawWireSphere(
            pinchPosition,
            grabRadius
        );
    }
}