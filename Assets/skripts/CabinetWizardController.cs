using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CabinetWizardController : MonoBehaviour
{
    [Header("Екрани візарда")]
    public GameObject step1Screen;
    public GameObject step2Screen;
    public GameObject step3Screen;

    [Header("Текстові елементи")]
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI resultSummaryText;
    public TextMeshProUGUI sliderValueLabel;

    [Header("Елементи форми")]
    public Toggle powerToggle;
    public Slider voltageSlider;
    public TMP_InputField technicianIdInput;

    private void Start()
    {
        if (voltageSlider != null)
        {
            voltageSlider.onValueChanged.AddListener(UpdateSliderLabel);
            UpdateSliderLabel(voltageSlider.value);
        }

        GoToStep1();
    }

    public void UpdateSliderLabel(float value)
    {
        if (sliderValueLabel != null)
            sliderValueLabel.text = $"Контрольна напруга: {value:0} В";
    }

    public void GoToStep1()
    {
        step1Screen.SetActive(true);
        step2Screen.SetActive(false);
        step3Screen.SetActive(false);
        headerText.text = "Журнал допуску: Крок 1 з 3";
    }

    public void GoToStep2()
    {
        step1Screen.SetActive(false);
        step2Screen.SetActive(true);
        step3Screen.SetActive(false);
        headerText.text = "Параметри щитка: Крок 2 з 3";
    }

    public void SubmitAndShowResult()
    {
        step1Screen.SetActive(false);
        step2Screen.SetActive(false);
        step3Screen.SetActive(true);
        headerText.text = "Акт перевірки: Крок 3 з 3";

        string techId = string.IsNullOrWhiteSpace(technicianIdInput.text) ? "Не вказано" : technicianIdInput.text;
        string powerStatus = powerToggle.isOn ? "УВІМКНЕНО" : "ВИМКНЕНО";
        float voltage = voltageSlider.value;

        bool isPassed = powerToggle.isOn && voltage >= 360f;

        resultSummaryText.text = $"<b>АКТ ДОПУСКУ ДО ЕЛЕКТРОЩИТКА 380В</b>\n\n" +
                                 $"• Технік (ID): <b>{techId}</b>\n" +
                                 $"• Головний рубильник: <b>{powerStatus}</b>\n" +
                                 $"• Зафіксована напруга: <b>{voltage} В</b>\n\n" +
                                 $"<color={(isPassed ? "#22C55E" : "#EF4444")}>" +
                                 $"ВЕРДИКТ: {(isPassed ? "ДОПУСК НАДАНО (НОРМА)" : "ВІДХИЛЕНО: НЕБЕЗПЕЧНІ ПАРАМЕТРИ")}</color>";
    }

    public void ResetWizard()
    {
        if (powerToggle != null) powerToggle.isOn = false;
        if (voltageSlider != null) voltageSlider.value = 0;
        if (technicianIdInput != null) technicianIdInput.text = string.Empty;
        GoToStep1();
    }
}