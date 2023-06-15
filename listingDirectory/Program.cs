using listingDirectory;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static public int SelectedIndex = 0;
        static void Main(string[] args)
        {
            var path = "C:\\games\\aplikacje serwerowe stadnik";
            Console.WriteLine("main:>" + path);
            var dirs = ShowDirs(path);
            var files = ShowFiles(path);
            preform(dirs);
            Console.ResetColor();
            //while (Console.ReadKey().KeyChar != 'z')
            //{
            //    //Console.WriteLine(SelectedIndex);
            //    var key = Console.ReadKey().KeyChar;

            //    if (key == 'w')
            //    {
            //        var path1 = dirs.ElementAt(SelectedIndex).FullName;
            //        //Console.WriteLine(path1);
            //        dirs = ShowDirs(path1);
            //        files = ShowFiles(path1);
            //    }
            //    if (key == 'd')
            //    {
            //        Console.WriteLine("a");
            //        SelectedIndex = SelectedIndex + 1;
            //        if(SelectedIndex > dirs.Count()-1)
            //            SelectedIndex = dirs.Count()-1;
            //        var path1 = dirs.ElementAt(SelectedIndex).FullName;
            //        dirs = ShowDirs(path);
            //        files = ShowFiles(path);
            //    }
            //    if (key == 'a')
            //    {
            //        SelectedIndex = SelectedIndex - 1;
            //        if (SelectedIndex < 0)
            //            SelectedIndex = 0;
            //        var path1 = dirs.ElementAt(SelectedIndex).FullName;
            //        dirs = ShowDirs(path);
            //        files = ShowFiles(path);
            //    }
            //    if (key == 's')
            //    {
            //        SelectedIndex = 0;
            //        //if(dirs.ElementAt(SelectedIndex))
            //        Console.WriteLine(dirs.Count());
            //        var path1 = "C:\\games\\aplikacje serwerowe stadnik";
            //        if (dirs.Count() !=0)
            //        {
            //            path1 = dirs.ElementAt(SelectedIndex).FullName;
            //            var arr = path1.Split('\\').SkipLast(2);
            //            path1 = string.Join('\\', arr);
            //        }

            //        //if (path1)
            //            //path1 = path;
            //        Console.WriteLine(path1);
            //        path = path1;
            //        dirs = ShowDirs(path1);
            //        files = ShowFiles(path1);

            //    }
            //}
            Console.ReadKey();
        }

        private static IEnumerable<DirectoryInfo> ShowDirs(string path)
        {
            var dirInfo = new DirectoryInfo(path);

            var dirs = dirInfo.EnumerateDirectories("*", new EnumerationOptions { RecurseSubdirectories = false });
            foreach (var (item, index) in dirs.Select((value, i) => (value, i)))
            {
                if (index == SelectedIndex)
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ResetColor();
                Console.WriteLine(item);
            }

            return dirs;
        }
        private static IEnumerable<string> ShowFiles(string path)
        {

            var files = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly);
            Console.ResetColor();
            Console.WriteLine("===files==");
            foreach (var file in files)
            {
                Console.ResetColor();
                Console.WriteLine(file);
            }
            Console.WriteLine("==========");
            return files;
        }

        static async Task preform(IEnumerable<DirectoryInfo> dirs)
        {
            await Task.Run(() =>
            {
                ConnectWebSocket(dirs);
            });

        }
        private static async Task<WebSocketReceiveResult> ConnectWebSocket(IEnumerable<DirectoryInfo> dirs)
        {
            //Console.WriteLine("Sending message: ");
            string url = "ws://172.20.10.3:1337";
            ClientWebSocket websocket = new ClientWebSocket();
            await websocket.ConnectAsync(new Uri(url), CancellationToken.None);

            byte[] incomingData = new byte[1024];
            Boolean flag = false;
            double y = -10;
            WebSocketReceiveResult result = await websocket.ReceiveAsync(new ArraySegment<byte>(incomingData), CancellationToken.None);
            //Console.WriteLine("aaa");]
            var firstCal = Encoding.UTF8.GetString(incomingData, 0, result.Count);
            var path = "C:\\games\\aplikacje serwerowe stadnik";
            while (!result.CloseStatus.HasValue)
            {
                var data = Encoding.UTF8.GetString(incomingData, 0, result.Count);
                //Console.WriteLine("Received message: " + data);
                Paramets getResult = JsonConvert.DeserializeObject<Paramets>(data);
                //Console.WriteLine("Received: " + getResult.x);
                result = await websocket.ReceiveAsync(new ArraySegment<byte>(incomingData), CancellationToken.None);
                IEnumerable<string> files;
                if ((getResult.y < 0.1 && getResult.y > -0.1) && (getResult.z < 0.1 && getResult.z > -0.1) && flag)
                {
                    flag = false;
                }
                if (flag)
                {
                    continue;
                }

                //getResult.y < -0.60 && getResult.y > -0.70
                if (getResult.y > 0.60)
                {
                    Console.Clear();
                    y = getResult.y;
                    var path1 = dirs.ElementAt(SelectedIndex).FullName;
                    SelectedIndex = 0;
                    Console.WriteLine("main:>" + path1);
                    path = path1;
                    dirs = ShowDirs(path1);
                    files = ShowFiles(path1);
                    path = path1;
                    flag = true;
                    continue;
                }

                if (getResult.y < -0.60)
                {
                    Console.Clear();
                    SelectedIndex = 0;
                    var path1 = path;
                    //if(dirs.ElementAt(SelectedIndex))
                    if (dirs.Count() != 0)
                    {
                        path1 = dirs.ElementAt(SelectedIndex).FullName;
                        var arr = path1.Split('\\').SkipLast(2);
                        path1 = string.Join('\\', arr);
                    }
                    else
                    {
                        var arr = path1.Split('\\').SkipLast(1);
                    path1 = string.Join('\\', arr);

                    }
                    //if (path1)
                    //path1 = path;
                    Console.WriteLine("main:>" + path1);
                    path = path1;
                    dirs = ShowDirs(path1);
                    files = ShowFiles(path1);
                    flag = true;
                    continue;
                }
                if (getResult.z > 0.60)
                {
                    Console.Clear();
                    SelectedIndex = SelectedIndex + 1; 
                    if (SelectedIndex > dirs.Count())
                        SelectedIndex = dirs.Count() - 1;
                    var path1 = dirs.ElementAt(SelectedIndex).FullName;
                    Console.WriteLine("main:>" + path);
                    dirs = ShowDirs(path);
                    files = ShowFiles(path);
                    flag = true;
                    continue;
                }
                if (getResult.z < -0.60)
                {
                    Console.Clear();
                    SelectedIndex = SelectedIndex - 1;
                    if (SelectedIndex < 0)
                        SelectedIndex = 0;
                    var path1 = dirs.ElementAt(SelectedIndex).FullName;
                    Console.WriteLine("main:>" + path);
                    dirs = ShowDirs(path);
                    files = ShowFiles(path);
                    flag = true;
                    continue;
                }
                /*
                if(getResult.z > 0.60)
                      w prawo
                if(getResult.z < -0.60)
                    w lewo
                if(getResult.y > 0.60)
                    wybierz
                if(getResult.y < -0.60)
                    cofnij
                */

            }
            return result;
        }
    }
}