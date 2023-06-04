using Kernys.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelWorldsServer2;
using PixelWorldsServer2.World;

namespace PixelWorldsServer2
{
    public class LockWorldData : WorldItemBase
    {
        // Token: 0x06000C8C RID: 3212 RVA: 0x00043144 File Offset: 0x00041544
        public LockWorldData() : base(0, WorldInterface.BlockType.LockWorld)
        {
        }

        // Token: 0x06000C8D RID: 3213 RVA: 0x0004319C File Offset: 0x0004159C
        public LockWorldData(int itemId) : base(itemId, WorldInterface.BlockType.LockWorld)
        {
        }

        // Token: 0x06000C8E RID: 3214 RVA: 0x000431F4 File Offset: 0x000415F4
        public override BSONObject GetAsBSON()
        {
            BSONObject bsonobject = new BSONObject();
            base.WriteBaseDataToBSON(bsonobject, base.GetType().Name);
            bsonobject[LockWorldData.playerWhoOwnsLockIdKey] = this.playerWhoOwnsLockId;
            bsonobject[LockWorldData.playerWhoOwnsLockNameKey] = this.playerWhoOwnsLockName;
            bsonobject[LockWorldData.playersWhoHaveAccessToLockKey] = this.playersWhoHaveAccessToLock;
            bsonobject[LockWorldData.playersWhoHaveMinorAccessToLockKey] = this.playersWhoHaveMinorAccessToLock;
            bsonobject[LockWorldData.isOpenKey] = this.isOpen;
            bsonobject[LockWorldData.punchingAllowedKey] = this.punchingAllowed;
            bsonobject[LockWorldData.creationTimeKey] = this.creationTime;
            bsonobject[LockWorldData.lastActivatedTimeKey] = this.lastActivatedTime;
            bsonobject[LockWorldData.posXKey] = this.x;
            bsonobject[LockWorldData.posYKey] = this.y;
            return bsonobject;
        }

        // Token: 0x06000C8F RID: 3215 RVA: 0x000432B4 File Offset: 0x000416B4
        public override void SetViaBSON(BSONObject bson)
        {
            base.ReadBaseDataFromBSON(bson);
            if (bson.ContainsKey(LockWorldData.playerWhoOwnsLockIdKey))
            {
                this.playerWhoOwnsLockId = bson[LockWorldData.playerWhoOwnsLockIdKey];
            }
            if (bson.ContainsKey(LockWorldData.playerWhoOwnsLockNameKey))
            {
                this.playerWhoOwnsLockName = bson[LockWorldData.playerWhoOwnsLockNameKey];
            }
            if (bson.ContainsKey(LockWorldData.playersWhoHaveAccessToLockKey))
            {
                this.playersWhoHaveAccessToLock = bson[LockWorldData.playersWhoHaveAccessToLockKey];
            }
            if (bson.ContainsKey(LockWorldData.playersWhoHaveMinorAccessToLockKey))
            {
                this.playersWhoHaveAccessToLock = bson[LockWorldData.playersWhoHaveMinorAccessToLockKey];
            }
            if (bson.ContainsKey(LockWorldData.isOpenKey))
            {
                this.isOpen = bson[LockWorldData.isOpenKey];
            }
            if (bson.ContainsKey(LockWorldData.punchingAllowedKey))
            {
                this.punchingAllowed = bson[LockWorldData.punchingAllowedKey];
            }
            if (bson.ContainsKey(LockWorldData.creationTimeKey))
            {
                this.creationTime = bson[LockWorldData.creationTimeKey];
            }
            if (bson.ContainsKey(LockWorldData.lastActivatedTimeKey))
            {
                this.lastActivatedTime = bson[LockWorldData.lastActivatedTimeKey];
            }
            if (bson.ContainsKey(LockWorldData.posXKey))
            {
                this.x = bson[LockWorldData.posXKey].int32Value;
            }
            if (bson.ContainsKey(LockWorldData.posYKey))
            {
                this.y = bson[LockWorldData.posYKey].int32Value;
            }
        }

        // Token: 0x06000C90 RID: 3216 RVA: 0x000433D2 File Offset: 0x000417D2
        public override bool DoesValidate(BSONObject bson)
        {
            return this.ValidateKeys(bson);
        }

        // Token: 0x06000C91 RID: 3217 RVA: 0x000433E4 File Offset: 0x000417E4
        private bool ValidateKeys(BSONObject bson)
        {
            return bson.ContainsKey(LockWorldData.playerWhoOwnsLockIdKey) && bson.ContainsKey(LockWorldData.playerWhoOwnsLockNameKey) && bson.ContainsKey(LockWorldData.playersWhoHaveAccessToLockKey) && bson.ContainsKey(LockWorldData.isOpenKey) && bson.ContainsKey(LockWorldData.punchingAllowedKey) && bson.ContainsKey(LockWorldData.creationTimeKey) && bson.ContainsKey(LockWorldData.lastActivatedTimeKey);
        }

        // Token: 0x06000C92 RID: 3218 RVA: 0x0004345F File Offset: 0x0004185F
        public void SetPlayerWhoOwnsLockId(string newPlayerId)
        {
            this.playerWhoOwnsLockId = newPlayerId;
        }

        // Token: 0x06000C93 RID: 3219 RVA: 0x00043468 File Offset: 0x00041868
        public string GetPlayerWhoOwnsLockId()
        {
            return this.playerWhoOwnsLockId;
        }

        // Token: 0x06000C94 RID: 3220 RVA: 0x00043470 File Offset: 0x00041870
        public void SetPlayerWhoOwnsLockName(string newPlayerName)
        {
            this.playerWhoOwnsLockName = newPlayerName;
        }

        // Token: 0x06000C95 RID: 3221 RVA: 0x00043479 File Offset: 0x00041879
        public string GetPlayerWhoOwnsLockName()
        {
            return this.playerWhoOwnsLockName;
        }

        // Token: 0x06000C96 RID: 3222 RVA: 0x00043481 File Offset: 0x00041881
        public void SetIsOpen(bool newIsOpen)
        {
            this.isOpen = newIsOpen;
            this.isAnimationOn = !this.isOpen;
        }

        // Token: 0x06000C97 RID: 3223 RVA: 0x00043499 File Offset: 0x00041899
        public bool GetIsOpen()
        {
            return this.isOpen;
        }

        // Token: 0x06000C98 RID: 3224 RVA: 0x000434A1 File Offset: 0x000418A1
        public void SetIsPunchingAllowed(bool newPunchingAllowed)
        {
            this.punchingAllowed = newPunchingAllowed;
        }

        // Token: 0x06000C99 RID: 3225 RVA: 0x000434AA File Offset: 0x000418AA
        public bool GetIsPunchingAllowed()
        {
            return this.punchingAllowed;
        }

        // Token: 0x06000C9A RID: 3226 RVA: 0x000434B2 File Offset: 0x000418B2
        public void SetPlayersWhoHaveAccessToLock(List<string> newPlayersWhoHaveAccessToLock)
        {
            this.playersWhoHaveAccessToLock = newPlayersWhoHaveAccessToLock;
        }
        public void SetPlayersWhoHaveMinorAccessToLock(List<string> newPlayersWhoHaveMinorAccessToLock)
        {
            this.playersWhoHaveMinorAccessToLock = newPlayersWhoHaveMinorAccessToLock;
        }

        // Token: 0x06000C9B RID: 3227 RVA: 0x000434BB File Offset: 0x000418BB
        public List<string> GetPlayersWhoHaveAccessToLock()
        {
            return this.playersWhoHaveAccessToLock;
        }
        public List<string> GetPlayersWhoHaveMinorAccessToLock()
        {
            return this.playersWhoHaveMinorAccessToLock;
        }

        // Token: 0x06000C9C RID: 3228 RVA: 0x000434C3 File Offset: 0x000418C3
        public void AddPlayerToPlayersWhoHaveAccessToLock(string playerId)
        {
            if (this.playersWhoHaveAccessToLock.Count < Config.playersWhoHaveAccessToLockMaxAmount)
            {
                this.playersWhoHaveAccessToLock.Add(playerId);
            }
        }
        public void AddPlayerToPlayersWhoMinerHaveAccessToLock(string playerId)
        {
            if (this.playersWhoHaveMinorAccessToLock.Count < Config.playersWhoHaveAccessToLockMaxAmount)
            {
                this.playersWhoHaveMinorAccessToLock.Add(playerId);
            }
        }

        // Token: 0x06000C9D RID: 3229 RVA: 0x000434E6 File Offset: 0x000418E6
        public void RemovePlayerFromPlayersWhoHaveAccessToLock(string playerId)
        {
            this.playersWhoHaveAccessToLock.RemoveAll(combined => PlayerIdNameHelper.GetPlayerIdFromCombined(combined) == playerId);
        }
        public void RemovePlayerFromPlayersWhoHaveMinorAccessToLock(string playerId)
        {
            this.playersWhoHaveMinorAccessToLock.RemoveAll(combined => PlayerIdNameHelper.GetPlayerIdFromCombined(combined) == playerId);
        }

        // Token: 0x06000C9E RID: 3230 RVA: 0x000434F8 File Offset: 0x000418F8
        public bool DoesPlayerHaveAccessToLock(string playerId)
        {
            if (this.playerWhoOwnsLockId.Equals(playerId))
            {
                return true;
            }
            return this.playersWhoHaveMinorAccessToLock.Any(combined => PlayerIdNameHelper.GetPlayerIdFromCombined(combined) == playerId);
        }
        public bool DoesPlayerHaveMinorAccessToLock(string playerId)
        {
            if (this.playerWhoOwnsLockId.Equals(playerId))
            {
                return true;
            }
            return (this.playersWhoHaveAccessToLock.Any(combined => PlayerIdNameHelper.GetPlayerIdFromCombined(combined) == playerId) || this.playersWhoHaveMinorAccessToLock.Any(combined => PlayerIdNameHelper.GetPlayerIdFromCombined(combined) == playerId));

        }

        // Token: 0x06000C9F RID: 3231 RVA: 0x0004356F File Offset: 0x0004196F
        public void SetCreationTime(DateTime time)
        {
            this.creationTime = time;
        }

        // Token: 0x06000CA0 RID: 3232 RVA: 0x00043578 File Offset: 0x00041978
        public DateTime GetCreationTime()
        {
            return this.creationTime;
        }

        // Token: 0x06000CA1 RID: 3233 RVA: 0x00043580 File Offset: 0x00041980
        public void SetLastActivatedTime(DateTime time)
        {
            this.lastActivatedTime = time;
        }

        // Token: 0x06000CA2 RID: 3234 RVA: 0x00043589 File Offset: 0x00041989
        public DateTime GetLastActivatedTime()
        {
            return this.lastActivatedTime;
        }

        // Token: 0x04000AEC RID: 2796
        private static readonly string playerWhoOwnsLockIdKey = "playerWhoOwnsLockId";

        // Token: 0x04000AED RID: 2797
        private static readonly string playerWhoOwnsLockNameKey = "playerWhoOwnsLockName";

        // Token: 0x04000AEE RID: 2798
        private static readonly string playersWhoHaveAccessToLockKey = "playersWhoHaveAccessToLock";

        private static readonly string playersWhoHaveMinorAccessToLockKey = "playersWhoHaveMinorAccessToLock";

        // Token: 0x04000AEF RID: 2799
        private static readonly string isOpenKey = "isOpen";

        // Token: 0x04000AF0 RID: 2800
        private static readonly string punchingAllowedKey = "punchingAllowed";

        // Token: 0x04000AF1 RID: 2801
        private static readonly string creationTimeKey = "creationTime";

        // Token: 0x04000AF2 RID: 2802
        private static readonly string lastActivatedTimeKey = "lastActivatedTime";
        private static readonly string posXKey = "posX";
        private static readonly string posYKey = "posY";

        // Token: 0x04000AF3 RID: 2803
        private string playerWhoOwnsLockId = string.Empty;

        // Token: 0x04000AF4 RID: 2804
        private string playerWhoOwnsLockName = string.Empty;

        // Token: 0x04000AF5 RID: 2805
        private List<string> playersWhoHaveAccessToLock = new List<string>();
        private List<string> playersWhoHaveMinorAccessToLock = new List<string>();

        // Token: 0x04000AF6 RID: 2806
        private bool isOpen = false;

        // Token: 0x04000AF7 RID: 2807
        private bool punchingAllowed = false;

        // Token: 0x04000AF8 RID: 2808
        private DateTime creationTime = DateTime.MinValue;

        // Token: 0x04000AF9 RID: 2809
        private DateTime lastActivatedTime = DateTime.MinValue;

        public int x;
        public int y;
    }

}
