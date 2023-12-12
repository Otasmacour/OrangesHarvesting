# What this problem is about?
There's a tree that produces oranges, on every branch (edge, in chart parlance). The input tree is given with the number of nodes, edges, and oranges between them. Next, the input is followed by requirements in the form of numbers that express how many oranges need to be procured (harvested from the tree). Of course, one cannot harvest just any proportion of the oranges on a particular node. One can only ever harvest all the oranges on some node. So, in short, the whole problem is about how to harvest the tops of the tree so that as few oranges as possible are harvested in excess and, if possible at the time, so that exactly the number of oranges required by the requirement are harvested. The output of the program for n number of requests is to be n lines with numbers indicating how the request turned out. Either 0 for the exact number of oranges that could be harvested, any integer Z denoting the smallest possible remainder that was required to harvest the number of oranges specified in the request, or -1 when the request could not be fulfilled (a situation where there are no more oranges on the tree, even in the total, than the number specified in the request).
And a few more important rules, you can only harvest nodes "from above", of course you can harvest multiple nodes at once for one requirement, leading behind each other and ending at the top with a peak that no longer has any nodes with oranges above it, but you can't just harvest a node somewhere in the middle of a tree that has unharvested nodes above it, that would then make the task a lot easier.
# Example of correct input
```txt
12 - number of the nodes
11 - number of the edges
0 1 1 - the edge from node 0 to node 1, that contains 1 orange.
2 8 7 - the edge from node 2 to node 8, that contains 7 orange.
1 4 1 etce...
1 3 1........
1 2 1........
4 5 1........
4 6 1........
4 7 1........
3 11 1.......
2 10 1.......
2 9 1........
3 - number of the request
15 - request for 15 oranges
2 - request for 2 oranges
3 - request for 3 oranges
```
# Visual representation of graph, built from the above input
![20231209_203808](https://github.com/Otasmacour/SklizeniPomerancu/assets/111227700/d29a94e9-ab33-42c9-b12e-0256ee3565cf)
- The blue numbers mark the indexes of the nodes  
- The orange numbers represent the number of orange on each node. Why on the nodes instead of edges, in a way that makes sense given the format of the input. I figured that the given number of oranges is on the one of the two given edges that is further away from the starting node, (the root of the whole tree logically won't have any oranges on it).
- Finally, the red numbers in parentheses are the absolute values of the nodes. This is not an absolute value as you might think. The absolute value of a node is the number of oranges that would fall down the tree if a given node were "chopped off".
# Output to the above input
```txt
0 - no remainder, it harvested nodes number 2, 4, 5, 6, 7, 8, 9, 10 and 11
0 - again, no remainder, it harvested nodes number 1 and 3
-1 - because by this time all nodes have been harvested.
```
