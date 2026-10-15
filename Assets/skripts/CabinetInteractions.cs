using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CabinetInteractions : MonoBehaviour
{
    [Header("Дверцята")]
    public Transform doorHinge;
    public float openAngle = 115f;
    private bool _isDoorOpen = false;
    private Coroutine _doorAnim;

    [Header("Рубильник (90 градусів)")]
    public Transform leverPivot;
    public float leverSwitchAngle = -90f;
    public Toggle tabletPowerToggle;
    private bool _isSwitchOn = false;
    private Coroutine _leverAnim;

    [Header("Запобіжники")]
    public Renderer[] fuses;

    private void Awake()
    {
        // АВТО-ВІДНОВЛЕННЯ ЗВ'ЯЗКІВ, якщо вони злетіли після префаба:
        if (doorHinge == null)
        {
            doorHinge = transform.Find("Door_Hinge");
        }

        if (leverPivot == null)
        {
            leverPivot = transform.Find("Switch_Lever_Pivot");
        }

        if (openAngle == 0f)
        {
            openAngle = 115f;
        }

        if (fuses == null || fuses.Length == 0)
        {
            List<Renderer> foundFuses = new List<Renderer>();
            for (int i = 1; i <= 3; i++)
            {
                Transform f = transform.Find($"Fuse_Phase_{i}");
                if (f != null && f.TryGetComponent<Renderer>(out var r))
                    foundFuses.Add(r);
            }
            fuses = foundFuses.ToArray();
        }

        if (tabletPowerToggle == null)
        {
            GameObject toggleObj = GameObject.Find("Toggle_Power");
            if (toggleObj != null) tabletPowerToggle = toggleObj.GetComponent<Toggle>();
        }
    }

    private void Update()
    {
        // Перевірка кліку миші (підтримує і стару, і нову систему вводу)
        bool mouseClicked = false;
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            mouseClicked = true;
#endif
        if (Input.GetMouseButtonDown(0))
            mouseClicked = true;

        if (mouseClicked)
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Camera cam = Camera.main != null ? Camera.main : FindObjectOfType<Camera>();
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 15f))
        {
            // 1. Клік по дверцятах або ручці дверцят
            if (doorHinge != null && (hit.transform == doorHinge || hit.transform.IsChildOf(doorHinge) || hit.transform.name.Contains("Door")))
            {
                ToggleDoor();
                return;
            }

            // 2. Клік по рубильнику
            if (leverPivot != null && (hit.transform == leverPivot || hit.transform.IsChildOf(leverPivot) || hit.transform.name.Contains("Switch")))
            {
                ToggleLever();
                return;
            }

            // 3. Клік по запобіжниках
            if (fuses != null)
            {
                for (int i = 0; i < fuses.Length; i++)
                {
                    if (fuses[i] != null && (hit.transform == fuses[i].transform || hit.transform.IsChildOf(fuses[i].transform)))
                    {
                        InspectFuse(i);
                        return;
                    }
                }
            }
        }
    }

    [ContextMenu("Відчинити / Зачинити дверцята")]
    public void ToggleDoor()
    {
        if (doorHinge == null)
        {
            doorHinge = transform.Find("Door_Hinge");
            if (doorHinge == null)
            {
                Debug.LogError("[ЩИТОК] Об'єкт Door_Hinge не знайдено в ієрархії щитка!");
                return;
            }
        }

        _isDoorOpen = !_isDoorOpen;
        if (_doorAnim != null) StopCoroutine(_doorAnim);
        _doorAnim = StartCoroutine(AnimateDoor(_isDoorOpen));
        Debug.Log($"<color=yellow>[ЩИТОК] Клік по дверцятах! Стан: {(_isDoorOpen ? "ВІДЧИНЕНО" : "ЗАЧИНЕНО")}</color>");
    }

    private IEnumerator AnimateDoor(bool open)
    {
        Quaternion targetRot = open ? Quaternion.Euler(0, openAngle, 0) : Quaternion.identity;
        Quaternion startRot = doorHinge.localRotation;
        float elapsed = 0f;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            doorHinge.localRotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            yield return null;
        }
        doorHinge.localRotation = targetRot;
    }

    [ContextMenu("Перемкнути рубильник")]
    public void ToggleLever()
    {
        if (leverPivot == null) leverPivot = transform.Find("Switch_Lever_Pivot");

        _isSwitchOn = !_isSwitchOn;
        if (_leverAnim != null) StopCoroutine(_leverAnim);
        _leverAnim = StartCoroutine(AnimateLeverRoutine(_isSwitchOn));

        if (tabletPowerToggle != null)
        {
            tabletPowerToggle.isOn = _isSwitchOn;
        }

        Debug.Log($"<color=cyan>[ЩИТОК] Клік по рубильнику! Стан: {(_isSwitchOn ? "УВІМКНЕНО (90°, живлення подано)" : "ВИМКНЕНО (0°)")}</color>");
    }

    private IEnumerator AnimateLeverRoutine(bool stateOn)
    {
        Quaternion targetRot = stateOn ? Quaternion.Euler(0, 0, leverSwitchAngle) : Quaternion.identity;
        Quaternion startRot = leverPivot != null ? leverPivot.localRotation : Quaternion.identity;
        float elapsed = 0f;
        float duration = 0.15f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (leverPivot != null)
                leverPivot.localRotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            yield return null;
        }
        if (leverPivot != null) leverPivot.localRotation = targetRot;
    }

    public void InspectFuse(int index)
    {
        if (fuses != null && index < fuses.Length && fuses[index] != null)
        {
            fuses[index].material.color = new Color(0.1f, 1f, 0.2f);
            string phaseName = index == 0 ? "A (L1)" : index == 1 ? "B (L2)" : "C (L3)";
            Debug.Log($"<color=green>[ТЕСТ МУЛЬТИМЕТРОМ] Запобіжник {phaseName}: СПРАВНИЙ! Ланцюг замкнено.</color>");
        }
    }
}