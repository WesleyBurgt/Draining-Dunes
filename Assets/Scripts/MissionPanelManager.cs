using UnityEngine;

public class MissionPanelManager : MonoBehaviour
{
    [SerializeField] private DeliverySystem _deliverySystem;
    [SerializeField] private PreMissionPanel _preMissionPanel;
    [SerializeField] private MidMissionPanel _midMissionPanel;
    [SerializeField] private EndMissionPanel _endMissionPanel;


    void Start()
    {
        _endMissionPanel.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        _preMissionPanel.gameObject.SetActive(_deliverySystem.WantsToStartMission != null);
        _midMissionPanel.gameObject.SetActive(_deliverySystem.currentMission != null);
        _endMissionPanel.gameObject.SetActive(_deliverySystem.EndedMission != null);
    }
}
