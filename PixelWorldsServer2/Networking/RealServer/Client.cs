using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using Newtonsoft.Json.Linq;
using System.Threading;
using Kernys.Bson;
using System.IO;
namespace RealPW
{

    public class Client
    {
        #region Useless test
        public static byte[] aa = new byte[1024];
        #endregion
        public Client instance;
        public static int dataBufferSize = 1024;

        public static string ip = "44.194.163.69";
        public static int port = 10001;
        public int myId = 0;
        public TCP tcp;
        public BSONObject WorldData;
        public string task;
        public string taskStatus = "not_finished";
        public string WorldName;

        public void ConnectToServer()
        {
            tcp = new TCP(this);
            tcp.Connect();

        }
        public void GetWorldData(string WorldName)
        {
            tcp = new TCP(this);
            tcp.Connect();
            this.task = "CloneWorld";
            this.WorldName = WorldName;

        }

        public class TCP
        {
            public Client client;
            public TCP(Client client)
            {
                this.client = client;
            }
            public static byte[] LastPaket = new byte[0];
            public static TcpClient socket;
            public static bool AutoFish = false;
            public static NetworkStream stream;
            public static byte[] receiveBuffer;

            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };

                receiveBuffer = new byte[dataBufferSize];
                socket.BeginConnect(Client.ip, Client.port, ConnectCallback, socket);
            }

            private void ConnectCallback(IAsyncResult _result)
            {
                socket.EndConnect(_result);

                if (!socket.Connected)
                {
                    return;
                }

                stream = socket.GetStream();
                Packets.Start();
                Packets.SendMessages();
                ReceiveLength = stream.Read(receiveBuffer, 0, dataBufferSize);
                if (client.task == "CloneWorld")
                    ReceiveMessageCallbackCloneWorld();
            }
            public static StateObject stateObject = new StateObject();
            public static int ReceiveLength = -1;
            public void ReceiveMessageCallbackCloneWorld()
            {
                object obj = Packets.socketLock;
                lock (obj)
                {
                    try
                    {
                        int _byteLength = ReceiveLength;
                        if (_byteLength <= 0)
                        {
                            return;
                        }
                        Array.Copy(receiveBuffer, stateObject.buffer, _byteLength);
                        Socket workSocket = stateObject.workSocket;
                        if (stateObject.data == null)
                        {
                            ReceiveLength = BitConverter.ToInt32(stateObject.buffer, 0);
                            Console.WriteLine("Receiving " + ReceiveLength + "bytes, please wait.");
                            stateObject.data = new byte[ReceiveLength - 4];
                            Array.Copy(stateObject.buffer, 4, stateObject.data, 0, _byteLength - 4);
                            stateObject.bytesReadToData = _byteLength - 4;
                        }
                        else
                        {
                            Array.Copy(stateObject.buffer, 0, stateObject.data, stateObject.bytesReadToData, _byteLength);
                            stateObject.bytesReadToData += _byteLength;
                            Console.WriteLine("Received " + stateObject.bytesReadToData + "bytes.");
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
                                    ReceiveCloneWorldCallback((BSONObject)packets["m" + i]);
                                }
                                Console.WriteLine("Packet Length : " + stateObject.data.Length);
                                Console.WriteLine("0x" + BitConverter.ToString(stateObject.data).Replace("-", ", 0x"));
                                stateObject = new StateObject();
                            }
                            Packets.SendMessages();
                            ReceiveLength = stream.Read(receiveBuffer, 0, dataBufferSize);
                            ReceiveMessageCallbackCloneWorld();

                        }
                        else
                        {
                            ReceiveLength = stream.Read(receiveBuffer, 0, dataBufferSize);
                            ReceiveMessageCallbackCloneWorld();
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            private void ReceiveCloneWorldCallback(BSONObject packet)
            {
                try
                {
                    if ((string)packet["ID"] == "VChk")
                    {
                        Packets.LogIn("us-east-1:dcd564f1-8573-40d6-a3b7-e118c2443947", "8vFidfnsRjdXa10iP7RzUQ5/nl/On0kv0k6xx86Zk/g=");
                    }
                    else if ((string)packet["ID"] == "ST")
                    {
                        if (Packets.isRTP)
                        {
                            Packets.isRTP = false;
                            Tools.STWait(2000);
                        }
                        else if (packet.ContainsKey("SSLp"))
                        {
                        }
                        else
                        {
                            Packets.ST();
                        }
                    }
                    else if ((string)packet["ID"] == "GPd")
                    {
                        Packets.EnterWorld(client.WorldName);
                    }
                    else if ((string)packet["ID"] == "TTjW")
                    {
                        Thread.Sleep(200);
                        Packets.EnterWorld2(client.WorldName);
                    }
                    else if ((string)packet["ID"] == "GWC")
                    {
                        client.WorldData = SimpleBSON.Load(PixelWorldsServer2.Util.LZMAHelper.DecompressLZMA(packet["W"].binaryValue));
                        TCP.stream.Close(0);
                        TCP.socket.Close();
                        client.taskStatus = "Finished";

                    }
                    else if ((string)packet["ID"] == "OoIP")
                    {
                        IPAddress[] addresslist = Dns.GetHostAddresses(packet["IP"].stringValue);
                        if (addresslist.Length > 0 && !IsLocalIpAddress(addresslist[0]))
                        {
                            Client.ip = addresslist[0].ToString();
                            stream.Close();
                            socket.Close();
                            this.Connect();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }


            }
            public static bool IsLocalIpAddress(IPAddress ipAddress)
            {
                // Get the local IP addresses of the machine
                IPAddress[] localIpAddresses = Dns.GetHostAddresses(Dns.GetHostName());

                // Check if the provided IP address matches any of the local IP addresses
                foreach (IPAddress localIpAddress in localIpAddresses)
                {
                    if (IPAddress.Equals(ipAddress, localIpAddress))
                    {
                        return true;
                    }
                }

                return false;
            }

        }
    }
}
