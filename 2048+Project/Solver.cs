using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace _2048_Project
{
    public class Solver
    {
        public int[,] grid; // A variable that resembles the Solver's class board.

        #region Constructors
        /// <summary>
        /// General: A Constructor to the Solver class.
        /// </summary>
        public Solver()
        {
            this.grid = new int[4, 4];
        }
        
        /// <summary>
        /// General: A copy Constructor to the Solver class.
        /// </summary>
        /// <param name="s"> A Solver to copy. </param>
        public Solver(Solver s)
        {
            this.grid = new int[4, 4];
            // Copying all the values from the received Solver's grid to the current state's grid:
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    this.grid[r, c] = s.grid[r, c];
        }
        #endregion

        #region Random AI Algorithm
        /// <summary>
        /// General: A Random AI.
        /// Process: Randomly picking a move to perform.
        /// </summary>
        /// <returns> A Key representing a move to perform. </returns>
        public static Keys RandomAI()
        {
            // Variables Definition:
            Random rnd = new Random();
            int val = rnd.Next(0, 4); // Randomly picking a number between 0 to 3, each value represents a Key.
            Keys key = Keys.None;

            // Using a switch case to determine what key was randomly picked:
            switch (val)
            {
                case 0:
                    key = Keys.Left;
                    break;
                case 1:
                    key = Keys.Right;
                    break;
                case 2:
                    key = Keys.Up;
                    break;
                case 3:
                    key = Keys.Down;
                    break;
            }

            // Returning the chosen key:
            return key;
        }
        #endregion

        #region Performing Movement
        /// <summary>
        /// General: The main function which is in charge of performing a move.
        /// Process: "Pushing" the board towards the direction represented by the received key.
        ///          Using an outer loop which runs 4 times which resembles every row/column in
        ///          the board, each time collecting the values of a specific row/column, then
        ///          performing the movement on them and eventually inserting them back to the
        ///          table which represents the board.
        /// </summary>
        /// <param name="key"> A Key representing a direction to move the board towards.  </param>
        public void pushBoard(Keys key)
        {
            // An outer loop that runs 4 times (the amount of rows/cols in the board):
            for (int t = 0; t < 4; t++)
            {
                // Defining a list of ints (max - 4 items) to contain all the values which
                // are different than 0 in the current row.
                List<int> items = new List<int>(4);
                switch (key)
                {
                    #region Movement Left
                    case Keys.Left:
                        for (int c = 0; c < 4; c++)
                            if (this.grid[t, c] != 0)
                                items.Add(this.grid[t, c]);

                        // Fusing tiles:
                        for (int i = 0; i < items.Count - 1; i++)
                        {
                            if (items[i] == items[i + 1])
                            {
                                items[i]++;
                                items.RemoveAt(i + 1);
                            }
                        }

                        // Writing the fused tiles back to the row:
                        for (int c = 0; c < 4; c++)
                            this.grid[t, c] = (c < items.Count ? items[c] : 0);
                        break;
                    #endregion

                    #region Movement Right
                    case Keys.Right:
                        for (int c = 0; c < 4; c++)
                            if (this.grid[t, c] != 0)
                                items.Add(this.grid[t, c]);

                        //Consolidate duplicates
                        for (int i = items.Count - 1; i > 0; i--)
                        {
                            if (items[i] == items[i - 1])
                            {
                                items[i]++;
                                items.RemoveAt(i - 1);
                                i--;
                            }
                        }

                        //Write the data back to the row
                        for (int i = 0; i < 4; i++)
                            this.grid[t, 4 - 1 - i] = (items.Count - 1 - i >= 0 ? items[items.Count - 1 - i] : 0);
                        break;
                    #endregion

                    #region Movement Up
                    case Keys.Up:
                        for (int r = 0; r < 4; r++)
                            if (this.grid[r, t] != 0)
                                items.Add(this.grid[r, t]);

                        //Consolidate duplicates
                        for (int i = 0; i < items.Count - 1; i++)
                        {
                            if (items[i] == items[i + 1])
                            {
                                items[i]++;
                                items.RemoveAt(i + 1);
                            }
                        }

                        //Write the data back to the row
                        for (int r = 0; r < 4; r++)
                            this.grid[r, t] = (r < items.Count ? items[r] : 0);
                        break;
                    #endregion

                    #region Movement Down
                    case Keys.Down:
                        for (int r = 0; r < 4; r++)
                            if (this.grid[r, t] != 0)
                                items.Add(this.grid[r, t]);

                        //Consolidate duplicates
                        for (int i = items.Count - 1; i > 0; i--)
                        {
                            if (items[i] == items[i - 1])
                            {
                                items[i]++;
                                items.RemoveAt(i - 1);
                                i--;
                            }
                        }

                        //Write the data back to the row
                        for (int i = 0; i < 4; i++)
                            this.grid[3 - i, t] = (items.Count - 1 - i >= 0 ? items[items.Count - 1 - i] : 0);
                        break;
                        #endregion
                }
            }
        }
        #endregion

        #region ExpectiMax Algorithm & Calculations
        /// <summary>
        /// General: The main AI function in the project. This function is in charge of performing all the necessary
        ///          calculations and using all the relevant functions in order to return a score representing the
        ///          score that the original received Solver deserves.
        /// Process: This is a recursive function which is based on the ExpectiMax Algorithm.
        ///          For full explanation in regards to how this Algorithm works, please check my project portfolio.
        ///          To keep it simple, what basically happens in this recursive function is that we create a Tree.
        ///          The "Tree" we create contains nodes, while each of the nodes in the tree is a Solver representing
        ///          a possible outcome based on the original Solver. We calculate the rating for each node in the tree and
        ///          then choose the AVERAGE of the childs of every node as the node's value. We continue doing this
        ///          all the way up until reaching the root, which is the original Solver that the function received.
        ///          For the root, we pick the MAX of it's childs, and then we return it as the "bestValue" which
        ///          represents the BEST outcome based on the received Solver's board.
        /// </summary>
        /// <param name="root"> A Solver that we want to get an ExpectiMax rating for. </param>
        /// <param name="depth"> The depth to which we want the ExpectiMax algorithm to reach. </param>
        /// <param name="player"> A bool to determine whether it's the "Player's" (AI's) turn or the oponnent (Computer). </param>
        /// <returns> Returning the rating for the original received Solver. </returns>
        public static double ExpectiMax(Solver root, int depth, bool player)
        {
            // Variables Definition:
            double bestValue = 0, val = 0;

            // If depth is 0 then we can stop the recursion and return the root's:
            if (depth == 0)
                return root.GetRating();

            // The algorithm's turn:
            if (player)
            {
                // Setting bestValue as the min value of double at the start of each calculation:
                bestValue = double.MinValue;

                // Initializing moves with all the possible moves for the current state:
                Dictionary<Keys, Solver> moves = root.getAllMoveStates();

                // If we can't move then the game is over - WORST CASE:
                if (moves.Count == 0)
                    return double.MinValue;

                // Calculating the bestValue for every possible state in the board:
                foreach (KeyValuePair<Keys, Solver> st in moves)
                {
                    // Recursion - Explained:
                    // First, recursively calling the ExpectiMax algorithm to continue it's run
                    // until reaching depth = 0. Then, when we get a result, choosing the better
                    // option between the 2: val, bestValue. val represents the current value
                    // received from the recursive call to ExpectiMax and bestValue represents
                    // the highest value received so far. This, combined with the call to the
                    // getAllMoveStates function which returns a list of all the possible moves
                    // for the current state, allows us to pick the best move to perform in the
                    // current Solver's state.

                    // Summary:
                    // Basically, what we do here is to get the best possible move from a list of
                    // moves that contains all the possible moves for the current state.
                    val = ExpectiMax(st.Value, depth - 1, false);
                    bestValue = Math.Max(val, bestValue);
                }
            }
            // "Computer" turn:
            else
            {
                // Getting all the possible states based on a given move:
                List<Solver> moves = root.getAllRandom();
                int i = 0;
                foreach (Solver st in moves)
                {
                    // Math - Explained:
                    // We use the variable i to help us determine the INDEX of the child in the "tree" we created.
                    // Since we built the tree in such way that we have both a Solver with the value 2 and a Solver with
                    // the value 4 initialized for every FREE CELL in the board, we know that childs with even index
                    // will be initialized with the value 1 (representing the value 2), while those with an odd index
                    // will be initialized with the value 2 (representing the value 4). We know that in the game itself,
                    // the chances of getting 2 are 90% while the chances of getting 4 are 10%, therefore we can determine
                    // by the index what we should multiply by (0.9 OR 0.1). We also multiply by 1 divided by half of the
                    // possible moves (which represents the amount of even/odd nodes). At last, we multiply by the result
                    // of the recursive call to Expectimax with depth-1 and the player being set to true.

                    // Summary:
                    // To sum it up, this part is in charge of getting the average of every level in the tree:
                    bestValue += (1.0 / (moves.Count / 2) * ((i % 2 == 0) ? 0.9 : 0.1)) * ExpectiMax(st, depth - 1, true);
                    i++; // Index of child in the "tree".
                }
            }

            // Returning the bestValue:
            return bestValue;
        }
        
        /// <summary>
        /// General: Calculating the rating for a given "node" in the "tree" we created.
        /// Process: Using 2 for loops to scan the board and setting "weight" to each node to keep the most
        ///          significant node, the node with the highest valued tile, at the top right corner of the board.
        /// Math:    Basically, we rate a "node", which represents a possible board as a result of a movement here.
        ///          In order to rate the "node", we use the following calculation: score = log2(x)*(1/4)^y,
        ///          having x = the value of every tile in the board, and y representing the tile's index in the board.
        ///          This way, we can assure that on MOST cases, we will have the most signifact tile (the tile with
        ///          the highest value) at the TOP RIGHT corner of the board.
        /// </summary>
        /// <returns> Returns a double representing the score that the current Solver instance received. </returns>
        public double GetRating()
        {
            // Variables Definition:
            double score = 0, weight = 1;

            // Using 2 for loops to iterate through the board.
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    score += Math.Pow(2, this.grid[i, j]) * weight;
                    weight *= 0.25;
                }
            }

            // Output - Returning the score that the "node" received:
            return score;
        }
        #endregion

        #region ExpectiMax Helping Functions
        /// <summary>
        /// General: Finding all the possible moves which will affect the board.
        /// Process: Performing a movement on a copy of the current Solver, and checking whether
        ///          the copy of the Solver's board (after the movement) is different from the original 
        ///          Solver's board. If it's different, then adding it to a Dictionary which contains
        ///          all the possible moves that will lead to a different Solver's board from the current
        ///          Solver's board.
        /// </summary>
        /// <returns> Returns a Dictionary containing all the possible moves & their matching Solvers. </returns>
        public Dictionary<Keys, Solver> getAllMoveStates()
        {
            // Creating a dictionary of up to 4 Keys-Solver pairs which we will return at the end of the function:
            Dictionary<Keys, Solver> allMoves = new Dictionary<Keys, Solver>(4);
            // Defining every possible move that can be performed:
            Keys[] possibleMoves = { Keys.Left, Keys.Right, Keys.Up, Keys.Down };

            // Creating an instance which will be used to replicate the current Solver:
            Solver next;

            // Generating and storing the board's state after "movement" to the direction that key points
            // towards. Checking the board's state after performing "movement" to every direction:
            foreach (Keys key in possibleMoves)
            {
                next = new Solver(this);
                next.pushBoard(key);
                if (!this.equalTo(next))
                    allMoves.Add(key, next);
            }
            
            // Returning the dictionary with up to 4 Keys. The Dictionary contains the state of
            // the board after performing every possible movement at a given time, if the movement
            // causes any changed to the board:
            return allMoves;
        }
        
        /// <summary>
        /// General: Finding all the FREE cells in the current Solver's board.
        /// Process: Creating a list of points to which we add values by using 2 for loops
        ///          to scan the current Solver's board, each time checking if the current
        ///          value in the Solver's board isn't initialized (it's value is 0). If it
        ///          isn't initialized, then adding a point representing the cell to the list
        ///          of points we created.
        /// </summary>
        /// <returns> Returning a list of points representing the indexes of the uninitialized cells in the current Solver's board. </returns>
        private List<Point> GetFreeCells()
        {
            // Variables Definition:
            List<Point> free = new List<Point>(); // A list of points to store the indexes of the "free" cells.

            // Using 2 for loops to add every empty cell from the grid to the list:
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    if (this.grid[r, c] == 0)
                        free.Add(new Point(r, c));
            
            // Output - a list of the "free" cells:
            return free;
        }
        
        /// <summary>
        /// General: Generating a list of Solvers with boards representing each possible outcome as
        ///          a result of a new tile being added to the current Solver's board.
        /// Process: First, getting all the free cells in the board using the GetFreeCells we created
        ///          above. Then, for each free cell in the board, generating 2 COPY Solvers on which
        ///          at the value in the index of the free cell is initialized to "1" (representing 2)
        ///          or initialized to "2" (representing 4).
        /// </summary>
        /// <returns> Returning a list of Solvers containing all the Solvers for every free cell. </returns>
        public List<Solver> getAllRandom()
        {
            // Variables Definition:
            List<Solver> res = new List<Solver>(); // A new list of Solver.
            List<Point> free = this.GetFreeCells(); // A list of all the free cells in the board.
            Solver next = new Solver();

            // Generating the board's state twice:
            // 1. In case the current cell's value will be 2.
            // 2. In case the current cell's value will be 4. 
            foreach (Point curPoint in free)
            {
                next = new Solver(this);
                next.grid[curPoint.X, curPoint.Y] = 1;
                res.Add(next);

                next = new Solver(this);
                next.grid[curPoint.X, curPoint.Y] = 2;
                res.Add(next);
            }

            // Returning the list of states generated for each free cell in the board:
            return res;
        }
        
        /// <summary>
        /// General: Checking if the current Solver is equal to another Solver instance.
        /// Process: Checking whether ALL the cells in the current Solver's grid are equal
        ///          to the other Solver's grid.
        /// </summary>
        /// <param name="another"> Another Solver to check whether it's equal to the current Solver or not. </param>
        /// <returns> Returns True if both Solvers grids are equal or False if not. </returns>
        public bool equalTo(Solver another)
        {
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    if (this.grid[r, c] != another.grid[r, c])
                        return false;

            return true;
        }
        #endregion

        /// <summary>
        /// General: A public ToString to help the debugging process be easier.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    string number = "";

                    if (this.grid[r, c] != 0)
                    {
                        number = ((int)Math.Pow(2, this.grid[r, c])).ToString();
                    }
                    sb.Append(number.PadLeft(5, ' '));

                }
                sb.AppendLine();

            }
            sb.AppendLine(" ---- ---- ---- ----");

            return sb.ToString();
        }
    }
}
