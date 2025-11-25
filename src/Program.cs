using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store.Preview.InstallControl;

static class Program
{
    static Program() => AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
    {
        MessageBox.Show($"{args.ExceptionObject}".Trim(), "Patchstop", MessageBoxButtons.OK, MessageBoxIcon.Error);
        Environment.Exit(1);
    };

    static void Main()
    {
        NotifyIcon icon = new()
        {
            Visible = true,
            Text = "Patchstop",
            ContextMenu = new(),
            Icon = SystemIcons.Error
        };

        AppInstallManager manager = new()
        {
            AutoUpdateSetting = AutoUpdateSetting.Disabled
        };

        icon.ContextMenu.MenuItems.Add("Exit", (_, _) =>
        {
            icon.Dispose();
            Environment.Exit(0);
        });

        foreach (var item in manager.AppInstallItems)
        {
            if (item.InstallType != AppInstallType.Update) continue;
            try { item.Cancel(); } catch { }
        }

        manager.ItemStatusChanged += (sender, args) =>
        {
            if (args.Item.InstallType != AppInstallType.Update) return;
            try { args.Item.Cancel(); } catch { }
        };

        Application.Run();
    }
}