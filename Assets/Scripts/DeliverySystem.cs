using UnityEngine;

public class DeliverySystem : MonoBehaviour
{
#nullable enable

    private DeliveryPort[] deliveryPorts;
    private DeliveryMissionHandler deliveryMissionHandler;
    public CarControl carControl;

    public DeliveryPort? CurrentDestinationDeliveryPort { get { return deliveryMissionHandler.CurrentDestinationDeliveryPort; } }


    [Header("Mission reward")]
    public int baseMissionReward = 100;
    public float missionRewardDistanceMultiplier = 1f;
    public float missionRewardSpeedMultiplier = 1f;

    void Start()
    {
        deliveryPorts = GetComponentsInChildren<DeliveryPort>();
        deliveryMissionHandler = new DeliveryMissionHandler(deliveryPorts, baseMissionReward, missionRewardDistanceMultiplier, missionRewardSpeedMultiplier);
    }

    void Update()
    {
        float MaxCarSpeedForHandlingMissions = 1f;
        if (carControl.CurrentSpeed < MaxCarSpeedForHandlingMissions)
        {
            HandleMissionSigns();
        }
    }

    void HandleMissionSigns()
    {
        foreach (DeliveryPort deliveryPort in deliveryPorts)
        {
            switch (deliveryPort.missionSign)
            {
                case MissionSign.StartMission:
                    {
                        AssignMission(deliveryPort);
                        RepairAndRefuelCar();
                        return;
                    }
                case MissionSign.EndMission:
                    {
                        CompleteMission();
                        RepairAndRefuelCar();
                        return;
                    }
            }
        }
    }

    void AssignMission(DeliveryPort deliveryPort)
    {
        deliveryMissionHandler.AssignMission(deliveryPort);
    }

    void CompleteMission()
    {
        if (deliveryMissionHandler.currentMission != null)
        {
            carControl.money += deliveryMissionHandler.currentMission.Reward(Time.time);
            deliveryMissionHandler.CompleteMission();
        }
    }

    void RepairAndRefuelCar()
    {
        if (carControl.money >= Mathf.RoundToInt(carControl.damagePercentage))
        {
            carControl.money -= Mathf.RoundToInt(carControl.damagePercentage);
            carControl.ResetDamage();
        }

        if (carControl.money >= Mathf.RoundToInt(carControl.FuelTankSize - carControl.Fuel))
        {
            carControl.money -= Mathf.RoundToInt(carControl.FuelTankSize - carControl.Fuel);
            carControl.ResetFuel();
        }
    }
}
