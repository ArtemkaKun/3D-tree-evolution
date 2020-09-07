namespace Components.World
{
    public struct SimulationStatus
    {
        public int WorldAge { get; set; }
        public int TreeGeneration { get; set; }
        public int ForestSize { get; set; }
        
        public bool IsSimulationRun { get; set; }

        public SimulationStatus(int worldAge, int treeGeneration, int forestSize, bool isSimulationRun)
        {
            WorldAge = worldAge;
            TreeGeneration = treeGeneration;
            ForestSize = forestSize;
            IsSimulationRun = isSimulationRun;
        }
    }
}