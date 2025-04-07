using UnityEngine;

public class NavigationArrow : MonoBehaviour
{
    [SerializeField] private DeliverySystem _deliverySystem;
    private MeshRenderer _meshRenderer;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    void LateUpdate()
    {
        Debug.Log(_deliverySystem.CurrentDestinationDeliveryPort);
        if (_deliverySystem.CurrentDestinationDeliveryPort != null)
        {
            _meshRenderer.enabled = true;
            float navSpeed = 1000f;
            float singleStep = navSpeed * Time.deltaTime;
            Vector3 targetDirection = _deliverySystem.CurrentDestinationDeliveryPort.transform.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            Debug.DrawRay(transform.position, newDirection, Color.red);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        else
        {
            _meshRenderer.enabled = false;
        }
    }
}
