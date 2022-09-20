using System.Linq;

namespace HackTheGalaxy
{
    internal class Node
    {
        public int value;
        public int x;
        public int y;

        public Node(int x, int y, int value = 0)
        {
            this.value = value;
            this.x = x;
            this.y = y;
        }

        public void SetValue(int value)
        {
            if (this.value == 0)
            {
                this.value = value;
            }
        }

        public override string ToString()
        {
            return $"{value.ToString("00")}";
        }
    }
}
