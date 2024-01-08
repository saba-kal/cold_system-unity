using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GroupFormationManager : MonoBehaviour
{
    public int SelectedUnitCount { get; set; }
    public Vector3 FormationPosition { get; set; }

    [SerializeField] private GameObject _formationPositionIndicatorPrefab;
    [SerializeField] private float _circleFormationRadius = 5f;

    private List<GameObject> _formationPositionIndicators = new List<GameObject>();

    private void Start()
    {
        for (var i = 0; i < 5; i++)
        {
            var fromationPositionIndicator = Instantiate(_formationPositionIndicatorPrefab);
            fromationPositionIndicator.transform.parent = transform;
            fromationPositionIndicator.SetActive(false);
            _formationPositionIndicators.Add(fromationPositionIndicator);
        }
    }

    private void Update()
    {
        transform.position = FormationPosition;
        HideUnselectedUnitMoveIndicators();
        PlaceIndicatorsInCircle();
        AdjustIndicatorHeights();
    }

    public List<Vector3> GetFormationPositions()
    {
        return _formationPositionIndicators.Select(m => m.transform.position).ToList();
    }

    private void HideUnselectedUnitMoveIndicators()
    {
        for (var i = 0; i < _formationPositionIndicators.Count; i++)
        {
            _formationPositionIndicators[i].SetActive(i < SelectedUnitCount);
        }
    }

    private void PlaceIndicatorsInCircle()
    {
        if (SelectedUnitCount == 1)
        {
            _formationPositionIndicators[0].transform.localPosition = Vector3.zero;
            return;
        }

        var adjustedRadius = _circleFormationRadius * SelectedUnitCount / 5f;

        if (SelectedUnitCount == 2)
        {
            _formationPositionIndicators[0].transform.localPosition = new Vector3(adjustedRadius, 0, 0);
            _formationPositionIndicators[1].transform.localPosition = new Vector3(-adjustedRadius, 0, 0);
            return;
        }

        for (var i = 0; i < SelectedUnitCount; i++)
        {
            var circlePosition = i / (float)SelectedUnitCount;
            var x = Mathf.Sin(circlePosition * Mathf.PI * 2.0f) * adjustedRadius;
            var z = Mathf.Cos(circlePosition * Mathf.PI * 2.0f) * adjustedRadius;
            _formationPositionIndicators[i].transform.localPosition = new Vector3(x, 0, z);
        }
    }

    private void AdjustIndicatorHeights()
    {
        if (SelectedUnitCount == 1)
        {
            return;
        }

        foreach (var indicator in _formationPositionIndicators)
        {
            var from = indicator.transform.position + new Vector3(0, 1000, 0);
            if (Physics.Raycast(from, Vector3.down, out var hit, 2000f, LayerMask.GetMask(Constants.GROUND_LAYER)))
            {
                indicator.transform.position = hit.point;
            }
        }
    }
}