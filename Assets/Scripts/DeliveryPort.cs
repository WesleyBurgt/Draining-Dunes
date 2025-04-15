using UnityEngine;

public class DeliveryPort : MonoBehaviour
{
    public DeliveryMission nextMission;
    public DeliveryMission assignedMission;
    [HideInInspector] public MissionSign missionSign = MissionSign.NoMission;

    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        CarControl carControl = other.gameObject.GetComponent<CarControl>();
        if (carControl == null) 
        {
            return;
        }

        if (assignedMission == null)
        {
            missionSign = MissionSign.StartMission;
        }
        else if (this == assignedMission.endDeliveryPort)
        {
            missionSign = MissionSign.EndMission;
        }
    }

    void Update()
    {

    }
}