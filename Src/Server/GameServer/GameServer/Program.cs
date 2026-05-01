using Common;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            FileInfo fi = new FileInfo("log4net.xml");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(fi);
            Log.Init("GameServer");

            Log.Info("Config Loading...");
            var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
            string connectStr = config.GetConnectionString("DefaultConnection");
            string ip = config["ServerSettings:IP"];
            int port = config.GetValue<int>("ServerSettings:Port");
            Log.Info("Config Loaded");

            Log.Info("Game Server Init");
            GameServer server = new GameServer();
            server.Init(port, ip, connectStr);
            server.Start();
            Console.WriteLine("Game Server Running......");
            CommandHelper.Run();
            Log.Info("Game Server Exiting...");
            server.Stop();
            Log.Info("Game Server Exited");
        }
    }
}
