using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AndroidWindows
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MonoAndroid : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D backgroundTexture;
        Player player;
        KeyboardState prevousKbState;
        List<Explosion> explosionList = new List<Explosion>();
        List<Shot> shotList = new List<Shot>();
        Texture2D laserTexture;

        List<Meteor> meteorList = new List<Meteor>();
        Texture2D meteorBigTexture;
        Texture2D meteorMediumTexture;
        Texture2D meteorSmallTexture;
        SoundEffect laserSound;
        SoundEffect explosionSound;
        Texture2D explosionTexture;

        Random rnd = new Random();

        public MonoAndroid()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = Globals.ScreenHeight;
            graphics.PreferredBackBufferWidth = Globals.ScreenWidth;
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
            player = new Player(this);
            Components.Add(player);
            ResetMeteors();

            base.Initialize();
        }

        public void ResetMeteors()
        {
            while ( meteorList.Count < 10)
            {
                var angle = rnd.Next() * MathHelper.TwoPi;
                var m = new Meteor(MeteorType.Big)
                {
                    Position = new Vector2(Globals.GameArea.Left + (float)rnd.NextDouble() * Globals.GameArea.Width,
                    Globals.GameArea.Top + (float)rnd.NextDouble() * Globals.GameArea.Height),
                    Rotation = angle,
                    Speed = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * rnd.Next(20, 60) / 30.0f
                };
                if (!Globals.RespawnArea.Contains(m.Position))
                    meteorList.Add(m);
            }
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundTexture = Content.Load<Texture2D>("background");
            laserTexture = Content.Load<Texture2D>("laser");
            meteorBigTexture = Content.Load<Texture2D>("BrownMeteor_Big");
            meteorMediumTexture = Content.Load<Texture2D>("BrownMeteor_Medium");
            meteorSmallTexture = Content.Load<Texture2D>("BrownMeteor_Small");


            laserSound = Content.Load<SoundEffect>("laserSound");
            explosionSound = Content.Load<SoundEffect>("explosionSound");
            explosionTexture = Content.Load<Texture2D>("explosion");

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            KeyboardState state = Keyboard.GetState();
            
            if (state.IsKeyDown(Keys.Up))
            {
                player.Accelerate();
            }
            if (state.IsKeyDown(Keys.Left))
            {
                player.Rotation -= 0.05f;
            }
            if (state.IsKeyDown(Keys.Down))
            {
                player.Break();
            }
            else if (state.IsKeyDown(Keys.Right))
            {
                player.Rotation += 0.05f;
            }

            if (state.IsKeyDown(Keys.Space))
            {
                Shot s = player.Shoot();
                if (s != null)
                {
                    laserSound.Play();
                    shotList.Add(s);

                }
            }


            foreach (Shot shot in shotList)
            {
                shot.Update(gameTime);
                Meteor meteor = meteorList.FirstOrDefault(m => m.CollidesWith(shot));
                if(meteor!= null)
                {
                    meteorList.Remove(meteor);
                    meteorList.AddRange(Meteor.BreakMeteor(meteor));
                    explosionList.Add(new Explosion() {
                        Position = meteor.Position,
                        Scale = meteor.ExplosionScale });
                    shot.IsDead = true;
                    explosionSound.Play();
                }
            }

            foreach (Explosion explosion in explosionList)
                explosion.Update(gameTime);


            foreach (Meteor meteor in meteorList)
            {
                meteor.Update(gameTime);
            }

            shotList.RemoveAll(s => s.IsDead || !Globals.GameArea.Contains(s.Position));

            explosionList.RemoveAll(e => e.IsDead);
            

            player.Update(gameTime);
            prevousKbState = state;
            // TODO: Add your update logic here

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

            for(int y=0; y<Globals.ScreenHeight; y+= backgroundTexture.Height)
            {
                for(int x= 0; x<Globals.ScreenWidth; x+= backgroundTexture.Width)
                {
                    spriteBatch.Draw(backgroundTexture, new Vector2(x, y), Color.White);
                }
            }



            player.Draw(spriteBatch);

            foreach (Shot s in shotList)
            {
                spriteBatch.Draw(laserTexture, s.Position, null, Color.White, s.Rotation, new Vector2(laserTexture.Width / 2, laserTexture.Height / 2), 1.0f, SpriteEffects.None, 0f);
            }

            foreach(Meteor meteor in meteorList)
            {
                Texture2D meteorTexture = meteorSmallTexture;
                switch(meteor.Type)
                {
                    case MeteorType.Big: meteorTexture = meteorBigTexture; break;
                    case MeteorType.Medium: meteorTexture = meteorMediumTexture; break;
                }
                spriteBatch.Draw(meteorTexture, meteor.Position, null, Color.White, meteor.Rotation,
                    new Vector2(meteorTexture.Width / 2, meteorTexture.Height / 2), 1.0f, SpriteEffects.None, 0f);
            }

            foreach(Explosion explosion in explosionList)
            {
                spriteBatch.Draw(explosionTexture, explosion.Position, null, explosion.Color, explosion.Rotation,
                    new Vector2(explosionTexture.Width / 2, explosionTexture.Height / 2), explosion.Scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
