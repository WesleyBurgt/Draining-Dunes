using UnityEngine;

public class DeliveryMission
{
    public DeliveryPort startDeliveryPort { get; private set; }
    public DeliveryPort endDeliveryPort { get; private set; }
    public float deliveryDistance { get { return CalculateDeliveryDistance(); } }
    public float startTime { get; private set; }
    public int baseReward { get; private set; }
    public float rewardDistanceMultiplier;
    public float rewardSpeedMultiplier;

    public DeliveryMission(DeliveryPort _startDeliveryPort, DeliveryPort _endDeliveryPort, float _startTime, int _baseReward, float _rewardDistanceMultiplier, float _rewardSpeedMultiplier)
    {
        startDeliveryPort = _startDeliveryPort;
        endDeliveryPort = _endDeliveryPort;
        startTime = _startTime;
        baseReward = _baseReward;
        rewardDistanceMultiplier = _rewardDistanceMultiplier;
        rewardSpeedMultiplier = _rewardSpeedMultiplier;
    }

    public int Reward(float endTime)
    {
        float deliveryTime = endTime - startTime;

        if (deliveryTime <= 0.01f)
        {
            deliveryTime = 0.01f;
        }

        float distanceBonus = deliveryDistance * rewardDistanceMultiplier;
        float speedBonus = (deliveryDistance / deliveryTime) * rewardSpeedMultiplier;
        int reward = Mathf.RoundToInt(baseReward + distanceBonus + speedBonus);

        return reward;
    }

    float CalculateDeliveryDistance()
    {
        return Vector3.Distance(
                startDeliveryPort.transform.position,
                endDeliveryPort.transform.position
        );
    }
}
