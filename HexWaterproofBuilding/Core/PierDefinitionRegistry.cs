using HexWaterproofBuilding.Models;
using System;
using System.Collections.Generic;

namespace HexWaterproofBuilding.Core
{
    internal static class PierDefinitionRegistry
    {
        private static readonly Dictionary<string, PierDefinition> Definitions = 
            new Dictionary<string, PierDefinition>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "hex_pier_log_4_vertical",
                    new PierDefinition("hex_pier_log_4_vertical", 4f, -4f, 0.6f, 0f)
                },
                {
                    "hex_pier_stone_pillar",
                    new PierDefinition("hex_pier_stone_pillar", 2f, -3f, 0.6f, -1f)
                }
            };

        internal static PierDefinition Get(string prefabName)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                return null;
            }

            PierDefinition definition;

            if (Definitions.TryGetValue(prefabName, out definition))
            {
                return definition;
            }

            return null;
        }
    }
}