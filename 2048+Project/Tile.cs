using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace _2048_Project
{
    // A public enum which will help us define during the game at which state the current tile is:
    public enum TileState
    {
        Normal,
        Upgrade,
        Delete
    }
    public class Tile
    {
        #region Variables Definition
        // _size = Size of the tile's "Physical Body", _gap = Distance between each tile on the board,
        // _boardGap = Distance between board boundaries to tiles.
        private Point _size = new Point(80, 80), _gap = new Point(11, 12), _boardGap = new Point(12, 13);
        private Rectangle _tileRect; // A variable to represent the "physical body" of the tile.
        private int _value = 0; // A variable to store the tile's value.
        private TileState _tileState; // A variable to represent the tile's state.
        public Point from, to; // Defining the points from where we move the tile and to where we move it.
        
        public static SpriteBatch spriteBatch; // Drawing with the same spriteBatch from the SceneManager.
        public static int ticks = 0; // A counter to "animate" every tile's movement by moving each tile maxTicks times.
        public static int maxTicks = 8; // The amount of times we want to "animate" the tile's movement.
        private static Dictionary<int, Texture2D> _tilesTextures; // A dictionary to store tiles textures.
        #endregion

        #region Getters & Setters
        // Defining necessary Getters & Setters:
        public int Value { get => _value; set => _value = value; }
        public TileState TileState { get => _tileState; set => _tileState = value; }

        // A Setter to the textures of the tiles:
        public static void SetTextures(Dictionary<int, Texture2D> dict)
        {
            _tilesTextures = dict;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// General: A Constructor to the Tile class.
        /// </summary>
        /// <param name="point"> A point by which we will initialize the Tile class. </param>
        public Tile(Point point)
        {
            Random rnd = new Random(); // Defining a random instance to randomly choose a value for a new tile.
           
            // Choosing a random value for the new tile (90% - 2, 10% - 4):
            _value = rnd.Next(0, 10) < 9 ? 2 : 4;

            // Initializing the to & from instances
            from = new Point(point.X, point.Y);
            to = new Point(point.X, point.Y);

            _tileRect = new Rectangle(Board.offset, this._size);

            // Setting the state of the block to normal:
            _tileState = TileState.Normal;

            // Creating a "physical body" for the tile:
            GetRect();
        }
        #endregion

        #region Helping Functions
        /// <summary>
        /// General: Upgrading a tile.
        /// Process: Changing the tile's state to normal & it's value to double it's previous value.
        /// </summary>
        /// <returns> Returns the value of the upgraded tile. </returns>
        public int Upgrade()
        {
            Value = Value << 1; // value = value * 2
            TileState = TileState.Normal; // Setting the tile state to normal.
            return Value; // Returning the value from the upgrade function (the value of the tile).
        }
        
        /// <summary>
        /// General: Initializing the positions of the tile's X and Y values.
        /// Process: Deciding where to "put" the tile based on the _gap, _size, _boardGap and tiles animated move.
        /// </summary>
        /// <returns> Returns the rectangle created for the new tile. </returns>
        public Rectangle GetRect()
        {
            // _boardGap = the gap from the board sides.
            // Board.offset.x / Board.offset.y = The locations of the board in the background.
            // The rest = Calculations to get the position of the tile in the board:
            _tileRect.X = _boardGap.X + Board.offset.X + (from.Y * (maxTicks - ticks) + to.Y * ticks) * (_size.X + _gap.X) / maxTicks;
            _tileRect.Y = _boardGap.Y + Board.offset.Y + (from.X * (maxTicks - ticks) + to.X * ticks) * (_size.Y + _gap.Y) / maxTicks;
            return _tileRect;
        }
        
        /// <summary>
        /// General: Drawing the image of the tile which matches the value of the current tile.
        /// </summary>
        public void Draw()
        {
            spriteBatch.Draw(_tilesTextures[Value], GetRect(), Color.White);
        }

        /// <summary>
        /// General: Checking whether a given tile equals to the current tile.
        /// Process: Checking whether both of the tiles values are equal to each other's value and whether their state is equal to normal.
        /// </summary>
        /// <param name="other"> Another tile to check if it's equal to the current tile or not. </param>
        /// <returns></returns>
        public bool Equals(Tile other)
        {
            return this.Value == other.Value && this.TileState == other.TileState
                && this.TileState == TileState.Normal;
        }
        #endregion
    }
}