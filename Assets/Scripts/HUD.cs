using TMPro;
using UnityEngine;
public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _speedText;
    [SerializeField] private TMP_Text _fuelText;
    [SerializeField] private TMP_Text _damageText;
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private CarControl _carController;
    [SerializeField] private float _speedScale = 1;
    [SerializeField] public int _money;

    void Start()
    {
        
    }

    private void LateUpdate()
    {
        _speedText.text = Mathf.RoundToInt(_carController.CurrentSpeed * _speedScale).ToString() + "kmph";
        _fuelText.text = Mathf.RoundToInt(_carController.Fuel).ToString() + "L";
        _damageText.text = Mathf.RoundToInt(_carController.damagePercentage).ToString() + "% dmg";
        _moneyText.text = "$" + _money.ToString();
    }
}
