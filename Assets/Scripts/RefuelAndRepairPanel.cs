using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RefuelAndRepairPanel : MonoBehaviour
{
    [SerializeField] CarRepairAndRefuel _carRepairAndRefuel;
    [SerializeField] private Toggle _refuelToggle;
    [SerializeField] private TMP_Text _refuelCostText;
    [SerializeField] private Toggle _repairToggle;
    [SerializeField] private TMP_Text _repairCostText;
    [SerializeField] private TMP_Text _totalCostText;
    [SerializeField] private Button _purchaseButton;
    [SerializeField] private Button _cancelButton;

    void Start()
    {
        _purchaseButton.onClick.AddListener(PurchaseRefuelAndRepair);
        _cancelButton.onClick.AddListener(CancelPurchase);
    }

    void LateUpdate()
    {
        ShowCosts();
    }

    void CancelPurchase()
    {
        gameObject.SetActive(false);
    }

    void PurchaseRefuelAndRepair()
    {
        if (_refuelToggle.isOn)
        {
            _carRepairAndRefuel.RefuelCar();
        }
        if (_repairToggle.isOn)
        {
            _carRepairAndRefuel.RepairCar();
        }
    }

    void ShowCosts()
    {
        int totalCost = 0;

        if (_refuelToggle.isOn)
        {
            int refuelCost = _carRepairAndRefuel.GetRefuelCarCost();
            _refuelCostText.text = $"-${refuelCost}";
            totalCost += refuelCost;
        }
        else
        {
            _refuelCostText.text = string.Empty;
        }

        if (_repairToggle.isOn)
        {
            int repairCost = _carRepairAndRefuel.GetRepairCarCost();
            _repairCostText.text = $"-${repairCost}";
            totalCost += repairCost;
        }
        else
        {
            _repairCostText.text = string.Empty;
        }

        _totalCostText.text = $"-${totalCost}";
    }
}
