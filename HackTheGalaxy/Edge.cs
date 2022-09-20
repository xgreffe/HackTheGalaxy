namespace HackTheGalaxy
{
    internal class Edge
    {
        public Node nodeA;
        public Node nodeB;

        public Edge(Node nodeA, Node nodeB)
        {
            this.nodeA = nodeA;
            this.nodeB = nodeB;
        }

        public override string ToString()
        {
            return $"{nodeA} -> {nodeB}";
        }
    }
}
