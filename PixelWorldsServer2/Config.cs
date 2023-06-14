using BasicTypes;
using PixelWorldsServer2.DataManagement;
using PixelWorldsServer2.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PixelWorldsServer2
{
    public enum BlockDirection
    {
        // Token: 0x04001BBF RID: 7103
        None,
        // Token: 0x04001BC0 RID: 7104
        Center,
        // Token: 0x04001BC1 RID: 7105
        Up,
        // Token: 0x04001BC2 RID: 7106
        Right,
        // Token: 0x04001BC3 RID: 7107
        Down,
        // Token: 0x04001BC4 RID: 7108
        Left,
        // Token: 0x04001BC5 RID: 7109
        END_OF_ENUM
    }

    public class KeyTriple<T1, T2, T3>
    {
        public T1 Key { get; set; }
        public T2 Value1 { get; set; }
        public T3 Value2 { get; set; }

        public KeyTriple(T1 key, T2 value1, T3 value2)
        {
            Key = key;
            Value1 = value1;
            Value2 = value2;
        }
    }
    public class Config
    {
        public static readonly int playersWhoHaveAccessToLockMaxAmount = 30;
        public static readonly int maxRecentWorlds = 30;

        public static WorldInterface.BlockType getOrbBlockType(int orb)
        {
            WorldInterface.LayerBackgroundType orba = (WorldInterface.LayerBackgroundType)orb;
            switch (orba)
            {
                case WorldInterface.LayerBackgroundType.ForestBackground:
                    return WorldInterface.BlockType.OrbForestBackground;
                case WorldInterface.LayerBackgroundType.NightBackground:
                    return WorldInterface.BlockType.OrbNightBackground;
                case WorldInterface.LayerBackgroundType.SpaceBackground:
                    return WorldInterface.BlockType.OrbSpaceBackground;
                case WorldInterface.LayerBackgroundType.DesertBackground:
                    return WorldInterface.BlockType.OrbDesertBackground;
                case WorldInterface.LayerBackgroundType.IceBackground:
                    return WorldInterface.BlockType.OrbIceBackground;
                case WorldInterface.LayerBackgroundType.StarBackground:
                    return WorldInterface.BlockType.OrbStarBackground;
                case WorldInterface.LayerBackgroundType.CandyBackground:
                    return WorldInterface.BlockType.OrbCandyBackground;
                case WorldInterface.LayerBackgroundType.HalloweenTowerBackground:
                    return WorldInterface.BlockType.OrbHalloweenTowerBackground;
                case WorldInterface.LayerBackgroundType.CemeteryBackground:
                    return WorldInterface.BlockType.OrbCemeteryBackground;
                case WorldInterface.LayerBackgroundType.NetherBackground:
                    return WorldInterface.BlockType.OrbNetherBackground;
                case WorldInterface.LayerBackgroundType.CityBackground:
                    return WorldInterface.BlockType.OrbCityBackground;
                case WorldInterface.LayerBackgroundType.BlueSkyBackground:
                    return WorldInterface.BlockType.OrbBlueSkyBackground;
                case WorldInterface.LayerBackgroundType.JetRaceBackground:
                    return WorldInterface.BlockType.OrbJetRaceBackground;

                default:
                    return WorldInterface.BlockType.None;
            }
        }
        public static WorldInterface.BlockType getWeatherBlockType(int weather)
        {
            WorldInterface.WeatherType weth = (WorldInterface.WeatherType)weather;
            switch (weth)
            {
                case WorldInterface.WeatherType.None:
                    return WorldInterface.BlockType.OrbWeatherNone;
                case WorldInterface.WeatherType.HeavyRain:
                    return WorldInterface.BlockType.OrbWeatherHeavyRain;
                case WorldInterface.WeatherType.PixelTrail:
                    return WorldInterface.BlockType.OrbWeatherPixelTrail;
                case WorldInterface.WeatherType.SandStorm:
                    return WorldInterface.BlockType.OrbWeatherSandStorm;
                case WorldInterface.WeatherType.LightRain:
                    return WorldInterface.BlockType.OrbWeatherLightRain;
                case WorldInterface.WeatherType.LightSnow:
                    return WorldInterface.BlockType.OrbWeatherLightSnow;
                case WorldInterface.WeatherType.SnowStorm:
                    return WorldInterface.BlockType.OrbWeatherSnowStorm;
                case WorldInterface.WeatherType.DeepNether:
                    return WorldInterface.BlockType.OrbWeatherDeepNether;
                case WorldInterface.WeatherType.Halloween:
                    return WorldInterface.BlockType.OrbWeatherHalloween;
                case WorldInterface.WeatherType.HalloweenTower:
                    return WorldInterface.BlockType.OrbWeatherHalloweenTower;
                case WorldInterface.WeatherType.Hearts:
                    return WorldInterface.BlockType.OrbWeatherHearts;
                case WorldInterface.WeatherType.Mining:
                    return WorldInterface.BlockType.OrbWeatherMining;
                case WorldInterface.WeatherType.AuroraBorealis:
                    return WorldInterface.BlockType.OrbWeatherAuroraBorealis;
                case WorldInterface.WeatherType.Armageddon:
                    return WorldInterface.BlockType.OrbWeatherArmageddon;
                case WorldInterface.WeatherType.END_OF_ENUM:
                    return WorldInterface.BlockType.None;

                default:
                    return WorldInterface.BlockType.None;
            }
        }

        public static (float, float) ConvertMapPointToWorldPoint(int x, int y, int tileSizeX = 80, int tileSizeY = 60)
        {
            return ((float)x * tileSizeX, (float)y * tileSizeY);
        }

        public static (float, float) ConvertPlayerMapPointToWorldPoint(int x, int y, int tileSizeX = 80, int tileSizeY = 60)
        {
            return ((float)x * tileSizeX, (float)y * tileSizeY - tileSizeY * 0.5f);
        }


        public static (int, int) ConvertWorldPointToMapPoint(float x, float y, int tileSizeX = 80, int tileSizeY = 60)
        {
            return ((int)((x + tileSizeX * 0.5f) / tileSizeX), (int)((y + tileSizeY * 0.5f) / tileSizeY));
        }
        public static BlockDirection GetBlockDirection(WorldInterface.BlockType blockType)
        {
            return Config.defaultBlockDirection;
        }
        private static BlockDirection defaultBlockDirection = BlockDirection.None;

        public static int BlockTypeAndInventoryItemTypeToInt(WorldInterface.BlockType blockType, InventoryItemType inventoryItemType)
        {
            return (int)((WorldInterface.BlockType)((int)inventoryItemType << 24) | blockType);
        }
        public static InventoryKey IntToInventoryKey(int asInt)
        {
            return new InventoryKey((WorldInterface.BlockType)(asInt & 16777215), (InventoryItemType)(asInt >> 24));
        }
    }
}


