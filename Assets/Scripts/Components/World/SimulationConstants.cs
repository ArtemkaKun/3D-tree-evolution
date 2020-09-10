﻿namespace Components.World
{
    public struct SimulationConstants
    {
        public int MaxTreeAge { get; set; }
        public int StartSeedEnergy { get; set; }
        public int TreeGenesCount { get; set; }
        public int CellEnergyUsage { get; set; }
        public int StartForestSize { get; set; }

        public SimulationConstants(int maxTreeAge, int startSeedEnergy, int treeGenesCount, int cellEnergyUsage, int startForestSize)
        {
            MaxTreeAge = maxTreeAge;
            StartSeedEnergy = startSeedEnergy;
            TreeGenesCount = treeGenesCount;
            CellEnergyUsage = cellEnergyUsage;
            StartForestSize = startForestSize;
        }
    }
}