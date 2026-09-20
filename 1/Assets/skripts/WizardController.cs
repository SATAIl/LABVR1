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
    }

    public void SubmitAndShowResult()
    {
        step1Screen.SetActive(false);
        step2Screen.SetActive(false);
        step3Screen.SetActive(true);
        headerText.text = "Підсумок перевірки: Крок 3 з 3";

        string opId = string.IsNullOrWhiteSpace(operatorIdInput.text) ? "Не вказано" : operatorIdInput.text;
        string power = powerToggle.isOn ? "Увімкнено" : "Вимкнено";
        float pressure = pressureSlider.value;

        resultSummaryText.text = $"<b>Звіт перевірки стенду:</b>\n\n" +
                                 $"• Оператор: {opId}\n" +
                                 $"• Живлення системи: {power}\n" +
                                 $"• Встановлений тиск: {pressure}%\n\n" +
                                 $"<color={(powerToggle.isOn ? "green" : "red")}>" +
                                 $"Статус: {(powerToggle.isOn ? "ГОТОВО ДО ЕКСПЛУАТАЦІЇ" : "ПОМИЛКА: ВІДСУТНЄ ЖИВЛЕННЯ")}</color>";
    }

    public void ResetWizard()
    {
        if (powerToggle != null) powerToggle.isOn = false;
        if (pressureSlider != null) pressureSlider.value = 0;
        if (operatorIdInput != null) operatorIdInput.text = string.Empty;
        GoToStep1();
    }
}