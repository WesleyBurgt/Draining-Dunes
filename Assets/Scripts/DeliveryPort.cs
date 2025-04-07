using UnityEngine;

public class DeliveryPort : MonoBehaviour
{
    public bool startMission = false;
    public bool destination = false;

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

        carControl.ResetDamage();
        carControl.ResetFuel();

        if (this.destination)
        {
            destination = false;
            carControl.money += 100;
        }
        else
        {
            startMission = true;
        }
    }

    void Update()
    {

    }
}
