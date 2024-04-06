using System;
using System.Collections.Generic;
using System.Text;
using AngkorWat.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.ShortestPath;

internal class Dijkstra
{
    internal static List<string> Run(UniverseState state, string from, string to)
    {
        // Создание графа
        var graph = new AdjacencyGraph<string, Edge<string>>();

        var edgeWeights = new Dictionary<Edge<string>, double>();

        foreach (var edgeArray in state.Routes)
        {
            string source = edgeArray[0];
            string target = edgeArray[1];
            double weight = double.Parse(edgeArray[2]);

            if (!graph.ContainsVertex(source))
                graph.AddVertex(source);

            if (!graph.ContainsVertex(target))
                graph.AddVertex(target);

            var edge = new Edge<string>(source, target);

            graph.AddEdge(edge);
            edgeWeights.Add(edge, weight);
        }

        string sourceVertex = from;
        string targetVertex = to;

        // Поиск кратчайшего пути
        var shortestPaths = graph.ShortestPathsDijkstra(edge => edgeWeights[edge], sourceVertex);

        shortestPaths(targetVertex, out IEnumerable<Edge<string>> shortestPathEdges);

        // Форматирование и вывод кратчайшего пути
        //Console.WriteLine("Shortest Path from " + sourceVertex + " to " + targetVertex + ":");
        //foreach (var edge in shortestPathEdges)
        //{
        //    Console.WriteLine(edge.Source + " -> " + edge.Target);
        //}

        return shortestPathEdges.Select(e => e.Target).ToList();
    }

    internal static Dictionary<string, int> RunAllFromSource(Data data, string from)
    {
        var ret = new Dictionary<string, int>();

        // Создание графа
        var graph = new AdjacencyGraph<string, Edge<string>>();
        var edgeWeights = new Dictionary<Edge<string>, double>();
        foreach (var edgeArray in data.Routes)
        {
            string source = edgeArray.LocationFrom.Name;
            string target = edgeArray.LocationTo.Name;
            double weight = edgeArray.Cost;

            if (!graph.ContainsVertex(source))
                graph.AddVertex(source);

            if (!graph.ContainsVertex(target))
                graph.AddVertex(target);

            var edge = new Edge<string>(source, target);

            graph.AddEdge(edge);
            edgeWeights.Add(edge, weight);
        }

        var shortestPaths = graph.ShortestPathsDijkstra(edge => edgeWeights[edge], from);

        foreach (var vertex in graph.Vertices)
        {
            if (from == vertex)
            {
                // Поиск кратчайшего пути
                continue;
            }

            shortestPaths(vertex, out var troute);

            var weight = troute.Sum(edge => edgeWeights[edge]);

            ret.Add(vertex, (int)Math.Round(weight));
        }

        return ret;
    }
}