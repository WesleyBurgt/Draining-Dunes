using TMPro;
using UnityEngine;
public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _speedText;
    [SerializeField] private TMP_Text _fuelText;
    [SerializeField] private TMP_Text _damageText;
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private TMP_Text _distanceText;
    [SerializeField] private CarControl _carController;
    [SerializeField] private DeliverySystem _deliverySystem;
    [SerializeField] private float _speedScale = 1;

    void Start()
    {

    }

    private void LateUpdate()
    {
        _speedText.text = Mathf.RoundToInt(_carController.CurrentSpeed * _speedScale).ToString() + "kmph";
        _fuelText.text = Mathf.RoundToInt(_carController.Fuel).ToString() + "L";
        _damageText.text = Mathf.RoundToInt(_carController.damagePercentage).ToString() + "% dmg";
        _moneyText.text = "$" + _carController.money.ToString();
        DisplayDistanceText();
    }

    private void DisplayDistanceText()
    {
        var deliveryPort = _deliverySystem.CurrentDestinationDeliveryPort;

        if (deliveryPort != null)
        {
            Collider portCollider = deliveryPort.GetComponent<Collider>();

            Vector3 closestPoint = portCollider.ClosestPoint(_carController.transform.position);
            float distance = Vector3.Distance(_carController.transform.position, closestPoint);

            _distanceText.enabled = true;
            _distanceText.text = Mathf.RoundToInt(distance).ToString() + "m";
        }
        else
        {
            _distanceText.enabled = false;
        }
    }
}
