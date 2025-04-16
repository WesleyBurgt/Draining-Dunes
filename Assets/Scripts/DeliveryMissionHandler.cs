using System.Linq;
using UnityEngine;

public class DeliveryMissionHandler
{
#nullable enable

    private DeliveryPort[] deliveryPorts;
    private System.Random random = new System.Random();

    public DeliveryMission? currentMission;
    public DeliveryPort? CurrentDestinationDeliveryPort { get { return GetCurrentDestinationDeliveryPort(); } }

    private int baseMissionReward;
    private float missionRewardDistanceMultiplier;
    private float missionRewardSpeedMultiplier;

    public DeliveryMissionHandler(DeliveryPort[] _deliveryPorts, int _baseMissionReward, float _missionRewardDistanceMultiplier, float _missionRewardSpeedMultiplier)
    {
        deliveryPorts = _deliveryPorts;
        baseMissionReward = _baseMissionReward;
        missionRewardDistanceMultiplier = _missionRewardDistanceMultiplier;
        missionRewardSpeedMultiplier = _missionRewardSpeedMultiplier;

        foreach (var deliveryPort in deliveryPorts)
        {
            deliveryPort.nextMission = GetNewMission(deliveryPort);
        }
    }

    DeliveryPort? GetCurrentDestinationDeliveryPort()
    {
        return currentMission?.endDeliveryPort;
    }

    public DeliveryMission GetNewMission(DeliveryPort startDeliveryPort)
    {
        DeliveryPort destinationDeliveryPort = GetRandomDeliveryPortExluding(startDeliveryPort);
        DeliveryMission mission = new DeliveryMission(startDeliveryPort, destinationDeliveryPort, baseMissionReward, missionRewardDistanceMultiplier, missionRewardSpeedMultiplier);
        return mission;
    }

    DeliveryPort GetRandomDeliveryPortExluding(DeliveryPort exludedDeliveryPort)
    {
        DeliveryPort[] filteredDeliveryPorts = deliveryPorts.Where(o => o != exludedDeliveryPort).ToArray();
        int index = random.Next(0, filteredDeliveryPorts.Length);
        DeliveryPort deliveryPort = filteredDeliveryPorts[index];
        return deliveryPort;
    }

    public void AssignMission(DeliveryMission deliveryMission)
    {
        if (currentMission == null)
        {
            currentMission = deliveryMission;
            deliveryMission.startTime = Time.time;

            deliveryMission.startDeliveryPort.assignedMission = deliveryMission;
            deliveryMission.endDeliveryPort.assignedMission = deliveryMission;
            deliveryMission.startDeliveryPort.missionSign = MissionSign.MissionInProgress;
            deliveryMission.endDeliveryPort.missionSign = MissionSign.MissionInProgress;

            deliveryMission.startDeliveryPort.nextMission = GetNewMission(deliveryMission.startDeliveryPort);
        }
    }

    public void CompleteMission()
    {
        if (currentMission != null)
        {
            DeliveryPort startDeliveryPort = currentMission.startDeliveryPort;
            DeliveryPort endDeliveryPort = currentMission.endDeliveryPort;

            startDeliveryPort.assignedMission = null;
            endDeliveryPort.assignedMission = null;
            startDeliveryPort.missionSign = MissionSign.NoMission;
            endDeliveryPort.missionSign = MissionSign.NoMission;

            currentMission = null;
        }
    }
}