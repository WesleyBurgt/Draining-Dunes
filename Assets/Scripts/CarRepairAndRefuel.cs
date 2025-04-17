using UnityEngine;

public class CarRepairAndRefuel : MonoBehaviour
{
    [SerializeField] private CarControl carControl;
    [SerializeField] private float damageCostMultiplier = 1f;
    [SerializeField] private float fuelCostMultiplier = 1f;
    
    public int GetRepairCarCost()
    {
        int cost = Mathf.RoundToInt(carControl.damagePercentage * damageCostMultiplier);
        return cost;
    }

    public int GetRefuelCarCost()
    {
        int baseCost = Mathf.RoundToInt(carControl.FuelTankSize - carControl.Fuel);
        int cost = Mathf.RoundToInt(baseCost * fuelCostMultiplier);
        return cost;
    }

    public void RepairCar()
    {
        if (carControl.money >= GetRepairCarCost())
        {
            carControl.money -= GetRepairCarCost();
            carControl.ResetDamage();
        }
    }

    public void RefuelCar()
    {
        if (carControl.money >= GetRefuelCarCost())
        {
            carControl.money -= GetRefuelCarCost();
            carControl.ResetFuel();
        }
    }

}
