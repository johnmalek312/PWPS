using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Kernys.Bson;
namespace RealPW
{
    static class Packets
    {
        public static bool isRTP = false;
        public static NetworkStream stream { get { return RealPW.Client.TCP.stream; } }
        public static byte[] map = new byte[0];
        public static List<BSONObject> messagesToSend = new List<BSONObject>();
        public static List<BSONObject> messagesToReceive = new List<BSONObject>();
        public static BSONObject obj = new BSONObject();
        public static readonly object sendLock = new object();
        public static readonly object messagesLock = new object();
        public static readonly object socketLock = new object();
        public static readonly object packetsLock = new object();
        public static List<byte[]> packetsForNetworkClient = new List<byte[]>();
        public static void Start()
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "VChk";
            obji["OS"] = "WindowsPlayer";
            obji["OSt"] = 3;
            AddOneMessageToList(obji);
        }
        public static void LogIn(string cognito = "us-east-1:fcb56a31-0b69-438d-ad70-fa1915fc34ef", string token = "ivlxxg1x2uOVMWw6/HXN3Io+9ASVnY4Wj+qbogq3k/Y=")
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "GPd";
            obji["CoID"] = cognito;
            obji["Tk"] = token;
            obji["cgy"] = 877;
            AddOneMessageToList(obji);
        }
        public static void ST()
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "ST";
            obji["T"] = DateTime.UtcNow.Ticks;
            AddOneMessageToList(obji);
        }
        public static void BotToTutorial(string worldname = "TUTORIAL2")
        {
            BSONObject Gw = new BSONObject();
            Gw["ID"] = "Gw"; //string
            Gw["eID"] = "Start"; //string
            Gw["W"] = worldname; //string
            Gw["WB"] = 0; //int
            Packets.AddOneMessageToList(Gw);
            BSONObject A = new BSONObject();
            A["ID"] = "A"; //string
            A["AE"] = 2; //int
            Packets.AddOneMessageToList(A);
            A = new BSONObject();
            A["ID"] = "A"; //string
            A["AE"] = 6; //int
            Packets.AddOneMessageToList(A);
            A = new BSONObject();
            A["ID"] = "A"; //string
            A["AE"] = 14; //int
            Packets.AddOneMessageToList(A);
            A = new BSONObject();
            A["ID"] = "A"; //string
            A["AE"] = 24; //int
            Packets.AddOneMessageToList(A);
            BSONObject GSb = new BSONObject();
            GSb["ID"] = "GSb"; //string
            Packets.AddOneMessageToList(GSb);
        }
        public static void BotToTutorialConfirm(string worldname="TUTORIAL2")
        {
            Packets.EnterWorldConfirm1(worldname);
            Packets.EnterWorldConfirm2();
        }
        public static void CheckEnteredWorld(string WorldName)
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "MWli";
            obji["WN"] = WorldName;
            AddOneMessageToList(obji);
        }
        public static void EnterWorld(string WorldName)
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "TTjW";
            obji["W"] = WorldName;
            obji["Amt"] = 0;
            AddOneMessageToList(obji);
        }
        public static void EnterWorld2(string WorldName)
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "Gw";
            obji["eID"] = "";
            obji["W"] = WorldName;
            AddOneMessageToList(obji);
            obji = new BSONObject();
            obji["ID"] = "A";
            obji["AE"] = "6";
            AddOneMessageToList(obji);
            obji = new BSONObject();
            obji["ID"] = "A";
            obji["AE"] = "14";
            AddOneMessageToList(obji);
            obji = new BSONObject();
            obji["ID"] = "A";
            obji["AE"] = "24";
            AddOneMessageToList(obji);
            obji = new BSONObject();
            obji["ID"] = "GSb";
            AddOneMessageToList(obji);

        }
        public static void EnterWorldConfirm0()
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "p";
            AddOneMessageToList(obji);

        }
        public static void EnterWorldConfirm1(string WorldName)
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "ULS";
            obji["LS"] = WorldName;
            AddOneMessageToList(obji);

        }
        public static void EnterWorldConfirm2()
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "cZL";
            obji["CZL"] = "2";
            AddOneMessageToList(obji);
            obji = new BSONObject();
            obji["ID"] = "cZva";
            obji["Amt"] = "0.5";
            AddOneMessageToList(obji);
            obji = new BSONObject();
            obji["ID"] = "rOP";
            AddOneMessageToList(obji);
            obji = new BSONObject();
            obji["ID"] = "rAIp";
            AddOneMessageToList(obji);
            obji = new BSONObject();
            obji["ID"] = "rAI";
            AddOneMessageToList(obji);
        }
        public static void EnterWorldConfirm3()
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "RtP";
            AddOneMessageToList(obji);
        }
        public static void MakePostion(byte[] pM, double x, double y, int a = 1, int d = 3, bool tp = true)
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "mp";
            obji["pM"] = pM;
            AddOneMessageToList(obji);
            obji = new BSONObject();
            obji["ID"] = "mP";
            obji["t"] = DateTime.UtcNow.Ticks;
            obji["x"] = x;
            obji["y"] = y;
            obji["a"] = a;
            obji["d"] = d;
            obji["tp"] = tp;
            AddOneMessageToList(obji);
        }
        public static void MakePostion(byte[] pM)
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "mp";
            obji["pM"] = pM;
            AddOneMessageToList(obji);
            obji = new BSONObject();
            obji["ID"] = "mP";
            AddOneMessageToList(obji);
        }
        public static void ToBSONObject(this Position pos)
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "mp";
            obji["pM"] = pos.pM;
            AddOneMessageToList(obji);
            obji = new BSONObject();
            obji["ID"] = "mP";
            obji["t"] = DateTime.UtcNow.Ticks;
            obji["x"] = pos.x;
            obji["y"] = pos.y;
            obji["a"] = pos.a;
            obji["d"] = pos.d;
            AddOneMessageToList(obji);
        }
        /*public static BSONObject RetPostion(byte[] pM, double x, double y, int a = 1, int d = 3, bool tp = true)
        {
            BSONObject obj2 = new BSONObject();
            obj2["mc"] = mc;
            obj2["m" + (mc - 2)] = new BSONObject();
            obj2["m" + (mc - 2)]["ID"] = "mp";
            obj2["m" + (mc - 2)]["pM"] = pM;
            AddOneMessageToList(obji);
            obj2["m" + (mc - 1)] = new BSONObject();
            obj2["m" + (mc - 1)]["ID"] = "mP";
            obj2["m" + (mc - 1)]["t"] = DateTime.UtcNow.Ticks;
            obj2["m" + (mc - 1)]["x"] = x;
            obj2["m" + (mc - 1)]["y"] = y;
            obj2["m" + (mc - 1)]["a"] = a;
            obj2["m" + (mc - 1)]["d"] = d;
            AddOneMessageToList(obji);
        }*/
        public static void mPWait()
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "mP";
            AddOneMessageToList(obji);
        }
        public static void CheckPlaceLure(int LureID, int x, int y)
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "TrTFFMP";
            obji["BT"] = LureID;
            obji["x"] = x;
            obji["y"] = y;
            AddOneMessageToList(obji);
        }
        public static void PlaceLure(int LureID, int x, int y, int MGT = 2)
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "MGSt";
            obji["MGT"] = MGT;
            obji["x"] = x;
            obji["y"] = y;
            obji["BT"] = LureID;
            AddOneMessageToList(obji);
        }
        public static void CatchStrike(int LS = 2, int MGT = 2)
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "MGA";
            obji["MGT"] = MGT;
            obji["MGD"] = DateTime.UtcNow.Ticks;
            obji["LS"] = LS;
            AddOneMessageToList(obji);
        }
        public static void CheckCatchFish(int LS = 1, int MGT = 2, int vI = 1031, int idx = 1352, double Amt = -0.5)
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "MGA";
            obji["MGT"] = MGT;
            obji["MGD"] = DateTime.UtcNow.Ticks;
            obji["LS"] = LS;
            obji["vI"] = vI;
            obji["Idx"] = idx;
            obji["Amt"] = Amt;
            AddOneMessageToList(obji);
        }
        /// <summary>
        /// Gender = 0 is boy, Gender = 1 is girl
        /// </summary>
        public static void PickCharacter(int Gender=0)
        {
            BSONObject CharC = new BSONObject();
            CharC["ID"] = "CharC"; //string
            CharC["Gnd"] = Gender; //int
            CharC["Ctry"] = 999; //int
            CharC["SCI"] = 7; //int
            Packets.AddOneMessageToList(CharC);
            BSONObject WeOwC = new BSONObject();
            WeOwC["ID"] = "WeOwC"; //string
            WeOwC["hBlock"] = 528; //int
            Packets.AddOneMessageToList(WeOwC);
            WeOwC = new BSONObject();
            WeOwC["ID"] = "WeOwC"; //string
            WeOwC["hBlock"] = 526; //int
            Packets.AddOneMessageToList(WeOwC);
        }
        public static void GFLi()
        {
            BSONObject GFLi = new BSONObject();
            GFLi["ID"] = "GFLI";
            Packets.AddOneMessageToList(GFLi);
        }
        public static void TutorialState(int state)
        {
            BSONObject TState = new BSONObject();
            TState["ID"] = "TState";
            TState["Tstate"] = state;
            Packets.AddOneMessageToList(TState);
        }
        public static void EnterPortal(int x, int y)
        {
            BSONObject PAoP = new BSONObject();
            PAoP["ID"] = "PAoP"; 
            PAoP["x"] = x;
            PAoP["y"] = y;
            Packets.AddOneMessageToList(PAoP);
        }
        public static void LeavePortal(int x,int y)
        {
            BSONObject mp = new BSONObject();
            mp["ID"] = "mp"; //string
            mp["pM"] = Tools.MergeArray<byte>(BitConverter.GetBytes(x), BitConverter.GetBytes(y)); //Binary
            Packets.AddOneMessageToList(mp);
            BSONObject mP = new BSONObject();
            mP["ID"] = "mP"; //string
            mP["t"] = DateTime.UtcNow.Ticks; //int64
            mP["x"] = (double)Tools.ConvertPlayerMapPointToWorldPoint(x,y).X; //double
            mP["y"] = (double)Tools.ConvertPlayerMapPointToWorldPoint(x, y).Y; //double
            mP["a"] = 1; //int
            mP["d"] = 3; //int
            Packets.AddOneMessageToList(mP);
            BSONObject PAiP = new BSONObject();
            PAiP["ID"] = "PAiP"; //string
            PAiP["x"] = x; //int
            PAiP["y"] = y; //int
            Packets.AddOneMessageToList(PAiP);
        }
        /*public static void CatchFish()
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "BUp";
            obji["Bi"] = Tools.FishID << 24;
            AddOneMessageToList(obji);
        }*/
        public static void HitBlock(int x, int y)
        {
            BSONObject HB = new BSONObject();
            HB["ID"] = "HB"; //string
            HB["x"] = x; //int
            HB["y"] = y; //int
            Packets.AddOneMessageToList(HB);
        }
        public static void SetBlock(int x, int y, int BlockID)
        {
            BSONObject SB = new BSONObject();
            SB["ID"] = "SB";
            SB["x"] = x;
            SB["y"] = y;
            SB["BlockType"] = BlockID;
            Packets.AddOneMessageToList(SB);
        }
        public static void SetSeed(int x, int y, int BlockID)
        {
            BSONObject SB = new BSONObject();
            SB["ID"] = "SS";
            SB["x"] = x;
            SB["y"] = y;
            SB["BlockType"] = BlockID;
            Packets.AddOneMessageToList(SB);
        }
        public static void LeaveWorld()
        {
            BSONObject obji = new BSONObject();
            obji["ID"] = "LW";
            AddOneMessageToList(obji);
        }
        public static void AddIfDoesNotContain(BSONObject toAdd)
        {
            object obj = messagesLock;
            lock (obj)
            {
                bool flag = false;
                for (int i = 0; i < messagesToSend.Count; i++)
                {
                    if (messagesToSend[i]["ID"] == toAdd["ID"])
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    messagesToSend.Add(toAdd);
                }
            }
        }
        public static void AddOrReplace(BSONObject toAdd)
        {
            object obj = messagesLock;
            lock (obj)
            {
                bool flag = false;
                for (int i = 0; i < messagesToSend.Count; i++)
                {
                    if (messagesToSend[i]["ID"] == toAdd["ID"])
                    {
                        messagesToSend[i] = toAdd;
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    messagesToSend.Add(toAdd);
                }

            }
        }
        ///<summary>
        /// state : true removes only MP, false removes mp, null removes both 
        ///</summary>
        public static void RemovePlayerPositionMessageIfExists(PlayerPositionType state = PlayerPositionType.Both)
        {
            object obj = messagesLock;
            lock (obj)
            {
                List<int> num = new List<int>();
                for (int i = 0; i < messagesToSend.Count; i++)
                {
                    switch (state)
                    {
                        case PlayerPositionType.MP:
                            {
                                if (messagesToSend[i]["ID"].stringValue == "MP")
                                {
                                    num.Add(i);
                                }
                                break;
                            }

                        case PlayerPositionType.mp:
                            {
                                if (messagesToSend[i]["ID"].stringValue == "mp")
                                {
                                    num.Add(i);
                                }
                                break;
                            }
                        case PlayerPositionType.Both:
                            {
                                if (messagesToSend[i]["ID"].stringValue == "MP")
                                {
                                    num.Add(i);
                                }
                                if (messagesToSend[i]["ID"].stringValue == "mp")
                                {
                                    num.Add(i);
                                }

                                break;
                            }
                    }
                }
                foreach (int m in num)
                {
                    messagesToSend.RemoveAt(m);
                }
            }
        }
        public static void RemoveMessageIfExists(string ID)
        {
            object obj = messagesLock;
            lock (obj)
            {
                List<int> num = new List<int>();
                for (int i = 0; i < messagesToSend.Count; i++)
                {
                    if (messagesToSend[i]["ID"].stringValue == ID)
                    {
                        num.Add(i);
                    }
                }
                foreach (int m in num)
                {
                    messagesToSend.RemoveAt(m);
                }
            }
        }
        public static void AddOneMessageToList(BSONObject toAdd)
        {
            object obj = messagesLock;
            lock (obj)
            {
                messagesToSend.Add(toAdd);
            }
        }
        public static bool AreThereAnyMessages()
        {
            object obj = messagesLock;
            bool result;
            lock (obj)
            {
                result = (messagesToSend.Count > 0);
            }
            return result;
        }
        public static bool SendMessages()
        {

            object obj = sendLock;
            lock (obj)
            {
                if (!AreThereAnyMessages())
                {
                    return false;
                }
                Console.WriteLine("Client ======================================================================================== Client");
                for (int i = 0; i < messagesToSend.Count; i++)
                {
                    ReadBSON(messagesToSend[i]);
                }
                byte[] array = TurnMessagesToBytesAndConsumeThem();
                Console.WriteLine("Packet Length : " + array.Length);
                MemoryStream memoryStream = new MemoryStream();
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write(array.Length + 4);
                    binaryWriter.Write(array);
                }
                byte[] array2 = memoryStream.ToArray();
                stream.Write(array2, 0, array2.Length);
                Console.WriteLine("0x" + BitConverter.ToString(array).Replace("-", ", 0x"));
            }
            return true;
        }
        public static void ClearMessages()
        {
            object obj = messagesLock;
            lock (obj)
            {
                messagesToSend.Clear();
            }
        }
        public static byte[] TurnMessagesToBytesAndConsumeThem()
        {
            BSONObject bsonobject = new BSONObject();
            object obj = messagesLock;
            lock (obj)
            {
                for (int i = 0; i < messagesToSend.Count; i++)
                {
                    bsonobject["m" + i] = messagesToSend[i];
                }
                bsonobject["mc"] = messagesToSend.Count;
                messagesToSend.Clear();
            }
            return SimpleBSON.Dump(bsonobject);
        }
        #region ReadBSON
        public static void ReadBSON(BSONObject SinglePacket, string Parent = "")
        {
            foreach (string Key in SinglePacket.Keys)
            {
                try
                {
                    BSONValue Packet = SinglePacket[Key];
                    switch (Packet.valueType)
                    {
                        case BSONValue.ValueType.String:
                            Console.WriteLine($"{Parent} = {Key} | {Packet.valueType} = {Packet.stringValue}");
                            //file.Write("\n" + $"{Parent} = {Key} | {Packet.valueType} = {Packet.stringValue}");
                            break;
                        case BSONValue.ValueType.Boolean:
                            Console.WriteLine($"{Parent} = {Key} | {Packet.valueType} = {Packet.boolValue}");
                            //file.Write("\n" + $"{Parent} = {Key} | {Packet.valueType} = {Packet.boolValue}");
                            break;
                        case BSONValue.ValueType.Int32:
                            Console.WriteLine($"{Parent} = {Key} | {Packet.valueType} = {Packet.int32Value}");
                            //file.Write("\n" + $"{Parent} = {Key} | {Packet.valueType} = {Packet.int32Value}");
                            break;
                        case BSONValue.ValueType.Int64:
                            Console.WriteLine($"{Parent} = {Key} | {Packet.valueType} = {Packet.int64Value}");
                            //file.Write("\n" + $"{Parent} = {Key} | {Packet.valueType} = {Packet.int64Value}");
                            break;
                        case BSONValue.ValueType.Binary: // BSONObject
                            try
                            {
                                Console.WriteLine($"{Parent} = {Key} | {Packet.valueType} = {Packet.binaryValue}");
                                //file.Write("\n" + $"{Parent} = {Key} | {Packet.valueType} = {Packet.binaryValue}");
                                ReadBSON(SimpleBSON.Load(Packet.binaryValue), Key);
                            }
                            catch
                            {
                                Console.WriteLine($"{Parent} = {Key} | {Packet.valueType} = [{BitConverter.ToString(Packet.binaryValue)}]");
                                //file.Write("\n" + $"{Parent} = {Key} | {Packet.valueType} = [{BitConverter.ToString(Packet.binaryValue)}]");
                            }
                            break;
                        case BSONValue.ValueType.Double:
                            Console.WriteLine($"{Parent} = {Key} | {Packet.valueType} = {Packet.doubleValue}");
                            //file.Write("\n" + $"{Parent} = {Key} | {Packet.valueType} = {Packet.doubleValue}");
                            break;
                        case BSONValue.ValueType.Array:
                            string bamboom = $"{Parent} = {Key} | {Packet.valueType} = [";
                            foreach (var strong in Packet.stringListValue)
                            {
                                bamboom += strong + ", ";
                            }
                            bamboom = bamboom.Remove(bamboom.Length - 2, 2);
                            bamboom += "]";
                            Console.WriteLine(bamboom);
                            //file.Write("\n" + bamboom);
                            break;
                        case BSONValue.ValueType.UTCDateTime:
                            Console.WriteLine($"{Parent} = {Key} | {Packet.valueType} = {Packet.dateTimeValue}");
                            //file.Write("\n" + $"{Parent} = {Key} | {Packet.valueType} = {Packet.dateTimeValue}");
                            break;
                        default:
                            Console.WriteLine($"{Parent} = {Key} | {Packet.valueType}");
                            //file.Write("\n" + $"{Parent} = {Key} | {Packet.valueType}");
                            ReadBSON((BSONObject)Packet, Key);
                            //Console.WriteLine(BitConverter.ToString(ObjectToByteArray(((Object)Packet))));

                            break;
                    }
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee);
                }
            }
        }
        #endregion
    }
    
    public enum Direction
    {
        Center,
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft,
        LeftMinor,
        UpUpLeft,
        UpUpRight
    }
    public enum AnimationNames
    {
        None,
        Idle,
        Move,
        Jump,
        StartFall,
        Fall,
        Hit,
        HitMove,
        Swim,
        FaceContent,
        FaceHappy,
        FaceSurprised,
        FaceDevastated,
        FaceTired,
        FaceHoldBreath,
        FaceFurious,
        FaceAngry,
        FaceAngryMutru,
        FaceDying,
        FaceSad,
        FaceLaugh,
        FaceWink,
        FaceGrin,
        FaceSuspicious,
        FaceGoofySmile,
        FaceJump1,
        FaceJump2,
        FaceJoyfull,
        FaceSleep1,
        FaceSleep2,
        FaceSerious,
        FaceVicious,
        FaceContentTalk,
        FaceHappyTalk,
        FaceSurprisedTalk,
        FaceTiredTalk,
        FaceHoldBreathTalk,
        FaceAngryTalk,
        FaceAngryMutruTalk,
        FaceSadTalk,
        FaceSuspiciousTalk,
        FaceSeriousTalk,
        FaceViciousTalk,
        HoldGun,
        Shoot,
        ShootMove,
        Dying,
        RockRock,
        Surprised,
        Dunno,
        Furious,
        PointForward,
        ThumbsUp,
        JollyArms,
        Depressed,
        DepressedLoop,
        Honk,
        Applause,
        Wave,
        Waveone,
        Wink,
        Tired,
        Sleep,
        JumpWings,
        Sit,
        SitDown,
        GetUp,
        TakeHit,
        ThrowBlock,
        Invisible,
        SleepInBed,
        SitInBath,
        FallParachute,
        Dabb,
        FaceDabbNFPalm,
        WoopWoop,
        ThisWay,
        Love,
        FUUU,
        Cheer,
        FPalm,
        Dance,
        LOL,
        Cuteness,
        Troll,
        FaceWoopWoop,
        FaceLove1,
        FaceLove2,
        FaceFUUU,
        FaceDance1,
        FaceDance2,
        FaceCuteness,
        FaceTroll1,
        FaceTroll2,
        MeGusta,
        FaceMeGusta,
        FaceHappyC,
        FaceHappyE,
        FaceHappyF,
        FaceHappyK,
        FaceHappyL,
        FaceHappyM,
        FaceMutruA,
        FaceMutruB,
        FaceGrinA,
        FaceGrinC,
        FaceGrinE,
        FaceGrinF,
        FaceGrinK,
        FaceGrinL,
        FaceGrinM,
        FaceLaughC,
        FaceLaughE,
        FaceLaughF,
        FaceLaughK,
        FaceLaughL,
        FaceSadL,
        FaceSuspiciousA,
        FaceSuspiciousF,
        FaceSuspiciousM,
        FaceSeriousC,
        FaceHappyCTalk,
        FaceHappyETalk,
        FaceHappyFTalk,
        FaceHappyKTalk,
        FaceHappyLTalk,
        FaceHappyMTalk,
        FaceMutruATalk,
        FaceMutruBTalk,
        FaceGrinATalk,
        FaceGrinCTalk,
        FaceGrinETalk,
        FaceGrinFTalk,
        FaceGrinKTalk,
        FaceGrinLTalk,
        FaceGrinMTalk,
        FaceLaughCTalk,
        FaceLaughETalk,
        FaceLaughFTalk,
        FaceLaughKTalk,
        FaceLaughLTalk,
        FaceSadLTalk,
        FaceSuspiciousATalk,
        FaceSuspiciousFTalk,
        FaceSuspiciousMTalk,
        FaceSeriousCTalk,
        UseRocket,
        FaceFPalm,
        END_OF_THE_ENUM
    }
    public enum PlayerPositionType
    {
        Both,
        MP,
        mp
    }
    public class StateObject
    {
        public Socket workSocket;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[1024];
        public byte[] data;
        public int bytesReadToData;
    }


}
