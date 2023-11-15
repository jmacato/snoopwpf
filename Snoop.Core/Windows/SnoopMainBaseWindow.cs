namespace Snoop.Windows;

using System;
using System.Windows;
using Snoop.Data;
using Snoop.Infrastructure;

public abstract class SnoopMainBaseWindow : SnoopBaseWindow
{
    private Window? ownerWindow;

    public object? RootObject { get; private set; }

    public abstract object? Target { get; set; }

    public Window Inspect(object rootObject)
    {
        ExceptionHandler.AddExceptionHandler(this.Dispatcher);

        this.RootObject = rootObject;

        this.Load(rootObject);

        this.ownerWindow = SnoopWindowUtils.FindOwnerWindow(this);

        if (TransientSettingsData.Current?.SetOwnerWindow == true)
        {
            this.Owner = this.ownerWindow;
        }
        else if (this.ownerWindow is not null)
        {
            // if we have an owner window, but the owner should not be set, we still have to close ourself if the potential owner window got closed
            this.ownerWindow.Closed += this.OnOwnerWindowOnClosed;
        }

        LogHelper.WriteLine("Showing snoop UI...");
        
        this.Show();
        this.Activate();

        LogHelper.WriteLine("Shown and activated snoop UI.");

        return this;
    }

    private void OnOwnerWindowOnClosed(object? o, EventArgs eventArgs)
    {
        if (this.ownerWindow is not null)
        {
            this.ownerWindow.Closed -= this.OnOwnerWindowOnClosed;
        }

        this.Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        ExceptionHandler.RemoveExceptionHandler(this.Dispatcher);

        base.OnClosed(e);
    }

    protected abstract void Load(object rootToInspect);
}