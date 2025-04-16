using TMPro;
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
        _preMissionAcceptButton.onClick.AddListener(AcceptMission);
        _preMissionCancelButton.onClick.AddListener(CancelMission);
        _endMissionOkButton.onClick.AddListener(EndMissionOk);
        _endMissionPanel.SetActive(false);
    }

    void LateUpdate()
    {
        _preMissionPanel.SetActive(_deliverySystem.WantsToStartMissionDeliveryPort != null);
        _midMissionPanel.SetActive(_deliverySystem.currentMission != null);
        if (_deliverySystem.EndedMission != null)
        {
            _endMissionPanel.SetActive(true);
            ShowMissionRewards(_deliverySystem.EndedMission);
            _deliverySystem.EndedMission = null;
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
        if (_endMissionRefuelToggle.isOn)
        {
            _deliverySystem.RefuelCar();
        }
        if (_endMissionRepairToggle.isOn)
        {
            _deliverySystem.RepairCar();
        }
        _endMissionPanel.SetActive(false);
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
            _endMissionRepairCostText.text = string.Empty;
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
