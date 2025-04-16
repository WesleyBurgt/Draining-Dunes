using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndMissionPanel : MonoBehaviour
{
    [SerializeField] private DeliverySystem _deliverySystem;
    [SerializeField] private TMP_Text _baseRewardText;
    [SerializeField] private TMP_Text _speedBonusText;
    [SerializeField] private Toggle _refuelToggle;
    [SerializeField] private TMP_Text _refuelCostText;
    [SerializeField] private Toggle _repairToggle;
    [SerializeField] private TMP_Text _repairCostText;
    [SerializeField] private TMP_Text _totalRewardText;
    [SerializeField] private Button _okButton;

    void Start()
    {
        _okButton.onClick.AddListener(EndMissionOk);
    }

    void LateUpdate()
    {
        ShowMissionRewards(_deliverySystem.EndedMission);
    }

    void EndMissionOk()
    {
        if (_refuelToggle.isOn)
        {
            _deliverySystem.RefuelCar();
        }
        if (_repairToggle.isOn)
        {
            _deliverySystem.RepairCar();
        }
        _deliverySystem.EndedMission = null;
    }

    void ShowMissionRewards(DeliveryMission endedMission)
    {
        int totalReward = endedMission.GetReward();

        _baseRewardText.text = $"${endedMission.GetBaseReward()}";
        _speedBonusText.text = $"${endedMission.GetSpeedBonus()}";

        if (_refuelToggle.isOn)
        {
            int refuelCost = _deliverySystem.GetRefuelCarCost();
            _refuelCostText.text = $"-${refuelCost}";
            totalReward -= refuelCost;
        }
        else
        {
            _refuelCostText.text = string.Empty;
        }

        if (_repairToggle.isOn)
        {
            int repairCost = _deliverySystem.GetRepairCarCost();
            _repairCostText.text = $"-${repairCost}";
            totalReward -= repairCost;
        }
        else
        {
            _repairCostText.text = string.Empty;
        }

        _totalRewardText.text = $"${totalReward}";
    }
}
