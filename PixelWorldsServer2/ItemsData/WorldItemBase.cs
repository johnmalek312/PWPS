using Kernys.Bson;
using System;
using PixelWorldsServer2.World;

namespace PixelWorldsServer2
{
    public abstract class WorldItemBase
    {
        // Token: 0x0600100B RID: 4107 RVA: 0x0003CB58 File Offset: 0x0003AF58
        public WorldItemBase(int id, WorldInterface.BlockType bt)
        {
            this.itemId = id;
            this.blockType = bt;
            WorldInterface.BlockType blockType = this.blockType;
            switch (blockType)
            {
                case WorldInterface.BlockType.LockWorld:
                    if (!((LockWorldData)this).GetIsOpen())
                    {
                        this.isAnimationOn = true;
                        this.useAnotherSprite = true;
                    }
                    else
                    {
                        this.isAnimationOn = false;
                        this.useAnotherSprite = false;
                    }
                    break;
                default:
                    switch (blockType)
                    {
                        case WorldInterface.BlockType.Chicken:
                        case WorldInterface.BlockType.Cow:
                        case WorldInterface.BlockType.Sheep:
                            this.isAnimationOn = true;
                            break;
                    }
                    break;
                case WorldInterface.BlockType.CheckPoint:
                    this.isAnimationOn = false;
                    this.useAnotherSprite = true;
                    break;
            }
            this.blockDirection = Config.GetBlockDirection(this.blockType);
        }

        // Token: 0x0600100C RID: 4108 RVA: 0x0003D018 File Offset: 0x0003B418
        protected void WriteBaseDataToBSON(BSONObject bson, string myClassName)
        {
            bson[WorldItemBase.classKey] = myClassName;
            bson[WorldItemBase.itemKey] = this.itemId;
            bson[WorldItemBase.blockTypeKey] = (int)this.blockType;
            bson[WorldItemBase.animationOnKey] = this.isAnimationOn;
            bson[WorldItemBase.blockDirectionKey] = (int)this.blockDirection;
            bson[WorldItemBase.useAnotherSpriteKey] = this.useAnotherSprite;
            bson[WorldItemBase.doDamageNowKey] = this.doDamageNow;
        }

        // Token: 0x0600100D RID: 4109 RVA: 0x0003D0BC File Offset: 0x0003B4BC
        protected void ReadBaseDataFromBSON(BSONObject bson)
        {
            this.itemId = bson[WorldItemBase.itemKey];
            this.blockType = (WorldInterface.BlockType)bson[WorldItemBase.blockTypeKey].int32Value;
            if (bson.ContainsKey(WorldItemBase.animationOnKey))
            {
                this.isAnimationOn = bson[WorldItemBase.animationOnKey];
            }
            if (bson.ContainsKey(WorldItemBase.blockDirectionKey))
            {
                this.blockDirection = (BlockDirection)bson[WorldItemBase.blockDirectionKey].int32Value;
            }
            if (bson.ContainsKey(WorldItemBase.useAnotherSpriteKey))
            {
                this.useAnotherSprite = bson[WorldItemBase.useAnotherSpriteKey];
            }
            if (bson.ContainsKey(WorldItemBase.doDamageNowKey))
            {
                this.doDamageNow = bson[WorldItemBase.doDamageNowKey];
            }
        }

        // Token: 0x0600100E RID: 4110 RVA: 0x0003D18D File Offset: 0x0003B58D
        public virtual BSONObject GetAsBSON()
        {
            return null;
        }

        // Token: 0x0600100F RID: 4111 RVA: 0x0003D190 File Offset: 0x0003B590
        public virtual void SetViaBSON(BSONObject bson)
        {
        }

        // Token: 0x06001010 RID: 4112 RVA: 0x0003D192 File Offset: 0x0003B592
        public virtual bool DoesValidate(BSONObject bson)
        {
            return true;
        }

        // Token: 0x06001011 RID: 4113 RVA: 0x0003D198 File Offset: 0x0003B598
        public static WorldInterface.BlockType GetBlockTypeViaClassNameInBSON(BSONObject bson)
        {
            string text = bson[WorldItemBase.classKey];
            return (WorldInterface.BlockType)Enum.Parse(typeof(WorldInterface.BlockType), text.Remove(text.Length - 4), true);
        }

        // Token: 0x06001012 RID: 4114 RVA: 0x0003D1D9 File Offset: 0x0003B5D9
        public bool DoItemIdMatch(BSONObject bson)
        {
            return this.itemId == bson[WorldItemBase.itemKey].int32Value;
        }

        // Token: 0x06001013 RID: 4115 RVA: 0x0003D1F3 File Offset: 0x0003B5F3
        public bool CanBeEdited()
        {
            return this.itemId != 0;
        }

        // Token: 0x06001014 RID: 4116 RVA: 0x0003D201 File Offset: 0x0003B601
        public void SetAnimationOn()
        {
            this.isAnimationOn = true;
        }

        // Token: 0x06001015 RID: 4117 RVA: 0x0003D20A File Offset: 0x0003B60A
        public void SetAnimationOff()
        {
            this.isAnimationOn = false;
        }

        // Token: 0x04000CDF RID: 3295
        public static readonly string classKey = "class";

        // Token: 0x04000CE0 RID: 3296
        public static readonly string itemKey = "itemId";

        // Token: 0x04000CE1 RID: 3297
        public static readonly string blockTypeKey = "blockType";

        // Token: 0x04000CE2 RID: 3298
        public static readonly string animationOnKey = "animOn";

        // Token: 0x04000CE3 RID: 3299
        public static readonly string blockDirectionKey = "direction";

        // Token: 0x04000CE4 RID: 3300
        public static readonly string useAnotherSpriteKey = "anotherSprite";

        // Token: 0x04000CE5 RID: 3301
        public static readonly string doDamageNowKey = "damageNow";

        // Token: 0x04000CE6 RID: 3302
        public int itemId;

        // Token: 0x04000CE7 RID: 3303
        public WorldInterface.BlockType blockType;

        // Token: 0x04000CE8 RID: 3304
        public BlockDirection blockDirection;

        // Token: 0x04000CE9 RID: 3305
        public bool isAnimationOn;

        // Token: 0x04000CEA RID: 3306
        public bool useAnotherSprite;

        // Token: 0x04000CEB RID: 3307
        public bool doDamageNow;

    }

}
