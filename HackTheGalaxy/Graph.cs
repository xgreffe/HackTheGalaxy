using System.Diagnostics;

namespace HackTheGalaxy
{
    internal class Graph
    {
        List<Node> nodes = new List<Node>();
        List<Edge> edges = new List<Edge>();
        List<Path> allPaths = new List<Path>();
        List<List<Path>> myPaths = new List<List<Path>>();

        public void Load(string[] input)
        {
            LoadNodes(input);

            LoadEdges(input);
        }

        private void LoadNodes(string[] input)
        {
            foreach (var node in input)
            {
                var valueString = String.Join("", node.Where(Char.IsDigit));
                var value = int.Parse(String.IsNullOrEmpty(valueString) ? "0" : valueString);

                int index = nodes.Count();
                var r = index / 8;
                var c = index % 8;

                nodes.Add(new Node(c, r, value));
            }
        }

        private void LoadEdges(string[] input)
        {
            foreach (var item in input.Select((value, i) => new { i, value }))
            {
                var inputNode = item.value;
                var index = item.i;

                var pos = new int[] { 0, 0 };
                var direction = String.Join("", inputNode.Where(Char.IsLetter));

                if (direction.Contains("U"))
                {
                    pos[1] = -1;
                }
                if (direction.Contains("D"))
                {
                    pos[1] = 1;
                }

                if (direction.Contains("L"))
                {
                    pos[0] = -1;
                }
                if (direction.Contains("R"))
                {
                    pos[0] = 1;
                }

                var node = nodes[index];
                if (pos.Any(v => v == 1 || v == -1))
                {
                    var currentRow = index / 8;
                    var currentCol = index % 8;

                    int row = currentRow + pos[1];
                    int col = currentCol + pos[0];
                    var nextIndex = row * 8 + col;

                    while (true)
                    {
                        var nodeA = node;
                        var nodeB = nodes[nextIndex];

                        if (((nodeA.value == 0) && (!nodes.Any(n => n.value == nodeB.value - 1))) || (nodeA.value != 0) && ((nodeB.value == 0) || (nodeB.value == nodeA.value + 1)))
                        {
                            this.edges.Add(new Edge(nodeA, nodeB));
                        }

                        row += pos[1];
                        col += pos[0];
                        nextIndex = row * 8 + col;

                        if (nextIndex < 0 || nextIndex >= nodes.Count() || col < 0 || col > 7)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void PrintNodes()
        {
            for(var i = 0; i < nodes.Count(); i++)
            {
                var node = nodes[i];

                if (i % 8 == 0)
                {
                    Console.WriteLine();
                }
                Console.Write($"{node}\t");
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        public List<List<Path>> Combine(IEnumerable<List<Path>> paths)
        {
            List<List<Path>> combos = new List<List<Path>>() { new List<Path>() };
            foreach (var inner in paths)
            {
                combos = combos.SelectMany(r => inner
                .Select(x => {
                    var n = new List<Path>(r);
                    if (x != null)
                    {
                        n.Add(x);
                    }
                    return n;
                })).ToList();
            }

            return combos;
        }

        public void PruneEdges()
        {
            var solvedNodes = this.nodes.Where(n => n.value != 0);

            this.edges.RemoveAll(e => e.nodeA.value != 0 && e.nodeB.value != 0 && e.nodeA.value + 1 != e.nodeB.value);
            this.edges.RemoveAll(e => solvedNodes.Any(n => n.value == e.nodeB.value - 1) && e.nodeA.value == 0);
            this.edges.RemoveAll(e => solvedNodes.Any(n => n.value == e.nodeA.value) && solvedNodes.Any(n => n.value == e.nodeA.value + 1) && e.nodeB.value == 0);
        }

        public List<List<Path>> Prune(IEnumerable<List<Path>> paths)
        {
            return paths.Where(c =>
            {
                var pathNodes = c.SelectMany(p => p.nodes).Distinct();
                return pathNodes.Count() - 1 == pathNodes.Last().value - pathNodes.First().value;
            }).ToList();
        }

        public List<List<Path>> FindPath(int start, int end)
        {
            var solvedNodes = nodes.Where(n => n.value != 0).OrderBy(n => n.value);

            myPaths.Clear();

            var first = solvedNodes.ElementAt(start);
            var second = solvedNodes.ElementAt(end);

            allPaths.Clear();

            printAllPaths(first, second);
            myPaths.Add(new List<Path>(allPaths));

            return myPaths;
        }

        public void Solve()
        {
            IEnumerable<Node> solvedNodes;

            List<List<Path>> finalList = new List<List<Path>>();

            while (true)
            {
                solvedNodes = nodes.Where(n => n.value != 0).OrderBy(n => n.value);
                finalList.Clear();
                for (var i = 0; i < solvedNodes.Count() - 1; i += 1)
                {
                    finalList.AddRange(FindPath(i, i + 1));
                }

                var solvablePaths = finalList.Where(c => c.Count() == 1 && c.Any(p => p.nodes.Any(nodes => nodes.value == 0))).SelectMany(c => c);

                if (!solvablePaths.Any()) { break; }

                foreach (var p in solvablePaths)
                {
                    p.Solve();
                }
                PruneEdges();
            }

            solvedNodes = nodes.Where(n => n.value != 0).OrderBy(n => n.value);
            finalList.Clear();
            for (var i = 0; i < solvedNodes.Count() - 1; i += 1)
            {
                finalList.AddRange(FindPath(i, i + 1));
            }

            Path finalPath = Merge(finalList);

            finalPath.Solve();

            PrintNodes();
        }

        public Path Merge(List<List<Path>> list)
        {
            if (list.Count() == 1) { return list.First().First(); }

            List<List<Path>> prunedList = new List<List<Path>>();

            if (list.All(c => c.Count() == 1))
            {
                return new Path(list.SelectMany(c => c.SelectMany(p => p.nodes)).Distinct().ToList());
            }
            else {
                var best = list.OrderBy(l => l.Count()).Last();
                var last = list.Last();

                if (best == last)
                {
                    list.Reverse();
                    best = list.Skip(1).Take(1).First();
                    list.Reverse();
                }
                var shouldTake = list.Skip(list.IndexOf(best)).Take(2);

                var pruned = Prune(Combine(shouldTake)).Select(c => new Path(c.SelectMany(p => p.nodes).Distinct().ToList())).ToList();
                prunedList.Add(pruned);
                prunedList.AddRange(list.Except(shouldTake));
                prunedList = prunedList.OrderBy(l => l.First().nodes.First().value).ToList();
            }

            return Merge(prunedList);
        }

        public void printAllPaths(Node s, Node d)
        {
            List<Node> isVisited = new List<Node>();
            List<Node> pathList = new List<Node>();

            // add source to path[]
            pathList.Add(s);

            // Call recursive utility
            printAllPathsUtil(s, d, isVisited, pathList);
        }

        private void printAllPathsUtil(Node s, Node d,
                                       List<Node> isVisited,
                                       List<Node> localPathList)
        {
            var first = localPathList.First();

            if (s == d)
            {
                if (localPathList.Count() - 1 != d.value - first.value)
                {
                    return;
                }

                allPaths.Add(new Path(new List<Node>(localPathList)));
                // if match found then no need
                // to traverse more till depth
                return;
            }

            if (localPathList.Count() > d.value - first.value)
            {
                return;
            }

            if (s.value != 0 && localPathList.Count() > s.value)
            {
                return;
            }

            if ((s == first) || (s.value == 0) || (s.value == first.value + localPathList.Count())) {

                // Mark the current node
                isVisited.Add(s);

                // Recur for all the vertices
                // adjacent to current vertex
                foreach (var edge in edges.Where(e => e.nodeA == s))
                {
                    if (!isVisited.Contains(edge.nodeB))
                    {
                        // store current node
                        // in path[]
                        localPathList.Add(edge.nodeB);
                        printAllPathsUtil(edge.nodeB, d, isVisited,
                                          localPathList);

                        // remove current node
                        // in path[]
                        localPathList.Remove(edge.nodeB);
                    }
                }

                // Mark the current node
                isVisited.Remove(s);
            }
        }
    }
}
