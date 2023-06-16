using System;
using Kernys.Bson;

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

    public PortalData() : base(0, World.BlockType.Portal)
    {
    }


    public PortalData(int itemId) : base(itemId, World.BlockType.Portal)
    {
    }

    public override BSONObject GetAsBSON()
    {
        BSONObject bsonobject = new BSONObject();
        base.WriteBaseDataToBSON(bsonobject, base.GetType().Name);
        bsonobject[PortalData.targetWorldIDKey] = this.targetWorldID;
        bsonobject[PortalData.targetEntryPointIDKey] = this.targetEntryPointID;
        bsonobject[PortalData.nameKey] = this.name;
        bsonobject[PortalData.entryPointIDKey] = this.entryPointID;
        bsonobject[PortalData.isLockedKey] = this.isLocked;
        bsonobject[PortalData.passwordKey] = this.password;
        bsonobject[PortalData.posXKey] = x;
        bsonobject[PortalData.posYKey] = y;
        return bsonobject;
    }

    public override void SetViaBSON(BSONObject bson)
    {
        base.ReadBaseDataFromBSON(bson);
        if (bson.ContainsKey(PortalData.targetWorldIDKey))
        {
            this.targetWorldID = bson[PortalData.targetWorldIDKey];
        }
        if (bson.ContainsKey(PortalData.targetEntryPointIDKey))
        {
            this.targetEntryPointID = bson[PortalData.targetEntryPointIDKey];
        }
        if (bson.ContainsKey(PortalData.nameKey))
        {
            this.name = bson[PortalData.nameKey];
        }
        if (bson.ContainsKey(PortalData.entryPointIDKey))
        {
            this.entryPointID = bson[PortalData.entryPointIDKey];
        }
        if (bson.ContainsKey(PortalData.isLockedKey))
        {
            this.isLocked = bson[PortalData.isLockedKey];
        }
        if (bson.ContainsKey(PortalData.passwordKey))
        {
            this.password = bson[PortalData.passwordKey];
        }
    }

    public override bool DoesValidate(BSONObject bson)
    {
        return this.ValidateKeys(bson);
    }

    private bool ValidateKeys(BSONObject bson)
    {
        return bson.ContainsKey(PortalData.targetWorldIDKey) && bson.ContainsKey(PortalData.targetEntryPointIDKey) && bson.ContainsKey(PortalData.nameKey) && bson.ContainsKey(PortalData.entryPointIDKey) && bson.ContainsKey(PortalData.isLockedKey) && bson.ContainsKey(PortalData.passwordKey);
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
