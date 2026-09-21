using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WizardController : MonoBehaviour
{
    [Header("Екрани візарда")]
    public GameObject step1Screen;
    public GameObject step2Screen;
    public GameObject step3Screen;

    [Header("Текстові поля")]
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI resultSummaryText;
    public TextMeshProUGUI sliderValueLabel;

    [Header("Елементи форми")]
    public Toggle powerToggle;
    public Slider pressureSlider;
    public TMP_InputField operatorIdInput;

    // Змінні для телеметрії (Лабораторна 5)
    private float wizardStartTime;
    private int validationErrorsCount = 0;

    private void Start()
    {
        if (pressureSlider != null)
        {
            pressureSlider.onValueChanged.AddListener(UpdateSliderLabel);
            UpdateSliderLabel(pressureSlider.value);
        }

        GoToStep1();
    }

    public void UpdateSliderLabel(float value)
    {
        if (sliderValueLabel != null)
            sliderValueLabel.text = $"Робочий тиск: {value:0}%";
    }

    public void GoToStep1()
    {
        step1Screen.SetActive(true);
        step2Screen.SetActive(false);
        step3Screen.SetActive(false);
        headerText.text = "Інструктаж: Крок 1 з 3";
    }

    public void GoToStep2()
    {
        step1Screen.SetActive(false);
        step2Screen.SetActive(true);
        step3Screen.SetActive(false);
        headerText.text = "Налаштування форми: Крок 2 з 3";

        // Фіксуємо початок роботи оператора
        wizardStartTime = Time.time;
        validationErrorsCount = 0;
    }

    public void SubmitAndShowResult()
    {
        float completionDuration = Time.time - wizardStartTime;
        string opId = string.IsNullOrWhiteSpace(operatorIdInput.text) ? "Operator_Default" : operatorIdInput.text;
        bool isPowerOk = powerToggle.isOn;
        float pressure = pressureSlider.value;

        // Фіксація помилок у діях користувача
        if (!isPowerOk) validationErrorsCount++;
        if (pressure < 20f) validationErrorsCount++;

        step1Screen.SetActive(false);
        step2Screen.SetActive(false);
        step3Screen.SetActive(true);
        headerText.text = "Підсумок перевірки: Крок 3 з 3";

        resultSummaryText.text = $"<b>Звіт перевірки стенду:</b>\n\n" +
                                 $"• Оператор: {opId}\n" +
                                 $"• Час проходження: {completionDuration:F1} сек\n" +
                                 $"• Виявлено зауважень: {validationErrorsCount}\n" +
                                 $"• Живлення: {(isPowerOk ? "Увімкнено" : "Вимкнено")}\n" +
                                 $"• Тиск: {pressure}%\n\n" +
                                 $"<color={(isPowerOk ? "green" : "red")}>" +
                                 $"Статус: {(isPowerOk ? "ГОТОВО ДО ЕКСПЛУАТАЦІЇ" : "ПОМИЛКА ЖИВЛЕННЯ")}</color>";

        // ФОРМУВАННЯ ТА ВІДПРАВКА ТЕЛЕМЕТРІЇ (POST)
        WizardTelemetryReport report = new WizardTelemetryReport
        {
            operatorId = opId,
            completionTimeSeconds = (float)Math.Round(completionDuration, 2),
            errorsCount = validationErrorsCount,
            systemPowerStatus = isPowerOk,
            targetPressure = pressure,
            timestamp = DateTime.UtcNow.ToString("o")
        };

        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.SendTelemetryReport(report);
        }
    }

    public void ResetWizard()
    {
        if (powerToggle != null) powerToggle.isOn = false;
        if (pressureSlider != null) pressureSlider.value = 0;
        if (operatorIdInput != null) operatorIdInput.text = string.Empty;
        GoToStep1();
    }
}