﻿namespace H.NotifyIcon;

#if !MACOS
[DependencyProperty<Guid>("Id",
    Description = "Unique ID." +
                  "Gets or sets the TrayIcon Id." +
                  "Use this for second TrayIcon in same app." +
                  "It will be used by the system to store your TrayIcon settings," +
                  "so it is recommended to make it fixed and unique for each application TrayIcon, not random." +
                  "Note: Windows associates a Guid with the path of the binary, so you must use the new Guid when you change the path.", Category = CategoryName)]
[DependencyProperty<string>("CustomName",
    Description = "Gets or sets the TrayIcon Name. Use this for second TrayIcon in same app.", Category = CategoryName)]
#endif
[DependencyProperty<Icon>("Icon", ClsCompliant = false,
    Description = "Gets or sets the icon to be displayed. Use this for dynamically generated System.Drawing.Icons.", Category = CategoryName)]
public partial class TaskbarIcon
{
    #region Constants

    /// <summary>
    /// Category name that is set on designer properties.
    /// </summary>
    public const string CategoryName = "NotifyIcon";

    #endregion

    #region Id
    
#if !MACOS
    [SupportedOSPlatform("windows5.1.2600")]
    partial void OnIdChanged(Guid newValue)
    {
        TrayIcon.UpdateId(newValue);
    }

    [SupportedOSPlatform("windows5.1.2600")]
    partial void OnCustomNameChanged(string? newValue)
    {
        TrayIcon.UpdateName(newValue ?? string.Empty);
        Id = TrayIcon.Id;
    }
#endif

    #endregion

    #region Icon/IconSource

    [SupportedOSPlatform("windows5.1.2600")]
    partial void OnIconChanged(Icon? oldValue, Icon? newValue)
    {
        oldValue?.Dispose();
        _ = UpdateIcon(newValue);
    }
    
    /// <summary>
    /// Updates TrayIcon.Icon without changing Icon property.
    /// </summary>
    /// <param name="value"></param>
    [CLSCompliant(false)]
    [SupportedOSPlatform("windows5.1.2600")]
    public bool UpdateIcon(Icon? value)
    {
#if !MACOS
        return TrayIcon.UpdateIcon((nint?)value?.Handle ?? 0);
#else
        return true;
#endif
    }

    #endregion

#if !HAS_MAUI
    
    #region Visibility

    [SupportedOSPlatform("windows5.1.2600")]
    partial void OnVisibilityChanged(Visibility newValue)
    {
        var state = newValue == Visibility.Visible
            ? IconVisibility.Visible
            : IconVisibility.Hidden;

        TrayIcon.UpdateVisibility(state);
    }

    #endregion

#endif

#if HAS_MAUI

    #region BindingContext
    
    private void UpdateBindingContext(
        BindableObject? target,
        object? newDataContextValue)
    {
        if (target == null)
        {
            return;
        }

        target.BindingContext = newDataContextValue ?? this;
    }

    private void OnBindingContextChanged(object? sender, EventArgs args)
    {
        //UpdateBindingContext(TrayPopupResolved, BindingContext);
        //UpdateBindingContext(TrayToolTipResolved, BindingContext);
    }

    #endregion

#else

    #region DataContext

    /// <summary>
    /// Updates the DataContextProperty of a given
    /// <see cref="FrameworkElement"/>. This method only updates target elements
    /// that do not already have a data context of their own, and either assigns
    /// the DataContext of the NotifyIcon, or the
    /// NotifyIcon itself, if no data context was assigned at all.
    /// </summary>
    private void UpdateDataContext(
        FrameworkElement? target,
        object? newDataContextValue)
    {
        //if there is no target or it's data context is determined through a binding
        //of its own, keep it
        if (target == null ||
            target.GetBindingExpression(DataContextProperty) != null)
        {
            return;
        }

        //if the target's data context is the NotifyIcon's old DataContext or the NotifyIcon itself,
        //update it
        //if (ReferenceEquals(this, target.DataContext) ||
        //    Equals(oldDataContextValue, target.DataContext))
        {
            //assign own data context, if available. If there is no data
            //context at all, assign NotifyIcon itself.
            target.DataContext = newDataContextValue ?? this;
        }
    }

    partial void OnDataContextChanged(object? newValue)
    {
        UpdateDataContext(TrayPopupResolved, newValue);
        UpdateDataContext(TrayToolTipResolved, newValue);
#if HAS_WPF
        UpdateDataContext(ContextMenu, newValue);
#else
        UpdateContextFlyoutDataContext(ContextFlyout, newValue);
#endif
    }

    #endregion

#endif
}
