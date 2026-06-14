using Common;
using GameInterFace;
using GameServer.Managers.Data;
using GameServer.Managers.Entities;
using GameServer.Managers.MBattle;
using GameServer.Services.Data;
using Network;
using System.Reflection;

namespace GameServer
{
    class GameServer
    {
        Thread thread;
        bool running = false;
        NetService network;

        public bool Init(int port, string ip, string connectStr)
        {
            Log.Info("GameServer Init...");
            DBService.Instance.Init(connectStr);
            DataManager.Instance.Load();
            ServiceInit(() =>
            {
                network = new NetService();
                network.Init(port, ip);
            },
            (object instance) =>
            {
                Log.Info($"{instance.GetType().Name} Initialized");
            }
            );
            thread = new Thread(new ThreadStart(this.Update));
            return true;
        }

        public void ServiceInit(Action OnFinished = null, Action<object> OnInited = null)
        {
            var types = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(t =>
            t.IsClass &&
            !t.IsAbstract &&
            typeof(IInitializable).IsAssignableFrom(t) // 非泛型接口判断
        )
        .ToList();
            foreach (var type in types)
            {
                // 递归查找 Instance 属性，包括父类
                PropertyInfo instanceProperty = null;
                var currentType = type;
                // 向上查找直到找到 Instance 属性或到达 Object 类为止
                while (currentType != null && instanceProperty == null)
                {
                    instanceProperty = currentType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                    currentType = currentType.BaseType;
                }

                if (instanceProperty != null)
                {

                    var instance = instanceProperty.GetValue(null);
                    if (instance is IInitializable initializable) // 直接转换为接口
                    {
                        initializable.Init(); // 直接调用方法
                        OnInited?.Invoke(instance);
                    }
                }
                else
                {
                    Log.Error($"No Instance property found for {type.Name}");
                }
            }
            OnFinished?.Invoke();
        }

        public void Start()
        {
            network.Start();
            running = true;
            thread.Start();
        }


        public void Stop()
        {
            running = false;
            thread.Join();
            network.Stop();
        }

        public void Update()
        {
            while (running)
            {
                var mapManager = MapManager.Instance;
                var arenaManager = ArenaManager.Instance;
                Time.Tick();
                Thread.Sleep(100);//一秒10帧
                //Console.WriteLine("{0} {1} {2} {3} {4}", Time.deltaTime, Time.frameCount, Time.ticks, Time.time, Time.realtimeSinceStartup);
                mapManager.Update();
                arenaManager.Update();
            }
        }
    }
}
