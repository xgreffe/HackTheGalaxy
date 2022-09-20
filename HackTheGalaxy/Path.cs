namespace HackTheGalaxy
{
    internal class Path
    {
        public List<Node> nodes;

        public Path(List<Node> nodes)
        {
            this.nodes = nodes;
        }

        public void Solve()
        {
            var start = this.nodes.First();
            for(var i = 0; i < nodes.Count(); i++)
            {
                var node = nodes[i];
                node.SetValue(start.value + i);
            }
        }

        public override string ToString()
        {
            return String.Join(" -> ", nodes);
        }
    }
}
