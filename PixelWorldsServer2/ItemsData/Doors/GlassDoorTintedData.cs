using Kernys.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelWorldsServer2.World;


namespace PixelWorldsServer2.ItemsData.Door
{
    public class GlassDoorTintedData : WorldItemBase , IDoor
    {
        private static readonly string isLockedKey = "isLocked";
        private static readonly string posXKey = "posX";
        private static readonly string posYKey = "posY";
        public bool isLocked = true;
        public int x;
        public int y;
        public GlassDoorTintedData() : base(0, WorldInterface.BlockType.GlassDoorTinted)
        {
        }

        public GlassDoorTintedData(int itemId) : base(itemId, WorldInterface.BlockType.GlassDoorTinted)
        {
        }

        public override BSONObject GetAsBSON()
        {
            BSONObject bsonobject = new BSONObject();
            WriteBaseDataToBSON(bsonobject, GetType().Name);
            bsonobject[isLockedKey] = isLocked;
            bsonobject[posXKey] = x;
            bsonobject[posYKey] = y;
            return bsonobject;
        }

        public override void SetViaBSON(BSONObject bson)
        {
            ReadBaseDataFromBSON(bson);
            if (bson.ContainsKey(isLockedKey))
            {
                isLocked = bson[isLockedKey];
            }
            if (bson.ContainsKey(posXKey))
            {
                this.x = bson[posXKey].int32Value;
            }
            if (bson.ContainsKey(posYKey))
            {
                this.y = bson[posYKey].int32Value;
            }
        }


        public override bool DoesValidate(BSONObject bson)
        {
            return ValidateKeys(bson);
        }

        private bool ValidateKeys(BSONObject bson)
        {
            return bson.ContainsKey(isLockedKey);
        }

        public void SetIsLocked(bool newValue)
        {
            isLocked = newValue;
        }

        public bool GetIsLocked()
        {
            return isLocked;
        }
    }
}
