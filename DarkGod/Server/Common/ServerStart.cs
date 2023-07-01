using System.Threading;

namespace Server.Common
{

    /// <summary>
    /// 服务器入口
    /// </summary>
    public class ServerStart
    {
        public static void Main(string[] args)
        {
            ServerRoot.Instance.Init();

            //防止服务退出
            while (true)
            {
                ServerRoot.Instance.Update();
                Thread.Sleep(20);
            }
        }
    }
}