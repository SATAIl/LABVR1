using System;

[Serializable]
public class WeatherCurrentData
{
    public string time;
    public float temperature_2m;
    public int relative_humidity_2m;
}

[Serializable]
public class WeatherApiResponse
{
    public float latitude;
    public float longitude;
    public WeatherCurrentData current;
}

[Serializable]
public class WizardTelemetryReport
{
    public string operatorId;
    public float completionTimeSeconds;
    public int errorsCount;
    public bool systemPowerStatus;
    public float targetPressure;
    public string timestamp;
}