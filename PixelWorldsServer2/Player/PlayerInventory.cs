using PixelWorldsServer2.DataManagement;
using PixelWorldsServer2.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PixelWorldsServer2
{
    public enum ItemFlags
    {
        IS_SEED = 1 << 9,
        IS_WEARABLE = 1 << 10
    }

    public struct InventoryKey : IEquatable<InventoryKey>
    {

        public InventoryKey(WorldInterface.BlockType bt, InventoryItemType itt)
        {
            this.blockType = bt;
            this.itemType = itt;
        }


        public static int InventoryKeyToInt(InventoryKey ik)
        {
            return InventoryKey.BlockTypeAndInventoryItemTypeToInt(ik.blockType, ik.itemType);
        }


        public static int BlockTypeAndInventoryItemTypeToInt(WorldInterface.BlockType blockType, InventoryItemType inventoryItemType)
        {
            return (int)((WorldInterface.BlockType)((int)inventoryItemType << 24) | blockType);
        }


        public static InventoryKey IntToInventoryKey(int asInt)
        {
            return new InventoryKey((WorldInterface.BlockType)(asInt & 16777215), (InventoryItemType)(asInt >> 24));
        }

        public bool Equals(InventoryKey other)
        {
            return this.blockType == other.blockType && this.itemType == other.itemType;
        }

        public override bool Equals(object other)
        {
            return other is InventoryKey && this.Equals((InventoryKey)other);
        }

        public override int GetHashCode()
        {
            return (int)((WorldInterface.BlockType)((int)this.itemType << 24) | this.blockType);
        }

        public static bool operator ==(InventoryKey lhs, object rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(InventoryKey lhs, object rhs)
        {
            return !(lhs == rhs);
        }

        public static InventoryKey GetNoneBlockKey()
        {
            return new InventoryKey(WorldInterface.BlockType.None, InventoryItemType.Block);
        }

        public static List<int> GetInventoryKeysAsIntList(List<InventoryKey> iks)
        {
            List<int> list = new List<int>(iks.Count);
            for (int i = 0; i < iks.Count; i++)
            {
                list.Add(InventoryKey.InventoryKeyToInt(iks[i]));
            }
            return list;
        }

        public static List<InventoryKey> IntListToInventoryKeyList(List<int> intList)
        {
            List<InventoryKey> list = new List<InventoryKey>(intList.Count);
            for (int i = 0; i < intList.Count; i++)
            {
                list.Add(InventoryKey.IntToInventoryKey(intList[i]));
            }
            return list;
        }

        public override string ToString()
        {
            return this.blockType.ToString() + " " + this.itemType.ToString();
        }

        public WorldInterface.BlockType blockType;

        public InventoryItemType itemType;

        private const int magicValue = 24;
    }


    public class PlayerInventoryManager
    {

        private List<InventoryKey> itemList = new List<InventoryKey>();
        public List<InventoryKey> Items => itemList;

        public Animation.HotSpots[] AnimHotSpots;

        public Player p;
        public PlayerInventoryManager(Player pl)
        {
            p = pl;
        }

        public void vipEsyaVer()
        {
            // Mağzadaki alınamayan vip itemleri adminlere özel verme komutu (beta)
            this.AddItemToInventory((WorldInterface.BlockType)934, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)935, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)1293, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)881, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)3086, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)3085, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)3087, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)3088, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)3824, InventoryItemType.WearableItem, 1);

        }

        public void wingsPack()
        {
            // wings pack
            this.AddItemToInventory((WorldInterface.BlockType)608, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)1350, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)2292, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)1298, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4268, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)3481, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4768, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4197, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)2608, InventoryItemType.WearableItem, 1);
        }

        public void modPack()
        {

            // Mod pack
            this.AddItemToInventory((WorldInterface.BlockType)2096, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)1038, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)794, InventoryItemType.WearableItem, 1);
        }

        public Dictionary<int, short> handPack()
        {
            Dictionary<int, short> inv = new Dictionary<int, short>();
            this.AddItemToInventory((WorldInterface.BlockType)4762, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)3482, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)3483, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4281, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)1306, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)1305, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)2293, InventoryItemType.WearableItem, 1);
            return inv;
        }






        public void RegularDefaultInventory()
        {
            // bunch of cool items
            this.AddItemToInventory((WorldInterface.BlockType)750, InventoryItemType.Consumable, 500);
            this.AddItemToInventory((WorldInterface.BlockType)4265, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4267, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4269, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)2152, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)3176, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)592, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)2275, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)2358, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4879, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4880, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4882, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4890, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4891, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4893, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4894, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4895, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4896, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4897, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4898, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4899, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4900, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4901, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4902, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4903, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4904, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4905, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4906, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)4907, InventoryItemType.WearableItem, 1);
            this.AddItemToInventory((WorldInterface.BlockType)3482, InventoryItemType.WearableItem, 1);
        }


        public void AddItemToInventory(InventoryKey ik, short addAmount = 1)
        {
            this.AddItemToInventory(ik.blockType, ik.itemType, addAmount);
        }


        // Token: 0x06000DD5 RID: 3541 RVA: 0x00046AF4 File Offset: 0x00044EF4
        public void AddItemToInventory(WorldInterface.BlockType blockType, InventoryItemType inventoryItemType, short addAmount = 1)
        {
            if (addAmount < 1)
            {
                return;
            }
            int num = InventoryKey.BlockTypeAndInventoryItemTypeToInt(blockType, inventoryItemType);
            if (p.Data.Inventory.ContainsKey(num))
            {
                p.Data.Inventory[num] = (short)(p.Data.Inventory[num] + addAmount);
            }
            else
            {
                p.Data.Inventory[num] = addAmount;
            }
        }

        // Token: 0x06000DDB RID: 3547 RVA: 0x00046D2E File Offset: 0x0004512E
        public void RemoveItemsFromInventory(InventoryKey inventoryKey, short amount = 1)
        {
            this.RemoveItemsFromInventory(inventoryKey.blockType, inventoryKey.itemType, amount);
        }

        public void RemoveItemsFromInventory(WorldInterface.BlockType blockType, InventoryItemType inventoryItemType, short amount = 1)
        {
            if (amount < 1)
            {
                return;
            }
            int key = InventoryKey.BlockTypeAndInventoryItemTypeToInt(blockType, inventoryItemType);
            if (p.Data.Inventory.ContainsKey(key))
            {
                short num = (short)(p.Data.Inventory[key] - amount);
                if (num > 0)
                {
                    p.Data.Inventory[key] = num;
                }
                else
                {
                    p.Data.Inventory.Remove(key);
                }
            }
        }

        // Token: 0x06000DDF RID: 3551 RVA: 0x00046DE0 File Offset: 0x000451E0
        public bool HasItemAmountInInventory(InventoryKey inventoryKey, short amount = 1)
        {
            WorldInterface.BlockType blockType = inventoryKey.blockType;
            InventoryItemType itemType = inventoryKey.itemType;
            int key = InventoryKey.BlockTypeAndInventoryItemTypeToInt(blockType, itemType);
            if (p.Data.Inventory.ContainsKey(key))
            {
                int num = (int)p.Data.Inventory[key];
                if (num >= (int)amount)
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x06000DE0 RID: 3552 RVA: 0x00046E30 File Offset: 0x00045230
        public bool HasItemAmountInInventory(WorldInterface.BlockType blockType, InventoryItemType inventoryItemType, short amount=1)
        {
            int key = InventoryKey.BlockTypeAndInventoryItemTypeToInt(blockType, inventoryItemType);
            if (p.Data.Inventory.ContainsKey(key))
            {
                int num = (int)p.Data.Inventory[key];
                if (num >= (int)amount)
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x06000DE1 RID: 3553 RVA: 0x00046E70 File Offset: 0x00045270
        public short GetCount(InventoryKey inventoryKey)
        {
            int key = InventoryKey.InventoryKeyToInt(inventoryKey);
            if (p.Data.Inventory.ContainsKey(key))
            {
                return p.Data.Inventory[key];
            }
            return 0;
        }

        // Token: 0x06000DE2 RID: 3554 RVA: 0x00046EA4 File Offset: 0x000452A4
        public Dictionary<InventoryKey, short> GetCounts()
        {
            Dictionary<InventoryKey, short> dictionary = new Dictionary<InventoryKey, short>();
            foreach (KeyValuePair<int, short> keyValuePair in p.Data.Inventory)
            {
                dictionary[InventoryKey.IntToInventoryKey(keyValuePair.Key)] = keyValuePair.Value;
            }
            return dictionary;
        }


        // Token: 0x06000DE4 RID: 3556 RVA: 0x00046FA0 File Offset: 0x000453A0
        public bool IsItemAvailable(InventoryKey inventoryKey)
        {
            return this.IsItemAvailable(inventoryKey.blockType, inventoryKey.itemType);
        }

        // Token: 0x06000DE5 RID: 3557 RVA: 0x00046FB6 File Offset: 0x000453B6
        public bool IsItemAvailable(WorldInterface.BlockType blockType, InventoryItemType inventoryItemType)
        {
            return p.Data.Inventory.ContainsKey(InventoryKey.BlockTypeAndInventoryItemTypeToInt(blockType, inventoryItemType));
        }
        // Token: 0x06000DE7 RID: 3559 RVA: 0x0004700D File Offset: 0x0004540D
        public bool CanTransfer(InventoryKey inventoryKey, short amount)
        {
            return 0 < amount && amount <= this.GetCount(inventoryKey);
        }
        public byte[] GetInventoryAsBinary()
        {
            int num = 6;
            if (p.Data.Inventory == null || p.Data.Inventory.Count < 1)
            {
                return new byte[]
                {
                1
                };
            }
            byte[] array = new byte[num * p.Data.Inventory.Count];
            int num2 = 0;
            foreach (KeyValuePair<int, short> keyValuePair in p.Data.Inventory)
            {
                byte[] bytes = BitConverter.GetBytes(keyValuePair.Key);
                byte[] bytes2 = BitConverter.GetBytes(keyValuePair.Value);
                Buffer.BlockCopy(bytes, 0, array, num2, 4);
                num2 += 4;
                Buffer.BlockCopy(bytes2, 0, array, num2, 2);
                num2 += 2;
            }
            return array;
        }

        public Dictionary<int, short> InitInventoryFromBinary(byte[] binary = null)
        {
            int num = 6;
            p.Data.Inventory = new Dictionary<int, short>();
            if (binary == null || binary.Length < num)
            {

                return null;
            }
            for (int i = 0; i < binary.Length; i += num)
            {
                p.Data.Inventory[BitConverter.ToInt32(binary, i)] = BitConverter.ToInt16(binary, i + 4);
            }
            return p.Data.Inventory;
        }
        public void ClearInventory()
        {
            p.Data.Inventory.Clear();
        }
    }
}
