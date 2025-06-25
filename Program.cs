using Microsoft.Toolkit.Uwp.Notifications;

namespace RdpScopeTogglerToastListener
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            bool createdNew;
            using (var mutex = new Mutex(true, "RdpScopeTogglerToastMutex", out createdNew))
            {
                if (!createdNew)
                    return; // Already running → exiting

                // NotifyIcon — Taskbar icon
                NotifyIcon notifyIcon = new()
                {
                    Icon = new System.Drawing.Icon("Assets/remote-desktop.ico"),
                    Visible = true,
                    Text = "Rdp Scope Toggler Listener"
                };

                // Listening to goodbye
                ToastNotificationManagerCompat.OnActivated += toastArgs => { };

                string path = @"C:\ProgramData\RdpScopeToggler";
                string fileName = "ToastMessage.txt";
                string fullFilePath = Path.Combine(path, fileName);

                Directory.CreateDirectory(path);

                var watcher = new FileSystemWatcher(path, fileName)
                {
                    NotifyFilter = NotifyFilters.LastWrite
                };

                watcher.Changed += (s, e) =>
                {
                    try
                    {
                        Thread.Sleep(100);
                        string content = File.ReadAllText(fullFilePath).Trim();

                        if (!string.IsNullOrWhiteSpace(content))
                        {
                            new ToastContentBuilder()
                                .AddText("Rdp Scope Toggler")
                                .AddText(content)
                                .Show();
                        }
                    }
                    catch { }
                };

                watcher.EnableRaisingEvents = true;

                // Keeps the process active without a window
                Application.Run();
            }
        }
    }
}
