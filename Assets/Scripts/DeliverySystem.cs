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

    [HideInInspector] public DeliveryPort? WantsToStartMissionDeliveryPort;

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
                        WantsToStartMissionDeliveryPort = deliveryPort;
                        deliveryPort.missionSign = MissionSign.NoMission;
                        return;
                    }
                case MissionSign.EndMission:
                    {
                        CompleteMission();
                        return;
                    }
            }
        }
    }

    public void AssignMission(DeliveryPort deliveryPort)
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

    public void RepairCar()
    {
        if (carControl.money >= Mathf.RoundToInt(carControl.damagePercentage))
        {
            carControl.money -= Mathf.RoundToInt(carControl.damagePercentage);
            carControl.ResetDamage();
        }
    }

    public void RefuelCar()
    {
        if (carControl.money >= Mathf.RoundToInt(carControl.FuelTankSize - carControl.Fuel))
        {
            carControl.money -= Mathf.RoundToInt(carControl.FuelTankSize - carControl.Fuel);
            carControl.ResetFuel();
        }
    }
}
