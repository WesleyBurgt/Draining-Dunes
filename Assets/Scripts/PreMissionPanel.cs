using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PreMissionPanel : MonoBehaviour
{
    [SerializeField] private DeliverySystem _deliverySystem;
    [SerializeField] private TMP_Text _missionText;
    [SerializeField] private TMP_Text _rewardText;
    [SerializeField] private Toggle _preMissionRefuelToggle;
    [SerializeField] private TMP_Text _preMissionRefuelCostText;
    [SerializeField] private Toggle _preMissionRepairToggle;
    [SerializeField] private TMP_Text _preMissionRepairCostText;
    [SerializeField] private Button _preMissionAcceptButton;
    [SerializeField] private Button _preMissionCancelButton;

    void Start()
    {
        _preMissionAcceptButton.onClick.AddListener(AcceptMission);
        _preMissionCancelButton.onClick.AddListener(CancelMission);
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

            if (_preMissionRefuelToggle.isOn)
            {
                int refuelCost = _deliverySystem.GetRefuelCarCost();
                _preMissionRefuelCostText.text = $"-${refuelCost}";
            }
            else
            {
                _preMissionRefuelCostText.text = string.Empty;
            }

            if (_preMissionRepairToggle.isOn)
            {
                int repairCost = _deliverySystem.GetRepairCarCost();
                _preMissionRepairCostText.text = $"-${repairCost}";
            }
            else
            {
                _preMissionRepairCostText.text = string.Empty;
            }
        }
    }

    void AcceptMission()
    {
        _deliverySystem.AssignMission(_deliverySystem.WantsToStartMission);
        if (_preMissionRefuelToggle.isOn)
        {
            _deliverySystem.RefuelCar();
        }
        if (_preMissionRepairToggle.isOn)
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
