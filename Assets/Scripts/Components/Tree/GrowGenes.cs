namespace Components.Tree
{
    public struct GrowGenes
    {
        public int Up;
        public int Down;
        public int Forward;
        public int Back;
        public int Left;
        public int Right;

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