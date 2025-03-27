using TMPro;
using UnityEngine;
public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _speedText;
    [SerializeField] private TMP_Text _fuelText;
    [SerializeField] private TMP_Text _damageText;
    [SerializeField] private CarControl _carController;

    void Start()
    {
        
    }

    private void LateUpdate()
    {
        _speedText.text = Mathf.RoundToInt(_carController.CurrentSpeed).ToString() + "kmph";
        _fuelText.text = Mathf.RoundToInt(_carController.Fuel).ToString() + "L";
        _damageText.text = Mathf.RoundToInt(_carController.damagePercentage).ToString() + "% dmg";
    }
}
