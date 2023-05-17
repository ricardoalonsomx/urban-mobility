# Urban Mobility: Modeling with Multi-Agent Systems
This project aims to simulate the urban mobility of a city using **Mesa's Framework for Python** (mounted in a Flask server) for the system's logic and Unity for the 3D modeling, incorporating from 1 to 100 agents (vehicles), each with a specific destination. [Video demonstration.](https://youtu.be/I9mrDMuF6wc)

## City building automation
The city is created based on a [text file](/Flask/base.txt). The symbol definition is:
- '^': one squared meter of street pointing to the north.
- 'v': one squared meter of street pointing to the north.
- '>': one squared meter of street pointing to the east.
- '<': one squared meter of street pointing to the west.

- 'S': semaphor in a vertical intersection.
- 's': semaphor in a horizontal intersection.

- '#': normal building, which will be tinted in blue.
- 'D': destination building, which will be tinted in red.

## Shortest-path algorithm
The logic of the system incorporates the Floyd-Warshall algorithm to find the shortest route for each vehicle. The [script](/Flask/floyd_warshall.py) has two main functions:
1. floyd: creates the map for the algorithm, and stores it in a [file](/Flask/floyd_map.py), so next time the script is executed it won't need to redo the map (unless it detects a change in [the creation file](/Flask/base.txt)).
2. floyd-route: finds the shortest route between two coordinates in the map.
