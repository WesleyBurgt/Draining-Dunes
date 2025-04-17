using UnityEngine;

public class DeliveryMission
{
    public DeliveryPort startDeliveryPort { get; private set; }
    public DeliveryPort endDeliveryPort { get; private set; }
    public float deliveryDistance { get { return CalculateDeliveryDistance(); } }
    public float startTime;
    public float endTime;
    public int baseReward { get; private set; }
    public float rewardDistanceMultiplier;
    public float rewardSpeedMultiplier;

    public DeliveryMission(DeliveryPort _startDeliveryPort, DeliveryPort _endDeliveryPort, int _baseReward, float _rewardDistanceMultiplier, float _rewardSpeedMultiplier)
    {
        startDeliveryPort = _startDeliveryPort;
        endDeliveryPort = _endDeliveryPort;
        baseReward = _baseReward;
        rewardDistanceMultiplier = _rewardDistanceMultiplier;
        rewardSpeedMultiplier = _rewardSpeedMultiplier;
    }

    public int GetReward()
    {
        int reward = GetBaseReward() + GetSpeedBonus();
        return reward;
    }

    public int GetBaseReward()
    {
        float distanceBonus = deliveryDistance * rewardDistanceMultiplier;
        int reward = Mathf.RoundToInt(baseReward + distanceBonus);
        return reward;
    }

    public int GetSpeedBonus()
    {
        if (endTime != 0f)
        {
            float deliveryTime = endTime - startTime;

            if (deliveryTime <= 0.01f)
            {
                deliveryTime = 0.01f;
            }

            float speedBonus = (deliveryDistance / deliveryTime) * rewardSpeedMultiplier;
            int roundedSpeedBonus = Mathf.RoundToInt(speedBonus);
            return roundedSpeedBonus;
        }
        return 0;
    }

    public float GetSpeedBonusPerKMPH()
    {
        float dollarsPerKMPH = rewardSpeedMultiplier / 3.6f;
        return dollarsPerKMPH;
    }

    float CalculateDeliveryDistance()
    {
        return Vector3.Distance(
                startDeliveryPort.transform.position,
                endDeliveryPort.transform.position
        );
    }
}
