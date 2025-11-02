using System;
using System.Linq;
using System.Threading;
using Windows.ApplicationModel.Store.Preview.InstallControl;

static class Program
{
    static void Main()
    {
        AppInstallManager appInstallManager = new();

        switch (appInstallManager.AutoUpdateSetting)
        {
            case AutoUpdateSetting.Disabled:
                Console.WriteLine($"[{DateTime.Now}] Automatic updates are disabled.");
                break;

            case AutoUpdateSetting.Enabled:
                Console.WriteLine($"[{DateTime.Now}] Automatic updates were enabled, is now disabled");
                appInstallManager.AutoUpdateSetting = AutoUpdateSetting.Disabled;
                break;

            case AutoUpdateSetting.DisabledByPolicy:
                Console.WriteLine($"[{DateTime.Now}] Automatic updates are forced disabled via GPO.");
                break;

            case AutoUpdateSetting.EnabledByPolicy:
                Console.WriteLine($"[{DateTime.Now}] Automatic updates are forced enabled via GPO, please disable it.");
                break;
        }

        foreach (var appInstallItem in appInstallManager.AppInstallItems.Concat(appInstallManager.AppInstallItemsWithGroupSupport))
        {
            if (appInstallItem.InstallType is not AppInstallType.Update) continue;
            Console.WriteLine($"[{DateTime.Now}] Cancelling update for \"{appInstallItem.PackageFamilyName}\"...");
            try { appInstallItem.Cancel(); } catch { }
        }

        appInstallManager.ItemStatusChanged += (sender, args) =>
        {
            if (args.Item.InstallType is not AppInstallType.Update) return;
            Console.WriteLine($"[{DateTime.Now}] Cancelling update for \"{args.Item.PackageFamilyName}\"...");
            try { args.Item.Cancel(); } catch { }
        };


        Console.WriteLine($"[{DateTime.Now}] Monitoring for any updates & cancelling them...");
        Thread.Sleep(Timeout.Infinite);
    }
}