using TMPro;
using UnityEngine;
public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _speedText;
    [SerializeField] private CarControl _carController;
    [SerializeField] private float _speedScale = 1;

    void Start()
    {
        
    }

    private void LateUpdate()
    {
        _speedText.text = Mathf.RoundToInt(_carController.CurrentSpeed * _speedScale).ToString() + "kmph";
    }
}
