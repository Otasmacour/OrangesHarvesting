using System;
using System.Collections.Generic;
using System.IO;

namespace SklizeniPomerancu
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var item = InputPreparation();
            Tree tree = new Tree();
            tree.BuildGraph(item.Item1, item.numberOfNodes);
            foreach(int PočetPomerancuKolikChceKamion in item.orangesToHarvest)
            {
                if(tree.debugMode)
                {
                    Console.Write("Zbytek: ");
                }
                Console.WriteLine(tree.Solve(PočetPomerancuKolikChceKamion));
            }
            Console.ReadLine();
        }
        static (List<(int, int , int)> , int numberOfNodes, List<int> orangesToHarvest) InputPreparation()
        {
            List<(int Node1, int Node2, int numberOfOranges)> values = new List<(int, int, int)>();
            List<string> input = new List<string>();
            using (StreamReader sr = new StreamReader(@"C:\Users\macou\source\repos\SklizeniPomerancu\SklizeniPomerancu\input.txt"))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    input.Add(s);
                }
            }
            int index = 1;
            for (int i = 2; i < int.Parse(input[1])+2; i++)
            {
                index++;
                string line1 = input[i];
                String[] line = line1.Split(' ');
                int node1 = int.Parse(line[0]);
                int node2 = int.Parse(line[1]);
                int numberOfOrangess = int.Parse(line[2]);
                values.Add((node1, node2, numberOfOrangess));
            }
            index++;
            int numberOfOranges = int.Parse(input[index]);
            index++;
            List<int> orangesToHarvest = new List<int>();
            for(int i = 0; i < numberOfOranges; i++)
            {
                orangesToHarvest.Add(int.Parse(input[index]));
                index++;
            }
            return (values, int.Parse(input[0]), orangesToHarvest);
        }
    }
    class Node : INode
    {
        public int index { get; set; }
        public List<INode> adjacent { get; set; }
        public int absoluteValue { get; set; }
        public int orangesNumber { get; set; }
        public INode descendant { get; set; }
        public List<INode> parents { get; set; }
        public List<INode> adjacentParents { get; set; }
    }
    class Tree : ITree
    {
        public bool debugMode = false;
        public List<INode> graphNodes { get; set ; }
        public INode start { get; set; }
        public Dictionary<INode, int> depths { get; set; }
        public void BuildGraph(List<(int Node1, int Node2, int numberOfOranges)> values, int numberOfNodes)
        {
            graphNodes = new List<INode>();
            for (int i = 0; i < numberOfNodes; i++)
            {
                Node vrchol = new Node();
                vrchol.index = i;
                vrchol.adjacent = new List<INode>();
                vrchol.parents = new List<INode>();
                vrchol.adjacentParents = new List<INode>();
                graphNodes.Add(vrchol);
            }
            start = graphNodes[0];
            foreach (var item in values)
            {
                graphNodes[item.Node1].adjacent.Add(graphNodes[item.Node2]);
                graphNodes[item.Node1].adjacent.Add(graphNodes[item.Node2]);
            }
            depths = new Dictionary<INode, int>();
            Queue<INode> queue = new Queue<INode>();
            queue.Enqueue(start);
            depths.Add(start, 0);
            while (queue.Count > 0)
            {
                INode vrchol = queue.Dequeue();
                List<INode> sousedi = vrchol.adjacent;
                foreach (INode soused in sousedi)
                {
                    if (depths.ContainsKey(soused) == false)
                    {
                        depths.Add(soused, depths[vrchol] + 1);
                        queue.Enqueue(soused);
                    }
                }
            }
            foreach (var item in values)
            {
                INode vrchol1 = graphNodes[item.Node1];
                INode vrchol2 = graphNodes[item.Node2];
                if (depths[vrchol1] > depths[vrchol2])
                {
                    vrchol1.orangesNumber = item.numberOfOranges;
                    vrchol2.adjacentParents.Add(vrchol1);
                }
                else
                {
                    vrchol2.orangesNumber = item.numberOfOranges;
                    vrchol1.adjacentParents.Add(vrchol2);
                }
            }
            AssignPArentsToNodes();
        }
        public int Solve(int theNumberOfOrangesRequested)
        {
            if (debugMode) { Console.WriteLine("Chce tolik pomerančů: " + theNumberOfOrangesRequested.ToString()); }
            UpdateAbsoluteValuesOfAllNodes();
            if(NumberOfOrangesOnTheWholeGraph() < theNumberOfOrangesRequested)
            {
                if(debugMode) { Console.WriteLine("Počet všech pomerančů na stromě: "+NumberOfOrangesOnTheWholeGraph()); }
                return -1;
            }
            bool itIsGoingToBeEasy = false;
            foreach (INode vrchol in graphNodes)
            {
                if (vrchol.absoluteValue == theNumberOfOrangesRequested)
                {
                    if (debugMode){Console.WriteLine("Sklidit: " + vrchol.index.ToString());}
                    itIsGoingToBeEasy = true;
                    HarvestNodes(new List<INode> { vrchol });
                    return 0;
                }
            }
            if(itIsGoingToBeEasy == false)
            {
                List<List<INode>> combinations = CombinationsOfAllTheNumbersThatCanBeAddedUp();
                List<INode> nodesToHarvest = selectNodesToHarvest(theNumberOfOrangesRequested, combinations);
                int sumOfAbsoluteValuesOfNodes = SumOfAbsoluteValuesOfNodes(nodesToHarvest);
                HarvestNodes(nodesToHarvest);
                if (debugMode)
                {
                    Console.WriteLine("Počet kombinací: " + combinations.Count);
                    Console.WriteLine("Vrcholy vhodné ke sklizení:");
                    foreach (INode vrcgol in nodesToHarvest)
                    {
                        Console.WriteLine(vrcgol.index.ToString());
                    }
                }
                return (sumOfAbsoluteValuesOfNodes - theNumberOfOrangesRequested);
            }
            return 0;
        }      
        public List<INode> selectNodesToHarvest(int theNumberOfOrangesRequested, List<List<INode>> allCorrectCombinations)
        {
            List<INode> result = new List<INode>();
            foreach(List<INode> combinations in allCorrectCombinations)
            {
                if(SumOfAbsoluteValuesOfNodes(combinations) == theNumberOfOrangesRequested)
                {
                    return combinations;
                }
                else if(result.Count == 0 && SumOfAbsoluteValuesOfNodes(combinations) >= theNumberOfOrangesRequested)
                {
                    result = combinations;
                }
                else if (result.Count > 0 && SumOfAbsoluteValuesOfNodes(combinations) >= theNumberOfOrangesRequested && SumOfAbsoluteValuesOfNodes(combinations) < SumOfAbsoluteValuesOfNodes(result))
                {
                    result = combinations;
                }
            }
            return result;
        }
        public void HarvestNodes(List<INode> nodesToHarvest)
        {
            foreach (INode node in nodesToHarvest)
            {
                node.orangesNumber = 0;
                node.absoluteValue = 0;
                foreach (INode parent in node.parents)
                {
                    parent.orangesNumber = 0;
                    node.absoluteValue = 0;
                }
            }
        }
        public int SumOfAbsoluteValuesOfNodes(List<INode> nodesToAddUp)
        {
            int sum = 0;
            foreach(INode node in nodesToAddUp)
            {
                sum += node.absoluteValue;
            }
            return sum;
        }
        public  List<List<INode>> CreateAllCombinations(List<INode> nodes)
        {
            List<List<INode>> result = new List<List<INode>>();
            for (int i = 1; i <= nodes.Count; i++)
            {
                var combinationsOfThatLength = CreateCombinations(nodes, i);
                result.AddRange(combinationsOfThatLength);
            }
            return result;
        }
        public List<List<INode>> CreateCombinations(List<INode> nodes, int lenght)
        {
            List<List<INode>> combination = new List<List<INode>>();
            Combination(nodes, 0, lenght, new List<INode>(), combination);
            List<List<INode>> combinationsToRemove = new List<List<INode>>();
            foreach(List<INode> nodesOfTheCombination in combination)
            {
                bool ok = true;
                foreach(INode node1 in nodesOfTheCombination)
                {
                    foreach (INode node2 in nodesOfTheCombination)
                    {
                        if (node1.parents.Contains(node2) == true || node2.parents.Contains(node1) == true)
                        {
                            ok = false;
                        }
                    }
                }
                if(ok == false)
                {
                    combinationsToRemove.Add(nodesOfTheCombination);
                }
            }
            foreach(var item in  combinationsToRemove)
            {
                combination.Remove(item);
            }
            return combination;
        }
        static void Combination(List<INode> nodes, int index, int length, List<INode> currentCombination, List<List<INode>> allCombinations)
        {
            if (length == 0)
            {
                allCombinations.Add(new List<INode>(currentCombination));
                return;
            }
            for (int i = index; i < nodes.Count; i++)
            {
                currentCombination.Add(nodes[i]);
                Combination(nodes, i + 1, length - 1, currentCombination, allCombinations);
                currentCombination.RemoveAt(currentCombination.Count - 1);
            }
        }
        public void AssignPArentsToNodes()
        {
            foreach(INode adjacentNode in start.adjacent)
            {
                ParentsOfNodes(adjacentNode);
            }
        }
        public void UpdateAbsoluteValuesOfAllNodes()
        {
            foreach (INode adjacentNode in start.adjacent)
            {
                AbsoluteValueOfParent(adjacentNode);
            }
            if(debugMode)
            {
                Console.WriteLine("Absolutní hodnoty vrcholů:");
                foreach (INode node in graphNodes)
                {
                    Console.WriteLine(node.index.ToString() + ": " + node.absoluteValue);
                }
            }      
        }
        public List<INode> ParentsOfNodes(INode node)
        {
            List<INode> parents = new List<INode>();
            foreach (INode adjacentNode in node.adjacent)
            {
                if (depths[adjacentNode] > depths[node])
                {
                    if (adjacentNode.adjacent.Count == 1)
                    {
                        parents.Add(adjacentNode);
                    }
                    else
                    {
                        List<INode> parentsOfAdjacentNode = ParentsOfNodes(adjacentNode);
                        parents.AddRange(parentsOfAdjacentNode);
                        parents.Add(adjacentNode);
                    }
                }
            }
            node.parents = parents;
            return parents;
        }     
        public int AbsoluteValueOfParent(INode node)
        {
            if(node.parents.Count == 0)
            {
                node.absoluteValue = node.orangesNumber;
                return node.absoluteValue;
            }
            else
            {
                int valueOfParent = 0;
                foreach(INode parent in node.adjacentParents)
                {
                    valueOfParent += AbsoluteValueOfParent(parent);
                }
                node.absoluteValue = valueOfParent + node.orangesNumber;
                return node.absoluteValue;
            }
        }
        public int NumberOfOrangesOnTheWholeGraph()
        {
            int number = 0;
            foreach(INode node in start.adjacentParents)
            {
                if(node != start)
                {
                    number += node.absoluteValue;
                }
            }
            if (debugMode) { Console.WriteLine("Hodnota všech je: " + number); }
            return number;
        }
        public List<List<INode>> CombinationsOfAllTheNumbersThatCanBeAddedUp()
        {
            List<List<INode>> allCombination = new List<List<INode>>();
            foreach (INode node in graphNodes)
            {
                if (node != start)
                {
                    List<INode> nodesToCombinations = new List<INode>();
                    foreach (INode nodeToCombination in graphNodes)
                    {
                        if (nodeToCombination == start)
                        {
                        }
                        else if (nodeToCombination == node)
                        {
                            nodesToCombinations.Add(nodeToCombination);
                        }
                        else if (node.parents.Contains(nodeToCombination) == false && nodeToCombination.parents.Contains(node) == false)
                        {
                            nodesToCombinations.Add(nodeToCombination);
                        }
                    }
                    List<List<INode>> combinations = CreateAllCombinations(nodesToCombinations);
                    allCombination.AddRange(combinations);
                }
            }
            return allCombination;
        }
    }
    interface INode
    {
        int index { get; set; }
        List<INode> adjacent { get; set; }
        int absoluteValue { get; set; }
        int orangesNumber { get; set; }
        INode descendant { get; set; }
        List<INode> parents { get; set; }
        List<INode> adjacentParents { get; set; }
    }
    interface ITree
    {
        List <INode> graphNodes { get; set; }
        INode start { get; set; }
        Dictionary<INode, int> depths { get; set; }
        void BuildGraph(List<(int Node1, int Node2, int numberOfOranges)> values, int numberOfNodes);
        int Solve(int theNumberOfOrangesRequested);
        List<INode> selectNodesToHarvest(int theNumberOfOrangesRequested, List<List<INode>> allCorrectCombinations);
        void HarvestNodes(List<INode> nodesToHarvest);
        int SumOfAbsoluteValuesOfNodes(List<INode> nodesToAddUp);
        List<List<INode>> CreateAllCombinations(List<INode> nodes);
        List<List<INode>> CreateCombinations(List<INode> nodes, int length);
        void AssignPArentsToNodes();
        void UpdateAbsoluteValuesOfAllNodes();
        List<INode> ParentsOfNodes(INode node);
        int AbsoluteValueOfParent(INode node);
        int NumberOfOrangesOnTheWholeGraph();
    }
}