using UnityEngine;
using UnityEngine.UI;

public class MissionPanelManager : MonoBehaviour
{
    [SerializeField] private DeliverySystem _deliverySystem;

    [Header("Pre Mission Panel")]
    [SerializeField] private GameObject _preMissionPanel;
    [SerializeField] private Toggle _preMissionRefuelToggle;
    [SerializeField] private Toggle _preMissionRepairToggle;
    [SerializeField] private Button _preMissionAcceptButton;
    [SerializeField] private Button _preMissionCancelButton;

    [Header("Mid Mission Panel")]
    [SerializeField] private GameObject _midMissionPanel;

    [Header("End Mission Panel")]
    [SerializeField] private GameObject _endMissionPanel;
    [SerializeField] private Button _endMissionOkButton;

    void Start()
    {
        _preMissionAcceptButton.onClick.AddListener(AcceptMission);
        _preMissionCancelButton.onClick.AddListener(CancelMission);
        _endMissionOkButton.onClick.AddListener(EndMissionOk);
        _endMissionPanel.SetActive(false);
    }

    void LateUpdate()
    {
        _preMissionPanel.SetActive(_deliverySystem.WantsToStartMissionDeliveryPort != null);
        _midMissionPanel.SetActive(_deliverySystem.currentMission != null);
        if (_deliverySystem.EndMissionSignal)
        {
            _endMissionPanel.SetActive(true);
            _deliverySystem.EndMissionSignal = false;
        }
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

    void EndMissionOk()
    {
        _endMissionPanel.SetActive(false);
    }
}
