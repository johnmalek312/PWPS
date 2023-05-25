using PixelWorldsServer2.DataManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PixelWorldsServer2.Database
{
    public struct ShopResult
    {
        public int price;
        public List<KeyTriple<int, InventoryItemType, int>> items;
    }
    public class Shop
    {
        public static Dictionary<string, ShopResult> offers = new Dictionary<string, ShopResult>();

        public static void AddShopOffer(string name, int price, params KeyTriple<int, InventoryItemType, int>[] items)
        {
            ShopResult sr = new ShopResult();
            sr.items = new List<KeyTriple<int, InventoryItemType, int>>(items);
            sr.price = price;

            offers[name] = sr;
        }
        public enum InventoryItemType : byte
        {
            Block,
            BlockBackground,
            Seed,
            BlockWater,
            WearableItem,
            Weapon,
            Throwable,
            Consumable,
            Shard,
            Blueprint,
            Familiar,
            FAMFood,
            BlockWiring
        }
        public static bool ContainsItem(int itemId)
        {
            foreach (var res in offers.Values)
            {
                if (res.items.Any(item => item.Key == itemId))
                    return true;
            }

            return false;
        }

        public static void Init()
        {
            AddShopOffer("WorldLock", 3500, new KeyTriple<int, InventoryItemType, int>(413, InventoryItemType.Block, 1));
            AddShopOffer("PlatinumLock", 3500 * 100, new KeyTriple<int, InventoryItemType, int>(796, InventoryItemType.Block, 1));
            AddShopOffer("PetFoodDogPremium", 1250, new KeyTriple<int, InventoryItemType, int>(3856, InventoryItemType.Block, 1));
            AddShopOffer("SmallLock", 100, new KeyTriple<int, InventoryItemType, int>(410, InventoryItemType.Block, 1));
            AddShopOffer("MediumLock", 500, new KeyTriple<int, InventoryItemType, int>(411, InventoryItemType.Block, 1));
            AddShopOffer("CapeDracula", 1500, new KeyTriple<int, InventoryItemType, int>(1298, InventoryItemType.Block, 1));
            AddShopOffer("LargeLock", 1000, new KeyTriple<int, InventoryItemType, int>(412, InventoryItemType.Block, 1));
            AddShopOffer("BattleLock", 3000, new KeyTriple<int, InventoryItemType, int>(1132, InventoryItemType.Block, 1));
            AddShopOffer("BattleWorldLock", 7500, new KeyTriple<int, InventoryItemType, int>(3060, InventoryItemType.Block, 1));
            AddShopOffer("DarkWorldLock", 30000, new KeyTriple<int, InventoryItemType, int>(882, InventoryItemType.Block, 1));
            AddShopOffer("FrostWings", 100000, new KeyTriple<int, InventoryItemType, int>(2608, InventoryItemType.Block, 1));
            AddShopOffer("PixieWings", 7500, new KeyTriple<int, InventoryItemType, int>(586, InventoryItemType.Block, 1));
            AddShopOffer("GreenContactLenses", 7500, new KeyTriple<int, InventoryItemType, int>(906, InventoryItemType.Block, 1));
            AddShopOffer("SodaJetpack", 400000, new KeyTriple<int, InventoryItemType, int>(881, InventoryItemType.Block, 1));
            AddShopOffer("BlueContactLenses", 7500, new KeyTriple<int, InventoryItemType, int>(905, InventoryItemType.Block, 1));
            AddShopOffer("BrownContactLenses", 7500, new KeyTriple<int, InventoryItemType, int>(908, InventoryItemType.Block, 1));
            AddShopOffer("SilverContactLenses", 7500, new KeyTriple<int, InventoryItemType, int>(909, InventoryItemType.Block, 1));
            AddShopOffer("RedContactLenses", 150000, new KeyTriple<int, InventoryItemType, int>(609, InventoryItemType.Block, 1));
            AddShopOffer("AlienLenses", 300000, new KeyTriple<int, InventoryItemType, int>(3088, InventoryItemType.Block, 1));
            AddShopOffer("GoblinRing", 60000, new KeyTriple<int, InventoryItemType, int>(935, InventoryItemType.Block, 1));
            AddShopOffer("FrostRing", 150000, new KeyTriple<int, InventoryItemType, int>(934, InventoryItemType.Block, 1));
            AddShopOffer("DemonRing", 150000, new KeyTriple<int, InventoryItemType, int>(1293, InventoryItemType.Block, 1));
            AddShopOffer("LemonRing", 200000, new KeyTriple<int, InventoryItemType, int>(3085, InventoryItemType.Block, 1));
            AddShopOffer("OceanRing", 225000, new KeyTriple<int, InventoryItemType, int>(3086, InventoryItemType.Block, 1));
            AddShopOffer("RoseRing", 250000, new KeyTriple<int, InventoryItemType, int>(3087, InventoryItemType.Block, 1));
            AddShopOffer("FishingRodBambooBasic", 500, new KeyTriple<int, InventoryItemType, int>(2406, InventoryItemType.Block, 1));
            AddShopOffer("FishingRodFiberglassBasic", 2500, new KeyTriple<int, InventoryItemType, int>(2410, InventoryItemType.Block, 1));
            AddShopOffer("FishingRodCarbonFiberBasic", 15000, new KeyTriple<int, InventoryItemType, int>(2414, InventoryItemType.Block, 1));
            AddShopOffer("FishingRodTitaniumBasic", 35000, new KeyTriple<int, InventoryItemType, int>(2418, InventoryItemType.Block, 1));
            AddShopOffer("FishingRodUpgradeStation", 10000, new KeyTriple<int, InventoryItemType, int>(2506, InventoryItemType.Block, 1));
            AddShopOffer("FishingScoreBoard", 15000, new KeyTriple<int, InventoryItemType, int>(2535, InventoryItemType.Block, 1));
            AddShopOffer("FishingRecycler", 20000, new KeyTriple<int, InventoryItemType, int>(2504, InventoryItemType.Block, 1));
            AddShopOffer("SupportHoodie", 10000, new KeyTriple<int, InventoryItemType, int>(879, InventoryItemType.Block, 1));
            AddShopOffer("SpiritB", 10000, new KeyTriple<int, InventoryItemType, int>(3483, InventoryItemType.Block, 1));
            AddShopOffer("DuaalBlades", 10000, new KeyTriple<int, InventoryItemType, int>(4281, InventoryItemType.Block, 1));
            AddShopOffer("PinkSupportHoodie", 10000, new KeyTriple<int, InventoryItemType, int>(1023, InventoryItemType.Block, 1));
            AddShopOffer("WingsDemon", 12000, new KeyTriple<int, InventoryItemType, int>(215, InventoryItemType.Block, 1));
            AddShopOffer("Fertilizer", 150, new KeyTriple<int, InventoryItemType, int>(1070, InventoryItemType.Block, 1));
            AddShopOffer("SpiritBlade", 150, new KeyTriple<int, InventoryItemType, int>(1306, InventoryItemType.Block, 1));
            AddShopOffer("ScorcherWings", 1520202020, new KeyTriple<int, InventoryItemType, int>(4768, InventoryItemType.Block, 1));
            AddShopOffer("FertilizerLarge", 300, new KeyTriple<int, InventoryItemType, int>(1499, InventoryItemType.Block, 1));
            AddShopOffer("FertilizerLarge", 3002222, new KeyTriple<int, InventoryItemType, int>(2293, InventoryItemType.Block, 1));
            AddShopOffer("Snowman", 55000, new KeyTriple<int, InventoryItemType, int>(1458, InventoryItemType.Block, 1));
            AddShopOffer("Penguin", 70000, new KeyTriple<int, InventoryItemType, int>(1463, InventoryItemType.Block, 1));
            AddShopOffer("Scythe", 70000, new KeyTriple<int, InventoryItemType, int>(1305, InventoryItemType.Block, 1));
            AddShopOffer("Bunny", 35000, new KeyTriple<int, InventoryItemType, int>(1095, InventoryItemType.Block, 1));
            AddShopOffer("Crow", 50000, new KeyTriple<int, InventoryItemType, int>(1093, InventoryItemType.Block, 1));
            AddShopOffer("Mini-bot", 65000, new KeyTriple<int, InventoryItemType, int>(1100, InventoryItemType.Block, 1));
            AddShopOffer("Gremlin", 75000, new KeyTriple<int, InventoryItemType, int>(1086, InventoryItemType.Block, 1));
            AddShopOffer("FAMEvolverator", 35000, new KeyTriple<int, InventoryItemType, int>(1126, InventoryItemType.Block, 1));
            AddShopOffer("FAMFoodMachine", 50000, new KeyTriple<int, InventoryItemType, int>(1125, InventoryItemType.Block, 1));
            AddShopOffer("VirtualPetDog", 10000, new KeyTriple<int, InventoryItemType, int>(3822, InventoryItemType.Block, 1));
            AddShopOffer("VirtualPetCat", 10000, new KeyTriple<int, InventoryItemType, int>(3823, InventoryItemType.Block, 1));
            AddShopOffer("PetFoodDogBasic", 500, new KeyTriple<int, InventoryItemType, int>(3855, InventoryItemType.Block, 1));
            AddShopOffer("PetFoodCatBasic", 500, new KeyTriple<int, InventoryItemType, int>(3857, InventoryItemType.Block, 1));
            AddShopOffer("PetFoodCatPremium", 1250, new KeyTriple<int, InventoryItemType, int>(3858, InventoryItemType.Block, 1));
            AddShopOffer("PetFoodSlimeBasic", 500, new KeyTriple<int, InventoryItemType, int>(3859, InventoryItemType.Block, 1));
            AddShopOffer("PetFoodSlimePremium", 1250, new KeyTriple<int, InventoryItemType, int>(3860, InventoryItemType.Block, 1));
            AddShopOffer("ModHoodie", 125233230, new KeyTriple<int, InventoryItemType, int>(1038, InventoryItemType.Block, 1));
            AddShopOffer("CthulhuWings", 1250000000, new KeyTriple<int, InventoryItemType, int>(1350, InventoryItemType.Block, 1));
            AddShopOffer("BanHammer", 1250000000, new KeyTriple<int, InventoryItemType, int>(731, InventoryItemType.Block, 1));
            AddShopOffer("JakeKatana", 1250000000, new KeyTriple<int, InventoryItemType, int>(606, InventoryItemType.Block, 1));
            AddShopOffer("JakeKatana", 1250000000, new KeyTriple<int, InventoryItemType, int>(4197, InventoryItemType.Block, 1));
            AddShopOffer("SantaBeard", 1250000000, new KeyTriple<int, InventoryItemType, int>(545, InventoryItemType.Block, 1));
            AddShopOffer("TormentorWings", 1250000000, new KeyTriple<int, InventoryItemType, int>(2292, InventoryItemType.Block, 1));
            AddShopOffer("DarkifritWings", 1250000000, new KeyTriple<int, InventoryItemType, int>(4268, InventoryItemType.Block, 1));
            AddShopOffer("DarkSpriteWings", 5125500, new KeyTriple<int, InventoryItemType, int>(3481, InventoryItemType.Block, 1));
            AddShopOffer("PetMedicineBasic", 500, new KeyTriple<int, InventoryItemType, int>(3861, InventoryItemType.Block, 1));
            AddShopOffer("OrbLightingLesserDark", 75000, new KeyTriple<int, InventoryItemType, int>(4141, InventoryItemType.Block, 1));
            AddShopOffer("OrbLightingDark", 75000, new KeyTriple<int, InventoryItemType, int>(3922, InventoryItemType.Block, 1));
            AddShopOffer("OrbLightingNone", 2000, new KeyTriple<int, InventoryItemType, int>(3921, InventoryItemType.Block, 1));
            AddShopOffer("OrbWeatherNone", 1000, new KeyTriple<int, InventoryItemType, int>(3370, InventoryItemType.Block, 1));
            AddShopOffer("OrbWeatherLightRain", 10000, new KeyTriple<int, InventoryItemType, int>(3444, InventoryItemType.Block, 1));
            AddShopOffer("OrbWeatherSandStorm", 17500, new KeyTriple<int, InventoryItemType, int>(3443, InventoryItemType.Block, 1));
            AddShopOffer("WinterOrb", 10000, new KeyTriple<int, InventoryItemType, int>(521, InventoryItemType.Block, 1));
            AddShopOffer("ForestOrb", 2000, new KeyTriple<int, InventoryItemType, int>(520, InventoryItemType.Block, 1));
            AddShopOffer("StarOrb", 5000, new KeyTriple<int, InventoryItemType, int>(524, InventoryItemType.Block, 1));
            AddShopOffer("SandOrb", 9000, new KeyTriple<int, InventoryItemType, int>(519, InventoryItemType.Block, 1));
            AddShopOffer("NightOrb", 12000, new KeyTriple<int, InventoryItemType, int>(522, InventoryItemType.Block, 1));
            AddShopOffer("CityOrb", 100, new KeyTriple<int, InventoryItemType, int>(1758, InventoryItemType.Block, 1));
            AddShopOffer("WeaponWiringTool", 7500, new KeyTriple<int, InventoryItemType, int>(3097, InventoryItemType.Block, 1));
            AddShopOffer("WiringTriggerLever", 250, new KeyTriple<int, InventoryItemType, int>(3100, InventoryItemType.Block, 1));
            AddShopOffer("WiringTriggerSwitch", 250, new KeyTriple<int, InventoryItemType, int>(3098, InventoryItemType.Block, 1));
            AddShopOffer("WiringTriggerButton", 350, new KeyTriple<int, InventoryItemType, int>(3099, InventoryItemType.Block, 1));
            AddShopOffer("WiringTriggerPressurePad", 500, new KeyTriple<int, InventoryItemType, int>(3101, InventoryItemType.Block, 1));
            AddShopOffer("WiringTriggerProximitySensor", 500, new KeyTriple<int, InventoryItemType, int>(3102, InventoryItemType.Block, 1));
            AddShopOffer("OnOffLight", 100, new KeyTriple<int, InventoryItemType, int>(3111, InventoryItemType.Block, 1));
            AddShopOffer("DisappearingBlock", 250, new KeyTriple<int, InventoryItemType, int>(3112, InventoryItemType.Block, 1));
            AddShopOffer("WiringLogicGateAND", 250, new KeyTriple<int, InventoryItemType, int>(3103, InventoryItemType.Block, 1));
            AddShopOffer("WiringLogicGateNAND", 250, new KeyTriple<int, InventoryItemType, int>(3104, InventoryItemType.Block, 1));
            AddShopOffer("WiringLogicGateOR", 250, new KeyTriple<int, InventoryItemType, int>(3105, InventoryItemType.Block, 1));
            AddShopOffer("WiringLogicGateNOR", 250, new KeyTriple<int, InventoryItemType, int>(3106, InventoryItemType.Block, 1));
            AddShopOffer("WiringLogicGateXOR", 250, new KeyTriple<int, InventoryItemType, int>(3107, InventoryItemType.Block, 1));
            AddShopOffer("WiringLogicGateXNOR", 250, new KeyTriple<int, InventoryItemType, int>(3108, InventoryItemType.Block, 1));
            AddShopOffer("WiringLogicGateNOT", 250, new KeyTriple<int, InventoryItemType, int>(3109, InventoryItemType.Block, 1));
            AddShopOffer("WiringLogicGateSIGNALDIVIDER", 150, new KeyTriple<int, InventoryItemType, int>(3146, InventoryItemType.Block, 1));
            AddShopOffer("WiringLogicGateTOGGLE", 500, new KeyTriple<int, InventoryItemType, int>(3167, InventoryItemType.Block, 1));
            AddShopOffer("WiringLogicGateDELAYTIMER", 500, new KeyTriple<int, InventoryItemType, int>(3143, InventoryItemType.Block, 1));
            AddShopOffer("WiringLogicGateSIGNALHOLDER", 500, new KeyTriple<int, InventoryItemType, int>(3144, InventoryItemType.Block, 1));
            AddShopOffer("WiringLogicGateTIMER", 500, new KeyTriple<int, InventoryItemType, int>(3145, InventoryItemType.Block, 1));
            AddShopOffer("WiringLogicGateRANDOMIZER", 500, new KeyTriple<int, InventoryItemType, int>(3183, InventoryItemType.Block, 1));
            AddShopOffer("ConsumableRedScroll", 150, new KeyTriple<int, InventoryItemType, int>(1402, InventoryItemType.Block, 1));
            AddShopOffer("ConsumableRedScroll10", 1000, new KeyTriple<int, InventoryItemType, int>(1402, InventoryItemType.Consumable, 10));
            AddShopOffer("RedPortal", 9000, new KeyTriple<int, InventoryItemType, int>(1799, InventoryItemType.Block, 1));
            AddShopOffer("JetRaceGroupPortal", 45000, new KeyTriple<int, InventoryItemType, int>(4373, InventoryItemType.Block, 1));
            AddShopOffer("ScreenshotForbidden", 100, new KeyTriple<int, InventoryItemType, int>(3442, InventoryItemType.Block, 1));
            AddShopOffer("ConsumableCameraWorld", 100, new KeyTriple<int, InventoryItemType, int>(1521, InventoryItemType.Block, 1));
            AddShopOffer("PrizeBox", 350, new KeyTriple<int, InventoryItemType, int>(966, InventoryItemType.Block, 1));
            AddShopOffer("AdTV", 500, new KeyTriple<int, InventoryItemType, int>(3052, InventoryItemType.Block, 1));
            AddShopOffer("Recall", 1000, new KeyTriple<int, InventoryItemType, int>(2343, InventoryItemType.Block, 1));
            AddShopOffer("RatingBoard", 1200, new KeyTriple<int, InventoryItemType, int>(293, InventoryItemType.Block, 1));
            AddShopOffer("DeathCounter", 2000, new KeyTriple<int, InventoryItemType, int>(970, InventoryItemType.Block, 1));
            AddShopOffer("EntrancePortal", 3750, new KeyTriple<int, InventoryItemType, int>(1078, InventoryItemType.Block, 1));
            AddShopOffer("MagicCauldron", 5750, new KeyTriple<int, InventoryItemType, int>(294, InventoryItemType.Block, 1));
            AddShopOffer("RuleBot", 10000, new KeyTriple<int, InventoryItemType, int>(1358, InventoryItemType.Block, 1));
            AddShopOffer("RuleBotPotion", 10000, new KeyTriple<int, InventoryItemType, int>(2332, InventoryItemType.Block, 1));
            AddShopOffer("RuleBotMount", 125000, new KeyTriple<int, InventoryItemType, int>(4367, InventoryItemType.Block, 1));
            AddShopOffer("BestSetPhotoBooth", 15000, new KeyTriple<int, InventoryItemType, int>(4491, InventoryItemType.Block, 1));
            AddShopOffer("SafeBox", 17500, new KeyTriple<int, InventoryItemType, int>(3576, InventoryItemType.Block, 1));
            AddShopOffer("Replicator", 20000, new KeyTriple<int, InventoryItemType, int>(847, InventoryItemType.Block, 1));
            AddShopOffer("ColorOMat", 25000, new KeyTriple<int, InventoryItemType, int>(3437, InventoryItemType.Block, 1));
            AddShopOffer("GravityModifier", 150000, new KeyTriple<int, InventoryItemType, int>(2008, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintJetPackSnow", 425000, new KeyTriple<int, InventoryItemType, int>(3525, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintNecklaceFrost", 80000, new KeyTriple<int, InventoryItemType, int>(1447, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintOrbSpaceBackground", 50000, new KeyTriple<int, InventoryItemType, int>(856, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintNecklaceGlimmer", 60000, new KeyTriple<int, InventoryItemType, int>(863, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintMaskTiki", 100000, new KeyTriple<int, InventoryItemType, int>(861, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintWeaponSwordLaserGreen", 350000, new KeyTriple<int, InventoryItemType, int>(853, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintWeaponSwordLaserRed", 350000, new KeyTriple<int, InventoryItemType, int>(854, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintWeaponSwordLaserBlue", 100, new KeyTriple<int, InventoryItemType, int>(855, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintShirtArmorKnight", 225000, new KeyTriple<int, InventoryItemType, int>(3342, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintPantsArmorKnight", 225000, new KeyTriple<int, InventoryItemType, int>(3343, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintHatHelmetArmorKnight", 225000, new KeyTriple<int, InventoryItemType, int>(3344, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintWeaponSwordKnight", 350000, new KeyTriple<int, InventoryItemType, int>(3345, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintCapeDark", 475000, new KeyTriple<int, InventoryItemType, int>(857, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintWeaponSwordLaserClaymore", 500000, new KeyTriple<int, InventoryItemType, int>(3091, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintJetPackPlasma", 500000, new KeyTriple<int, InventoryItemType, int>(862, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintWeaponSwordFlaming", 725000, new KeyTriple<int, InventoryItemType, int>(864, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintWingsValkyria", 750000, new KeyTriple<int, InventoryItemType, int>(1289, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintWingsMechanicalGolden", 950000, new KeyTriple<int, InventoryItemType, int>(3089, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintJetPackLongJumpAncientGolem", 500000, new KeyTriple<int, InventoryItemType, int>(4779, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintWingsBackgoyle", 750000, new KeyTriple<int, InventoryItemType, int>(4777, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintJetPackLongJumpExplosive", 500000, new KeyTriple<int, InventoryItemType, int>(4775, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintWingsIonThrusters", 500000, new KeyTriple<int, InventoryItemType, int>(4760, InventoryItemType.Block, 1));
            AddShopOffer("BlueprintBackBackpackWanderer", 335000, new KeyTriple<int, InventoryItemType, int>(4746, InventoryItemType.Block, 1));
            AddShopOffer("KiddieRide", 10000, new KeyTriple<int, InventoryItemType, int>(1129, InventoryItemType.Block, 1));
            AddShopOffer("DoorClan", 1000, new KeyTriple<int, InventoryItemType, int>(3559, InventoryItemType.Block, 1));
            AddShopOffer("ClanQuestBot", 7500, new KeyTriple<int, InventoryItemType, int>(3500, InventoryItemType.Block, 1));
            AddShopOffer("ClanTotem", 9000, new KeyTriple<int, InventoryItemType, int>(3466, InventoryItemType.Block, 1));
            AddShopOffer("DoorFactionDark", 2000, new KeyTriple<int, InventoryItemType, int>(3602, InventoryItemType.Block, 1));
            AddShopOffer("DoorFactionLight", 2000, new KeyTriple<int, InventoryItemType, int>(3603, InventoryItemType.Block, 1));
            AddShopOffer("CheckPointFactionDark", 3500, new KeyTriple<int, InventoryItemType, int>(3598, InventoryItemType.Block, 1));
            AddShopOffer("CheckPointFactionLight", 3500, new KeyTriple<int, InventoryItemType, int>(3599, InventoryItemType.Block, 1));
            AddShopOffer("PortalFactionDark", 4000, new KeyTriple<int, InventoryItemType, int>(3600, InventoryItemType.Block, 1));
            AddShopOffer("DarkPixieWings", 4000, new KeyTriple<int, InventoryItemType, int>(608, InventoryItemType.Block, 1));
            AddShopOffer("PortalFactionLight", 100, new KeyTriple<int, InventoryItemType, int>(3601, InventoryItemType.Block, 1));
            AddShopOffer("BattleScoreBoardFaction", 8000, new KeyTriple<int, InventoryItemType, int>(3597, InventoryItemType.Block, 1));
            AddShopOffer("LockBattleFaction", 7500, new KeyTriple<int, InventoryItemType, int>(3596, InventoryItemType.Block, 1));
            AddShopOffer("LockWorldBattleFaction", 9000, new KeyTriple<int, InventoryItemType, int>(3606, InventoryItemType.Block, 1));
            AddShopOffer("KeyWorld", 1000000000, new KeyTriple<int, InventoryItemType, int>(418, InventoryItemType.Block, 1));
            AddShopOffer("LockDiamond", 1000000000, new KeyTriple<int, InventoryItemType, int>(415, InventoryItemType.Block, 1));
            AddShopOffer("TokenLock", 1000000000, new KeyTriple<int, InventoryItemType, int>(1135, InventoryItemType.Block, 1));
            AddShopOffer("Water", 1000000000, new KeyTriple<int, InventoryItemType, int>(110, InventoryItemType.Block, 1));
            AddShopOffer("PortalEntrance", 1000000000, new KeyTriple<int, InventoryItemType, int>(18, InventoryItemType.Block, 1));
            AddShopOffer("Suitİnvisable", 1000000000, new KeyTriple<int, InventoryItemType, int>(2096, InventoryItemType.Block, 1));



            //  Bugged invisable items fix: Not useful items
            AddShopOffer("TODO1", 1000000000, new KeyTriple<int, InventoryItemType, int>(37, InventoryItemType.Block, 1));
            AddShopOffer("TODO2", 1000000000, new KeyTriple<int, InventoryItemType, int>(38, InventoryItemType.Block, 1));
            AddShopOffer("TODO3", 1000000000, new KeyTriple<int, InventoryItemType, int>(39, InventoryItemType.Block, 1));
            AddShopOffer("TODO4", 1000000000, new KeyTriple<int, InventoryItemType, int>(40, InventoryItemType.Block, 1));
            AddShopOffer("TODO5", 1000000000, new KeyTriple<int, InventoryItemType, int>(41, InventoryItemType.Block, 1));
            AddShopOffer("TODO6", 1000000000, new KeyTriple<int, InventoryItemType, int>(42, InventoryItemType.Block, 1));
            AddShopOffer("TODO7", 1000000000, new KeyTriple<int, InventoryItemType, int>(43, InventoryItemType.Block, 1));
            AddShopOffer("TODO8", 1000000000, new KeyTriple<int, InventoryItemType, int>(44, InventoryItemType.Block, 1));
            AddShopOffer("TODO9", 1000000000, new KeyTriple<int, InventoryItemType, int>(45, InventoryItemType.Block, 1));
            AddShopOffer("TODO10", 1000000000, new KeyTriple<int, InventoryItemType, int>(46, InventoryItemType.Block, 1));
            AddShopOffer("TODO11", 1000000000, new KeyTriple<int, InventoryItemType, int>(47, InventoryItemType.Block, 1));
            AddShopOffer("TODO12", 1000000000, new KeyTriple<int, InventoryItemType, int>(48, InventoryItemType.Block, 1));
            AddShopOffer("TODO13", 1000000000, new KeyTriple<int, InventoryItemType, int>(606, InventoryItemType.Block, 1));
            AddShopOffer("WALLBUG", 1000000000, new KeyTriple<int, InventoryItemType, int>(2048, InventoryItemType.Block, 1));
            AddShopOffer("WALLBUG1", 1000000000, new KeyTriple<int, InventoryItemType, int>(2049, InventoryItemType.Block, 1));
            AddShopOffer("WALLBUG2", 1000000000, new KeyTriple<int, InventoryItemType, int>(2050, InventoryItemType.Block, 1));

        }
    }
}
