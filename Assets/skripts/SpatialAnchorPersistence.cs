using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OVRSpatialAnchor))]
public class SpatialAnchorPersistence : MonoBehaviour
{
    private const string AnchorUuidKey = "MR_ElectricalCabinet_Anchor_UUID";
    private const string PosXKey = "MR_Backup_PosX";
    private const string PosYKey = "MR_Backup_PosY";
    private const string PosZKey = "MR_Backup_PosZ";

    private OVRSpatialAnchor _spatialAnchor;

    [Header("Статус нативного якоря Meta")]
    public string currentAnchorUUID;
    public bool isAnchorLoadedSuccessfully = false;

    private void Awake()
    {
        _spatialAnchor = GetComponent<OVRSpatialAnchor>();
    }

    private void Start()
    {
        StartCoroutine(InitializeSpatialAnchorRoutine());
    }

    private IEnumerator InitializeSpatialAnchorRoutine()
    {
        yield return new WaitForSeconds(0.4f);

        if (PlayerPrefs.HasKey(AnchorUuidKey))
        {
            LoadMetaSpatialAnchor();
        }
        else
        {
            StartCoroutine(CreateAndSaveMetaSpatialAnchor());
        }
    }

    private IEnumerator CreateAndSaveMetaSpatialAnchor()
    {
        // Очікуємо створення нативного дескриптора якоря
        while (_spatialAnchor != null && !_spatialAnchor.Created)
        {
            yield return null;
        }

        if (_spatialAnchor != null && _spatialAnchor.Created)
        {
            // Справжнє збереження просторового якоря Meta XR
            _spatialAnchor.Save((anchor, success) =>
            {
                if (success)
                {
                    currentAnchorUUID = anchor.Uuid.ToString();
                    PlayerPrefs.SetString(AnchorUuidKey, currentAnchorUUID);
                    SaveTransformBackup();
                    Debug.Log($"<color=green>[OVRSpatialAnchor] Нативний якір Meta успішно збережено в Local Persistence! UUID: {currentAnchorUUID}</color>");
                }
            });
        }
        else
        {
            // Емуляція в Editor (якщо симулятор працює без апаратного HMD)
            currentAnchorUUID = Guid.NewGuid().ToString();
            PlayerPrefs.SetString(AnchorUuidKey, currentAnchorUUID);
            SaveTransformBackup();
            Debug.Log($"<color=green>[OVRSpatialAnchor (Editor Fallback)] Якір ініціалізовано з UUID: {currentAnchorUUID}</color>");
        }
    }

    private void LoadMetaSpatialAnchor()
    {
        currentAnchorUUID = PlayerPrefs.GetString(AnchorUuidKey);

        if (Guid.TryParse(currentAnchorUUID, out Guid targetGuid))
        {
            // Нативне відновлення просторового якоря Meta за списком UUID
            var options = new OVRSpatialAnchor.LoadOptions
            {
                Timeout = 0,
                Uuids = new[] { targetGuid }
            };

            OVRSpatialAnchor.LoadUnboundAnchors(options, unboundAnchors =>
            {
                if (unboundAnchors != null && unboundAnchors.Length > 0)
                {
                    unboundAnchors[0].Localize((anchor, success) =>
                    {
                        if (success)
                        {
                            anchor.BindTo(_spatialAnchor);
                            isAnchorLoadedSuccessfully = true;
                            Debug.Log($"<color=yellow>[OVRSpatialAnchor] Успішно відновлено нативний просторовий якір Meta за UUID: {targetGuid}</color>");
                        }
                    });
                }
                else
                {
                    RestoreTransformBackup();
                }
            });
        }
        else
        {
            RestoreTransformBackup();
        }
    }

    private void SaveTransformBackup()
    {
        PlayerPrefs.SetFloat(PosXKey, transform.position.x);
        PlayerPrefs.SetFloat(PosYKey, transform.position.y);
        PlayerPrefs.SetFloat(PosZKey, transform.position.z);
        PlayerPrefs.Save();
    }

    private void RestoreTransformBackup()
    {
        float x = PlayerPrefs.GetFloat(PosXKey, transform.position.x);
        float y = PlayerPrefs.GetFloat(PosYKey, transform.position.y);
        float z = PlayerPrefs.GetFloat(PosZKey, transform.position.z);
        transform.position = new Vector3(x, y, z);
        isAnchorLoadedSuccessfully = true;
        Debug.Log($"<color=yellow>[OVRSpatialAnchor] Позицію планшета відновлено з локальної пам'яті за UUID {currentAnchorUUID}: ({x:F2}, {y:F2}, {z:F2})</color>");
    }

    [ContextMenu("Скинути якір (Clear Cache)")]
    public void ResetAnchorData()
    {
        PlayerPrefs.DeleteKey(AnchorUuidKey);
        PlayerPrefs.DeleteKey(PosXKey);
        PlayerPrefs.DeleteKey(PosYKey);
        PlayerPrefs.DeleteKey(PosZKey);
        PlayerPrefs.Save();
        currentAnchorUUID = string.Empty;
        isAnchorLoadedSuccessfully = false;
        Debug.Log("[OVRSpatialAnchor] Локальний кеш якоря очищено.");
    }
}