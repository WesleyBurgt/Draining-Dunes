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

    [HideInInspector] public CarRepairAndRefuel CarRepairAndRefuel;
    [HideInInspector] public DeliveryMission? WantsToStartMission;
    [HideInInspector] public DeliveryMission? EndedMission;
    [HideInInspector] public string StopCarWarning = string.Empty;

    void Start()
    {
        deliveryPorts = GetComponentsInChildren<DeliveryPort>();
        CarRepairAndRefuel = GetComponent<CarRepairAndRefuel>();
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
                        if (currentMission != null)
                        {
                            deliveryPort.missionSign = MissionSign.NoMission;
                            return;
                        }

                        if (mayHandleMissions)
                        {
                            WantsToStartMission = deliveryPort.nextMission;
                            deliveryPort.missionSign = MissionSign.NoMission;
                            StopCarWarning = string.Empty;
                        }
                        else
                        {
                            StopCarWarning = "Stop to take on missions";
                        }
                        return;
                    }
                case MissionSign.EndMission:
                    {
                        if (mayHandleMissions)
                        {
                            CompleteMission();
                            StopCarWarning = string.Empty;
                        }
                        else
                        {
                            StopCarWarning = "Stop to complete mission";
                        }
                        return;
                    }
                case MissionSign.CancelMission:
                    {
                        WantsToStartMission = null;
                        EndedMission = null;
                        deliveryPort.missionSign = MissionSign.NoMission;
                        StopCarWarning = string.Empty;
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
}
