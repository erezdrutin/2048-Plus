using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace _2048_Project
{
    // A public enum which will help the SceneManager determine which scene should be drawn:
    public enum DrawState
    {
        Menu, // The Menu scene should be drawn.
        PlayerMode, // The Player Mode Scene should be drawn.
        ChooseDifficulty, // The Choose Difficulty Scene should be drawn.
        PlayVsAi, // The Player vs Ai Scene should be drawn.
        ChooseAi, // The Choose Ai Scene should be drawn.
        WatchAi // The View Ai Scene should be drawn.
    }

    // A public class inheriting from the Microsoft's XNA Framework's Game Class:
    public class SceneManager : Game
    {
        #region Variables Definition
        private SpriteBatch spriteBatch; // Defining a utility to help us draw the game.
        private SpriteFont font; // Defining a utility to help us determine which font to use when drawing the game.
        private Dictionary<int, Texture2D> _blockTextures; // Defining a dictionary on which we will store tiles textures.
        private List<Texture2D> _sceneTextures; // Defining a list on which we will store textures relevant to the current scene.
        private List<Rectangle> _sceneRectangles; // Defining a list on which we will store rectangles relevant to the current scene.
        private Board _board, _opponentBoard; // Defining 2 board instances.
        private bool _lockMouse = false; // _lockMouse's purpose is to disable infinity clicks on a button from 1 click.
        public static DrawState _scene = DrawState.Menu; // A DrawState instance which will help the SceneManager determine which scene to draw.
        private const double _delay = 0.1; // Creating a delay of 0.1 seconds between each move the player makes.
        private double _remainingDelay = _delay; // Saving the remaining delay since the previous movement.
        private GameTime _gameTime; // Storing the time between each movement performed by the AI.
        private MouseState lastMouseState, currentMouseState; // MouseState instances to only allow single click on a button at a time.
        #endregion

        #region Constructors
        /// <summary>
        /// General: A constructor for the SceneManager class.
        /// </summary>
        public SceneManager()
        {
            // Initializing a screen with the size 750x550
            GraphicsDeviceManager  graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 750,
                PreferredBackBufferHeight = 550
            };
            
            Content.RootDirectory = "Content"; // The root directory that contains our assets for the game.
            this.Window.Title = "2048+"; // The name of the window of the game.
            InitializeScene(); // Calling a function to initialize the scene - Creating a list of rectangles to hold assets based on the current scene.

            // Initializing the tiles textures Dictionary to be an empty dictionary:
            _blockTextures = new Dictionary<int, Texture2D>();
            Tile.SetTextures(_blockTextures);
        }
        #endregion

        #region Overriding MonoGame Functions
        /// <summary>
        /// General: Allows the game to perform any initialization it needs to before starting to run.
        ///          This is where it can query for any required services and load any non-graphic related content.
        /// Process: Calling base.Initialize which will enumerate through any components and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true; // Setting the mouse to be visible while in game:
            base.Initialize();
        }

        /// <summary>
        /// General: In this function, we load textures to the game in order to draw the scene in the game
        ///          at any given moment. This function will be called when the game should draw a scene.
        /// Process: Using a switch case and the _scene parameter to determine which scene should be drawn,
        ///          and according to the scene deciding whether to initialize a list of Texture2Ds and a list of tile blocks.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Tile.spriteBatch = spriteBatch;
            _sceneTextures = new List<Texture2D>(); // Initializing the list of Textures.

            // Loading Menu related content:
            // Loading data relevant to all the scenes except the Menu & the Choose Ai:
            #region Load Tile Blocks
            if (_scene != DrawState.Menu && _scene != DrawState.ChooseAi && _blockTextures.Count == 0)
            {
                // Initializing the Tile's spriteBatch to the current spriteBatch (so that later
                // we can use the spriteBatch to draw from the Tile class):
                Tile.spriteBatch = spriteBatch;

                // Initializing the _blockTextures Dictionary to contain 
                // each tile's values next to it's Texture:
                int val = 2;
                while (val <= 32768)
                {
                    _blockTextures.Add(val, Content.Load<Texture2D>("tiles/block_" + val));
                    val <<= 1; // val = val * 2.
                }
            }
            #endregion

            // Using a switch case to load the relevant data to the current scene:
            switch (_scene)
            {
                #region Load Menu
                case DrawState.Menu:
                    _sceneTextures.Add(Content.Load<Texture2D>("Main_Background")); // Loading the scene image.
                    break;
                #endregion

                #region Load Player Mode
                case DrawState.PlayerMode:
                    _sceneTextures.Add(Content.Load<Texture2D>("PlayerMode_Background")); // Loading the background image.
                    _sceneTextures.Add(Content.Load<Texture2D>("restartBtn")); // Loading an image to display restart button.
                    _sceneTextures.Add(Content.Load<Texture2D>("game_won")); // Loading an image to display on a win.
                    _sceneTextures.Add(Content.Load<Texture2D>("game_over")); // Loading an image to display on a loss.
                    _sceneTextures.Add(Content.Load<Texture2D>("back")); // Loading an image to display "back" button.
                    font = Content.Load<SpriteFont>("projFont"); // Loading the font to display the game score with.
                    break;
                #endregion

                #region Load Choose Difficulty
                case DrawState.ChooseDifficulty:
                    _sceneTextures.Add(Content.Load<Texture2D>("choose_difficulty")); // Loading the scene image.
                    break;
                #endregion

                #region Load Play Vs Ai
                case DrawState.PlayVsAi:
                    _sceneTextures.Add(Content.Load<Texture2D>("player_vs_ai")); // Loading the background image.
                    _sceneTextures.Add(Content.Load<Texture2D>("restartBtn")); // Loading an image to display restart button.
                    _sceneTextures.Add(Content.Load<Texture2D>("game_won_vs_ai")); // Loading an image to display on a win.
                    _sceneTextures.Add(Content.Load<Texture2D>("game_over_vs_ai")); // Loading an image to display on a loss.
                    _sceneTextures.Add(Content.Load<Texture2D>("back")); // Loading an image to display "back" button.
                    font = Content.Load<SpriteFont>("projFont"); // Loading the font to display the game score with.
                    break;
                #endregion

                #region Load Choose Ai
                case DrawState.ChooseAi:
                    _sceneTextures.Add(Content.Load<Texture2D>("choose_ai")); // Loading the scene image.
                    break;
                #endregion

                #region Load View Ai
                case DrawState.WatchAi:
                    _sceneTextures.Add(Content.Load<Texture2D>("ai_mode")); // Loading the background image.
                    _sceneTextures.Add(Content.Load<Texture2D>("restartBtn")); // Loading an image to display restart button.
                    _sceneTextures.Add(Content.Load<Texture2D>("game_won")); // Loading an image to display on a win.
                    _sceneTextures.Add(Content.Load<Texture2D>("game_over")); // Loading an image to display on a loss.
                    _sceneTextures.Add(Content.Load<Texture2D>("back")); // Loading an image to display "back" button.
                    font = Content.Load<SpriteFont>("AiFont"); // Loading the font to display the game score with.
                    break;
                    #endregion
            }
        }

        /// <summary>
        /// General: This function is called when the game should draw itself.
        /// Process: Using a switch case to decide what scene textures to draw. 
        /// </summary>
        /// <param name="gameTime"> Provides a snapshot of timing values. </param>
        protected override void Draw(GameTime gameTime)
        {
            // Starting the spriteBatch (which will allow us to draw the game):
            spriteBatch.Begin();

            switch (_scene)
            {
                #region Draw Menu
                case DrawState.Menu:
                    // Drawing each of the textures from the sceneTextures list and sceneRectangles list.
                    // Each texture is drawn with a "body" (a matching rectangle), which will later help us
                    // detect clicks on buttons, etc:
                    for (int i = 0; i < _sceneTextures.Count; i++)
                    {
                        spriteBatch.Draw(_sceneTextures[i], _sceneRectangles[i], Color.White);
                    }
                    break;
                #endregion

                #region Draw Player Mode
                case DrawState.PlayerMode:
                    // Drawing the background:
                    spriteBatch.Draw(_sceneTextures[0], _sceneRectangles[0], Color.White);
                    // Drawing new game button:
                    spriteBatch.Draw(_sceneTextures[1], _sceneRectangles[2], Color.White);
                    // Drawing "back" button:
                    spriteBatch.Draw(_sceneTextures[4], _sceneRectangles[4], Color.White);
                    // Drawing game Score:
                    spriteBatch.DrawString(font, _board.Score.ToString(), new Vector2(617, 196), Color.White);
                    // Drawing game Turns:
                    spriteBatch.DrawString(font, _board.Turns.ToString(), new Vector2(50, 196), Color.White);
                    // Drawing the board & tiles:
                    _board.DrawTiles();

                    // If the board's state is either Win/Lose - Drawing a matching texture:
                    if (_board.toDraw == DrawGameState.Win)
                    {
                        spriteBatch.Draw(_sceneTextures[2], _sceneRectangles[3], Color.White);
                    }
                    else if (_board.toDraw == DrawGameState.Lose)
                    {
                        spriteBatch.Draw(_sceneTextures[3], _sceneRectangles[3], Color.White);
                    }
                    break;
                #endregion

                #region Draw Choose Difficulty
                case DrawState.ChooseDifficulty:
                    // Drawing the background:
                    spriteBatch.Draw(_sceneTextures[0], _sceneRectangles[0], Color.White);
                    break;

                #endregion

                #region Draw Play Vs Ai
                case DrawState.PlayVsAi:
                    // Drawing the background:
                    spriteBatch.Draw(_sceneTextures[0], _sceneRectangles[0], Color.White);
                    // Drawing new game button:
                    spriteBatch.Draw(_sceneTextures[1], _sceneRectangles[2], Color.White);
                    // Drawing "back" button:
                    spriteBatch.Draw(_sceneTextures[4], _sceneRectangles[4], Color.White);
                    // Drawing Player's Turns:
                    spriteBatch.DrawString(font, _board.Score.ToString(), new Vector2(50, 309), Color.White);
                    // Drawing Player's Score:
                    spriteBatch.DrawString(font, _board.Turns.ToString(), new Vector2(50, 440), Color.White);
                    // Drawing AI's Turns:
                    spriteBatch.DrawString(font, _opponentBoard.Score.ToString(), new Vector2(617, 309), Color.White);
                    // Drawing AI's Score:
                    spriteBatch.DrawString(font, _opponentBoard.Turns.ToString(), new Vector2(617, 440), Color.White);
                    // Drawing the board & tiles:
                    if (_opponentBoard.toDraw != DrawGameState.Lose)
                    {
                        _opponentBoard.DrawTiles();
                    }
                    else
                    {
                        _board.DrawTiles();
                    }

                    // Picking a winner to the game once both players have lost:
                    if (_board.toDraw == DrawGameState.Lose)
                    {
                        if (_board.Score > _opponentBoard.Score)
                        {
                            spriteBatch.Draw(_sceneTextures[2], _sceneRectangles[3], Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(_sceneTextures[3], _sceneRectangles[3], Color.White);
                        }
                    }
                    break;
                #endregion

                #region Draw Choose Ai
                case DrawState.ChooseAi:
                    // Drawing the background:
                    spriteBatch.Draw(_sceneTextures[0], _sceneRectangles[0], Color.White);
                    break;
                #endregion

                #region Draw View Ai
                case DrawState.WatchAi:
                    // Drawing the background:
                    spriteBatch.Draw(_sceneTextures[0], _sceneRectangles[0], Color.White);
                    // Drawing new game button:
                    spriteBatch.Draw(_sceneTextures[1], _sceneRectangles[2], Color.White);
                    // Drawing "back" button:
                    spriteBatch.Draw(_sceneTextures[4], _sceneRectangles[4], Color.White);
                    // Drawing game Score:
                    spriteBatch.DrawString(font, _board.Score.ToString(), new Vector2(617, 196), Color.White);
                    // Drawing game Turns:
                    spriteBatch.DrawString(font, _board.Turns.ToString(), new Vector2(50, 196), Color.White);
                    // Drawing the board & tiles:
                    _board.DrawTiles();
                    

                    // If the board's state is either Win/Lose - Drawing a matching texture:
                    if (_board.toDraw == DrawGameState.Lose)
                    {
                        spriteBatch.Draw(_sceneTextures[3], _sceneRectangles[3], Color.White);
                    }
                    break;
                    #endregion
            }

            // Ending the spriteBatch (which means that we "stopped" drawing):
            spriteBatch.End();

            // Sending base.Draw gameTime's value: 
            base.Draw(gameTime);
        }
        
        /// <summary>
        /// General: Allows the game to run logic such as detecting mouse clicks, detecting collisions,
        ///          gathering input and updating score. For example - when the Menu scnee is loaded and the user
        ///          clicks a button to load a different scene, this function will detemine what to do.
        /// Process: This function is being called each time the frame is rendered. Using a switch case,
        ///          we can determine which scene is currently drawn and accordingly load and run logic on it.
        /// </summary>
        /// <param name="gameTime"> Provides a snapshot of timing values. </param>
        protected override void Update(GameTime gameTime)
        {
            // Calling a function whicbh updates the current scene according to _scene's value:
            bool flag = false; // Defining a flag to determine whether the user clicked on a button or not.
            switch (_scene)
            {
                #region Update Menu
                case DrawState.Menu:
                    // Detecting a SINGLE user click on the left button:
                    lastMouseState = currentMouseState; // The active state from the last frame is now old.
                    currentMouseState = Mouse.GetState(); // Get the mouse state relevant for this frame.
                    if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
                    {
                        // Detecting user left click on the player mode button:
                        if (_sceneRectangles[1].Contains(Mouse.GetState().Position))
                        {
                            UnloadScene(); // Unloading the Menu scene.
                            _scene = DrawState.PlayerMode; // Setting the scene to Player Mode.
                            flag = true;
                        }
                        // Detecting user left click on the player vs ai button:
                        else if (_sceneRectangles[2].Contains(Mouse.GetState().Position))
                        {
                            UnloadScene(); // Unloading the Menu scene.
                            _scene = DrawState.ChooseDifficulty; // Setting the scene to Player Vs Ai Mode.
                            flag = true;
                        }
                        // Detecting user click on the view ai button:
                        else if (_sceneRectangles[3].Contains(Mouse.GetState().Position))
                        {
                            UnloadScene(); // Unloading the Menu scene.
                            _scene = DrawState.ChooseAi; // Setting the scene to View Ai Mode.
                            flag = true;
                        }
                        if (flag == true)
                        {
                            InitializeScene(); // Initializing the Updated scene.
                            LoadContent(); // Loading the Updated scene.
                            _lockMouse = true; // Locking the mouse.
                        }
                    }
                    else
                        _lockMouse = false; // Unlocking the mouse.
                    break;
                #endregion

                #region Update Player Mode
                case DrawState.PlayerMode:
                    // Removing the time on which we got here from the remaining delay in order to
                    // evaluate whether 0.1 seconds passed since the last movement that the user performed:
                    _remainingDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    
                    // Determining whether 0.1 seconds passed since the last turn by the user ot not:
                    if (_remainingDelay <= 0)
                    {
                        _board.ControlKeyboard(); // Letting the board be affected by the keyboard.
                        _remainingDelay = _delay; // Resetting the remainingDelay (to check the next turn).
                    }

                    // Detecting a SINGLE user click on the left button:
                    lastMouseState = currentMouseState; // The active state from the last frame is now old.
                    currentMouseState = Mouse.GetState(); // Get the mouse state relevant for this frame.
                    if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
                    {
                        // Detecting user clicks on the restart button:
                        if (_sceneRectangles[2].Contains(Mouse.GetState().Position))
                        {
                            if (!_lockMouse)
                            {
                                _board = new Board(); // Restarting the game.
                                _lockMouse = true; // Locking the mouse.
                            }
                        }
                        // Detecting user clicks on the back button:
                        else if (_sceneRectangles[4].Contains(Mouse.GetState().Position))
                        {
                            if (!_lockMouse)
                            {
                                UnloadScene(); // Unloading the current scene.
                                _scene = DrawState.Menu; // Loading Menu Scene.
                                InitializeScene(); // Initializing the Updated scene.
                                LoadContent(); // Loading the Updated scene.
                                _lockMouse = true; // Locking the mouse.
                            }
                        }
                    }
                    else
                    {
                        _lockMouse = false;
                    }
                    break;
                #endregion

                #region Update Choose Difficulty
                case DrawState.ChooseDifficulty:
                    // Detecting a SINGLE user click on the left button:
                    lastMouseState = currentMouseState; // The active state from the last frame is now old.
                    currentMouseState = Mouse.GetState(); // Get the mouse state relevant for this frame.
                    int depth = 0; // A variable to store the depth to set to the AI.
                    if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
                    {
                        // Detecting user left click on the Easy Button:
                        if (_sceneRectangles[1].Contains(Mouse.GetState().Position))
                        {
                            depth = 0; // On Easy mode, the depth of the AI is 0.
                            flag = true; // Detecting whether the user ended up clicking on a button or not.
                        }
                        // Detecting user left click on the Hard Button:
                        else if (_sceneRectangles[2].Contains(Mouse.GetState().Position))
                        {
                            depth = 2; // On Hard mode, the depth of the AI is 2.
                            flag = true; // Detecting whether the user ended up clicking on a button or not.
                        }
                        // Detecting user left click on the Expert Button:
                        else if (_sceneRectangles[3].Contains(Mouse.GetState().Position))
                        {
                            depth = -1; // On Expert mode, the depth of the AI is initialized to -1 since it will set the AI to adaptive.
                            flag = true; // Detecting whether the user ended up clicking on a button or not.
                        }

                        if (flag == true)
                        {
                            UnloadScene(); // Unloading the scene.
                            _board = new Board(); // Creating a new board instance (for the player).
                            _opponentBoard = new Board(AIState.Ex, depth); // Creating a new board instance (for the AI).
                            _scene = DrawState.PlayVsAi; // Setting the scene to Play Vs Ai.
                            InitializeScene(); // Initializing the Updated scene.
                            LoadContent(); // Loading the Updated scene.
                            _lockMouse = true; // Locking the mouse.
                        }
                    }
                    else
                        _lockMouse = false; // Unlocking the mouse.
                    break;
                #endregion

                #region Update Play Vs Ai
                case DrawState.PlayVsAi:
                    // Letting the player play the game once the AI loses:
                    if (_opponentBoard.toDraw == DrawGameState.Lose)
                    {
                        // Removing the time on which we got here from the remaining delay in order to
                        // evaluate whether 0.1 seconds passed since the last movement that the user performed:
                        _remainingDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        // Determining whether 0.1 seconds passed since the last turn by the user ot not:
                        if (_remainingDelay <= 0)
                        {
                            _board.ControlKeyboard(); // Letting the board be affected by the keyboard.
                            _remainingDelay = _delay; // Resetting the remainingDelay (to check the next turn).
                        }

                        // Detecting a SINGLE user click on the left button:
                        lastMouseState = currentMouseState; // The active state from the last frame is now old.
                        currentMouseState = Mouse.GetState(); // Get the mouse state relevant for this frame.
                        if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
                        {
                            // Detecting user clicks on the restart button:
                            if (_sceneRectangles[2].Contains(Mouse.GetState().Position))
                            {
                                if (!_lockMouse)
                                {
                                    _opponentBoard = new Board(_opponentBoard.boardAI, _opponentBoard.Depth); // Restarting the AI's game.
                                    _board = new Board(); // Restarting the user's game.
                                    _lockMouse = true; // Locking the mouse.
                                }
                            }
                            // Detecting user clicks on the back button:
                            else if (_sceneRectangles[4].Contains(Mouse.GetState().Position))
                            {
                                if (!_lockMouse)
                                {
                                    UnloadScene(); // Unloading the current scene.
                                    _scene = DrawState.Menu; // Loading Menu Scene.
                                    InitializeScene(); // Initializing the Updated scene.
                                    LoadContent(); // Loading the Updated scene.
                                    _lockMouse = true; // Locking the mouse.
                                }
                            }
                        }
                        else
                        {
                            _lockMouse = false;
                        }
                    }
                    // The AI didn't lose yet, which means that we should let it play:
                    else
                    {
                        if (_gameTime == null)
                        {
                            _gameTime = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);
                        }
                        if (gameTime.TotalGameTime.TotalSeconds - _gameTime.TotalGameTime.TotalSeconds > 0.02)
                        {
                            switch (_opponentBoard.boardAI)
                            {
                                case AIState.Randi:
                                    _opponentBoard.RandomAI(); // Loading the view ai screen with a random moves AI.
                                    break;
                                case AIState.Ex:
                                    _opponentBoard.ExpectiMaxAI(); // Loading the view ai screen with the Expectiminimax AI.
                                    break;
                            }
                            _gameTime = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);
                        }
                        

                        // Detecting a SINGLE user click on the left button:
                        lastMouseState = currentMouseState; // The active state from the last frame is now old.
                        currentMouseState = Mouse.GetState(); // Get the mouse state relevant for this frame.
                        if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
                        {
                            // Detecting user clicks on the restart button:
                            if (_sceneRectangles[2].Contains(Mouse.GetState().Position))
                            {
                                if (!_lockMouse)
                                {
                                    _opponentBoard = new Board(_opponentBoard.boardAI, _opponentBoard.Depth); // Restarting the AI's game.
                                    _board = new Board(); // Restarting the player's game.
                                    _lockMouse = true; // Locking the mouse.
                                }
                            }
                            // Detecting user clicks on the back button:
                            else if (_sceneRectangles[4].Contains(Mouse.GetState().Position))
                            {
                                if (!_lockMouse)
                                {
                                    UnloadScene(); // Unloading the current scene.
                                    _scene = DrawState.Menu; // Loading Menu Scene.
                                    InitializeScene(); // Initializing the Updated scene.
                                    LoadContent(); // Loading the Updated scene.
                                    _lockMouse = true; // Locking the mouse.
                                }
                            }
                        }
                        else
                        {
                            _lockMouse = false;
                        }
                    }
                    break;
                #endregion

                #region Update Choose Ai
                case DrawState.ChooseAi:
                    // Detecting a SINGLE user click on the left button:
                    lastMouseState = currentMouseState; // The active state from the last frame is now old.
                    currentMouseState = Mouse.GetState(); // Get the mouse state relevant for this frame.
                    if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
                    {
                        // Detecting user left click on the Randi Button:
                        if (_sceneRectangles[1].Contains(Mouse.GetState().Position))
                        {
                            UnloadScene(); // Unloading the Choose Ai Scene.
                            _scene = DrawState.WatchAi; // Setting the scene to Watch Ai Mode.
                            _board = new Board(); // Creating a new board instance.
                            _board.boardAI = AIState.Randi; // Setting the board's ai to randi.
                            flag = true;
                        }
                        // Detecting user left click on the Ex Button:
                        if (_sceneRectangles[2].Contains(Mouse.GetState().Position))
                        {
                            UnloadScene(); // Unloading the Choose Ai Scene.
                            _scene = DrawState.WatchAi; // Setting the scene to Watch Ai Mode.
                            _board = new Board(AIState.Ex, -1); // Creating a new board instance with an adaptive AI.
                            flag = true;
                        }
                    }
                    if (flag == true)
                    {
                        InitializeScene(); // Initializing the Updated scene.
                        LoadContent(); // Loading the Updated scene.
                        _lockMouse = true; // Locking the mouse.
                    }
                    else
                    {
                        _lockMouse = false;
                    }
                    break;
                #endregion

                #region Update View Ai
                case DrawState.WatchAi:
                    // Calculating how much time passed since the previous AI's move until the time on which the program got here.
                    // We are doing this in order to prevent the AI from performing "multiple" moves at the same time (since there is a delay that
                    // occurs between each movement due to the tiles animated movement):
                    if (_gameTime == null)
                    {
                        _gameTime = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);
                    }
                    if (gameTime.TotalGameTime.TotalSeconds - _gameTime.TotalGameTime.TotalSeconds > 0)
                    {
                        switch (_board.boardAI)
                        {
                            case AIState.Randi:
                                _board.RandomAI(); // Loading the view ai screen with a random moves AI.
                                break;
                            case AIState.Ex:
                                _board.ExpectiMaxAI(); // Loading the view ai screen with the Expectiminimax AI.
                                break;
                        }
                        _gameTime = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);
                    }
                    

                    // Detecting a SINGLE user click on the left button:
                    lastMouseState = currentMouseState; // The active state from the last frame is now old.
                    currentMouseState = Mouse.GetState(); // Get the mouse state relevant for this frame.
                    if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
                    {
                        // Detecting user clicks on the restart button:
                        if (_sceneRectangles[2].Contains(Mouse.GetState().Position))
                        {
                            if (!_lockMouse)
                            {
                                _board = new Board(_board.boardAI, _board.Depth); // Restarting the game.
                                _lockMouse = true; // Locking the mouse.
                            }
                        }
                        // Detecting user clicks on the back button:
                        else if (_sceneRectangles[4].Contains(Mouse.GetState().Position))
                        {
                            if (!_lockMouse)
                            {
                                UnloadScene(); // Unloading the current scene.
                                _scene = DrawState.Menu; // Loading Menu Scene.
                                InitializeScene(); // Initializing the Updated scene.
                                LoadContent(); // Loading the Updated scene.
                                _lockMouse = true; // Locking the mouse.
                            }
                        }
                    }
                    else
                    {
                        _lockMouse = false;
                    }
                    break;
                    #endregion
            }
            
            // Exiting the game in case the user presses Escape:
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Updating the gameTime at the end of each run of this function:
            base.Update(gameTime);
        }
        #endregion

        #region Initializing & Unloading Scene
        /// <summary>
        /// General: Initializing the scene based on the value of _scene.
        /// Process: Using a switch to determine what to initialize when this function is called.
        /// </summary>
        public void InitializeScene()
        {
            _sceneRectangles = new List<Rectangle>(); // Initializing the list of Rectangles.
            _sceneRectangles.Add(new Rectangle(0, 0, 750, 550)); // A rectangle for the background of each scene.
            switch (_scene)
            {
                #region Initializing The Menu Scene:
                case DrawState.Menu:
                    _sceneRectangles.Add(new Rectangle(130, 225, 489, 51)); // Player Mode Button.
                    _sceneRectangles.Add(new Rectangle(130, 330, 489, 51)); // Player Vs Ai Button.
                    _sceneRectangles.Add(new Rectangle(130, 435, 489, 51)); // View Ai Button.
                    break;
                #endregion
                #region Initializing The Player Mode Scene:
                case DrawState.PlayerMode:
                    _sceneRectangles.Add(new Rectangle(180, 335, 180, 60)); // Creating a board rectangle.
                    _sceneRectangles.Add(new Rectangle(664, 20, 66, 66)); // Restart Button.
                    _sceneRectangles.Add(new Rectangle(182, 140, 385, 385)); // Creating a win/loss rectangle.
                    _sceneRectangles.Add(new Rectangle(26, 20, 66, 66)); // Creating a back rectangle.
                    _board = new Board(); // Creating a new board instance.
                    break;
                #endregion
                #region Initializing The Choose Difficulty Scene:
                case DrawState.ChooseDifficulty:
                    _sceneRectangles.Add(new Rectangle(130, 324, 489, 51)); // Creating an "Easy" Button Rectangle.
                    _sceneRectangles.Add(new Rectangle(130, 396, 489, 51)); // Creating an "Hard" Button Rectangle.
                    _sceneRectangles.Add(new Rectangle(130, 468, 489, 51)); // Creating an "Expert" Button Rectangle.
                    break;
                #endregion
                #region Initializing The Player Vs Ai Scene:
                case DrawState.PlayVsAi:
                    _sceneRectangles.Add(new Rectangle(180, 335, 180, 60)); // Creating a board rectangle.
                    _sceneRectangles.Add(new Rectangle(664, 20, 66, 66)); // Restart Button.
                    _sceneRectangles.Add(new Rectangle(182, 140, 385, 385)); // Creating a win/loss rectangle.
                    _sceneRectangles.Add(new Rectangle(26, 20, 66, 66)); // Creating a back rectangle.
                    break;
                #endregion
                #region Initializing The Choose Ai Scene:
                case DrawState.ChooseAi:
                    _sceneRectangles.Add(new Rectangle(131, 326, 489, 51)); // Creating a "Randi" button rectangle.
                    _sceneRectangles.Add(new Rectangle(131, 431, 489, 51)); // Creating an "Ex" button rectangle.
                    break;
                #endregion
                #region Initializing The Watch Ai Scene:
                case DrawState.WatchAi:
                    _sceneRectangles.Add(new Rectangle(180, 335, 180, 60)); // Creating a board rectangle.
                    _sceneRectangles.Add(new Rectangle(664, 20, 66, 66)); // Restart Button.
                    _sceneRectangles.Add(new Rectangle(182, 140, 385, 381)); // Creating a win/loss rectangle.
                    _sceneRectangles.Add(new Rectangle(26, 20, 66, 66)); // Creating a back rectangle.
                    break;
                #endregion
            }
        }
        
        /// <summary>
        /// General: Unloading the textures of the current scene.
        /// Process: Initializing the _sceneTextures variable of the scene to a new list of Texture2Ds.
        /// </summary>
        public void UnloadScene()
        {
            // Initializing the list of Textures so that the SceneManager could load the next scene:
            _sceneTextures = new List<Texture2D>();
        }
        #endregion
    }
}