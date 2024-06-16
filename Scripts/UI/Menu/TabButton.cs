using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(MaskableGraphic))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    [SerializeField] private string _value;
    [SerializeField] private TabMenu _menu;
    [Space]
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private GameObject _panel;
    [Space]
    [SerializeField] private Color _textNormalColor;
    [SerializeField] private Color _textHighlighterColor;
    [SerializeField] private Color _textSelectedColor;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _highlighterColor;
    [SerializeField] private Color _selectedColor;

    private MaskableGraphic _maskableGraphic;
    private bool _isHovered = false;

    public GameObject panel => _panel;

    public string value
    {
        get => _value;
        set
        {
            _value = value;
            if (_valueText != null)
                _valueText.text = value;
        }
    }

    public bool isSelect => _menu == null ? false : _menu.SelectedTab == this;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovered = true;
        UpdateColor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;
        UpdateColor();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _menu.OpenTab(this);
    }

    private void OnValidate()
    {
        if (_valueText != null)
            _valueText.text = _value;

        UpdateColor();
    }

    public void UpdateColor()
    {
        if (_maskableGraphic == null)
            _maskableGraphic = GetComponent<MaskableGraphic>();

        _maskableGraphic.color = _isHovered ? _highlighterColor :
                                isSelect ? _selectedColor :
                                _normalColor;

        if (_valueText != null)
            _valueText.color = _isHovered ? _textHighlighterColor :
                                isSelect ? _textSelectedColor :
                                _textNormalColor;
    }
}
