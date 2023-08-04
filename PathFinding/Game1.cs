#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Net;
//using Microsoft.Xna.Framework.Storage;

#endregion

namespace Pathfinder
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        //sprite texture for tiles, player, and ai bot
        Texture2D tile1Texture;
        Texture2D tile2Texture;
        Texture2D aiTexture;
        Texture2D playerTexture;

        //objects representing the level map, bot, and player 
        private Level level;
        private AiBotASTAR bot;
        private Player player;

        //Graph - Representing grid in graph datastructure
        Graph g;
        double[,] graph_matrix;

        int trails;

        //screen size and frame rate
        private const int TargetFrameRate = 50;
        private const int BackBufferWidth = 600;
        private const int BackBufferHeight = 600;

        public Game1()
        {
            //constructor
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "../../Content";
            //set frame rate
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);
            //load level map
            level = new Level();
            level.Loadmap("../../Content/4.txt");


            g = new Graph(level);
            graph_matrix = g.GenerateGraph();

            //instantiate bot and player objects 
            Coord2 botPosition = new Coord2(10, 20);
            Coord2 targetPosition = new Coord2(30, 20);

            player = new Player(targetPosition.X, targetPosition.Y);
            bot = new AiBotASTAR(botPosition, targetPosition, graph_matrix, level.tiles.GetLength(0), level);
            //bot = new AiBotLRTA(graph_matrix, botPosition, targetPosition, level.tiles.GetLength(0));

            /*
            trails = 10;
            for (int i = 0; i < trails; i++)
            {


                //instantiate bot and player objects
                Coord2 botPosition = new Coord2(10, 20);
                Coord2 targetPosition = new Coord2(30, 20);
                

                player = new Player(targetPosition.X, targetPosition.Y);
                bot = new AiBotLRTA(graph_matrix, botPosition, targetPosition, level.tiles.GetLength(0));

                bot.initialization(level, player);
    
                
                if (i != 0)
                {
                    bot.nodeList.Clear();
                    bot.LoadTrailInfo();
                }

                bot.SaveTrailInfo();
                
            }
            */
            
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = BackBufferWidth;
            graphics.PreferredBackBufferHeight = BackBufferHeight;
            Window.Title = "Pathfinder";
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //load the sprite textures
            Content.RootDirectory = "../../Content";
            tile1Texture = Content.Load<Texture2D>("tile1");
            tile2Texture = Content.Load<Texture2D>("tile2");
            aiTexture = Content.Load<Texture2D>("ai");
            playerTexture = Content.Load<Texture2D>("target");
            font = Content.Load<SpriteFont>("StateCost");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //player movement: read keyboard
            KeyboardState keyState = Keyboard.GetState();
            Coord2 currentPos = new Coord2();
            currentPos = player.GridPosition;
            if(keyState.IsKeyDown(Keys.Up))
            {
                currentPos.Y -= 1;
                player.SetNextLocation(currentPos, level);
            }
            else if (keyState.IsKeyDown(Keys.Down))
            {
                currentPos.Y += 1;
                player.SetNextLocation(currentPos, level);
            }
            else if (keyState.IsKeyDown(Keys.Left))
            {
                currentPos.X -= 1;
                player.SetNextLocation(currentPos, level);
            }
            else if (keyState.IsKeyDown(Keys.Right))
            {
                currentPos.X += 1;
                player.SetNextLocation(currentPos, level);
            }   

            //update bot and player
            bot.Update(gameTime, level, player);
            player.Update(gameTime, level);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            //draw level map
            DrawGrid();
            //draw bot
            spriteBatch.Draw(aiTexture, bot.ScreenPosition, Color.White);
            //drawe player
            spriteBatch.Draw(playerTexture, player.ScreenPosition, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGrid()
        {
            //draws the map grid
            int sz = level.GridSize;
            for (int x = 0; x < sz; x++)
            {
                for (int y = 0; y < sz; y++)
                {
                    Coord2 pos = new Coord2((x*15), (y*15));
                    if (level.tiles[x, y] == 0)
                    {
                        if (bot.GetType() == typeof(AiBotASTAR))
                        {
                            if (bot.path.Contains(new Coord2(x, y)))
                            {
                                spriteBatch.Draw(tile1Texture, pos, Color.Red);
                            }
                            else
                            {
                                spriteBatch.Draw(tile1Texture, pos, Color.White);
                            }
                        }
                        else
                        {
                            spriteBatch.Draw(tile1Texture, pos, Color.White);
                        }
                    }
                    else
                    {
                        spriteBatch.Draw(tile2Texture, pos, Color.White);
                    }

                    /*
                    if (bot.GetType() == typeof(AiBotLRTA))
                    {
                        int vertex = bot.GridPosition2Vertex(new Coord2(x, y));
                        LRTANode temp = new LRTANode();
                        temp.stateCost = bot.LRTA_vertex_statecost(vertex);

                        if (temp.stateCost != 0)
                        {
                            spriteBatch.DrawString(font, temp.stateCost.ToString(), new Vector2(pos.X, pos.Y), Color.Black);
                        }
                    }
                    */
                    
                }
            }
        }
    }
}
