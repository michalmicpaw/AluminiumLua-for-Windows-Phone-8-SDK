using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Resources;
using System.Text;

namespace WindowsPhoneGame3
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static AluminumLua.LuaParser prs;
        public static AluminumLua.LuaContext ctx;
        public static bool runAtStartup = false;

        public static Color backgroundColor;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;

            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
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

            base.Initialize();
        }

        private string returnStringFile(string resName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var strResources = "WindowsPhoneGame3." + resName;
            //var strResources = assembly.GetName().Name + "." +  resName;

            var rStream = assembly.GetManifestResourceStream(strResources);

            System.Diagnostics.Debug.WriteLine(resName);

            var streamReader = new StreamReader(rStream);

            string str = streamReader.ReadToEnd();

            System.Diagnostics.Debug.WriteLine(str + "\n-------------");

            return str;
        }

        private void SaveFileToIsoStore(IsolatedStorageFile iso, string[] files)
        {
            foreach (var path in files)
            {
                var fileName = Path.GetFileName(path);

                System.Diagnostics.Debug.WriteLine(fileName + "++++++++++++++++");

                if (iso.FileExists(fileName))
                    iso.DeleteFile(fileName);

                IsolatedStorageFileStream strumien = iso.CreateFile(fileName);

                byte[] bytes = Encoding.UTF8.GetBytes(returnStringFile(fileName));

                strumien.Write(bytes, 0, bytes.Length);

                strumien.Close();
            }
        }

        public static AluminumLua.LuaObject changeBackgoundColor(AluminumLua.LuaObject[] args)
        {
            if (args.Length == 3)
            {
                foreach (var component in args)
                {
                    if (component.Type != AluminumLua.LuaType.number)
                        prs.Err("changeBackgroundColor Error: bad argument (not a number)");

                    double whole = Math.Round(component.AsNumber());
                    double rest = component.AsNumber() - whole;

                    if ((component.AsNumber() < 0) || (component.AsNumber() > 255))
                        prs.Err("changeBackgroundColor Error: bad argument ( < 0) or (255 < )");

                    else if (Math.Abs(rest) > 0)
                        prs.Err("changeBackgroundColor Error: bad argument (floating point)");
                }

                backgroundColor = new Color((int)(args[0].AsNumber()), (int)(args[1].AsNumber()), (int)(args[2].AsNumber()));
            }

            else
                prs.Err("changeBackgroundColor Error: bad number of arguments");

            return true;
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundColor = new Color(0, 0, 0);

            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();

            SaveFileToIsoStore(iso, new[] { "main.lua" });

            ctx = new AluminumLua.LuaContext();
            ctx.AddBasicLibrary();
            ctx.AddIoLibrary();

            ctx.SetGlobal("setBgColor", changeBackgoundColor);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);

            if (runAtStartup == false)
            {
                runAtStartup = true;

                try
                {
                    prs = new AluminumLua.LuaParser(ctx, "main.lua");
                    prs.Parse();
                }

                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
