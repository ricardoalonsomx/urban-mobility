# Urban Mobility: Modeling with Multi-Agent Systems
This project aims to simulate the urban mobility of a city using **Mesa's Framework for Python** (mounted in a Flask server) for the system's logic and Unity for the 3D modeling, incorporating from 1 to 100 agents (vehicles), each with a specific destination.

## City building automation
The city is created based on a text file ([base.txt](/Flask/base.txt)).

## Shortest-path algorithm
The logic of our system incorporates the Floyd-Warshall algorithm to find the shortest route for each vehicle.
