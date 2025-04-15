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
    }

    DeliveryPort? GetCurrentDestinationDeliveryPort()
    {
        return currentMission?.endDeliveryPort;
    }

    DeliveryMission GetMission(DeliveryPort startDeliveryPort)
    {
        float startTime = Time.time;
        DeliveryPort destinationDeliveryPort = GetRandomDeliveryPortExluding(startDeliveryPort);
        DeliveryMission mission = new DeliveryMission(startDeliveryPort, destinationDeliveryPort, startTime, baseMissionReward, missionRewardDistanceMultiplier, missionRewardSpeedMultiplier);
        return mission;
    }

    DeliveryPort GetRandomDeliveryPortExluding(DeliveryPort exludedDeliveryPort)
    {
        DeliveryPort[] filteredDeliveryPorts = deliveryPorts.Where(o => o != exludedDeliveryPort).ToArray();
        int index = random.Next(0, filteredDeliveryPorts.Length);
        DeliveryPort deliveryPort = filteredDeliveryPorts[index];
        return deliveryPort;
    }

    public void AssignMission(DeliveryPort deliveryPort)
    {
        if (currentMission == null)
        {
            currentMission = GetMission(deliveryPort);
        }

        deliveryPort.missionSign = MissionSign.NoMission;
    }

    public void CompleteMission()
    {
        if (currentMission != null)
        {
            currentMission.RemoveFromDeliveryPorts();
            currentMission = null;
        }
    }
}