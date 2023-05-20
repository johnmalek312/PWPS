using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows;
using System.IO;
using System.Net;
using System.Numerics;
using System.Net.Sockets;
using Kernys.Bson;
using System.Threading;
using System.Diagnostics;

namespace RealPW
{
    
    class ConfigData
    {
        public static float tileSizeX { get; set; } = 0.32f;
        public static float tileSizeY { get; set; } = 0.32f;
        public static Point CurrentPosition = new Point();
    }
    class Tools
    {
        public static readonly NetworkStream stream = RealPW.Client.TCP.stream;
        public static byte[] WalkingBytes = new byte[RealPW.Client.dataBufferSize];
        public static List<string> FishingPacketID = new List<string> { "TrTFFMP", "MGSt", "MGA", "CmB" }; 
        public static int x;
        public static int y;
        public static byte[] receiveBufferr = new byte[RealPW.Client.dataBufferSize];
        static StateObject stateObject = new StateObject();
        static int dataBufferSize = 1024;
        public static int ReceiveLengthh = -1;
        public static void WalkWait()
        {
            object obj = Packets.socketLock;
            lock (obj)
            {
                try
                {
                    if (ReceiveLengthh <= 0)
                    {
                        Console.WriteLine("Error Wait Is 0");
                        return;
                    }
                    Array.Copy(WalkingBytes, stateObject.buffer, ReceiveLengthh);
                    Socket workSocket = stateObject.workSocket;
                    if (stateObject.data == null)
                    {
                        int num2 = BitConverter.ToInt32(stateObject.buffer, 0);
                        stateObject.data = new byte[num2 - 4];
                        Array.Copy(stateObject.buffer, 4, stateObject.data, 0, ReceiveLengthh - 4);
                        stateObject.bytesReadToData = ReceiveLengthh - 4;
                    }
                    else
                    {
                        Array.Copy(stateObject.buffer, 0, stateObject.data, stateObject.bytesReadToData, ReceiveLengthh);
                        stateObject.bytesReadToData += ReceiveLengthh;
                    }
                    if (stateObject.bytesReadToData == stateObject.data.Length)
                    {
                        object obj2 = Packets.packetsLock;
                        lock (obj2)
                        {
                            BSONObject packets = null;
                            try
                            {
                                packets = SimpleBSON.Load(stateObject.data);
                            }
                            catch (Exception e) { Console.WriteLine(e); }

                            if (packets == null || !packets.ContainsKey("mc"))
                                return;

                            Console.WriteLine("Server ======================================================================================== Server");
                            for (int i = 0; i < packets["mc"]; i++)
                            {
                                Packets.ReadBSON((BSONObject)packets["m" + i]);
                            }
                            Console.WriteLine("Packet Length : " + stateObject.data.Length);
                            Console.WriteLine("0x" + BitConverter.ToString(stateObject.data).Replace("-", ", 0x"));
                            stateObject = new StateObject();
                        }
                    }
                    else
                    {
                        stream.Read(WalkingBytes, 0, dataBufferSize);
                        WalkWait();
                    }

                }
                catch (Exception ex)
                {
                    stateObject = new StateObject();
                    ReceiveLengthh = -1;
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static void PlayerGoTo(NetworkStream stream,byte[] BlockLayer, int StartX, int StartY, int FinalX, int FinalY, int slowness = 250, int a = 2, int d = 3, bool duble = false)
        {

            List<Point> Path = PathFinder.FindPath(BlockLayer, StartX, StartY, FinalX, FinalY);
            if (Path.Count!=0) 
            {
                for(int i = 0; i < Path.Count; i++)
                {
                    Position pos = new Position(Path[i], a, d);
                    Thread.Sleep(slowness);
                    pos.ToBSONObject();
                    Packets.SendMessages();
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    ReceiveLengthh = stream.Read(WalkingBytes,0, RealPW.Client.dataBufferSize);
                    WalkWait();
                    int time = slowness - (int)stopWatch.ElapsedMilliseconds;
                    if ( time > 0)
                        Thread.Sleep(time);
                    if (duble)
                    {
                        i++;
                    }
                }
                Position poss = new Position(FinalX, FinalY, 1, 3);
                poss.ToBSONObject();
                Packets.SendMessages();
                ConfigData.CurrentPosition.X = FinalX;
                ConfigData.CurrentPosition.Y = FinalY;
            }
        }
        public static void PlayerGoTo(NetworkStream stream, byte[] BlockLayer, Point StartPoint, int FinalX, int FinalY, int slowness = 250, int a = 2, int d=3, bool duble = false)
        {
            int StartX = StartPoint.X;
            int StartY = StartPoint.Y;
            PlayerGoTo(stream, BlockLayer, StartX, StartY, FinalX, FinalY, slowness, a, d, duble);
        }
        public static void PlayerGoTo(NetworkStream stream, byte[] BlockLayer, int StartX, int StartY, Point FinalPoint, int slowness = 250, int a = 2, int d = 3, bool duble = false)
        {
            int FinalX = FinalPoint.X;
            int FinalY = FinalPoint.Y;
            PlayerGoTo(stream, BlockLayer, StartX, StartY, FinalX, FinalY, slowness, a, d, duble);
        }
        public static void PlayerGoTo(NetworkStream stream, byte[] BlockLayer, Point StartPoint, Point FinalPoint, int slowness = 250, int a = 2, int d = 3, bool duble = false)
        {
            int StartX = StartPoint.X;
            int StartY = StartPoint.Y;
            int FinalX = FinalPoint.X;
            int FinalY = FinalPoint.Y;
            PlayerGoTo(stream, BlockLayer, StartX, StartY, FinalX, FinalY, slowness, a, d, duble);
        }
        public static void STWait(int millisecond, int slowness = 300)
        {
            for (int II = 0; II < millisecond;)
            {
                //if ((millisecond - II) <= slowness)
                //{
                //    Thread.Sleep(millisecond - II);
                //}
                //else
                Packets.ST();
                ClearNetStream(ref RealPW.Client.TCP.stream);
                Packets.SendMessages();
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                ReceiveLengthh = stream.Read(WalkingBytes, 0, RealPW.Client.dataBufferSize);
                WalkWait();
                int time = slowness - (int)stopWatch.ElapsedMilliseconds;
                stopWatch.Stop();
                Console.WriteLine(time + "           " + II);
                if (time > 0)
                {
                    Thread.Sleep(time);
                    II += time;
                }
                else
                {
                    break;
                }
                //Packets.ReadBSON(objj);

            }
        }
        public static void Wait(int millisecond, int slowness = 300)
        {
            for (int II = 0; II < millisecond;)
            {
                //if ((millisecond - II) <= slowness)
                //{
                //    Thread.Sleep(millisecond - II);
                //}
                //else
                Packets.mPWait();
                ClearNetStream(ref RealPW.Client.TCP.stream);
                Packets.SendMessages();
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                ReceiveLengthh = stream.Read(WalkingBytes, 0, RealPW.Client.dataBufferSize);
                WalkWait();
                int time = slowness - (int)stopWatch.ElapsedMilliseconds;
                if (II > 900)
                {

                }
                stopWatch.Stop();
                Console.WriteLine(time + "           " + II);
                if (time > 0)
                {
                    Thread.Sleep(time);
                    II += time;
                }
                else
                {
                    break;
                }
                //Packets.ReadBSON(objj);

            }
        }
        public static Vector2 ConvertWorldPointToMapPoint(float X, float Y)
        {
            Vector2 mapPoint = new Vector2();

            mapPoint.X = (int)((X + ConfigData.tileSizeX * 0.5f) / ConfigData.tileSizeX);
            mapPoint.Y = (int)((Y + ConfigData.tileSizeY * 0.5f) / ConfigData.tileSizeY);
            return mapPoint;
        }
        public static Vector2 ConvertWorldPointToMapPoint(Vector2 worldPoint)
        {
            Vector2 mapPoint = new Vector2();

            mapPoint.X = (int)((worldPoint.X + ConfigData.tileSizeX * 0.5f) / ConfigData.tileSizeX);
            mapPoint.Y = (int)((worldPoint.Y + ConfigData.tileSizeY * 0.5f) / ConfigData.tileSizeY);
            return mapPoint;
        }

        public static Vector2 ConvertPlayerMapPointToWorldPoint(int x, int y)
        {
            return new Vector2(x * ConfigData.tileSizeX, y * ConfigData.tileSizeY - ConfigData.tileSizeY * 0.5f);
        }
        public static Vector2 ConvertPlayerMapPointToWorldPoint(Vector2 MapPoint)
        {
            return new Vector2(MapPoint.X * ConfigData.tileSizeX, MapPoint.Y * ConfigData.tileSizeY - ConfigData.tileSizeY * 0.5f);
        }
        public static T[] MergeArray<T>(T[] array1,T[] array2)
        {
            T[] newArray = new T[array1.Length + array2.Length];
            Array.Copy(array1, newArray, array1.Length);
            Array.Copy(array2, 0, newArray, array1.Length, array2.Length);
            return newArray;
        }
            
        public static void ClearNetStream(ref NetworkStream stream)
        {
            byte[] junkbuffer = new byte[4096];
            while (stream.DataAvailable)
            {
                stream.Read(junkbuffer, 0, junkbuffer.Length);
            }
        }
    }
    public class Position
    {
        public byte[] pM { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public int a { get; set; } = 2;
        public int d { get; set; } = 3;
        public Position(byte[] PM,double X,double Y,int A =2,int D=3)
        {
            x = X;
            y = Y;
            pM = PM;
            a = A;
            d = D;
        }
        public Position(double X, double Y, int A = 2, int D = 3)
        {
            Vector2 MapPoint = Tools.ConvertWorldPointToMapPoint((float)X,(float)Y);
            x = X;
            y = Y;
            pM = Tools.MergeArray(BitConverter.GetBytes(MapPoint.X), BitConverter.GetBytes(MapPoint.Y));
            a = A;
            d = D;
        }
        public Position(byte[] PM, int A = 2, int D = 3)
        {
            Vector2 WorldPoint = Tools.ConvertPlayerMapPointToWorldPoint(BitConverter.ToInt32(PM,0), BitConverter.ToInt32(PM, 4));
            x = WorldPoint.X;
            y = WorldPoint.Y;
            pM = PM;
            a = A;
            d = D;
        }
        public Position(Point point, int A = 2, int D = 3)
        {
            x = Tools.ConvertPlayerMapPointToWorldPoint(point.X,point.Y).X;
            y = Tools.ConvertPlayerMapPointToWorldPoint(point.X, point.Y).Y;
            pM = Tools.MergeArray(BitConverter.GetBytes(point.X), BitConverter.GetBytes(point.Y));
            a = A;
            d = D;
        }
        public Position(int X, int Y = 2, int A = 2, int D = 3)
        {
            x = Tools.ConvertPlayerMapPointToWorldPoint(X, Y).X;
            y = Tools.ConvertPlayerMapPointToWorldPoint(X, Y).Y;
            pM = Tools.MergeArray(BitConverter.GetBytes(X), BitConverter.GetBytes(Y));
            a = A;
            d = D;
        }
    }
}
