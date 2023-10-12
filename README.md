# Qlearning mission - VS Code Console
mission carried out within Bandai Namco Studios

For this mission, I have to code in C# an AI that:

- Generates a 10*10 grid, where is placed randomly a "hero" character (here it is a heroine ^^) and a "treasure."
- Code a q-learning "Exploration" function that randomly moves the heroine from square to square and learns which are the shortest paths to get to the treasure.
o       When the heroine arrives at the treasure, the game is won, a new game begins the grid is reset (same starting point for the treasure, and random for the heroine).
o       Each box has a score {U, D, L, R}, stored in the variable qVector (up, down, left, right). It represents the Q-table and updates its values based on the hero's movements. The qVector is regularly filled with the values of the moves the heroine made.
o       When the heroine arrives at the treasure, the AI gets a reward of 10 points. Thanks to the Bellman formula which is recursive, the rewards get distributed on the grid and are stored in the qVector.
- Write an "Exploitation" function that initializes the grid once and uses the qVector to move the hero to the treasure in as few moves as possible.
![3](https://github.com/Annabel64/2D-qLearning/assets/76532104/a37a1f4e-9e78-453b-b6cd-96ae4a1d097c =250x)
![4](https://github.com/Annabel64/2D-qLearning/assets/76532104/bdafb5b8-43f1-4006-aa5c-78ff7ced46c9 =250x)


I had never coded q-learning7 algorithms. At school, we had seen the theory and I had inquired by myself as I was interested. This mission allowed me to understand how this type of algorithm works, and to code everything from scratch without using any library that does everything for us. It taught me a lot.

![qVector](https://github.com/Annabel64/2D-qLearning/assets/76532104/9510917f-01ca-40fe-a466-ce42410204bd)
Here on the left is the visual representation of the qVector. We can see that the square with the most points is the square containing the treasure, and the more the square is far from the treasure, the less points it has. For example, here, the square (0,0) has the vector (u:0.18;d:0.37;l:0.18;r:0.37), because it is more interesting to go down or on the right, and the square (5,8) with the treasure has the vector (10;10;10;10), because it is the final square.

The training (Exploration) is quite quick: around 15 seconds for 5000 exploration games, and less than 3% errors.
![image](https://github.com/Annabel64/2D-qLearning/assets/76532104/7ec7a685-33af-4608-be7e-ca57a8450d8d)

A pillar of reinforcement learning is the Bellman equation. There are several versions of this equation, but the simplest is the one I used for this mission. Bellman introduces two notions that are used in the equation: the VALUE of a state, and the REWARD of an action. The equation states that the value of a state S depends only on the best action A that can be done from S (for example, A is going from S to S'). This value is:

  #### V(S) = BestAction(A) [ Reward (A: S -> S') + GAMMA * V(S') ]

The reward is worth +1 if you reach the treasure, -1 if you walk into an enemy, 0 if the square is nothing special (these are examples, you make the choice you want for the rewards). Gamma can go from 0 to 1. V(S') is the value of the state after action A. This formula is recursive (links the state S with the next state S’).
