// https://www.codingame.com/training/medium/skynet-revolution-episode-1

using System;
using System.Linq;
using System.Collections.Generic;

class Player
{
    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int totalNodes = int.Parse(inputs[0]);
        int totalLinks = int.Parse(inputs[1]);
        int totalExists = int.Parse(inputs[2]);

        var graph = new Graph(totalNodes);

        for (int i = 0; i < totalLinks; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int node1 = int.Parse(inputs[0]);
            int node2 = int.Parse(inputs[1]);

            graph.AddEdge(node1, node2);
        }
        for (int i = 0; i < totalExists; i++)
        {
            int exitIndex = int.Parse(Console.ReadLine());
            graph.Exits.Add(exitIndex);
        }

        // game loop
        while (true)
        {
            int skynetAgentNode = int.Parse(Console.ReadLine()); // The index of the node on which the Skynet agent is positioned this turn
            SeverLink(graph, skynetAgentNode);
        }
    }
    private static void SeverLink(Graph graph, int skyNetLocation)
    {
        var bfp = new BreadthFirstPaths(graph, skyNetLocation);
        var shortestExit = -1;
        var exitLength = Int32.MaxValue;

        foreach (var exit in graph.Exits)
        {
            if (bfp.HasPathTo(exit))
            {
                var currLen = bfp.PathTo(exit).Count();
                if (currLen < exitLength)
                {
                    exitLength = currLen;
                    shortestExit = exit;
                }
            }
        }

        var targetNode = bfp.PathTo(shortestExit).Skip(1).First();
        graph.RemoveEdge(skyNetLocation, targetNode);
        Console.WriteLine("{0} {1}", skyNetLocation, targetNode);
    }
}

public class Graph
{
    private readonly LinkedList<int>[] _adjacencies;

    public Graph(int vertices)
    {
        Vertices = vertices;
        Edges = 0;
        Exits = new List<int>();
        _adjacencies = new LinkedList<int>[vertices];

        for (var v = 0; v < Vertices; v++)
        {
            _adjacencies[v] = new LinkedList<int>();
        }
    }

    public List<int> Exits { get; }

    public int Vertices { get; }

    public int Edges { get; private set; }

    public IEnumerable<int> Adjacencent(int v) => _adjacencies[v];

    public void AddEdge(int v, int w)
    {
        _adjacencies[v].AddLast(w);
        _adjacencies[w].AddLast(v);
        Edges++;
    }

    public void RemoveEdge(int v, int w)
    {
        _adjacencies[v].Remove(w);
        _adjacencies[w].Remove(v);
    }
}

public class BreadthFirstPaths
{
    private readonly bool[] _marked;
    private readonly int[] _edgeTo;
    private readonly int _source;

    public BreadthFirstPaths(Graph graph, int source)
    {
        _marked = new bool[graph.Vertices];
        _edgeTo = new int[graph.Vertices];
        _source = source;
        Bfs(graph, source);
    }

    private void Bfs(Graph graph, int source)
    {
        _marked[source] = true;

        var queue = new Queue<int>();
        queue.Enqueue(source);
        while (queue.Count > 0)
        {
            var v = queue.Dequeue();
            foreach (var w in graph.Adjacencent(v).Where(w => !_marked[w]))
            {
                _edgeTo[w] = v;
                _marked[w] = true;
                queue.Enqueue(w);
            }
        }
    }

    public bool HasPathTo(int v) => _marked[v];

    public IEnumerable<int> PathTo(int v)
    {
        if (!HasPathTo(v)) return null;

        var path = new Stack<int>();
        for (var x = v; x != _source; x = _edgeTo[x])
        {
            path.Push(x);
        }
        path.Push(_source);

        return path;
    }
}