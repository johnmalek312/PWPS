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
        public List<KeyValuePair<int, InventoryItemType>> items;
    }
    public class Shop
    {
        public static Dictionary<string, ShopResult> offers = new Dictionary<string, ShopResult>();

        public static void AddShopOffer(string name, int price, params KeyValuePair<int, InventoryItemType>[] items)
        {
            ShopResult sr = new ShopResult();
            sr.items = new List<KeyValuePair<int, InventoryItemType>>(items);
            sr.price = price;

            offers[name] = sr;
        }

        public static bool ContainsItem(int itemId)
        {
            foreach (var res in offers.Values)
            {
                if (res.items.Any(item=>item.Key == itemId))
                    return true;
            }

            return false;
        }

        public static void Init()
        {
            AddShopOffer("WorldLock", 3500, new KeyValuePair<int, InventoryItemType>(413, InventoryItemType.Block));
            AddShopOffer("PlatinumLock", 3500 * 100, InventoryItemType.Block, 796);
            AddShopOffer("PetFoodDogPremium", 1250, InventoryItemType.Block, 3856);
            AddShopOffer("SmallLock", 100, InventoryItemType.Block, 410);
            AddShopOffer("MediumLock", 500, InventoryItemType.Block, 411);
            AddShopOffer("CapeDracula", 1500, InventoryItemType.Block, 1298);
            AddShopOffer("LargeLock", 1000, InventoryItemType.Block, 412);
            AddShopOffer("BattleLock", 3000, InventoryItemType.Block, 1132);
            AddShopOffer("BattleWorldLock", 7500, InventoryItemType.Block, 3060);
            AddShopOffer("DarkWorldLock", 30000, InventoryItemType.Block, 882);
            AddShopOffer("FrostWings", 100000, InventoryItemType.Block, 2608);
            AddShopOffer("PixieWings", 7500, InventoryItemType.Block, 586);
            AddShopOffer("GreenContactLenses", 7500, InventoryItemType.Block, 906);
            AddShopOffer("SodaJetpack", 400000, InventoryItemType.Block, 881);
            AddShopOffer("BlueContactLenses", 7500, InventoryItemType.Block, 905);
            AddShopOffer("BrownContactLenses", 7500, InventoryItemType.Block, 908);
            AddShopOffer("SilverContactLenses", 7500, InventoryItemType.Block, 909);
            AddShopOffer("RedContactLenses", 150000, InventoryItemType.Block, 609);
            AddShopOffer("AlienLenses", 300000, InventoryItemType.Block, 3088);
            AddShopOffer("GoblinRing", 60000, InventoryItemType.Block, 935);
            AddShopOffer("FrostRing", 150000, InventoryItemType.Block, 934);
            AddShopOffer("DemonRing", 150000, InventoryItemType.Block, 1293);
            AddShopOffer("LemonRing", 200000, InventoryItemType.Block, 3085);
            AddShopOffer("OceanRing", 225000, InventoryItemType.Block, 3086);
            AddShopOffer("RoseRing", 250000, InventoryItemType.Block, 3087);
            AddShopOffer("FishingRodBambooBasic", 500, InventoryItemType.Block, 2406);
            AddShopOffer("FishingRodFiberglassBasic", 2500, InventoryItemType.Block, 2410);
            AddShopOffer("FishingRodCarbonFiberBasic", 15000, InventoryItemType.Block, 2414);
            AddShopOffer("FishingRodTitaniumBasic", 35000, InventoryItemType.Block, 2418);
            AddShopOffer("FishingRodUpgradeStation", 10000, InventoryItemType.Block, 2506);
            AddShopOffer("FishingScoreBoard", 15000, InventoryItemType.Block, 2535);
            AddShopOffer("FishingRecycler", 20000, InventoryItemType.Block, 2504);
            AddShopOffer("SupportHoodie", 10000, InventoryItemType.Block, 879);
            AddShopOffer("SpiritB", 10000, InventoryItemType.Block, 3483);
            AddShopOffer("DuaalBlades", 10000, InventoryItemType.Block, 4281);
            AddShopOffer("PinkSupportHoodie", 10000, InventoryItemType.Block, 1023);
            AddShopOffer("WingsDemon", 12000, InventoryItemType.Block, 215);
            AddShopOffer("Fertilizer", 150, InventoryItemType.Block, 1070);
            AddShopOffer("SpiritBlade", 150, InventoryItemType.Block, 1306);
            AddShopOffer("ScorcherWings", 1520202020, InventoryItemType.Block, 4768);
            AddShopOffer("FertilizerLarge", 300, InventoryItemType.Block, 1499);
            AddShopOffer("FertilizerLarge", 3002222, InventoryItemType.Block, 2293);
            AddShopOffer("Snowman", 55000, InventoryItemType.Block, 1458);
            AddShopOffer("Penguin", 70000, InventoryItemType.Block, 1463);
            AddShopOffer("Scythe", 70000, InventoryItemType.Block, 1305);
            AddShopOffer("Bunny", 35000, InventoryItemType.Block, 1095);
            AddShopOffer("Crow", 50000, InventoryItemType.Block, 1093);
            AddShopOffer("Mini-bot", 65000, InventoryItemType.Block, 1100);
            AddShopOffer("Gremlin", 75000, InventoryItemType.Block, 1086);
            AddShopOffer("FAMEvolverator", 35000, InventoryItemType.Block, 1126);
            AddShopOffer("FAMFoodMachine", 50000, InventoryItemType.Block, 1125);
            AddShopOffer("VirtualPetDog", 10000, InventoryItemType.Block, 3822);
            AddShopOffer("VirtualPetCat", 10000, InventoryItemType.Block, 3823);
            AddShopOffer("PetFoodDogBasic", 500, InventoryItemType.Block, 3855);
            AddShopOffer("PetFoodCatBasic", 500, InventoryItemType.Block, 3857);
            AddShopOffer("PetFoodCatPremium", 1250, InventoryItemType.Block, 3858);
            AddShopOffer("PetFoodSlimeBasic", 500, InventoryItemType.Block, 3859);
            AddShopOffer("PetFoodSlimePremium", 1250, InventoryItemType.Block, 3860);
            AddShopOffer("ModHoodie", 125233230, InventoryItemType.Block, 1038);
            AddShopOffer("CthulhuWings", 1250000000, InventoryItemType.Block, 1350);
            AddShopOffer("BanHammer", 1250000000, InventoryItemType.Block, 731);
            AddShopOffer("JakeKatana", 1250000000, InventoryItemType.Block, 606);
            AddShopOffer("JakeKatana", 1250000000, InventoryItemType.Block, 4197);
            AddShopOffer("SantaBeard", 1250000000, InventoryItemType.Block, 545);
            AddShopOffer("TormentorWings", 1250000000, InventoryItemType.Block, 2292);
            AddShopOffer("DarkifritWings", 1250000000, InventoryItemType.Block, 4268);
            AddShopOffer("DarkSpriteWings", 5125500, InventoryItemType.Block, 3481);
            AddShopOffer("PetMedicineBasic", 500, InventoryItemType.Block, 3861);
            AddShopOffer("OrbLightingLesserDark", 75000, InventoryItemType.Block, 4141);
            AddShopOffer("OrbLightingDark", 75000, InventoryItemType.Block, 3922);
            AddShopOffer("OrbLightingNone", 2000, InventoryItemType.Block, 3921);
            AddShopOffer("OrbWeatherNone", 1000, InventoryItemType.Block, 3370);
            AddShopOffer("OrbWeatherLightRain", 10000, InventoryItemType.Block, 3444);
            AddShopOffer("OrbWeatherSandStorm", 17500, InventoryItemType.Block, 3443);
            AddShopOffer("WinterOrb", 10000, InventoryItemType.Block, 521);
            AddShopOffer("ForestOrb", 2000, InventoryItemType.Block, 520);
            AddShopOffer("StarOrb", 5000, InventoryItemType.Block, 524);
            AddShopOffer("SandOrb", 9000, InventoryItemType.Block, 519);
            AddShopOffer("NightOrb", 12000, InventoryItemType.Block, 522);
            AddShopOffer("CityOrb", 100,    InventoryItemType.Block, 1758);
            AddShopOffer("WeaponWiringTool", 7500, InventoryItemType.Block, 3097);
            AddShopOffer("WiringTriggerLever", 250, InventoryItemType.Block, 3100);
            AddShopOffer("WiringTriggerSwitch", 250, InventoryItemType.Block, 3098);
            AddShopOffer("WiringTriggerButton", 350, InventoryItemType.Block, 3099);
            AddShopOffer("WiringTriggerPressurePad", 500, InventoryItemType.Block, 3101);
            AddShopOffer("WiringTriggerProximitySensor", 500, InventoryItemType.Block, 3102);
            AddShopOffer("OnOffLight", 100, InventoryItemType.Block, 3111);
            AddShopOffer("DisappearingBlock", 250, InventoryItemType.Block, 3112);
            AddShopOffer("WiringLogicGateAND", 250, InventoryItemType.Block, 3103);
            AddShopOffer("WiringLogicGateNAND", 250, InventoryItemType.Block, 3104);
            AddShopOffer("WiringLogicGateOR", 250, InventoryItemType.Block, 3105);
            AddShopOffer("WiringLogicGateNOR", 250, InventoryItemType.Block, 3106);
            AddShopOffer("WiringLogicGateXOR", 250, InventoryItemType.Block, 3107);
            AddShopOffer("WiringLogicGateXNOR", 250, InventoryItemType.Block, 3108);
            AddShopOffer("WiringLogicGateNOT", 250, InventoryItemType.Block, 3109);
            AddShopOffer("WiringLogicGateSIGNALDIVIDER", 150, InventoryItemType.Block, 3146);
            AddShopOffer("WiringLogicGateTOGGLE", 500, InventoryItemType.Block, 3167);
            AddShopOffer("WiringLogicGateDELAYTIMER", 500, InventoryItemType.Block, 3143);
            AddShopOffer("WiringLogicGateSIGNALHOLDER", 500, InventoryItemType.Block, 3144);
            AddShopOffer("WiringLogicGateTIMER", 500, InventoryItemType.Block, 3145);
            AddShopOffer("WiringLogicGateRANDOMIZER", 500, InventoryItemType.Block, 3183);
            AddShopOffer("ConsumableRedScroll", 150, InventoryItemType.Block, 1402);
            AddShopOffer("ConsumableRedScroll10", 1000, InventoryItemType.Block, 1402, 1402, 1402, 1402, 1402, 1402, 1402, 1402, 1402, 1402);
            AddShopOffer("RedPortal", 9000, InventoryItemType.Block, 1799);
            AddShopOffer("JetRaceGroupPortal", 45000, InventoryItemType.Block, 4373);
            AddShopOffer("ScreenshotForbidden", 100, InventoryItemType.Block, 3442);
            AddShopOffer("ConsumableCameraWorld", 100, InventoryItemType.Block, 1521);
            AddShopOffer("PrizeBox", 350, InventoryItemType.Block, 966);
            AddShopOffer("AdTV", 500, InventoryItemType.Block, 3052);
            AddShopOffer("Recall", 1000, InventoryItemType.Block, 2343);
            AddShopOffer("RatingBoard", 1200, InventoryItemType.Block, 293);
            AddShopOffer("DeathCounter", 2000, InventoryItemType.Block, 970);
            AddShopOffer("EntrancePortal", 3750, InventoryItemType.Block, 1078);
            AddShopOffer("MagicCauldron", 5750, InventoryItemType.Block, 294);
            AddShopOffer("RuleBot", 10000, InventoryItemType.Block, 1358);
            AddShopOffer("RuleBotPotion", 10000, InventoryItemType.Block, 2332);
            AddShopOffer("RuleBotMount", 125000, InventoryItemType.Block, 4367);
            AddShopOffer("BestSetPhotoBooth", 15000, InventoryItemType.Block, 4491);
            AddShopOffer("SafeBox", 17500, InventoryItemType.Block, 3576);
            AddShopOffer("Replicator", 20000, InventoryItemType.Block, 847);
            AddShopOffer("ColorOMat", 25000, InventoryItemType.Block, 3437);
            AddShopOffer("GravityModifier", 150000, InventoryItemType.Block, 2008);
            AddShopOffer("BlueprintJetPackSnow", 425000, InventoryItemType.Block, 3525);
            AddShopOffer("BlueprintNecklaceFrost", 80000, InventoryItemType.Block, 1447);
            AddShopOffer("BlueprintOrbSpaceBackground", 50000, InventoryItemType.Block, 856);
            AddShopOffer("BlueprintNecklaceGlimmer", 60000, InventoryItemType.Block, 863);
            AddShopOffer("BlueprintMaskTiki", 100000, InventoryItemType.Block, 861);
            AddShopOffer("BlueprintWeaponSwordLaserGreen", 350000, InventoryItemType.Block, 853);
            AddShopOffer("BlueprintWeaponSwordLaserRed", 350000, InventoryItemType.Block, 854);
            AddShopOffer("BlueprintWeaponSwordLaserBlue", 100, InventoryItemType.Block, 855);
            AddShopOffer("BlueprintShirtArmorKnight", 225000, InventoryItemType.Block, 3342);
            AddShopOffer("BlueprintPantsArmorKnight", 225000, InventoryItemType.Block, 3343);
            AddShopOffer("BlueprintHatHelmetArmorKnight", 225000, InventoryItemType.Block, 3344);
            AddShopOffer("BlueprintWeaponSwordKnight", 350000, InventoryItemType.Block, 3345);
            AddShopOffer("BlueprintCapeDark", 475000, InventoryItemType.Block,  857);
            AddShopOffer("BlueprintWeaponSwordLaserClaymore", 500000, InventoryItemType.Block, 3091);
            AddShopOffer("BlueprintJetPackPlasma", 500000, InventoryItemType.Block, 862);
            AddShopOffer("BlueprintWeaponSwordFlaming", 725000, InventoryItemType.Block, 864);
            AddShopOffer("BlueprintWingsValkyria", 750000, InventoryItemType.Block, 1289);
            AddShopOffer("BlueprintWingsMechanicalGolden", 950000, InventoryItemType.Block, 3089);
            AddShopOffer("BlueprintJetPackLongJumpAncientGolem", 500000, InventoryItemType.Block, 4779);
            AddShopOffer("BlueprintWingsBackgoyle", 750000, InventoryItemType.Block, 4777);
            AddShopOffer("BlueprintJetPackLongJumpExplosive", 500000, InventoryItemType.Block, 4775);
            AddShopOffer("BlueprintWingsIonThrusters", 500000, InventoryItemType.Block, 4760);
            AddShopOffer("BlueprintBackBackpackWanderer", 335000, InventoryItemType.Block, 4746);
            AddShopOffer("KiddieRide", 10000, InventoryItemType.Block, 1129);
            AddShopOffer("DoorClan", 1000, InventoryItemType.Block, 3559);
            AddShopOffer("ClanQuestBot", 7500, InventoryItemType.Block, 3500);
            AddShopOffer("ClanTotem", 9000, InventoryItemType.Block, 3466);
            AddShopOffer("DoorFactionDark", 2000, InventoryItemType.Block, 3602);
            AddShopOffer("DoorFactionLight", 2000, InventoryItemType.Block, 3603);
            AddShopOffer("CheckPointFactionDark", 3500, InventoryItemType.Block, 3598);
            AddShopOffer("CheckPointFactionLight", 3500, InventoryItemType.Block, 3599);
            AddShopOffer("PortalFactionDark", 4000, InventoryItemType.Block, 3600);
            AddShopOffer("DarkPixieWings", 4000, InventoryItemType.Block, 608);
            AddShopOffer("PortalFactionLight", 100, InventoryItemType.Block, 3601);
            AddShopOffer("BattleScoreBoardFaction", 8000, InventoryItemType.Block, 3597);
            AddShopOffer("LockBattleFaction", 7500, InventoryItemType.Block, 3596);
            AddShopOffer("LockWorldBattleFaction", 9000, InventoryItemType.Block, 3606);
            AddShopOffer("KeyWorld", 1000000000, InventoryItemType.Block, 418);
            AddShopOffer("LockDiamond", 1000000000, InventoryItemType.Block, 415);
            AddShopOffer("TokenLock", 1000000000, InventoryItemType.Block, 1135);
            AddShopOffer("Water", 1000000000, InventoryItemType.Block, 110);
            AddShopOffer("PortalEntrance", 1000000000, InventoryItemType.Block, 18);
            AddShopOffer("Suitİnvisable", 1000000000, InventoryItemType.Block, 2096);



            //  Bugged invisable items fix: Not useful items
            AddShopOffer("TODO1", 1000000000, InventoryItemType.Block, 37);
            AddShopOffer("TODO2", 1000000000, InventoryItemType.Block, 38);
            AddShopOffer("TODO3", 1000000000, InventoryItemType.Block, 39);
            AddShopOffer("TODO4", 1000000000, InventoryItemType.Block, 40);
            AddShopOffer("TODO5", 1000000000, InventoryItemType.Block, 41);
            AddShopOffer("TODO6", 1000000000, InventoryItemType.Block, 42);
            AddShopOffer("TODO7", 1000000000, InventoryItemType.Block, 43);
            AddShopOffer("TODO8", 1000000000, InventoryItemType.Block, 44);
            AddShopOffer("TODO9", 1000000000, InventoryItemType.Block, 45);
            AddShopOffer("TODO10", 1000000000, InventoryItemType.Block, 46);
            AddShopOffer("TODO11", 1000000000, InventoryItemType.Block, 47);
            AddShopOffer("TODO12", 1000000000, InventoryItemType.Block, 48);
            AddShopOffer("TODO13", 1000000000, InventoryItemType.Block, 606);
            AddShopOffer("WALLBUG", 1000000000, InventoryItemType.Block, 2048);
            AddShopOffer("WALLBUG1", 1000000000, InventoryItemType.Block, 2049);
            AddShopOffer("WALLBUG2", 1000000000, InventoryItemType.Block, 2050);

        }
    }
}
