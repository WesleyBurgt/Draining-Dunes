using UnityEngine;

public class GasStationRepairShop : MonoBehaviour
{
    [SerializeField] private RefuelAndRepairPanel _refuelAndRepairPanel;
    [SerializeField] private GameObject _stopPanel;

    private bool panelShown;

    void Start()
    {
        _refuelAndRepairPanel.gameObject.SetActive(false);
        _stopPanel.SetActive(false);
        panelShown = false;
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
        if (!panelShown)
        {
            if (mayRefuelAndRepair)
            {
                _refuelAndRepairPanel.gameObject.SetActive(true);
                _stopPanel.SetActive(false);
                panelShown = true;
            }
            else
            {
                _stopPanel.SetActive(true);
            }
        }
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
        panelShown = false;
    }
}
