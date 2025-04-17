using UnityEngine;

public class GasStationRepairShop : MonoBehaviour
{
    [SerializeField] private RefuelAndRepairPanel _refuelAndRepairPanel;
    [SerializeField] private GameObject _stopPanel;

    void Start()
    {
        _refuelAndRepairPanel.gameObject.SetActive(false);
        _stopPanel.SetActive(false);
    }

    void OnTriggerStay(Collider other)
    {
        CarControl carControl = other.gameObject.GetComponent<CarControl>();
        if (carControl == null)
        {
            return;
        }

        float maxCarSpeedForRefuelAndRepair = 1f;
        bool mayRefuelAndRepair = carControl.CurrentSpeed < maxCarSpeedForRefuelAndRepair;

        _refuelAndRepairPanel.gameObject.SetActive(mayRefuelAndRepair);
        _stopPanel.SetActive(!mayRefuelAndRepair);
    }

    void OnTriggerExit(Collider other)
    {
        CarControl carControl = other.gameObject.GetComponent<CarControl>();
        if (carControl == null)
        {
            return;
        }
        _refuelAndRepairPanel.gameObject.SetActive(false);
        _stopPanel.SetActive(false);
    }
}
