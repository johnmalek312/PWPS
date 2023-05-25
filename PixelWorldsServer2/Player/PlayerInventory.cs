using PixelWorldsServer2.DataManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PixelWorldsServer2
{
    public enum InventoryItemType : byte
    {
        // Token: 0x04000BED RID: 3053
        Block,
        // Token: 0x04000BEE RID: 3054
        BlockBackground,
        // Token: 0x04000BEF RID: 3055
        Seed,
        // Token: 0x04000BF0 RID: 3056
        BlockWater,
        // Token: 0x04000BF1 RID: 3057
        WearableItem,
        // Token: 0x04000BF2 RID: 3058
        Weapon,
        // Token: 0x04000BF3 RID: 3059
        Throwable,
        // Token: 0x04000BF4 RID: 3060
        Consumable,
        // Token: 0x04000BF5 RID: 3061
        Shard,
        // Token: 0x04000BF6 RID: 3062
        Blueprint
    }
    public enum ItemFlags
    {
        IS_SEED = 1 << 9,
        IS_WEARABLE = 1 << 10
    }
    public class InventoryItem
    {
        public short itemID;
        public short flags;
        public short amount;

        public InventoryItem(short itemID = 0, short flags = 0, short amount = 1)
        {
            this.itemID = itemID;
            this.flags = flags;
            this.amount = amount;
        }
    }
    public class PlayerInventory
    {

        private List<InventoryItem> itemList = new List<InventoryItem>();
        public List<InventoryItem> Items => itemList;

        public Animation.HotSpots[] AnimHotSpots;

        public InventoryItem Get(int id, short flags = 0)
        {
            foreach (InventoryItem i in itemList)
            {
                if (i.itemID == id && i.flags == flags)
                    return i;
            }

            return null;
        }

        public PlayerInventory(byte[] data = null)
        {
            if (data == null)
                return;

            AnimHotSpots = new Animation.HotSpots[(int)Animation.HotSpots.END_OF_THE_ENUM + 1];

            Load(data);
        }

        // 0: success, -1 any error, higher than 0: left to be handled.
        public int Add(InventoryItem invItem)
        {
            var item = Get(invItem.itemID, invItem.flags);

            if (item == null)
            {
                Items.Add(invItem);
                return 0;
            }

            item.amount += invItem.amount;

            if (item.amount > 999)
            {
                int h = item.amount - 999;
                item.amount = 999;
                return h;
            }

            return 0;
        }

        public int Remove(InventoryItem invItem)
        {
            var item = Get(invItem.itemID, invItem.flags);

            if (item == null)
                return -1;

            if (item.amount <= 1)
            {
                Items.Remove(item);
                return 0;
            }

            item.amount -= invItem.amount;
            return invItem.amount;
        }

        public byte[] Serialize()
        {
            using (var stream = new MemoryStream())
            {
                using (var bw = new BinaryWriter(stream))
                {
                    foreach (var item in Items)
                    {
                        bw.Write(item.itemID);
                        bw.Write(item.flags);
                        bw.Write(item.amount);
                    }
                }

                return stream.ToArray();
            }
        }

        public void Load(byte[] data)
        {
            if (data.Length % 6 != 0)
            {
                Util.Log("Inventory data doesn't have correct length?! May be corrupted!!");
                return;
            }

            int items = data.Length / 6;
            using (var stream = new MemoryStream(data))
            {
                using (var bw = new BinaryReader(stream))
                {
                    for (int i = 0; i < items; i++)
                    {
                        short id = bw.ReadInt16();
                        short flags = bw.ReadInt16();
                        short amount = bw.ReadInt16();

                        Items.Add(new InventoryItem(id, flags, amount));
                    }
                }
            }
        }
        public void vipEsyaVer()
        {

            // Mağzadaki alınamayan vip itemleri adminlere özel verme komutu (beta)
            Items.Add(new InventoryItem(934, (short)ItemFlags.IS_WEARABLE, 201));
            Items.Add(new InventoryItem(935, (short)ItemFlags.IS_WEARABLE, 201));
            Items.Add(new InventoryItem(1293, (short)ItemFlags.IS_WEARABLE, 201));
            Items.Add(new InventoryItem(881, (short)ItemFlags.IS_WEARABLE, 201));
            Items.Add(new InventoryItem(3086, (short)ItemFlags.IS_WEARABLE, 201));
            Items.Add(new InventoryItem(3085, (short)ItemFlags.IS_WEARABLE, 201));
            Items.Add(new InventoryItem(3087, (short)ItemFlags.IS_WEARABLE, 201));
            Items.Add(new InventoryItem(3088, (short)ItemFlags.IS_WEARABLE, 201));
            Items.Add(new InventoryItem(3824, (short)ItemFlags.IS_WEARABLE, 25));

        }

        public void wingsPack()
        {

            // wings pack
            Items.Add(new InventoryItem(608, (short)ItemFlags.IS_WEARABLE, 25));
            Items.Add(new InventoryItem(1350, (short)ItemFlags.IS_WEARABLE, 25));
            Items.Add(new InventoryItem(2292, (short)ItemFlags.IS_WEARABLE, 25));
            Items.Add(new InventoryItem(1298, (short)ItemFlags.IS_WEARABLE, 25));
            Items.Add(new InventoryItem(4268, (short)ItemFlags.IS_WEARABLE, 25));
            Items.Add(new InventoryItem(3481, (short)ItemFlags.IS_WEARABLE, 25));
            Items.Add(new InventoryItem(4768, (short)ItemFlags.IS_WEARABLE, 25));
            Items.Add(new InventoryItem(4197, (short)ItemFlags.IS_WEARABLE, 25));
            Items.Add(new InventoryItem(2608, (short)ItemFlags.IS_WEARABLE, 25));
        }

        public void modPack()
        {

            // Mod pack
            Items.Add(new InventoryItem(2096, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(1038, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(794, (short)ItemFlags.IS_WEARABLE, 1));
        }

        public void handPack()
        {

            // Hand
            Items.Add(new InventoryItem(4762, (short)ItemFlags.IS_WEARABLE, 25));
            Items.Add(new InventoryItem(3482, (short)ItemFlags.IS_WEARABLE, 25));
            Items.Add(new InventoryItem(4281, (short)ItemFlags.IS_WEARABLE, 25));
            Items.Add(new InventoryItem(1306, (short)ItemFlags.IS_WEARABLE, 25));
            Items.Add(new InventoryItem(1305, (short)ItemFlags.IS_WEARABLE, 25));
            Items.Add(new InventoryItem(2293, (short)ItemFlags.IS_WEARABLE, 25));
        }






        public void InitFirstSetup()
        {
            // bunch of cool items
            Items.Add(new InventoryItem(750, 0, 999));
            Items.Add(new InventoryItem(4265, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4267, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4269, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(2152, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(3176, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(592, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(2275, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(2358, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4879, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4880, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4882, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4890, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4891, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4893, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4894, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4895, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4896, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4897, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4898, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4899, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4900, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4901, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4902, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4903, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4904, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4905, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4906, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(4907, (short)ItemFlags.IS_WEARABLE, 1));
            Items.Add(new InventoryItem(3482, (short)ItemFlags.IS_WEARABLE, 1));
        }
    }
}
