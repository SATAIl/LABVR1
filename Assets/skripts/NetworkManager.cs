using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    public enum NetworkState { Idle, Loading, Success, Error }
    
    public event Action<NetworkState, string> OnNetworkStatusChanged;
    public event Action<WeatherApiResponse> OnWeatherDataReceived;
    public event Action<string> OnTelemetrySentSuccess;

    [Header("API Endpoints")]
    [SerializeField] private string weatherGetUrl = "https://api.open-meteo.com/v1/forecast?latitude=50.45&longitude=30.52&current=temperature_2m,relative_humidity_2m";
    [SerializeField] private string telemetryPostUrl = "https://jsonplaceholder.typicode.com/posts";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        FetchWeatherData();
    }

    public void FetchWeatherData()
    {
        StartCoroutine(GetWeatherRoutine());
    }

    public void SendTelemetryReport(WizardTelemetryReport report)
    {
        StartCoroutine(PostTelemetryRoutine(report));
    }
    // --- GET ЗАПИТ ---
    private IEnumerator GetWeatherRoutine()
    {
        OnNetworkStatusChanged?.Invoke(NetworkState.Loading, "Отримання даних датчиків...");

        using (UnityWebRequest request = UnityWebRequest.Get(weatherGetUrl))
        {
            request.timeout = 10;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || 
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                string errorMsg = $"Помилка GET: {request.error}";
                Debug.LogError(errorMsg);
                OnNetworkStatusChanged?.Invoke(NetworkState.Error, errorMsg);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"GET Відповідь: {jsonResponse}");

                try
                {
                    WeatherApiResponse data = JsonUtility.FromJson<WeatherApiResponse>(jsonResponse);
                    OnWeatherDataReceived?.Invoke(data);
                    OnNetworkStatusChanged?.Invoke(NetworkState.Success, "Дані датчиків цеху синхронізовано");
                }
                catch (Exception ex)
                {
                    OnNetworkStatusChanged?.Invoke(NetworkState.Error, "Помилка парсингу JSON");
                    Debug.LogError($"Помилка парсингу: {ex.Message}");
                }
            }
        }
    }
    
    private IEnumerator PostTelemetryRoutine(WizardTelemetryReport report)
    {
        OnNetworkStatusChanged?.Invoke(NetworkState.Loading, "Відправка телеметрії на сервер...");

        string jsonPayload = JsonUtility.ToJson(report);
        Debug.Log($"POST Пакет телеметрії: {jsonPayload}");

        using (UnityWebRequest request = new UnityWebRequest(telemetryPostUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 10;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || 
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                string errorMsg = $"Помилка POST: {request.error}";
                Debug.LogError(errorMsg);
                OnNetworkStatusChanged?.Invoke(NetworkState.Error, errorMsg);
            }
            else
            {
                string responseText = request.downloadHandler.text;
                Debug.Log($"Сервер підтвердив прийом телеметрії: {responseText}");
                OnTelemetrySentSuccess?.Invoke(responseText);
                OnNetworkStatusChanged?.Invoke(NetworkState.Success, "Звіт оператора успішно зареєстровано на сервері!");
            }
        }
    }
}