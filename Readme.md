# Urban Mobility: Modeling with Multi-Agent Systems
This project aims to simulate the urban mobility of a city using **Mesa's Framework for Python** (mounted in a Flask server) for the system's logic and Unity for the 3D modeling, incorporating from 1 to 100 agents (vehicles), each with a specific destination.

## City building automation
The city is created based on a text file ([base.txt](/Flask/base.txt)). The symbol definition is:
'^': one squared meter of street pointing to the north.
'v': one squared meter of street pointing to the north.
'>': one squared meter of street pointing to the east.
'<': one squared meter of street pointing to the west.

'S': semaphor in a vertical intersection.
's': semaphor in a horizontal intersection.

'#': normal building, which will be tinted in blue.
'D': destination building, which will be tinted in red.

## Shortest-path algorithm
The logic of our system incorporates the Floyd-Warshall algorithm to find the shortest route for each vehicle.
