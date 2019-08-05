using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Server
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Texture2D square;
        private SpriteFont font;
        private Vector2 size;

        Vector2 poz = new Vector2(100, 100);
        private float velocity = 10;
        private NetManager server;

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

            EventBasedNetListener listener = new EventBasedNetListener();
            server = new NetManager(listener);
            server.Start(9050);

            listener.ConnectionRequestEvent += request =>
            {
                if (server.PeersCount < 10 /* max connections */)
                    request.AcceptIfKey("SomeConnectionKey");
                else
                    request.Reject();
            };

            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got connection: {0}", peer.EndPoint); // Show peer ip
                NetDataWriter writer = new NetDataWriter();                 // Create writer class
                writer.Put("Hello client!");                                // Put some string
                peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability
            };


            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                var msg = dataReader.GetString(100 /* max length of string */);
                Console.WriteLine("We got: {0}", msg);
                if (!msg.Contains("Hello"))
                {
                    poz.Y = float.Parse(msg);
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

            font = Content.Load<SpriteFont>("pixelart");
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

            server.PollEvents();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic

            Move();


            base.Update(gameTime);
        }

        private void Move()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                poz.X -= velocity;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                poz.X += velocity;
         

            NetDataWriter writer = new NetDataWriter();                 // Create writer class
            writer.Put($"{poz.X}");                                // Put some string
            server.SendToAll(writer, DeliveryMethod.Unreliable);             // Send with reliability
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            //spriteBatch.Draw(square, new Rectangle((graphics.PreferredBackBufferWidth - 200) / 2, 96, 205, 30), Color.DimGray);
            //spriteBatch.Draw(square, new Rectangle((graphics.PreferredBackBufferWidth - 200) / 2, 96, 200, 25), Color.Black);
            //if (args.Length > 1)
            //    spriteBatch.DrawString(font, args[1], new Vector2((graphics.PreferredBackBufferWidth - size.X) / 2, 100), Color.White);
            //spriteBatch.DrawString(font, text2, new Vector2((graphics.PreferredBackBufferWidth - size2.X) / 2, 75), Color.White);





            spriteBatch.Draw(square, poz, Color.White);

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
