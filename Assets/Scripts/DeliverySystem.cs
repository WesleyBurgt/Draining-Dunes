using System.Linq;
using UnityEngine;

public class DeliverySystem : MonoBehaviour
{
    #nullable enable 
    
    private DeliveryPort[] deliveryPorts;
    public DeliveryPort? CurrentDestinationDeliveryPort { get { return GetCurrentDestinationDeliveryPort(); } }

    System.Random random = new System.Random();

    DeliveryPort? GetCurrentDestinationDeliveryPort()
    {
        foreach (var deliveryPort in deliveryPorts)
        {
            if (deliveryPort.destination == true)
            {
                return deliveryPort;
            }
        }
        return null;
    }

    void Start()
    {
        deliveryPorts = GetComponentsInChildren<DeliveryPort>();
    }

    void Update()
    {
        foreach (DeliveryPort deliveryPort in deliveryPorts)
        {
            if (deliveryPort.startMission)
            {
                AssignMission(deliveryPort);
                break;
            }
        }
    }

    void AssignMission(DeliveryPort deliveryPort)
    {
        if (!MissionInProgress())
        {
            DeliveryPort desinationDeliveryPort = GetRandomDeliveryPortExluding(deliveryPort);
            desinationDeliveryPort.destination = true;
            Debug.Log(desinationDeliveryPort.name);
        }
        deliveryPort.startMission = false;
    }

    bool MissionInProgress()
    {
        return deliveryPorts.Any(o => o.destination);
    }

    DeliveryPort GetRandomDeliveryPortExluding(DeliveryPort exludedDeliveryPort)
    {
        DeliveryPort[] filteredDeliveryPorts = deliveryPorts.Where(o => o != exludedDeliveryPort).ToArray();
        int index = random.Next(0, filteredDeliveryPorts.Length);
        DeliveryPort deliveryPort = filteredDeliveryPorts[index];
        return deliveryPort;
    }
}
