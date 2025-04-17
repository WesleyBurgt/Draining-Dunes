using UnityEngine;
using TMPro;

public class MidMissionPanel : MonoBehaviour
{
    [SerializeField] private DeliverySystem _deliverySystem;
    [SerializeField] private TMP_Text _deliveryPortNameText;

    void Start()
    {

    }

    void LateUpdate()
    {
        if (_deliverySystem.CurrentDestinationDeliveryPort != null)
        {
            _deliveryPortNameText.text = _deliverySystem.CurrentDestinationDeliveryPort.name;
        }
    }
}
