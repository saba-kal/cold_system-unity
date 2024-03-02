using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class PlayerUnitIndicator : MonoBehaviour
{
    [SerializeField] private GameObject _bottomUnitIndicator;
    [SerializeField] private GameObject _leftUnitIndicator;
    [SerializeField] private GameObject _topUnitIndicator;
    [SerializeField] private GameObject _rightUnitIndicator;

    private Camera _positionIndicatorCamera;
    private RectTransform _rectTransform;
    private Camera _camera;
    private Unit _unit;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _camera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_unit == null)
        {
            HideAllIndicators();
            return;
        }

        var unitWorldPosition = _unit.GetFieldOfViewStartPosition() + new Vector3(0, 2, 0);
        var viewportPosition = _camera.WorldToViewportPoint(unitWorldPosition);
        //var viewportPosition = RectTransformUtility.WorldToScreenPoint(_camera, unitWorldPosition);
        if (viewportPosition.z < 0)
        {
            //Target is behind the camera, so the position needs to be flipped.
            viewportPosition *= -1;
        }

        if (viewportPosition.y < 0)
        {
            //Because the camera is angled, targets that are below the bottom edge of the camera cause issues
            //with the offscreen indicators position calculation. To avoid this, we use a dedicated camera that
            //always points down.
            viewportPosition = _positionIndicatorCamera.WorldToViewportPoint(unitWorldPosition);
            //Target may not be offscreen for the top-down camera, so we need to manually set the y.
            viewportPosition.y = -0.1f;
        }

        const float screenMargin = 10.0f;
        var targetPosition = new Vector2(
            Mathf.Clamp(viewportPosition.x * Screen.width, screenMargin, Screen.width - screenMargin),
            Mathf.Clamp(viewportPosition.y * Screen.height, screenMargin, Screen.height - screenMargin));
        transform.position = targetPosition;

        UpdateDirctionalIndicators(viewportPosition);
    }

    public void Initialize(int unitNumber, Unit unit, Camera positionIndicatorCamera)
    {
        _unit = unit;
        _positionIndicatorCamera = positionIndicatorCamera;
        SetUnitNumber(unitNumber, _bottomUnitIndicator);
        SetUnitNumber(unitNumber, _leftUnitIndicator);
        SetUnitNumber(unitNumber, _topUnitIndicator);
        SetUnitNumber(unitNumber, _rightUnitIndicator);
    }

    private void SetUnitNumber(int unitNumber, GameObject unitIndicator)
    {
        unitIndicator.GetComponentInChildren<TextMeshProUGUI>().text = unitNumber.ToString();
    }

    private void UpdateDirctionalIndicators(Vector2 unitViewportPosition)
    {
        if (_unit == null)
        {
            return;
        }

        if (unitViewportPosition.x >= 0 && unitViewportPosition.x <= 1 &&
            unitViewportPosition.y <= 1)
        {
            // Unit is visible in viewport or past the bottom edge of the screen.
            _bottomUnitIndicator.SetActive(true);

            _leftUnitIndicator.SetActive(false);
            _topUnitIndicator.SetActive(false);
            _rightUnitIndicator.SetActive(false);
        }
        else if (unitViewportPosition.x >= 0 && unitViewportPosition.x <= 1 &&
            unitViewportPosition.y > 1)
        {
            // Unit past the top edge of the screen.
            _topUnitIndicator.SetActive(true);

            _leftUnitIndicator.SetActive(false);
            _bottomUnitIndicator.SetActive(false);
            _rightUnitIndicator.SetActive(false);
        }
        else if (unitViewportPosition.x < 0)
        {
            // Unit past the left edge of the screen.
            _leftUnitIndicator.SetActive(true);

            _topUnitIndicator.SetActive(false);
            _bottomUnitIndicator.SetActive(false);
            _rightUnitIndicator.SetActive(false);
        }
        else if (unitViewportPosition.x > 1)
        {
            // Unit past the right edge of the screen.
            _rightUnitIndicator.SetActive(true);

            _topUnitIndicator.SetActive(false);
            _bottomUnitIndicator.SetActive(false);
            _leftUnitIndicator.SetActive(false);
        }
    }

    private void HideAllIndicators()
    {
        _bottomUnitIndicator.SetActive(false);
        _leftUnitIndicator.SetActive(false);
        _topUnitIndicator.SetActive(false);
        _rightUnitIndicator.SetActive(false);
    }
}