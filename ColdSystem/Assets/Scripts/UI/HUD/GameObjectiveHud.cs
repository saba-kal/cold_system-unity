using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class GameObjectiveHud : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _objectiveText;

    private void Update()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("- Wave: ");
        stringBuilder.Append(EndlessWaveSpawner.WaveNumber);
        stringBuilder.Append("\n- Next Wave: ");
        stringBuilder.Append(TimeSpan.FromSeconds(EndlessWaveSpawner.TimeUntilNextWave).ToString("mm\\:ss"));
        stringBuilder.Append("\n- Enemies: ");
        stringBuilder.Append(EnemyUnitManager.Instance?.GetUnits().Where(u => u != null).Count() ?? 0);
        _objectiveText.SetText(stringBuilder);
    }
}
