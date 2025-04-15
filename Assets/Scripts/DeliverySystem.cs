using System.Linq;
using UnityEngine;

public class DeliverySystem : MonoBehaviour
{
#nullable enable

    private DeliveryPort[] deliveryPorts;
    public CarControl carControl;
    public DeliveryMission? currentMission;
    public DeliveryPort? CurrentDestinationDeliveryPort { get { return GetCurrentDestinationDeliveryPort(); } }
    public int baseMissionReward = 100;
    public float missionRewardDistanceMultiplier = 1f;
    public float missionRewardSpeedMultiplier = 1f;

    System.Random random = new System.Random();

    DeliveryPort? GetCurrentDestinationDeliveryPort()
    {
        return currentMission?.endDeliveryPort;
    }

    void Start()
    {
        deliveryPorts = GetComponentsInChildren<DeliveryPort>();
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
        if (currentMission == null)
        {
            DeliveryPort destinationDeliveryPort = GetRandomDeliveryPortExluding(deliveryPort);
            DeliveryMission mission = new DeliveryMission(deliveryPort, destinationDeliveryPort, Time.time, baseMissionReward, missionRewardDistanceMultiplier, missionRewardSpeedMultiplier);
            currentMission = mission;
        }

        deliveryPort.missionSign = MissionSign.NoMission;
    }

    void CompleteMission()
    {
        if (currentMission != null)
        {
            currentMission.RemoveFromDeliveryPorts();
            carControl.money += currentMission.Reward(Time.time);
            currentMission = null;
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

    DeliveryPort GetRandomDeliveryPortExluding(DeliveryPort exludedDeliveryPort)
    {
        DeliveryPort[] filteredDeliveryPorts = deliveryPorts.Where(o => o != exludedDeliveryPort).ToArray();
        int index = random.Next(0, filteredDeliveryPorts.Length);
        DeliveryPort deliveryPort = filteredDeliveryPorts[index];
        return deliveryPort;
    }
}
