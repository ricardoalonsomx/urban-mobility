# Urban Mobility: Modeling with Multi-Agent Systems
This project aims to simulate the urban mobility of a city using **Mesa's Framework for Python** (mounted in a Flask server) for the system's logic and Unity for the 3D modeling, incorporating from 1 to 100 agents (vehicles), each with a specific destination. [Video demonstration.](https://youtu.be/KMveiYLW0GA)

## Table of Contents
1. [City building automation](https://github.com/ricardoalonsomx/urban-mobility/#city-building-automation)
2. [Shortest-path algorithm](https://github.com/ricardoalonsomx/urban-mobility/#shortest-path-algorithm)
3. [Prerequisites](https://github.com/ricardoalonsomx/urban-mobility#prerequisites)
4. [Usage](https://github.com/ricardoalonsomx/urban-mobility#usage)

## City building automation
The city is created based on a [text file](/flask/base.txt). The symbol definition is:
- '^': one squared meter of street pointing to the north.
- 'v': one squared meter of street pointing to the south.
- '>': one squared meter of street pointing to the east.
- '<': one squared meter of street pointing to the west.

- 'S': semaphor in a vertical intersection.
- 's': semaphor in a horizontal intersection.

- '#': normal building, which will be tinted in blue.
- 'D': destination building, which will be tinted in red.

## Shortest-path algorithm
The logic of the system incorporates the Floyd-Warshall algorithm to find the shortest route for each vehicle. The [script](/flask/floyd_warshall.py) has two main functions:
1. floyd: creates the map for the algorithm, and stores it in a [file](/flask/floyd_map.py), so next time the script is executed it won't need to redo the map (unless it detects a change in [the creation file](/flask/base.txt)).
2. floyd-route: finds the shortest route between two coordinates in the map.

## Prerequisites:
- [Python](https://www.python.org/downloads/)
- [Flask](https://flask.palletsprojects.com/en/2.3.x/installation/)
- [Mesa](https://pypi.org/project/Mesa/)

## Usage
1. Download [UrbanMobility](https://github.com/ricardoalonsomx/urban-mobility/releases).
2. Run **FlaskServer.**
3. Run **UrbanMobility**.
