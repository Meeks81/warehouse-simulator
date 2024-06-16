using System.Collections.Generic;
using UnityEngine;

public class TabMenu : MonoBehaviour
{

    [SerializeField] private List<TabButton> _tabButtons;

    public TabButton SelectedTab { get; private set; }

    public void OpenTab(TabButton tabButton)
    {
        if (_tabButtons.Contains(tabButton) == false)
            throw new System.Exception("This panel is not in the list");
        if (tabButton.panel == null)
            throw new System.Exception("Tab can't be null");

        CloseAllPanels();
        tabButton.panel.SetActive(true);
        tabButton.UpdateColor();
        SelectedTab = tabButton;
    }

    public void CloseAllPanels()
    {
        SelectedTab = null;
        foreach (var item in _tabButtons)
        {
            if (item.panel != null)
                item.panel.SetActive(false);
            item.UpdateColor();
        }
    }

}
