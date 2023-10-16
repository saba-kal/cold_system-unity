using UnityEngine;

public class PlayerUnitInfoManager : MonoBehaviour
{

    private void Start()
    {
        var playerUnitManager = FindObjectOfType<PlayerUnitManager>();
        var units = playerUnitManager.GetUnits();
        var playerUnitInfos = GetComponentsInChildren<PlayerUnitInfo>();
        for (var i = 0; i < playerUnitInfos.Length; i++)
        {
            var unit = units.GetValueOrNull(i);
            if (unit != null)
            {
                playerUnitInfos[i].SetUnit(unit, i + 1);
            }
            else
            {
                playerUnitInfos[i].gameObject.SetActive(false);
            }
        }
    }
}