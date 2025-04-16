using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PreMissionPanel : MonoBehaviour
{
    [SerializeField] private DeliverySystem _deliverySystem;
    [SerializeField] private TMP_Text _missionText;
    [SerializeField] private TMP_Text _rewardText;
    [SerializeField] private Toggle _refuelToggle;
    [SerializeField] private TMP_Text _refuelCostText;
    [SerializeField] private Toggle _repairToggle;
    [SerializeField] private TMP_Text _repairCostText;
    [SerializeField] private Button _acceptButton;
    [SerializeField] private Button _cancelButton;

    void Start()
    {
        _acceptButton.onClick.AddListener(AcceptMission);
        _cancelButton.onClick.AddListener(CancelMission);
    }

    void LateUpdate()
    {
        if (_deliverySystem.WantsToStartMission != null)
        {
            string destinationName = _deliverySystem.WantsToStartMission.endDeliveryPort.name;
            string goods = "stone";
            _missionText.text = $"Deliver {goods} to {destinationName}";

            int baseMissionReward = _deliverySystem.WantsToStartMission.GetBaseReward();
            float dollarsPerKMPH = _deliverySystem.WantsToStartMission.GetSpeedBonusPerKMPH();
            _rewardText.text = $"Base Reward: ${baseMissionReward}\nSpeed Bonus: +${dollarsPerKMPH:F2} per kmph of average speed";

            if (_refuelToggle.isOn)
            {
                int refuelCost = _deliverySystem.GetRefuelCarCost();
                _refuelCostText.text = $"-${refuelCost}";
            }
            else
            {
                _refuelCostText.text = string.Empty;
            }

            if (_repairToggle.isOn)
            {
                int repairCost = _deliverySystem.GetRepairCarCost();
                _repairCostText.text = $"-${repairCost}";
            }
            else
            {
                _repairCostText.text = string.Empty;
            }
        }
    }

    void AcceptMission()
    {
        _deliverySystem.AssignMission(_deliverySystem.WantsToStartMission);
        if (_refuelToggle.isOn)
        {
            _deliverySystem.RefuelCar();
        }
        if (_repairToggle.isOn)
        {
            _deliverySystem.RepairCar();
        }
        _deliverySystem.WantsToStartMission = null;
    }

    void CancelMission()
    {
        _deliverySystem.WantsToStartMission = null;
    }
}
