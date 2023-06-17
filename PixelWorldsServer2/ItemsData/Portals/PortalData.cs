using System;
using Kernys.Bson;
using PixelWorldsServer2.World;
namespace PixelWorldsServer2.ItemsData.Portal
{
    public class PortalData : WorldItemBase, IPortal
    {

        private static readonly string targetWorldIDKey = "targetWorldID";

        private static readonly string targetEntryPointIDKey = "targetEntryPointID";

        private static readonly string nameKey = "name";

        private static readonly string entryPointIDKey = "entryPointID";

        private static readonly string isLockedKey = "isLocked";

        private static readonly string passwordKey = "password";

        public string targetWorldID = string.Empty;

        public string targetEntryPointID = string.Empty;

        public string name = string.Empty;

        public string entryPointID = string.Empty;

        public bool isLocked;

        public string password = string.Empty;

        private static readonly string posXKey = "posX";
        private static readonly string posYKey = "posY";
        public int x;
        public int y;
        public PortalData() : base(0, WorldInterface.BlockType.Portal)
        {
        }


        public PortalData(int itemId) : base(itemId, WorldInterface.BlockType.Portal)
        {
        }

        public override BSONObject GetAsBSON()
        {
            BSONObject bsonobject = new BSONObject();
            base.WriteBaseDataToBSON(bsonobject, base.GetType().Name);
            bsonobject[targetWorldIDKey] = this.targetWorldID;
            bsonobject[targetEntryPointIDKey] = this.targetEntryPointID;
            bsonobject[nameKey] = this.name;
            bsonobject[entryPointIDKey] = this.entryPointID;
            bsonobject[isLockedKey] = this.isLocked;
            bsonobject[passwordKey] = this.password;
            bsonobject[posXKey] = x;
            bsonobject[posYKey] = y;
            return bsonobject;
        }

        public override void SetViaBSON(BSONObject bson)
        {
            base.ReadBaseDataFromBSON(bson);
            if (bson.ContainsKey(targetWorldIDKey))
            {
                this.targetWorldID = bson[targetWorldIDKey];
            }
            if (bson.ContainsKey(targetEntryPointIDKey))
            {
                this.targetEntryPointID = bson[targetEntryPointIDKey];
            }
            if (bson.ContainsKey(nameKey))
            {
                this.name = bson[nameKey];
            }
            if (bson.ContainsKey(entryPointIDKey))
            {
                this.entryPointID = bson[entryPointIDKey];
            }
            if (bson.ContainsKey(isLockedKey))
            {
                this.isLocked = bson[isLockedKey];
            }
            if (bson.ContainsKey(passwordKey))
            {
                this.password = bson[passwordKey];
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
            return this.ValidateKeys(bson);
        }

        private bool ValidateKeys(BSONObject bson)
        {
            return bson.ContainsKey(targetWorldIDKey) && bson.ContainsKey(targetEntryPointIDKey) && bson.ContainsKey(nameKey) && bson.ContainsKey(entryPointIDKey) && bson.ContainsKey(isLockedKey) && bson.ContainsKey(passwordKey);
        }

        public void SetTargetWorldID(string newValue)
        {
            this.targetWorldID = newValue;
        }

        public string GetTargetWorldID()
        {
            return this.targetWorldID;
        }

        public void SetTargetEntryPointID(string newValue)
        {
            this.targetEntryPointID = newValue;
        }

        public string GetTargetEntryPointID()
        {
            return this.targetEntryPointID;
        }

        public void SetName(string newValue)
        {
            this.name = newValue;
        }

        public string GetName()
        {
            return this.name;
        }

        public void SetEntryPointID(string newValue)
        {
            this.entryPointID = newValue;
        }

        public string GetEntryPointID()
        {
            return this.entryPointID;
        }

        public void SetIsLocked(bool newValue)
        {
            this.isLocked = newValue;
        }

        public bool GetIsLocked()
        {
            return this.isLocked;
        }
    }
}