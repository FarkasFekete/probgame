using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace probgame
{
    public class Game1 : Game
    {
        // A grafikus eszközök kezelésére szolgáló objektum
        GraphicsDeviceManager graphics;

        // A képek rajzolására szolgáló objektum
        SpriteBatch spriteBatch;

        // A labirintus elrendezését tároló tömb
        int[,] maze;

        // A játékos pozícióját tároló tömb
        int[] player;

        // A fal és a padló képeinek referenciái
        Texture2D wallTexture;
        Texture2D floorTexture;

        // A képek mérete pixelben
        int tileSize = 32;

        // A játéktér mérete mezőben
        int rows = 15;
        int cols = 20;


        float timer;

        // A konstruktor, ami létrehozza a játék objektumot
        public Game1()
        {

            timer = 0f;

            graphics = new GraphicsDeviceManager(this);

            // Beállítja a képernyő méretét a játéktér alapján
            graphics.PreferredBackBufferWidth = tileSize * cols;
            graphics.PreferredBackBufferHeight = tileSize * rows;

            // Betölti a tartalom kezelőt
            Content.RootDirectory = "Content";
        }

        // A játék inicializálására szolgáló metódus
        protected override void Initialize()
        {
            // Létrehozza a labirintus tömböt
            maze = new int[rows, cols];

            // Létrehozza a játékos tömböt
            player = new int[2];

            // Meghívja a labirintus generáló algoritmust
            GenerateMaze(0, 0, rows - 1, cols - 1);

            // Megkeresi a játékos kezdő pozícióját
            FindStartPosition();

            base.Initialize();
        }

        // A tartalom betöltésére szolgáló metódus
        protected override void LoadContent()
        {
            // Létrehozza a SpriteBatch objektumot
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Betölti a fal és a padló képeit
            wallTexture = Content.Load<Texture2D>("fal");
            floorTexture = Content.Load<Texture2D>("padlo");
        }

        // A játék logikáját frissítő metódus
        protected override void Update(GameTime gameTime)
        {

            // Lekérdezi a billentyűzet állapotát
            KeyboardState keyboardState = Keyboard.GetState();

            // Ha a játékos megnyomja az ESC billentyűt, akkor kilép a játékból
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Ha a játékos megnyomja a fel billentyűt, akkor mozog felfelé, ha lehetséges
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                MovePlayerCustom(-1, 0);
            }

            // Ha a játékos megnyomja a le billentyűt, akkor mozog lefelé, ha lehetséges
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                MovePlayerCustom(1, 0);
            }

            // Ha a játékos megnyomja a bal billentyűt, akkor mozog balra, ha lehetséges
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                MovePlayerCustom(0, -1);
            }

            // Ha a játékos megnyomja a jobb billentyűt, akkor mozog jobbra, ha lehetséges
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                MovePlayerCustom(0, 1);
            }

            base.Update(gameTime);
        }

        // A billentyűzet eseményeit kezelő metódus
        //void KeyboardHandler(object sender, KeyboardEventArgs e)
        //{
        //    // Ha a játékos megnyomja az ESC billentyűt, akkor kilép a játékból
        //    if (e.Key == Keys.Escape && e.State == KeyState.Down)
        //    {
        //        Exit();
        //    }

        //    // Ha a játékos megnyomja a fel billentyűt, akkor mozog felfelé, ha lehetséges
        //    if (e.Key == Keys.Up && e.State == KeyState.Down)
        //    {
        //        MovePlayerCustom(-1, 0);
        //    }

        //    // Ha a játékos felengedi a fel billentyűt, akkor megáll
        //    if (e.Key == Keys.Up && e.State == KeyState.Up)
        //    {
        //        MovePlayerCustom(0, 0);
        //    }

        //    // Ha a játékos megnyomja a le billentyűt, akkor mozog lefelé, ha lehetséges
        //    if (e.Key == Keys.Down && e.State == KeyState.Down)
        //    {
        //        MovePlayerCustom(1, 0);
        //    }

        //    // Ha a játékos felengedi a le billentyűt, akkor megáll
        //    if (e.Key == Keys.Down && e.State == KeyState.Up)
        //    {
        //        MovePlayerCustom(0, 0);
        //    }

        //    // Ha a játékos megnyomja a bal billentyűt, akkor mozog balra, ha lehetséges
        //    if (e.Key == Keys.Left && e.State == KeyState.Down)
        //    {
        //        MovePlayerCustom(0, -1);
        //    }

        //    // Ha a játékos felengedi a bal billentyűt, akkor megáll
        //    if (e.Key == Keys.Left && e.State == KeyState.Up)
        //    {
        //        MovePlayerCustom(0, 0);
        //    }

        //    // Ha a játékos megnyomja a jobb billentyűt, akkor mozog jobbra, ha lehetséges
        //    if (e.Key == Keys.Right && e.State == KeyState.Down)
        //    {
        //        MovePlayerCustom(0, 1);
        //    }

        //    // Ha a játékos felengedi a jobb billentyűt, akkor megáll
        //    if (e.Key == Keys.Right && e.State == KeyState.Up)
        //    {
        //        MovePlayerCustom(0, 0);
        //    }
        //}

        // A játék grafikáját rajzoló metódus
        protected override void Draw(GameTime gameTime)
        {
            // Kitölti a képernyőt fekete színnel
            GraphicsDevice.Clear(Color.Black);

            // Elkezdi a SpriteBatch rajzolását
            spriteBatch.Begin();

            // Végigmegy a labirintus tömbön, és kirajzolja a megfelelő képeket
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // Kiszámolja a kép pozícióját a képernyőn
                    int x = j * tileSize;
                    int y = i * tileSize;

                    // Ha a tömb értéke 1, akkor falat rajzol
                    if (maze[i, j] == 1)
                    {
                        spriteBatch.Draw(wallTexture, new Rectangle(x, y, tileSize, tileSize), Color.White);
                    }
                    // Ha a tömb értéke 0, akkor padlót rajzol
                    else if (maze[i, j] == 0)
                    {
                        spriteBatch.Draw(floorTexture, new Rectangle(x, y, tileSize, tileSize), Color.White);
                    }
                }
            }

            // Kirajzolja a játékost a megfelelő pozícióban
            spriteBatch.Draw(floorTexture, new Rectangle(player[1] * tileSize, player[0] * tileSize, tileSize, tileSize), Color.Red);

            // Befejezi a SpriteBatch rajzolását
            spriteBatch.End();

            base.Draw(gameTime);
        }

        // A labirintus generáló algoritmus, ami a rekurzív osztás módszerét használja
        void GenerateMaze(int top, int left, int bottom, int right)
        {
            // Ellenőrzi, hogy a rész legalább 2x2-es méretű-e
            if (bottom - top < 2 || right - left < 2)
            {
                // Ha nem, akkor nem osztja tovább
                return;
            }

            // Véletlenszerűen kiválasztja, hogy függőleges vagy vízszintes falat húz
            bool vertical = new Random().Next(2) == 0;

            // Véletlenszerűen kiválasztja, hogy a fal hol legyen
            int wall;

            // Véletlenszerűen kiválasztja, hogy a falon hol legyen a nyílás
            int hole;

            if (vertical)
            {
                // Függőleges fal esetén
                // A fal oszlopát a bal és a jobb határ között választja
                wall = new Random().Next(left + 1, right);

                // A nyílás sorát a felső és az alsó határ között választja
                hole = new Random().Next(top, bottom + 1);

                // Végigmegy a fal oszlopán, és beállítja a tömb értékét 1-re, kivéve a nyílás helyén
                for (int i = top; i <= bottom; i++)
                {
                    if (i != hole)
                    {
                        maze[i, wall] = 1;
                    }
                }

                // Rekurzívan meghívja a függvényt a fal két oldalán lévő részekre
                GenerateMaze(top, left, bottom, wall - 1);
                GenerateMaze(top, wall + 1, bottom, right);
            }
            else
            {
                // Vízszintes fal esetén
                // A fal sorát a felső és az alsó határ között választja
                wall = new Random().Next(top + 1, bottom);

                // A nyílás oszlopát a bal és a jobb határ között választja
                hole = new Random().Next(left, right + 1);

                // Végigmegy a fal során, és beállítja a tömb értékét 1-re, kivéve a nyílás helyén
                for (int j = left; j <= right; j++)
                {
                    if (j != hole)
                    {
                        maze[wall, j] = 1;
                    }
                }

                // Rekurzívan meghívja a függvényt a fal két oldalán lévő részekre
                GenerateMaze(top, left, wall - 1, right);
                GenerateMaze(wall + 1, left, bottom, right);
            }
        }
            // A játékos kezdő pozíciójának megkeresésére szolgáló metódus
            void FindStartPosition()
            {
                // Véletlenszerűen kiválaszt egy sort és egy oszlopot
                int row = new Random().Next(0, rows);
                int col = new Random().Next(0, cols);

                // Ellenőrzi, hogy a cella járható-e
                if (maze[row, col] == 0)
                {
                    // Ha igen, akkor beállítja a játékos pozícióját
                    player[0] = row;
                    player[1] = col;
                }
                else
                {
                    // Ha nem, akkor újra próbálkozik
                    FindStartPosition();
                }
            }
            // A játékos mozgatására és ütközésére szolgáló metódus
            void MovePlayerCustom(int rowOffset, int colOffset)
            {
                // Kiszámolja a játékos célpozícióját a megadott irány alapján
                int targetRow = player[0] + rowOffset;
                int targetCol = player[1] + colOffset;

                // Ellenőrzi, hogy a célpozíció a játéktéren belül van-e
                if (targetRow >= 0 && targetRow < rows && targetCol >= 0 && targetCol < cols)
                {
                    // Ellenőrzi, hogy a célpozíció járható-e
                    if (maze[targetRow, targetCol] == 0)
                    {
                        // Ha igen, akkor frissíti a játékos pozícióját
                        player[0] = targetRow;
                        player[1] = targetCol;
                    }
                }
            }
    }
}
