using System;
using System.Data;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Mission3
{

    class Grid
    {

        #region ====================================== Definition of the grid ===========================
        // C# program which returns a 10*10 grid, 
        // where is placed randomly a "heroine" character and a "treasure". 
        // Each square has a qVector (U, D, L, R) for top bottom left right, 
        // which is initially initialized to (0,0,0,0), 
        // except for the treasure which is (10, 10, 10, 10)
        public string[,] squares;
        public double[,][] qVector;
        public const string HEROINE = "👩 ";
        public const string TREASURE = "💰 ";
        public const string FOREST = "🌳 ";
        public const string WAY = "🦶🏼 ";
        public const string DEPARTURE = "🏁 ";
        public const string HAPPY = "👸 ";
        int[] coordTreasure;
        int[] coordHero;

        public Random random;

        public Grid(int square = 10)
        {
            random = new Random();
            squares = new string[square, square];
            qVector = new double[square, square][];

            int rowT = random.Next(0, 10);
            int colT = random.Next(0, 10);
            this.coordTreasure = new int[] { rowT, colT };
            this.coordHero = InitGrid(square);

            // init the vector once and for all
            for (int row = 0; row < square; row++)
            {
                for (int col = 0; col < square; col++)
                {
                    this.qVector[row, col] = new double[] { 0, 0, 0, 0 };
                }
            }

            this.qVector[coordTreasure[0], coordTreasure[1]] = new double[] { 10, 10, 10, 10 };
        }

        public int[] InitGrid(int square = 10)
        {
            // initialisation grid
            for (int row = 0; row < square; row++)
            {
                for (int col = 0; col < square; col++)
                {
                    this.squares[row, col] = FOREST;
                }
            }

            // add heroine (doesn't have to have the same coodinates as treasure)
            int rowH = random.Next(0, 10);
            int colH = random.Next(0, 10);
            while (rowH == this.coordTreasure[0] & colH == this.coordTreasure[1])
            {
                rowH = random.Next(0, 10);
                colH = random.Next(0, 10);
            }
            this.squares[rowH, colH] = HEROINE; // l'heroine est mis aleatoirement sur la grille
            this.squares[this.coordTreasure[0], this.coordTreasure[1]] = TREASURE;
            int[] coordH = new int[] { rowH, colH };
            this.coordHero = coordH;
            return coordH;
        }

        public int[] NewPosition(int action)
        {
            int newRow = this.coordHero[0];
            int newCol = this.coordHero[1];

            if (action == 0 && this.coordHero[0] > 0) // up
            {
                newRow--;
            }
            else if (action == 1 && this.coordHero[0] < squares.GetLength(0) - 1) // down
            {
                newRow++;
            }
            else if (action == 2 && this.coordHero[1] > 0) // left
            {
                newCol--;
            }
            else if (action == 3 && this.coordHero[1] < squares.GetLength(1) - 1) // right
            {
                newCol++;
            }
            int[] newPos = new int[] { newRow, newCol };
            return newPos;
        }

        #endregion

        #region ====================================== Exploration ======================================

        public void Exploration(int numIterations = 5000, double ALPHA = 0.2, double GAMMA = 0.5)
        {

            for (int i = 0; i < numIterations; i++)
            {
                // PrintGrid();
                // move the heroine randomly
                int direction = random.Next(4);
                int[] newPos = NewPosition(direction);

                // calcul reward
                double reward = 0;
                if (newPos[0] == this.coordTreasure[0] && newPos[1] == this.coordTreasure[1])
                {
                    reward = 10;
                }
                // after chosing the direction, we look at the value in the qVector for this square
                double currentQ = qVector[this.coordHero[0], this.coordHero[1]][direction];

                double maxNextQ = qVector[newPos[0], newPos[1]][0]; // one of the values in the vector
                for (int j = 0; j < 4; j++)
                {
                    if (qVector[newPos[0], newPos[1]][j] > maxNextQ)
                    {
                        maxNextQ = qVector[newPos[0], newPos[1]][j]; //maxNextQ is the highest value in the vector of the square
                    }
                }

                // update value dans le qVector
                double updatedQ = currentQ + ALPHA * (reward + GAMMA * maxNextQ - currentQ); //"règle de mise à jour de Q-learning."
                qVector[this.coordHero[0], this.coordHero[1]][direction] = updatedQ;


                // if arrived at the treasure
                if (newPos[0] == this.coordTreasure[0] && newPos[1] == this.coordTreasure[1])
                {
                    // PrintGrid();
                    // Console.WriteLine("__________________________VICTORY__________________________");
                    // Console.WriteLine("___________________________________________________________\n");
                    InitGrid(this.squares.GetLength(0));
                    continue;
                }

                // move the heroine to the new position
                squares[this.coordHero[0], this.coordHero[1]] = WAY; // or FOREST, doesn't matter here (training)
                squares[newPos[0], newPos[1]] = HEROINE;
                this.coordHero = newPos;
            }
        }

        #endregion

        #region ====================================== Exploitation =====================================

        public List<int> Exploitation()
        {
            bool victory = false;
            int comp = 0;
            InitGrid(this.squares.GetLength(0));
            int minSteps = Math.Abs(this.coordHero[0] - this.coordTreasure[0]) + Math.Abs(this.coordHero[1] - this.coordTreasure[1]);
            while (!victory & comp < 100)
            {
                Console.WriteLine($"Step {comp}");
                comp += 1;
                PrintGrid();
                // Current position of the heroine
                int row = this.coordHero[0];
                int col = this.coordHero[1];

                // Find the direction w/ highest Q-value at the current position
                double maxQ = double.MinValue;
                int maxDirection = -1;
                for (int j = 0; j < 4; j++)
                {
                    if (qVector[row, col][j] > maxQ)
                    {
                        maxQ = qVector[row, col][j];
                        maxDirection = j;
                    }
                }

                // Update position 
                int[] newPos = NewPosition(maxDirection);
                
                this.coordHero = newPos;
                if (comp == 1)
                {
                    this.squares[row, col] = DEPARTURE;
                }
                else
                {
                    this.squares[row, col] = WAY;
                }
                this.squares[newPos[0], newPos[1]] = HEROINE;

                // If heroine reached the treasure
                if (newPos[0] == this.coordTreasure[0] && newPos[1] == this.coordTreasure[1])
                {
                    this.squares[newPos[0], newPos[1]] = HAPPY;
                    Console.WriteLine($"Step {comp}");
                    PrintGrid();
                    Console.WriteLine("__________________________VICTORY__________________________");
                    Console.WriteLine($"____________NUMBER OF STEPS : {comp} | MINIMUM : {minSteps}____________\n");
                    victory = true;
                }
            }
            List<int> retour = new List<int>{comp, minSteps};
            return retour;
        }


        #endregion

        #region ====================================== Visualisation ====================================

        public void PrintGrid()
        {
            for (int row = 0; row < 10; row++)
            {
                Console.Write(row + " ");
                for (int col = 0; col < 10; col++)
                {
                    Console.Write(squares[row, col].ToString());
                }
                Console.WriteLine();
            }
            Console.WriteLine("___________________________________________________________");
        }

        public DataTable GetQTable()
        {
            // exports the QTable into a datatable
            DataTable dt = new DataTable();
            for (int i = 0; i < this.squares.GetLength(0); i++)
            {
                dt.Columns.Add(i.ToString(), typeof(string));
                dt.Rows.Add(dt.NewRow());
            }

            for (int i = 0; i < this.squares.GetLength(0); i++)
            {
                for (int j = 0; j < this.squares.GetLength(0); j++)
                {
                    double[] values = qVector[i, j];
                    string strValues = string.Join(";", values);
                    dt.Rows[i][j] = strValues;
                }
            }

            return dt;
        }

        #endregion
    }

    class Program
    {


        #region ====================================== Compute error ===================================
        static public void ComputeError()
        {
            Dictionary<int, double> nbExploration = new Dictionary<int, double> { { 1000, 0.0 }, { 2000, 0.0 }, { 3000, 0.0 }, { 4000, 0.0 }, { 5000, 0.0 }, { 6000, 0.0 }, { 7000, 0.0 }, { 8000, 0.0 }, { 9000, 0.0 }, { 10000, 0.0 } };

            foreach (KeyValuePair<int, double> kvp in nbExploration)
            {
                double errorLoop = 0.0;
                for (int i = 0; i < 50; i++)
                {
                    Grid gridEval = new Grid();
                    gridEval.Exploration(kvp.Key);
                    List<int> retour = gridEval.Exploitation();
                    if (retour[0] == 100 | retour[0] != retour[1])
                    {
                        errorLoop += 1;
                    }
                }
                nbExploration[kvp.Key] = errorLoop*100/50;
            }
            Console.WriteLine($"Error = when the heroine hasn't find the treasure after 100 steps,\nor the nb of steps!= nb minimal of steps necesary.");
            foreach (KeyValuePair<int, double> kvp in nbExploration)
            {
                Console.WriteLine($"Error for {kvp.Key} explorations : {kvp.Value}% games");
            }
            Console.WriteLine("\n");
        }

        #endregion

        #region ====================================== Visualisation ===================================

        static public void SaveToCsv(DataTable dataset, string filename)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
            };

            using (var writer = new StreamWriter(filename))
            using (var csv = new CsvWriter(writer, config))
            {
                foreach (DataColumn column in dataset.Columns)
                {
                    csv.WriteField(column.ColumnName);
                }
                csv.NextRecord();

                foreach (DataRow row in dataset.Rows)
                {
                    for (var i = 0; i < dataset.Columns.Count; i++)
                    {
                        csv.WriteField(row[i]);
                    }
                    csv.NextRecord();
                }
            }
        }

        #endregion

        #region ====================================== Main ============================================

        static void Main()
        {
            // ====================================== Compute error ====================================
            //ComputeError();


            //====================================== 1 game ============================================
            Console.WriteLine("Example of a game :\n");

            Grid grid = new Grid();
            grid.Exploration(5000); //5000 perfect, en dessous commencent erreurs
            grid.Exploitation();

            SaveToCsv(grid.GetQTable(), "data csv/qTable.csv");

        
        }
        #endregion

    }
}
