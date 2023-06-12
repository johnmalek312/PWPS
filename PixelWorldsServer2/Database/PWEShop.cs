using Kernys.Bson;
using PixelWorldsServer2.DataManagement;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PixelWorldsServer2.Player;

namespace PixelWorldsServer2.Database
{
    public class PWEShopResult
    {
        public PWEShopResult(SQLiteDataReader reader)
        {

            this.ID = Convert.ToInt32(reader["ID"]);
            this.price = Convert.ToInt32(reader["price"]);
            this.itemID = Convert.ToInt32(reader["itemID"]);
            this.inventoryItemType = (InventoryItemType)(Convert.ToInt32(reader["inventoryItemType"]));
            this.sellerID = (string)reader["sellerID"];
            this.buyerID = (string)reader["sellerID"];
            this.creationTime = (long)reader["creationTime"];
            this.expirationTime = (long)reader["expirationTime"];
            this.amount = Convert.ToInt32(reader["amount"]);
            this.sold = Convert.ToBoolean(reader["sold"]);
        }
        public PWEShopResult()
        {

        }
        public int ID;
        public int price;
        public int itemID;
        public InventoryItemType inventoryItemType;
        public string sellerID;
        public string buyerID;
        public long creationTime;
        public long expirationTime;
        public int amount;
        public bool sold;

    }
    public class PWEShop
    {
        
    }

}
