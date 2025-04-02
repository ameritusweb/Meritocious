using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Meritocious.Blazor.Services;

public class TabService : IDisposable
{
    private readonly NavigationManager _navigationManager;
    private readonly List<TabInfo> _tabs = new();
    public event EventHandler<TabEventArgs> TabChanged;
    
    public IReadOnlyList<TabInfo> Tabs => _tabs.AsReadOnly();
    public string ActiveTabKey { get; private set; }

    public TabService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += OnLocationChanged;
    }

    public void OpenTab(string key, string title, string url, bool closable = true)
    {
        var existingTab = _tabs.FirstOrDefault(t => t.Url == url);
        if (existingTab != null)
        {
            ActivateTab(existingTab.Key);
            return;
        }

        var tab = new TabInfo
        {
            Key = key,
            Title = title,
            Url = url,
            Closable = closable
        };

        _tabs.Add(tab);
        ActivateTab(key);
        _navigationManager.NavigateTo(url);
    }

    public void ActivateTab(string key)
    {
        if (ActiveTabKey != key)
        {
            ActiveTabKey = key;
            var tab = _tabs.FirstOrDefault(t => t.Key == key);
            if (tab != null)
            {
                TabChanged?.Invoke(this, new TabEventArgs(tab));
            }
        }
    }

    public void CloseTab(string key)
    {
        var tabIndex = _tabs.FindIndex(t => t.Key == key);
        if (tabIndex != -1)
        {
            _tabs.RemoveAt(tabIndex);
            if (ActiveTabKey == key)
            {
                var newActiveTab = _tabs.ElementAtOrDefault(Math.Max(0, tabIndex - 1));
                if (newActiveTab != null)
                {
                    ActivateTab(newActiveTab.Key);
                    _navigationManager.NavigateTo(newActiveTab.Url);
                }
            }
        }
    }

    public void ReorderTabs(string draggedKey, string targetKey)
    {
        var draggedTab = _tabs.FirstOrDefault(t => t.Key == draggedKey);
        var targetTab = _tabs.FirstOrDefault(t => t.Key == targetKey);
        
        if (draggedTab != null && targetTab != null)
        {
            var draggedIndex = _tabs.IndexOf(draggedTab);
            var targetIndex = _tabs.IndexOf(targetTab);
            
            _tabs.RemoveAt(draggedIndex);
            _tabs.Insert(targetIndex, draggedTab);
        }
    }

    private void OnLocationChanged(object sender, LocationChangedEventArgs e)
    {
        var tab = _tabs.FirstOrDefault(t => t.Url == e.Location);
        if (tab != null && ActiveTabKey != tab.Key)
        {
            ActivateTab(tab.Key);
        }
    }

    public void Dispose()
    {
        _navigationManager.LocationChanged -= OnLocationChanged;
    }
}

public class TabInfo
{
    public string Key { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public bool Closable { get; set; }
}

public class TabEventArgs : EventArgs
{
    public TabInfo Tab { get; }
    public TabEventArgs(TabInfo tab) => Tab = tab;
}