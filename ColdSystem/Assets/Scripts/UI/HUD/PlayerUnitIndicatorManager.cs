using UnityEngine;


public class PlayerUnitIndicatorManager : MonoBehaviour
{
    [SerializeField] private Camera _positionIndicatorCamera;

    private void Start()
    {
        var playerUnitManager = FindObjectOfType<PlayerUnitManager>();
        var units = playerUnitManager.GetUnits();
        var playerUnitIndicators = GetComponentsInChildren<PlayerUnitIndicator>();
        for (var i = 0; i < playerUnitIndicators.Length; i++)
        {
            var unit = units.GetValueOrNull(i);
            if (unit != null)
            {
                playerUnitIndicators[i].Initialize(i + 1, unit, _positionIndicatorCamera);
            }
            else
            {
                playerUnitIndicators[i].gameObject.SetActive(false);
            }
        }
    }
}