using UnityEngine;

public class UnderwaterTrigger : MonoBehaviour
{
    public GameObject underwaterVolume; // Sleep hier je volume in
    void Start()
    {
        underwaterVolume.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera")) // Tag je camera als "MainCamera"
        {
            underwaterVolume.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            underwaterVolume.SetActive(false);
        }
    }
}

