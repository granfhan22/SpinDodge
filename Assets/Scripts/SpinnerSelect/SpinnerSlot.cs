using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpinnerSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Config")]
    public int slotIndex;
    public SpinnerData data;
    [SerializeField] private float hoverSpinSpeed = 180f;

    [Header("References")]
    [SerializeField] private RectTransform spinnerPreview;
    [SerializeField] private Image borderImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text statsText;

    private bool isHovered;

    private void Start()
    {
        PopulateUI();
        SetSelected(SpinnerSelection.SelectedIndex == slotIndex);
    }

    private void Update()
    {
        if (isHovered && spinnerPreview != null)
            spinnerPreview.Rotate(0f, 0f, hoverSpinSpeed * Time.deltaTime);
    }

    private void PopulateUI()
    {
        if (data == null) return;
        if (nameText != null) nameText.text = data.spinnerName;
        if (statsText != null) statsText.text = data.description;
    }

    public void SetSelected(bool selected)
    {
        if (borderImage != null) borderImage.enabled = selected;
    }

    public void OnPointerEnter(PointerEventData eventData) => isHovered = true;
    public void OnPointerExit(PointerEventData eventData) => isHovered = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance?.PlayButtonClick();
        SpinnerSelection.SelectedIndex = slotIndex;
        SpinnerSelectManager.Instance?.SelectSlot(slotIndex);
    }
}
