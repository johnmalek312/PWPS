using PixelWorldsServer2.Database;
using PixelWorldsServer2.Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PixelWorldsServer2.Player;

namespace PixelWorldsServer2.DataManagement
{
    public class PWEHelper
    {
        PWServer pServer = null;
        public PWEHelper(PWServer pServer)
        {
            this.pServer = pServer;
        }

        public List<PWEShopResult> GetPWEItemsByInventoryKey(int itemID, InventoryItemType inventoryItemType, int page = -1)
        {
            List<PWEShopResult> results = new List<PWEShopResult>();
            var sql = pServer.GetSQL();
            string query = "SELECT * FROM pwe WHERE @currentDateTime < expirationTime AND itemID = @itemID AND inventoryItemType = inventoryItemType AND sold = 0 ORDER BY price / amount ASC LIMIT 20 OFFSET " + page * 20 + ";";
            if (page == -1)
                query = "SELECT * FROM pwe WHERE @currentDateTime < expirationTime AND itemID = @itemID AND inventoryItemType = inventoryItemType AND sold = 0 ORDER BY price / amount ASC;";
            var cmd = sql.Make(query);
            cmd.Parameters.AddWithValue("@currentDateTime", DateTime.UtcNow.Ticks);

            cmd.Parameters.AddWithValue("@itemID", itemID);
            cmd.Parameters.AddWithValue("@inventoryItemType", (int)inventoryItemType);

            using (var reader = sql.PreparedFetchQuery(cmd))
            {
                while (reader != null)
                {
                    if (reader.Read())
                    {
                        results.Add(new PWEShopResult(reader));
                    }
                    else
                    {
                        break;
                    }

                }
            }

            return results;
        }
        public PWEShopResult CreatePWEShopResult(int price, int itemID, InventoryItemType inventoryItemType, string sellerID, long creationTime, long expirationTime, int amount, bool sold = false)
        {
            var sql = pServer.GetSQL();
            var cmd = sql.Make("INSERT INTO pwe (price, itemID, inventoryItemType, sellerID, creationTime, expirationTime, amount, sold) VALUES (@price, @itemID, @inventoryItemType, @sellerID, @creationTime, @expirationTime, @amount, @sold)");

            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@itemID", itemID);
            cmd.Parameters.AddWithValue("@inventoryItemType", (int)inventoryItemType);
            cmd.Parameters.AddWithValue("@sellerID", sellerID);
            cmd.Parameters.AddWithValue("@creationTime", creationTime);
            cmd.Parameters.AddWithValue("@expirationTime", expirationTime);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@sold", sold);

            if (sql.PreparedQuery(cmd) > 0)
            {

                var newResult = new PWEShopResult
                {
                    price = price,
                    itemID = itemID,
                    inventoryItemType = inventoryItemType,
                    sellerID = sellerID,
                    creationTime = creationTime,
                    expirationTime = expirationTime,
                    amount = amount,
                    sold = sold
                };

                return newResult;
            }
            return null;
        }
        public PWEShopResult GetPWEShopResultByCreationTicks(long creationTime)
        {
            var sql = pServer.GetSQL();
            var cmd = sql.Make("SELECT * FROM pwe WHERE creationTime = @creationTime AND sold = 0");

            cmd.Parameters.AddWithValue("@creationTime", creationTime);
            using (var reader = sql.PreparedFetchQuery(cmd))
            {
                if (reader != null)
                {
                    if (reader.Read())
                    {

                        var result = new PWEShopResult(reader);
                        return result;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        public PWEShopResult MarkAsSoldPWEShopResult(long creationTime, string buyerID = "0")
        {
            var sql = pServer.GetSQL();
            var cmd = sql.Make("SELECT * FROM pwe WHERE creationTime = @creationTime AND sold = 0");

            cmd.Parameters.AddWithValue("@creationTime", creationTime);
            using (var reader = sql.PreparedFetchQuery(cmd))
            {
                if (reader.Read())
                {
                    var updateCmd = sql.Make("UPDATE pwe SET sold = 1, buyerID = @buyerID WHERE creationTime = @creationTime");
                    updateCmd.Parameters.AddWithValue("@buyerID", buyerID);
                    updateCmd.Parameters.AddWithValue("@creationTime", creationTime);
                    if (sql.PreparedQuery(updateCmd) > 0)
                    {

                        var result = new PWEShopResult(reader); // Create the PWEShopResult object from the reader
                        result.sold = true; // Update the sold property in the PWEShopResult object
                        result.buyerID = buyerID; // Update the buyerID property in the PWEShopResult object

                        return result;
                    }
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

    }
}
