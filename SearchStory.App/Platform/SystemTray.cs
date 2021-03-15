using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
namespace SearchStory.App.Platform
{
#if Windows
    using System.Windows.Forms;
    using System.Diagnostics;
    public class SystemTrayIconManager : ITrayManager
    {
        public Task Instantiate()
        {
            System.Console.WriteLine("Windows icon created");
            System.Console.Read();
            Task.Run(
                () => Application.Run(new CustomTrayContext())
            );
            

            return Task.CompletedTask;
        }

        public class CustomTrayContext : ApplicationContext
        {
            private readonly NotifyIcon trayIcon;

            public CustomTrayContext()
            {
                // Initialize Tray Icon
                System.Console.WriteLine("HERE " + string.Join(", ", typeof(Program).Assembly.GetManifestResourceNames()));
                trayIcon = new NotifyIcon()
                {
                    Text = "SearchStory.App",
                    Visible = true,
                    // TODO: read from manifest if production
                    Icon = new System.Drawing.Icon(typeof(Program).Assembly.GetManifestResourceStream("SearchStory.App.wwwroot.favicon.ico")!),
                    BalloonTipTitle = "SearchStory",
                    BalloonTipText = "double click to open options"
                };
                trayIcon.DoubleClick += (sender, e) =>
                {
                    OpenApp("#");
                };
                
                OpenApp("#");
            }
            
            private static void OpenApp(string path)
            {
                var proc = new Process();
                proc.StartInfo.FileName = "cmd";
                proc.StartInfo.ArgumentList.Add("/c");
                proc.StartInfo.ArgumentList.Add("start");
                proc.StartInfo.ArgumentList.Add($"http://localhost:5000/{path}");
                proc.Start();
            }
        }
    }
#else
    public class SystemTrayIconManager : ITrayManager
    {
        public Task Instantiate()
        {
            System.Console.WriteLine("Non-windows platform. No icon created");
            return Task.CompletedTask;
        }
    }
#endif
    public interface ITrayManager
    {
        Task Instantiate();
    }
}