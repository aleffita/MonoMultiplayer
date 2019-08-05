using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Client
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Texture2D square;
        private NetManager client;
        private Vector2 poz;
        private float velocity = 10;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            EventBasedNetListener listener = new EventBasedNetListener();
            client = new NetManager(listener);
            client.Start();
            client.Connect("177.52.28.212" /* host ip or name */, 9050 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);
            poz.Y = 100;
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                var msg = dataReader.GetString(100 /* max length of string */);
                Console.WriteLine("We got: {0}", msg);
                if (!msg.Contains("Hello"))
                {
                    poz.X = float.Parse(msg);
                }
               

                dataReader.Recycle();
            };

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            square = Content.Load<Texture2D>("square");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            client.PollEvents();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                poz.Y -= velocity;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                poz.Y += velocity;
            // TODO: Add your update logic here

            NetDataWriter writer = new NetDataWriter();                 // Create writer class
            writer.Put($"{poz.Y}");                                // Put some string
            client.SendToAll(writer, DeliveryMethod.Unreliable);             // Send with reliability

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            // TODO: Add your drawing code here
            spriteBatch.Draw(square, poz, Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
