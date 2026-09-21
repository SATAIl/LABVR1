using UnityEngine;
using TMPro;

public class SpatialNetworkUI : MonoBehaviour
{
    [Header("UI Елементи")]
    [SerializeField] private TextMeshProUGUI networkStatusText;
    [SerializeField] private TextMeshProUGUI weatherDashboardText;

    private void OnEnable()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.OnNetworkStatusChanged += UpdateStatusBadge;
            NetworkManager.Instance.OnWeatherDataReceived += DisplayWeatherData;
        }
    }

    private void Start()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.OnNetworkStatusChanged += UpdateStatusBadge;
            NetworkManager.Instance.OnWeatherDataReceived += DisplayWeatherData;
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.OnNetworkStatusChanged -= UpdateStatusBadge;
            NetworkManager.Instance.OnWeatherDataReceived -= DisplayWeatherData;
        }
    }

    private void UpdateStatusBadge(NetworkManager.NetworkState state, string message)
    {
        if (networkStatusText == null) return;

        string colorHex = state switch
        {
            NetworkManager.NetworkState.Loading => "#FACC15", // Жовтий
            NetworkManager.NetworkState.Success => "#22C55E", // Зелений
            NetworkManager.NetworkState.Error => "#EF4444",   // Червоний
            _ => "#9CA3AF"                                   // Сірий
        };

        networkStatusText.text = $"<color={colorHex}>●</color> {message}";
    }

    private void DisplayWeatherData(WeatherApiResponse data)
    {
        if (weatherDashboardText == null || data?.current == null) return;

        weatherDashboardText.text = 
            $"<b>Клімат цеху (IoT Датчики):</b>\n" +
            $"• Температура: <b>{data.current.temperature_2m:F1} °C</b>\n" +
            $"• Вологість: <b>{data.current.relative_humidity_2m}%</b>";
    }
}