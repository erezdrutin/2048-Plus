using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace _2048_Project
{
    // A public enum which will help us determine at which game state we are at any given time:
    public enum DrawGameState
    {
        Play,
        Win,
        Lose
    }

    // A public enum which will help us determine which AI should play the game at any given time:
    public enum AIState
    {
        Randi,
        Ex,
        None
    }

    public class Board
    {
        #region Variables Definition
        public static Point offset = new Point(186, 140); // The indexes from which the board begins.
        private List<Tile> _tilesList = new List<Tile>(); // Creating a list of tiles.
        private int _score = 0; // A variable to store the game's score at any given moment.
        private int _turnsCounter = 0; // A variable to store how many turns have been made at any given moment.
        private bool _lockKeyboard = false; // Locking the keyboard between moves that are performed by the user.
        private bool _boardChanged = false; // Determining whether the board changed by a user's movement.
        private bool _won = false; // Determining when the user wins the game.
        public DrawGameState toDraw = DrawGameState.Play; // Defining a variable to help us determine what to draw.
        Keys[] _possibleMoves = { Keys.Up, Keys.Left, Keys.Right, Keys.Down }; // Initializing an array of possible moves.
        public AIState boardAI = AIState.None; // Defining a variable to help us determine what to draw.
        private int _depth; // A variable to store the depth to which the AI should reach.
        private bool _adaptive; // A variable to determine whether the AI should be "adaptive" or not.
        #endregion

        #region Getters & Setters
        public int Score { get => _score; set => _score = value; } // A getter and a setter to the score variable.
        public int Turns { get => _turnsCounter; set => _turnsCounter = value; } // A getter and a setter to the turns variable.
        public int Depth { get => _depth; set => _depth = value; } // A getter and a setter to the depth variable.
        #endregion

        #region Constructors
        /// <summary>
        /// General: A Constructor for the Board class.
        /// </summary>
        public Board()
        {
            GenerateRandomTile();
            GenerateRandomTile();
        }
        
        /// <summary>
        /// General: A Constructor of the Board object that receives and initializes the AIState and the depth for the board class.
        /// </summary>
        /// <param name="state">The state of the AI in the board. The function will initialize the boardAI's value to this instance.</param>
        /// <param name="depth">The depth of the AI. This function will initialize the _depth value of this class to this instance.</param>
        public Board(AIState state, int depth)
        {
            boardAI = state;
            // If depth == -1 --> The AI should be adaptive.
            if (depth == -1)
            {
                _depth = 4;
                _adaptive = true;
            }
            // Otherwise, setting the depth to the received value:
            else
                _depth = depth;

            GenerateRandomTile();
            GenerateRandomTile();
        }
        #endregion
        
        #region AI
        /// <summary>
        /// General: The main function which handles the random AI in the project.
        /// Process: Calling the RandomAI function from the Solver class and then performing the received move.
        /// </summary>
        public void RandomAI()
        {
            // Variables Definition:
            if (!_lockKeyboard && toDraw != DrawGameState.Lose)
            {
                MakeMove(Solver.RandomAI());
                _lockKeyboard = true;
            }
        }
        
        /// <summary>
        /// General: The main AI in the project. This function handles all the logic that lies between calculating the best move
        ///          to perform at a given time and between actually performing it on the board.
        /// Process: Calling the ExpectiMax function of the Solver class to calculate the score for every possible movement and it's effect
        ///          on the board. Then, iterating through those scores to find the best score, and performing the movement that received it.
        ///          Also, if the class's bool _adaptive equals to true, then changing the depth of the AI based on the amount of tiles on the board.
        /// </summary>
        public void ExpectiMaxAI()
        {
            Solver currentGame = new Solver(); // A variable to store a Solver instance.

            // Running AI only if the game didn't end yet and the keyboard isn't locked:
            if (!_lockKeyboard && toDraw != DrawGameState.Lose)
            {
                // Variables Definition:
                currentGame.grid = ListToMatrixAI();
                double bestScore = double.MinValue;
                List<Keys> newMoves = new List<Keys>();
                Dictionary<Keys, Solver> movesBest = new Dictionary<Keys, Solver>();
                Dictionary<Keys, Solver> moves = currentGame.getAllMoveStates();
                double moveRating = 0;
                
                // Adaptive AI:
                // If there are less than/equal to 10 tiles on the board, the AI's depth should be set to 4.
                // If there are 11-14 tiles on the board, the AI's depth should be set to 5.
                // If there are 14-16 tiles on the board, the AI's depth should be set to 6.
                if (_adaptive == true && _score >= 30000)
                {
                    if (_tilesList.Count >= 8 && _tilesList.Count <= 12)
                        _depth = 5;
                    else if (_tilesList.Count > 12)
                    {
                        _depth = 6;
                    }
                    else
                        _depth = 4;
                }
                
                // Calculating the rating for each possible move in the board at a given moment, and then adding it
                // to a list that contains the best movement (or movements if multiple movements share the same score):
                foreach (KeyValuePair<Keys, Solver> move in moves)
                {
                    moveRating = Solver.ExpectiMax(move.Value, _depth, false);

                    if (moveRating > bestScore)
                    {
                        bestScore = moveRating;
                        newMoves.Clear();
                    }

                    if (moveRating == bestScore)
                    {
                        newMoves.Add(move.Key);
                    }
                }
                // Eventually, performing a move if we received back a move to make:
                if (newMoves.Count > 0)
                    MakeMove(newMoves[0]);

                // Unlocking the keyboard after performing a movement:
                _lockKeyboard = true;
            }
        }

        /// <summary>
        /// General: Converting the list of tiles to matrix that contains their log2 values.
        /// Process: For each tile, based on it's X & Y values, placing it in the new table with a log2 value for it's actual value.
        ///          For example, if a tile had the value 8 and was placed in the index (3, 3), it will be added as a 3 (log2(8) = 3)
        ///          to the cell at the indexes (3, 3) in the matrix.
        /// </summary>
        /// <returns> Returns the new matrix initialized by the tiles list's log2 values. </returns>
        private int[,] ListToMatrixAI()
        {
            int[,] table = new int[4, 4];
            foreach (Tile curTile in _tilesList)
            {
                table[curTile.to.X, curTile.to.Y] = (int)Math.Log(curTile.Value, 2);
            }
            return table;
        }
        #endregion

        #region Perform Movement
        /// <summary>
        /// General: Letting the board be affected by keyboard keys presses.
        /// Process: Using a bool to lock the keyboard between moves that are performed by the user and calling
        ///          the MakeMove function with the key that the user pressed in order to perform that move.
        /// </summary>
        public void ControlKeyboard()
        {
            // Variables Definition:
            KeyboardState keyboard = Keyboard.GetState(); // Letting keyboard handle user's keyboard input.

            // Checking whether any of the pressed keys is an arrow key (up/down/left/right arrow):
            if (keyboard.GetPressedKeys().Intersect(_possibleMoves).Any())
            {
                // Checking whether the keyboard is locked or not (to make sure that we can "make a move"):
                if (!_lockKeyboard)
                {
                    // Calling the make move function and locking the keyboard:
                    MakeMove(keyboard.GetPressedKeys()[0]);
                    _lockKeyboard = true;
                }
            }
            else
            {
                // Else setting the lock keyboard to false because the entered key doesn't affect the
                // board as it isn't a part of the _possibleMoves array:
                _lockKeyboard = false;
            }
        }

        /// <summary>
        /// General: Initializing a matrix of SIGNED BYTES with values which will help us determine which tiles
        ///          were affected as a result of the user's movement and which weren't, and moving those that
        ///          were moved by changing their indexes in the table.
        /// Process: Initializing the matrix's values based on the TilesLists tiles' values, while for each tile
        ///          "performing" a movement and then setting a value for it in the matrix. Also, using a counter
        ///          so that later we will know the order on which each tile has been placed.
        /// </summary>
        /// <param name="key"> A key which resembles a movement to perform. </param>
        /// <returns> Returning the initialized matrix of SIGNED BYTES. </returns>
        private sbyte[,] InitTableAndMove(Keys key)
        {
            // Variables Definition:
            sbyte[,] table = new sbyte[4, 4] { { -1, -1, -1, -1 }, { -1, -1, -1, -1 },
                                               { -1, -1, -1, -1 }, { -1, -1, -1, -1 } };
            sbyte counter = 0; // A helping variable to go through the tiles in the tiles list.

            // Initializing each tile's matching slot in the table with the counter's value, while each time
            // increasing the counter. This will allow us to later access each tile which should be
            // changed by performing the move that the user selected:
            foreach (Tile curTile in _tilesList)
            {
                switch (key)
                {
                    case Keys.Up:
                        table[curTile.from.Y, curTile.from.X] = counter++;
                        break;
                    case Keys.Down:
                        table[3 - curTile.from.Y, 3 - curTile.from.X] = counter++;
                        break;
                    case Keys.Left:
                        table[curTile.from.X, curTile.from.Y] = counter++;
                        break;
                    case Keys.Right:
                        table[curTile.from.X, 3 - curTile.from.Y] = counter++;
                        break;
                }
            }

            // Returning the SIGNED BYTES matrix we created with the initialized values:
            return table;
        }
        
        /// <summary>
        /// General: Checking which tiles get fused as a result of the user's movement.
        /// Process: Using 3 loops to find and fuse tiles that should fuse as a result of the movement by the user.
        /// </summary>
        /// <param name="table"></param>
        /// <returns> Returning a Tuple that contains a table of SIGNED BYTES and a dictionary of the indexes of the
        ///           tiles that got fused during the movement. </returns>
        private Tuple<sbyte[,], Dictionary<int, int>> FuseTiles(Keys key)
        {
            sbyte[,] table = InitTableAndMove(key); // Defining and initializing a table for the current board based on the move.
            // Fusing all the tiles that should be fused as a result of the user's movement:
            bool change = false; // A bool to determine whether the board changed or not by an operation.
            Dictionary<int, int> fused = new Dictionary<int, int>(); // Dictionary of fused  tiles from the move.
            for (int i = 0; i < 4; i++)
            {
                do
                {
                    change = false; // Resetting change's value each run.
                    for (int j = 1; j < 4; j++)
                    {
                        // Checking whether both tiles values are equal, their state is Normal
                        // and they were affected by the user's movement:
                        if (table[i, j - 1] != -1 && table[i, j] != -1
                            && _tilesList[table[i, j-1]].Equals(_tilesList[table[i, j]]))
                        {
                            fused.Add(table[i, j - 1], table[i, j]); // Adding the fused tiles to the fused tiles dictionary.
                            _tilesList[table[i, j]].TileState = TileState.Upgrade; // Upgrading the first tile.
                            _tilesList[table[i, j - 1]].TileState = TileState.Delete; // Then deleting the 2nd tile.
                            table[i, j] = -1; // That position in table is no longer taken as the tile has been moved.
                            change = true; // A fuse occured therefore setting change's value to true.
                            break; // Ending the run of the for since a fuse has occurred.
                        }

                        // Checking whether the 2nd tile was moved as a result of the movement.
                        // If it was moved, we would like to switch between it's value in the table
                        // to the first tile's value in the table in order to move a tile by 1 to the
                        // matching direction (based on the movement direction) each time.
                        if (table[i, j - 1] == -1 && table[i, j] != -1)
                        {
                            table[i, j - 1] = table[i, j];
                            table[i, j] = -1;
                            change = true;
                            break;
                        }
                    }
                    // The _boardChanged bool will be set to true when a fuse occurs:
                    _boardChanged = _boardChanged || change;
                }
                while (change); // Because we can't determine how many fuses/movements will occour in a row.
            }

            // Returning both the updated table and the fused dictionary:
            return Tuple.Create(table, fused);
        }
        
        /// <summary>
        /// General: The main function that is in charge of performing a movement. This function used the FuseTiles function
        ///          that calls the InitTableAndMove function all in the sake of performing a movement.
        /// Process: Calling the FuseTiles to get the board after performing the fusing and movement of the tiles using a matrix.
        ///          Then, converting the received matrix back to the tiles list by setting the .to attribute of the matching tiles
        ///          to what it should be updated to based on the chosen movement, fusing the tiles that should get fused and starting
        ///          the tiles movement "animation".
        /// </summary>
        /// <param name="key"> A key representing the movement to perform. </param>
        public void MakeMove(Keys key)
        {
            // Variables Definition:
            _boardChanged = false; // Checking whether the movement changed the board.
            Tuple<sbyte[,], Dictionary<int, int>> temp = FuseTiles(key); // Fusing all the tiles that should be fused as a result of the user's movement.
            sbyte[,] table = temp.Item1; // Defining a table for the current board based on the move.
            Dictionary<int, int> fused = temp.Item2; // Creating a dictionary to store the tiles that fused due to the movement.

            // Performing the movement of each tile in the table:
            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    if (table[i, j] != -1)
                    {
                        switch (key)
                        {
                            // Movement Up - Moving to the current tile's indexes in the table:
                            case Keys.Up:
                                _tilesList[table[i, j]].to = new Point(j, i);
                                break;
                            // Movement Down - Moving to the current tile's indexes in the table with the exception of 3-j instead j:
                            case Keys.Down:
                                _tilesList[table[i, j]].to = new Point(3 - j, 3 - i);
                                break;
                            // Movement Left - Moving to the current tile's indexes in the table:
                            case Keys.Left:
                                _tilesList[table[i, j]].to = new Point(i, j);
                                break;
                            // Movement Right - Moving to the current tile's indexes in the table with the exceptions
                            //  of 3-i instead i and 3-j instead j:
                            case Keys.Right:
                                _tilesList[table[i, j]].to = new Point(i, 3 - j);
                                break;
                        }
                    }
                }
            }

            // Fusing the tiles in the fused tiles dictionary:
            foreach (KeyValuePair<int, int> pair in fused)
            {
                // Setting the 2nd tile's TO attribute to a new point with a matching tile from _tilesList at the
                // Key (the 1st tile's value) TO attribute:
                _tilesList[pair.Value].to = new Point(_tilesList[pair.Key].to.X, _tilesList[pair.Key].to.Y);
            }

            // Letting the animation begin again if the board has changed
            if (_boardChanged)
            {
                // Base amount of ticks for the tile:
                Tile.ticks = 0;
            }
        }
        #endregion

        #region Checking If the user lost:
        /// <summary>
        /// General: The main function that is in charge of checking whether the using lost or not at a given time.
        /// Process: Checking the adjacent columns and rows for every cell by each time checking 3 adjacent cells.
        ///          If at the end the value of the bool "alive" remains false, then updating the relevant things
        ///          in order to end the game.
        /// </summary>
        private void CheckIfLost()
        {
            bool alive = false;

            // Converting the tiles list to an array:
            var table = ListToMatrix();

            // Checking for adjacent columns and rows for every cell:
            for (int x = 0; x < 4 && !alive; x++)
            {
                for (int y = 0; y < 3 && !alive; y++)
                {
                    // Each time checking 3 cells. For example, take the following board:
                    // 2 2 0 0
                    // 2 0 0 0
                    // 0 0 0 0
                    // 0 0 0 0
                    // In the first run, we will check the 3 tiles that their value isn't 0.
                    // { x=0, y=0 -> (x, y), (x, y+1), (y+1, x) }
                    // In this way we will cover the whole board each time checking 3 adjacent tiles
                    // and if we won't find any possible "fusing", then we will know that the game has ended.
                    alive = (table[x, y] == table[x, y + 1]) || (table[y, x] == table[y + 1, x]);
                }
            }
            // If same is still false then the user lost therefore drawing the DrawGameState.Lose:
            if (!alive)
            {
                // Drawing the Losing screen:
                toDraw = DrawGameState.Lose;
            }
        }

        /// <summary>
        /// General: Converting the list to a matrix of ints on which each cell represents a tile on the board and it's value represents it's matching
        ///          tile from the tiles list value.
        /// Process: Using a foreach loop to iterate through the tiles on the list and storing the value of each tile in the matrix.
        /// </summary>
        /// <returns> Returning a matrix of ints representing the values of the tiles in the tiles list. </returns>
        private int[,] ListToMatrix()
        {
            int[,] table = new int[4, 4];
            foreach (Tile curTile in _tilesList)
            {
                table[curTile.to.X, curTile.to.Y] = curTile.Value;
            }
            return table;
        }
        #endregion
        
        #region Drawing & Generating Tiles
        /// <summary>
        /// General: The main function on this class which is in charge of drawing the tiles and
        ///          the game state when the game ends (either on a win or a loss).
        /// Process: Drawing the tiles from the tiles list and animating their movement by
        ///          incrementing Tile.ticks value each time this function is called. Also, every
        ///          time this function is called, checking whether the user won or not and
        ///          accordingly deciding whether to display a win/loss screen.
        /// </summary>
        public void DrawTiles()
        {
            // Drawing every tile in the tiles list:
            _tilesList.ToList().ForEach(block => block.Draw());

            // Updating the counter of the animation until reaching max ticks:
            if (Tile.ticks < Tile.maxTicks)
            {
                // Adding 1 to the counter of the animation:
                Tile.ticks++;
            }

            // Finishing the movement of the tile - updating each of the tiles' .from to .to value,
            // and if anything happened then also adding a new tile to the game:
            else
            {
                if (boardAI != AIState.None)
                {
                    _lockKeyboard = false; // CHECK WITH AI STUFF
                }
                //_lockKeyboard = false; // CHECK WITH AI STUFF

                if (_boardChanged)
                    _turnsCounter++; // Counting how many moves have been made by the user/ai.

                // Setting the "from" property of every tile to it's "to" property
                // since we finished animating it's movement:
                _tilesList.ForEach(block => block.from = new Point(block.to.X, block.to.Y));

                // Removing from the list any block that has the state "Delete":
                _tilesList.RemoveAll((Tile x) => { return x.TileState == TileState.Delete; });

                // Adding the value of any block.upgrade to the score variable:
                _tilesList.Where(block => block.TileState == TileState.Upgrade)
                      .ToList()
                      .ForEach(block => Score += block.Upgrade());

                // Checking if the PLAYER won (Won't display "YOU WIN" in an AI run):
                if (!_won && _tilesList.Any(block => block.Value == 2048) && boardAI == AIState.None || _tilesList.Any(block => block.Value == 32768))
                {
                    _won = true;
                    toDraw = DrawGameState.Win;
                }

                // Checking if the player lost:
                if (_tilesList.Count == 16)
                    CheckIfLost();

                // If the board has changed, then it means that we should generate a new tile
                // and set the _boardChanged bool to false again (so that in the next turn we
                // will be able to determine whether the board chagned or not):
                if (_boardChanged)
                {
                    _boardChanged = false;
                    GenerateRandomTile();
                }
            }
        }
        
        /// <summary>
        /// General: Generating and adding a new tile to the board.
        /// Process: Picking a random position on the board to put the new tile in and adding it. While adding it,
        ///          a random value (90% -> 2 || 10% -> 4) will be generated for the tile through the Tile's constructor.
        /// </summary>
        public void GenerateRandomTile()
        {
            // A random generator to generate random numbers:
            Random rnd = new Random();

            // Using a Sorted Set to maintain an ascending order of elements in the range 0-15.
            SortedSet<int> available = new SortedSet<int>(Enumerable.Range(0, 16));

            // Removing all the taken slots from the available sorted set:
            foreach (var block in _tilesList)
            {
                available.Remove(block.to.X * 4 + block.to.Y);
            }

            // Generating a random value between 0 to the count of the available slots:
            var num = available.ElementAt(rnd.Next(0, available.Count()));

            // Adding a tile that receives a point [(num/4, num%4)]:
            _tilesList.Add(new Tile(new Point(num >> 2, num % 4)));
        }
        #endregion
    }
}
