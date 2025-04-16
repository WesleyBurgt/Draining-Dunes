using UnityEngine;

public class DeliverySystem : MonoBehaviour
{
#nullable enable

    private DeliveryPort[] deliveryPorts;
    private DeliveryMissionHandler deliveryMissionHandler;
    public CarControl carControl;

    public DeliveryMission? currentMission { get { return deliveryMissionHandler.currentMission; } }
    public DeliveryPort? CurrentDestinationDeliveryPort { get { return deliveryMissionHandler.CurrentDestinationDeliveryPort; } }


    [Header("Mission reward")]
    public int baseMissionReward = 100;
    public float missionRewardDistanceMultiplier = 1f;
    public float missionRewardSpeedMultiplier = 1f;

    [HideInInspector] public DeliveryMission? WantsToStartMission;
    [HideInInspector] public DeliveryMission? EndedMission;

    void Start()
    {
        deliveryPorts = GetComponentsInChildren<DeliveryPort>();
        deliveryMissionHandler = new DeliveryMissionHandler(deliveryPorts, baseMissionReward, missionRewardDistanceMultiplier, missionRewardSpeedMultiplier);
    }

    void Update()
    {
        float MaxCarSpeedForHandlingMissions = 1f;
        bool mayHandleMissions = carControl.CurrentSpeed < MaxCarSpeedForHandlingMissions;
        HandleMissionSigns(mayHandleMissions);
    }

    void HandleMissionSigns(bool mayHandleMissions)
    {
        foreach (DeliveryPort deliveryPort in deliveryPorts)
        {
            switch (deliveryPort.missionSign)
            {
                case MissionSign.StartMission:
                    {
                        if (mayHandleMissions)
                        {
                            WantsToStartMission = deliveryPort.nextMission;
                            deliveryPort.missionSign = MissionSign.NoMission;
                        }
                        return;
                    }
                case MissionSign.EndMission:
                    {
                        if (mayHandleMissions)
                        {
                            CompleteMission();
                        }
                        return;
                    }
                case MissionSign.CancelMission:
                    {
                        WantsToStartMission = null;
                        EndedMission = null;
                        deliveryPort.missionSign = MissionSign.NoMission;
                        return;
                    }
            }
        }
    }

    public void RequestAnotherMission(DeliveryPort deliveryPort)
    {
        WantsToStartMission = deliveryPort.nextMission;
    }

    public void AssignMission(DeliveryMission deliveryMission)
    {
        deliveryMissionHandler.AssignMission(deliveryMission);
    }

    void CompleteMission()
    {
        if (deliveryMissionHandler.currentMission != null)
        {
            deliveryMissionHandler.currentMission.endTime = Time.time;
            EndedMission = deliveryMissionHandler.currentMission;
            carControl.money += deliveryMissionHandler.currentMission.GetReward();
            deliveryMissionHandler.CompleteMission();
        }
    }

    public int GetRepairCarCost()
    {
        int cost = Mathf.RoundToInt(carControl.damagePercentage);
        return cost;
    }

    public int GetRefuelCarCost()
    {
        int cost = Mathf.RoundToInt(carControl.FuelTankSize - carControl.Fuel);
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
