using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndMissionPanel : MonoBehaviour
{
    [SerializeField] private DeliverySystem _deliverySystem;
    [SerializeField] private TMP_Text _endMissionBaseRewardText;
    [SerializeField] private TMP_Text _endMissionSpeedBonusText;
    [SerializeField] private Toggle _endMissionRefuelToggle;
    [SerializeField] private TMP_Text _endMissionRefuelCostText;
    [SerializeField] private Toggle _endMissionRepairToggle;
    [SerializeField] private TMP_Text _endMissionRepairCostText;
    [SerializeField] private TMP_Text _endMissionTotalRewardText;
    [SerializeField] private Button _endMissionOkButton;

    void Start()
    {
        _endMissionOkButton.onClick.AddListener(EndMissionOk);
    }

    void LateUpdate()
    {
        ShowMissionRewards(_deliverySystem.EndedMission);
    }

    void EndMissionOk()
    {
        if (_endMissionRefuelToggle.isOn)
        {
            _deliverySystem.RefuelCar();
        }
        if (_endMissionRepairToggle.isOn)
        {
            _deliverySystem.RepairCar();
        }
        _deliverySystem.EndedMission = null;
    }

    void ShowMissionRewards(DeliveryMission endedMission)
    {
        _endMissionBaseRewardText.text = endedMission.GetBaseReward().ToString();
        _endMissionSpeedBonusText.text = endedMission.GetSpeedBonus().ToString();

        if (_endMissionRefuelToggle.isOn)
        {
            _endMissionRefuelCostText.text = _deliverySystem.GetRefuelCarCost().ToString();
        }
        else
        {
            _endMissionRefuelCostText.text = string.Empty;
        }

        if (_endMissionRepairToggle.isOn)
        {
            _endMissionRepairCostText.text = _deliverySystem.GetRepairCarCost().ToString();
        }
        else
        {
            _endMissionRepairCostText.text = string.Empty;
        }
    }
}
