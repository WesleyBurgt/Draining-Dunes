using UnityEngine;

public class NavigationArrow : MonoBehaviour
{
    [SerializeField] private DeliverySystem _deliverySystem;
    private MeshRenderer _meshRenderer;

    private Quaternion _initialLocalRotation;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _initialLocalRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        if (_deliverySystem.CurrentDestinationDeliveryPort != null)
        {
            _meshRenderer.enabled = true;
            float navSpeed = 1000f;
            float singleStep = navSpeed * Time.deltaTime;
            Vector3 targetDirection = _deliverySystem.CurrentDestinationDeliveryPort.transform.position - transform.position;

            if (targetDirection != Vector3.zero)
            {
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
                Quaternion lookRotation = Quaternion.LookRotation(newDirection);
                transform.rotation = lookRotation * _initialLocalRotation;
            }
        }
        else
        {
            _meshRenderer.enabled = false;
        }
    }
}
