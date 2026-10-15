using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CabinetTableBinder : MonoBehaviour
{
    [Header("Цифровий двійник щитка (об'єкт зі сцени або префаб)")]
    public GameObject cabinetObject;

    [Header("Посилання на OVRSceneManager (опціонально)")]
    public OVRSceneManager sceneManager;

    private GameObject _spawnedCabinet;

    private void Awake()
    {
        if (sceneManager == null)
            sceneManager = FindObjectOfType<OVRSceneManager>();

        // Якщо об'єкт не призначено вручну, автоматично знаходимо його в ієрархії
        if (cabinetObject == null)
            cabinetObject = GameObject.Find("ElectricalCabinet");

        // Ініціалізація семантики TABLE для тестової площини столу в Editor
        GameObject tableObj = GameObject.Find("RealTable_Plane");
        if (tableObj != null)
        {
            var classification = tableObj.GetComponent<OVRSemanticClassification>();
            if (classification != null)
            {
                var field = typeof(OVRSemanticClassification).GetField("_labels", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    var list = field.GetValue(classification) as List<string> ?? new List<string>();
                    if (!list.Contains(OVRSceneManager.Classification.Table))
                    {
                        list.Add(OVRSceneManager.Classification.Table);
                        field.SetValue(classification, list);
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        if (sceneManager != null)
            sceneManager.SceneModelLoadedSuccessfully += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (sceneManager != null)
            sceneManager.SceneModelLoadedSuccessfully -= OnSceneLoaded;
    }

    private void Start()
    {
        StartCoroutine(ScanAndSnapRoutine());
    }

    private void OnSceneLoaded()
    {
        Debug.Log("<color=cyan>[MR Scene API] Отримано подію SceneModelLoadedSuccessfully від OVRSceneManager!</color>");
        StartCoroutine(ScanAndSnapRoutine());
    }

    private IEnumerator ScanAndSnapRoutine()
    {
        yield return new WaitForSeconds(0.3f);

        OVRSceneAnchor[] anchors = FindObjectsOfType<OVRSceneAnchor>();
        bool tableFound = false;

        foreach (var anchor in anchors)
        {
            var classification = anchor.GetComponent<OVRSemanticClassification>();
            
            bool hasTableLabel = false;
            if (classification != null)
            {
                if (classification.Contains(OVRSceneManager.Classification.Table))
                {
                    hasTableLabel = true;
                }
                else if (classification.Labels != null)
                {
                    foreach (var label in classification.Labels)
                    {
                        if (string.Equals(label, OVRSceneManager.Classification.Table, System.StringComparison.OrdinalIgnoreCase))
                        {
                            hasTableLabel = true;
                            break;
                        }
                    }
                }
            }

            if (hasTableLabel)
            {
                tableFound = true;
                
                // Розрахунок поверхні столу: центр + половина товщини
                float tableTopY = anchor.transform.position.y + (anchor.transform.localScale.y * 0.5f);
                Vector3 cabinetPosition = new Vector3(anchor.transform.position.x, tableTopY + 0.35f, anchor.transform.position.z);

                if (cabinetObject != null)
                {
                    // Якщо об'єкт вже є на сцені — просто переміщуємо та примагнічуємо до столу
                    if (cabinetObject.scene.IsValid())
                    {
                        cabinetObject.transform.position = cabinetPosition;
                        cabinetObject.transform.rotation = anchor.transform.rotation;
                        Debug.Log($"<color=cyan>[MR Scene API] УСПІХ: Знайдено стіл '{anchor.name}'. Об'єкт '{cabinetObject.name}' примагнічено (Snap) до площини столу!</color>");
                    }
                    else // Якщо це префаб з папки Project
                    {
                        if (_spawnedCabinet == null)
                            _spawnedCabinet = Instantiate(cabinetObject, cabinetPosition, anchor.transform.rotation);
                        else
                            _spawnedCabinet.transform.position = cabinetPosition;

                        Debug.Log($"<color=cyan>[MR Scene API] УСПІХ: Знайдено стіл '{anchor.name}'. Префаб заспавнено на стільниці!</color>");
                    }
                }

                break;
            }
        }

        if (!tableFound)
        {
            Debug.LogWarning("[MR Scene API] Площину з семантикою TABLE не знайдено!");
        }
    }
}