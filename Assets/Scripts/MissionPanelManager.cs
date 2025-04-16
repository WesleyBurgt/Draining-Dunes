using UnityEngine;
using TMPro;

public class MissionPanelManager : MonoBehaviour
{
    [SerializeField] private DeliverySystem _deliverySystem;
    [SerializeField] private PreMissionPanel _preMissionPanel;
    [SerializeField] private MidMissionPanel _midMissionPanel;
    [SerializeField] private EndMissionPanel _endMissionPanel;
    [SerializeField] private GameObject _stopPanel;
    [SerializeField] private TMP_Text _stopText;


    void Start()
    {
        _endMissionPanel.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        _preMissionPanel.gameObject.SetActive(_deliverySystem.WantsToStartMission != null);
        _midMissionPanel.gameObject.SetActive(_deliverySystem.currentMission != null);
        _endMissionPanel.gameObject.SetActive(_deliverySystem.EndedMission != null);
        if (_deliverySystem.StopCarWarning != string.Empty)
        {
            _stopPanel.gameObject.SetActive(true);
            _stopText.text = _deliverySystem.StopCarWarning;
        }
        else
        {
            _stopPanel.gameObject.SetActive(false);
        }
    }
}
