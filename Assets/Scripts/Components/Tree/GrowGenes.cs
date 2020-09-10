namespace Components.Tree
{
    public struct GrowGenes
    {
        public int Up { get; }
        public int Down { get; }
        public int Forward { get; }
        public int Back { get; }
        public int Left { get; }
        public int Right { get; }

        public GrowGenes(int up, int down, int forward, int back, int left, int right)
        {
            Up = up;
            Down = down;
            Forward = forward;
            Back = back;
            Left = left;
            Right = right;
        }
    }
}