using System.Diagnostics;

namespace SnakeGame
{
    internal class Snake
    {
        public static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Title = "Змейка";

            string[] file = File.ReadAllLines("map.txt");

            Game game = new Game();

            char[,] map = game.GetMap(file);

            ConsoleKeyInfo pressedKey = new ConsoleKeyInfo();

            Task.Run(() =>
            {
                while (true)
                {
                    pressedKey = Console.ReadKey(true);
                }
            });

            Task.Run(() =>
            {
                while (true)
                {
                    game.Spawn(map);
                }
            });


            bool Over = false;

            int setX = 4;
            int setY = 4;

            int[] bX = new int[51];
            bX[0] = 4;
            int[] bY = new int[51];
            bY[0] = 4;
            int parts = 2;

            string text = "Нажмите любую стрелку чтобы начать! || Очки: ";
            int score = 24;


            while (true)
            {
                while (!Over)
                {
                    game.Begin(map, game);

                    game.DrawHead(ref setY, ref setX, pressedKey, game, ref file, ref bX, ref bY);

                    game.Special(pressedKey, ref parts, ref score, ref bX, ref bY, ref map, ref Over);

                    game.AllParts(ref bX, ref bY, ref parts);
                    game.DrawBody(pressedKey, ref bX, ref bY, ref parts, ref map, ref text);

                    game.DrawScore(text, score, pressedKey, ref setX, ref setY);

                    game.Win(ref score, ref Over);

                    Thread.Sleep(250);
                }

                game.Menu(ref pressedKey, ref Over, ref bX, ref bY, ref setX, ref setY, ref parts, ref score, ref map, file, game);
            }
        }
    }

    class Game
    {
        public void Win(ref int score, ref bool Over)
        {
            if (score == 25)
            {
                Over = true;

                Console.Beep(350, 60);
                Console.Beep(390, 60);
                Console.Beep(435, 90);
                Console.Beep(545, 800);

                Console.SetCursorPosition(0, 10);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Победа!!!     ");

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("\nR - заново\nQ - выйти");
            }
        }
        public void Menu(ref ConsoleKeyInfo pressedKey, ref bool Over, ref int[] bX, ref int[] bY, ref int setX, ref int setY, ref int parts, ref int score, ref char[,] map, string[] file, Game game)
        {

            while (pressedKey.Key != ConsoleKey.R || pressedKey.Key != ConsoleKey.Q)
            {
                if (pressedKey.Key == ConsoleKey.R)
                {
                    Console.Clear();
                    bX[0] = 4;
                    bY[0] = 4;
                    setX = 4;
                    setY = 4;
                    parts = 2;
                    score = 0;
                    map = game.GetMap(file);
                    Over = false;
                    break;
                }

                else if (pressedKey.Key == ConsoleKey.Q)
                    System.Environment.Exit(-1);
            }
        }

        public char[,] GetMap(string[] file)
        {
            char[,] map = new char[9, 9];

            for (int i = 0; i < file.Length; i++)
            {
                for (int j = 0; j < file[0].Length; j++)
                {
                    map[i, j] = file[i][j];
                }
            }

            return map;
        }
        public void DrawHead(ref int setY, ref int setX, ConsoleKeyInfo pressedKey, Game game, ref string[] file, ref int[] bX, ref int[] bY)
        {
            Console.SetCursorPosition(game.MoveX(pressedKey, file, ref setX, ref setY, ref bX), game.MoveY(pressedKey, file, ref setX, ref setY, ref bY));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("☻");
        }

        public void DrawScore(string text, int score, ConsoleKeyInfo pressedKey, ref int setX, ref int setY)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            if (pressedKey.Key == ConsoleKey.DownArrow || pressedKey.Key == ConsoleKey.UpArrow ||
            pressedKey.Key == ConsoleKey.RightArrow || pressedKey.Key == ConsoleKey.LeftArrow)
                text = "Очки:";
            Console.SetCursorPosition(0, 9);

            Console.Write($"{text} {score}                                         ");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.SetCursorPosition(10, 0);
            Console.Write("Пробел - пауза");
            Console.SetCursorPosition(10, 1);
            Console.Write("Двойное нажатие на стрелку - продожить");


            Console.SetCursorPosition(0, 12);
        }

        public void Begin(char[,] map, Game game)
        {
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            game.DrawMap(map);
        }

        public void DrawMap(char[,] map)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == '♥')
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(map[i, j]);
                        if (map[i + 1, j] != '♥' || map[i, j + 1] != '♥')
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }

                    else if (map[i, j] == '*' && (i + j) % 2 == 0)
                    {

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(map[i, j]);
                        if (map[i + 1, j] != '*' || map[i, j + 1] != '*')
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }

                    else if (map[i, j] == '*' && (i + j) % 2 != 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(map[i, j]);
                        if (map[i + 1, j] != '*' || map[i, j + 1] != '*')
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }

                    else
                    {
                        Console.Write(map[i, j]);
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }
                }
                Console.WriteLine();
            }
        }


        public int MoveX(ConsoleKeyInfo pressedKey, string[] file, ref int x, ref int y, ref int[] bX)
        {

            if (pressedKey.Key == ConsoleKey.RightArrow && file[x + 1][y] != '█')
            {
                x++;
                bX[0]++;
            }
            else if (pressedKey.Key == ConsoleKey.LeftArrow && file[x - 1][y] != '█')
            {
                x--;
                bX[0]--;
            }
            else if (file[x + 1][y] == '█' && (pressedKey.Key != ConsoleKey.UpArrow && pressedKey.Key != ConsoleKey.DownArrow && pressedKey.Key != ConsoleKey.Spacebar))
            {
                x = 1;
                bX[0] = 1;
            }
            else if (file[x - 1][y] == '█' && (pressedKey.Key != ConsoleKey.UpArrow && pressedKey.Key != ConsoleKey.DownArrow && pressedKey.Key != ConsoleKey.Spacebar))
            {
                x = 7;
                bX[0] = 7;
            }
            return x;
        }

        public int MoveY(ConsoleKeyInfo pressedKey, string[] file, ref int x, ref int y, ref int[] bY)
        {
            if (pressedKey.Key == ConsoleKey.UpArrow && file[x][y - 1] != '█')
            {
                y--;
                bY[0]--;
            }
            else if (pressedKey.Key == ConsoleKey.DownArrow && file[x][y + 1] != '█')
            {
                y++;
                bY[0]++;
            }
            else if (file[x][y + 1] == '█' && (pressedKey.Key != ConsoleKey.LeftArrow && pressedKey.Key != ConsoleKey.RightArrow && pressedKey.Key != ConsoleKey.Spacebar))
            {
                y = 1;
                bY[0] = 1;
            }
            else if (file[x][y - 1] == '█' && (pressedKey.Key != ConsoleKey.LeftArrow && pressedKey.Key != ConsoleKey.RightArrow && pressedKey.Key != ConsoleKey.Spacebar))
            {
                y = 7;
                bY[0] = 7;
            }
            return y;
        }


        public void Special(ConsoleKeyInfo pressedKey, ref int parts, ref int score, ref int[] bX, ref int[] bY, ref char[,] map, ref bool Over)
        {

            if (map[bY[0], bX[0]] == '♥')
            {
                score++;
                parts++;
                Console.Beep(600, 90);
                map[bY[0], bX[0]] = ' ';
            }

            if (map[bY[0], bX[0]] == '*')
            {
                Over = true;

                if (pressedKey.Key == ConsoleKey.Spacebar)
                    while (Over)
                    {
                        if (pressedKey.Key != ConsoleKey.DownArrow || pressedKey.Key != ConsoleKey.UpArrow ||
            pressedKey.Key != ConsoleKey.RightArrow || pressedKey.Key != ConsoleKey.LeftArrow)
                        {
                            Over = false;
                            break;
                        }
                    }

                if (Over == true)
                {
                    Console.Beep(290, 300);
                    Console.Beep(275, 500);
                    Console.Beep(255, 500);
                    Console.Beep(245, 700);

                    Console.SetCursorPosition(0, 10);

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Поражение     ");

                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("\nR - заново\nQ - выйти");
                }
            }
        } //тайлер пидор а вы знаете что смешав сперму с апельсиновым соком то вы будете как еблан? нет я не знал. а это правда? правда. всякую хуйню можно творить из подручных материалов.неужели??????? если захотите) пидор вы знаете.. вы.. вы самый интересный из всех моих одноразовых друзей♡ ведь в самолётах все одноразовое даже люди. о, ясно. пошел нахуй. спасибо. и вам это нравится? что? быть пидором     

        public void Spawn(char[,] map)
        {
            int empty = 0;

            Random rand = new Random();
            int randX = rand.Next(1, 8);
            int randY = rand.Next(1, 8);


            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == ' ' || map[i, j] == '*')
                    {
                        empty++;
                        if (empty == 49)
                        {
                            while (map[randX, randY] != ' ' && map[randX, randY] == '*')
                            {
                                randX = rand.Next(1, 8);
                                randY = rand.Next(1, 8);
                            }

                            if (map[randX, randY] == ' ' && map[randX, randY] != '*' && (randX != 4 && randY != 4))
                                map[randX, randY] = '♥';
                        }
                    }
                }
            }
        }
        public void AllParts(ref int[] bX, ref int[] bY, ref int parts)
        {
            for (int i = parts; i > 1; i--)
            {
                bX[i - 1] = bX[i - 2];
                bY[i - 1] = bY[i - 2];
            }
        }
        public void DrawBody(ConsoleKeyInfo pressedKey, ref int[] bX, ref int[] bY, ref int parts, ref char[,] map, ref string text)
        {
            if (pressedKey.Key == ConsoleKey.Spacebar)
                Console.ReadKey(true);

            for (int i = 2; i < parts; i++)
            {
                map[bY[i], bX[i]] = ' ';
                map[bY[i - 1], bX[i - 1]] = '*';
            }
        }
    }
}