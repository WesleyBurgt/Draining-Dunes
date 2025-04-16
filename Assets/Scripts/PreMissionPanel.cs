using UnityEngine;
using UnityEngine.UI;

public class PreMissionPanel : MonoBehaviour
{
    [SerializeField] private DeliverySystem _deliverySystem;
    [SerializeField] private Toggle _preMissionRefuelToggle;
    [SerializeField] private Toggle _preMissionRepairToggle;
    [SerializeField] private Button _preMissionAcceptButton;
    [SerializeField] private Button _preMissionCancelButton;

    void Start()
    {
        _preMissionAcceptButton.onClick.AddListener(AcceptMission);
        _preMissionCancelButton.onClick.AddListener(CancelMission);
    }

    void LateUpdate()
    {

    }

    void AcceptMission()
    {
        _deliverySystem.AssignMission(_deliverySystem.WantsToStartMissionDeliveryPort);
        if (_preMissionRefuelToggle.isOn)
        {
            _deliverySystem.RefuelCar();
        }
        if (_preMissionRepairToggle.isOn)
        {
            _deliverySystem.RepairCar();
        }
        _deliverySystem.WantsToStartMissionDeliveryPort = null;
    }

    void CancelMission()
    {
        _deliverySystem.WantsToStartMissionDeliveryPort = null;
    }
}
