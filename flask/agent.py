from types import CodeType
from mesa import Agent

class Road(Agent):
    """
    Obstacle agent. Just to add obstacles to the grid.
    """
    def __init__(self, unique_id, model, direction= "Left"):
        super().__init__(unique_id, model)
        self.direction = direction
        self.streetID = -1 # -1: no asignado, -2: cruce, >= 0: calle

    def step(self):
        pass

class Car(Agent):
    """
    Agent that moves randomly.
    Attributes:
        unique_id: Agent's ID 
        direction: Randomly chosen direction chosen from one of eight directions
    """
    def __init__(self, unique_id, model, init_pos):
        """
        Creates a new random agent.
        Args:
            unique_id: The agent's ID
            model: Model reference for the agent
        """
        #coordenada del destino al que debe moverse
        
        #solo para la clase de agente mesa
        super().__init__(unique_id, model)
        #destino al que llegar
        self.destination = self.model.destinys[self.random.randrange(0,len(self.model.destinys))]
        
        #obstaculo que tiene enfrente o si esta disponible
        self.cond = ""

        #direccion cardinal a la que se debera mover -> derecha izq abajo arriba
        self.dirCard = ""
        self.direction = "Up"
        self.obstacles = []
        self.coordsDirs = {}
        self.state = "Moving" # 1. Moving 2. Static
        self.route = self.model.searchRoute(init_pos, self.model.getClosestRoad(self.destination.pos))
        self.route.append(self.destination.pos)

        for a in self.model.grid.get_cell_list_contents(init_pos):
            if isinstance(a, Road) or isinstance(a, Traffic_Light):
                self.direction = a.direction

        # print("Route for car at", init_pos, "is", self.route)


    def move(self):
        """ 
        Determines if the agent can move in the direction that was chosen
        """
        directionCoords = {
            "Right": [(1, 1), (1, 0), (1, -1)],
            "Left": [(-1, 1), (-1,0), (-1,-1)],
            "Up": [(1, 1), (0, 1), (-1, 1)],
            "Down": [(-1, -1), (0, -1), (1, -1)]
        }
        canMove = False
        street = None
        thisPos = False
        if len(self.route) > 0:
            if self.route[0] != self.destination.pos:
                for a in self.model.grid.get_cell_list_contents(self.pos):
                    if isinstance(a, Road) or isinstance(a, Traffic_Light):
                        street = a
                for agent in self.model.grid.get_cell_list_contents(self.route[0]):
                    if isinstance(agent, Traffic_Light):
                        if street.direction in ["Right", "Left"] and self.pos[1] == self.route[1][1]:
                            self.route[0] = (self.route[0][0], self.pos[1])
                        elif street.direction in ["Up", "Down"] and self.pos[0] == self.route[1][0]:
                            self.route[0] = (self.pos[0], self.route[0][1])

                    if isinstance(agent, Road) or (isinstance(agent, Traffic_Light) and agent.state == 2):
                        canMove = True
                    elif isinstance(agent, Car):
                        if agent.state == "Moving":
                            canMove = False                    
                        break
            else:
                canMove = True
            
            if canMove:
                if self.pos[0] != self.route[0][0] and self.pos[1] == self.route[0][1]:
                    if self.pos[0] < self.route[0][0]:
                        self.direction = "Right"
                    else:
                        self.direction = "Left"
                elif self.pos[1] != self.route[0][1] and self.pos[0] == self.route[0][0]:
                    if self.pos[1] < self.route[0][1]:
                        self.direction = "Up"
                    else:
                        self.direction = "Down"
                        
                print(f"Se mueve de {self.pos} a {self.route[0]}")
                self.model.grid.move_agent(self, self.route.pop(0))
            else:
                print(f"Se queda en {self.pos} con intenciones de ir a", self.route[0] if len(self.route) > 0 else self.destination.pos)
        else:
            print(f"Llegó a su destino en {self.pos}")


    def step(self):
        """ 
        Determines the new direction it will take, and then moves
        """
        self.move()

class Traffic_Light(Agent):
    """
    Obstacle agent. Just to add obstacles to the grid.
    """
    def __init__(self, unique_id, model, state = False, timeToChange = 10):
        super().__init__(unique_id, model)
        self.state = 2 if state else 0 # 1: Red, 2: Yellow, 3: Green
        self.timeToChange = timeToChange
        self.direction = ""

    def step(self):
        pass

class Destination(Agent):
    """
    Obstacle agent. Just to add obstacles to the grid.
    """
    def __init__(self, unique_id, model):
        super().__init__(unique_id, model)

    def step(self):
        pass

class Obstacle(Agent):
    """
    Obstacle agent. Just to add obstacles to the grid.
    """
    def __init__(self, unique_id, model):
        super().__init__(unique_id, model)

    def step(self):
        pass
