using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tower_defence.Towers;

namespace tower_defence
{
    public class Program
    {
        static int level;
        static int coins;
        static int lives = 3;

        static char[,] map;
        static HashSet<(int x, int y)> path;

        static List<AbstractTower> towers = new List<AbstractTower>();

        static Dictionary<int, int> towerRanges = new Dictionary<int, int>()
        {
            { 1, 3 },
            { 2, 5 },
            { 3, 7 },
            { 4, 10}
        };

        #region DLLs
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_MAXIMIZE = 3;
        #endregion

        static void Main(string[] args)
        {
            IntPtr handle = GetConsoleWindow();
            ShowWindow(handle, SW_MAXIMIZE);
            coins = 3;
            level = 1;

            // load map from txt file
            map = new char[50, 50];
            using (StreamReader sr = new StreamReader("map.txt"))
            {
                for (int i = 0; i < 50; i++)
                {
                    string line = sr.ReadLine();
                    for (int j = 0; j < 50; j++)
                    {
                        map[i, j] = line[j];
                    }
                }
            }

            // find path of X
            path = new HashSet<(int x, int y)>();
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    if (map[i, j] == 'X')
                    {
                        path.Add((i, j));
                    }
                }
            }

            Console.SetBufferSize(300, 300);

            (int x, int y) coord;
            coord.x = 0;
            coord.y = 0;

            printMap();
            printUI();

            bool yay = true;
            while (yay)
            {
                yay = runGame();
            }


            // Console.ReadKey();
        }
        static (int, int) towerPos(int range)
        {
            printMap();
            ConsoleKeyInfo cki;
            (int x, int y) pos = (25, 25);
            (int, int) prevPos = (25, 25);
            do
            {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.UpArrow && pos.y > 0)
                {
                    pos = (pos.x, pos.y - 1);
                }
                else if (cki.Key == ConsoleKey.DownArrow && pos.y < 49)
                {
                    pos = (pos.x, pos.y + 1);
                }
                else if (cki.Key == ConsoleKey.LeftArrow && pos.x > 0)
                {
                    pos = (pos.x - 1, pos.y);
                }
                else if (cki.Key == ConsoleKey.RightArrow && pos.y < 49)
                {
                    pos = (pos.x + 1, pos.y);
                }
                printTowerRange(range, pos, prevPos);
                prevPos = pos;
            } while (cki.Key != ConsoleKey.Enter);
            // check if position is valid
            if (!path.Contains((pos.y, pos.x)))
            {
                return pos;
            }
            else
            {
                print(120, 16, "Invalid placement!");
                Thread.Sleep(1000);
                print(120, 16, "                ");
                return towerPos(range);
            }
        }
        static bool runGame()
        {
            // run game loop
            printUI();

            #region purchase towers
            // purchase towers
            int purchase = 0;
            do
            {
                purchase = purchaseMenu();
                print(120, 14, "                                                            ");
                print(120, 15, "                           ");
                printUI();
                int range = 0;
                (int x, int y) pos = (0, 0);
                if (purchase != 0)
                {
                    range = towerRanges[purchase];
                    pos = towerPos(range);
                }

                // tower position input


                // create tower

                switch (purchase)
                {
                    case 1:
                        towers.Add(new Towers.Archer(pos));
                        break;
                    case 2:
                        towers.Add(new Towers.Cannon(pos));
                        break;
                    case 3:
                        towers.Add(new Towers.Mage(pos));
                        break;
                    case 4:
                        towers.Add(new Towers.Ballista(pos));
                        break;
                    default:
                        break;
                }
                printMap();
            }
            while (purchase != 0 && coins > 0);



            #endregion
            printUI();
            ReleaseEnemies(2 * level, 9, (0, 19));
            if (lives <= 0)
            {
                print(120, 14, "Game Over! Press any key to exit");
                Console.ReadKey();
                return false;
            }
            coins += level;
            level++;
            return true;
        }
        static void printMap()
        {
            // print map allowing for character being twice as tall as it is wide
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    Console.SetCursorPosition(j * 2, i);
                    Console.Write(map[i, j]);
                }
            }
            foreach (AbstractTower tower in towers)
            {
                tower.printTower();
            }
        }
        static void printTowerRange(int range, (int x, int y) pos, (int x, int y) prevPos)
        {
            // for each position in range, print a circle

            for (int i = pos.y - range - 1; i <= pos.y + range + 1; i++)
            {
                for (int j = pos.x - range - 1; j <= pos.x + range + 1; j++)
                {
                    if (Math.Abs(i - pos.y) + Math.Abs(j - pos.x) <= range || pos == (j, i))
                    {
                        try
                        {
                            printHighlight(j * 2, i, map[i, j].ToString());
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            // ignore out of range
                        }
                    }
                    // unprint previous position with full range
                    else//if (Math.Abs(i - prevPos.y) + Math.Abs(j - prevPos.x) <= range)
                    {
                        try
                        {
                            print(j * 2, i, map[i, j].ToString());
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            // ignore out of range
                        }
                    }

                }
            }
            // reprint towers
            foreach (AbstractTower tower in towers)
            {
                tower.printTower();
            }
        }
        static void printShop(int highlightIndex)
        {
            print(120, 4, "---------------------------------------");
            print(120, 5, "|   Buy Towers                        |");
            print(120, 6, "|                                     |");
            if (highlightIndex == 1) { printHighlight(120, 7, "|   > Archer            |   1 coin    |"); }
            else { print(120, 7, "|   > Archer            |   1 coin    |"); }
            if (highlightIndex == 2) { printHighlight(120, 8, "|   > Cannon            |   2 coins   |"); }
            else { print(120, 8, "|   > Cannon            |   2 coins   |"); }
            if (highlightIndex == 3) { printHighlight(120, 9, "|   > Mage              |   3 coins   |"); }
            else { print(120, 9, "|   > Mage              |   3 coins   |"); }
            if (highlightIndex == 4) { printHighlight(120, 10, "|   > Ballista          |   5 coins   |"); }
            else { print(120, 10, "|   > Ballista          |   5 coins   |"); }
            print(120, 11, "|                                     |");
            print(120, 12, "---------------------------------------");
        }
        static int purchaseMenu()
        {
            ConsoleKeyInfo cki;
            int choice = 1;
            printShop(choice);
            print(120, 14, "Select a tower to buy with arrow keys and enter to confirm");
            print(120, 15, "Press ESC to quit the shop");

            do
            {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Escape)
                {
                    return 0;
                }
                else if (cki.Key == ConsoleKey.UpArrow)
                {
                    if (choice > 1)
                    {
                        choice--;
                        printShop(choice);
                    }
                }
                else if (cki.Key == ConsoleKey.DownArrow)
                {
                    if (choice < 4)
                    {
                        choice++;
                        printShop(choice);
                    }
                }
            } while (cki.Key != ConsoleKey.Enter);

            // check if enough coins
            switch (choice)
            {
                case 1:
                    if (coins >= 1)
                    {
                        coins -= 1;
                        return 1;
                    }
                    else
                    {
                        print(120, 16, "Not enough coins!");
                        Thread.Sleep(1000);
                        print(120, 16, "                 ");
                        return purchaseMenu();
                    }

                case 2:
                    if (coins >= 2)
                    {
                        coins -= 2;
                        return 2;
                    }
                    else
                    {
                        print(120, 16, "Not enough coins!");
                        Thread.Sleep(1000);
                        print(120, 16, "                 ");
                        return purchaseMenu();
                    }

                case 3:
                    if (coins >= 3)
                    {
                        coins -= 3;
                        return 3;
                    }
                    else
                    {
                        print(120, 16, "Not enough coins!");
                        Thread.Sleep(1000);
                        print(120, 16, "                 ");
                        return purchaseMenu();
                    }

                case 4:
                    if (coins >= 5)
                    {
                        coins -= 5;
                        return 4;
                    }
                    else
                    {
                        print(120, 16, "Not enough coins!");
                        Thread.Sleep(1000);
                        print(120, 16, "                 ");
                        return purchaseMenu();
                    }
                default:
                    print(120, 16, "Invalid choice!");
                    Thread.Sleep(1000);
                    print(120, 16, "                ");
                    return purchaseMenu();
            }
        }
        static void printUI()
        {
            print(120, 0, $"Level: {level}      ");
            print(120, 1, $"Coins: {coins}      ");
            print(120, 2, $"Lives: {lives}      ");
        }
        public static void print(int x, int y, string text)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(text);
        }
        public static void printHighlight(int x, int y, string text)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Write(text);
            Console.ResetColor();
        }
        static void ReleaseEnemies(int numEnemies, int maxHealth, (int, int) startPos)
        {
            int tick = 0;

            // generate new queue of enemies
            List<Enemy> enemyQueue = new List<Enemy>();
            Random rnd = new Random();
            for (int i = 0; i < numEnemies; i++)
            {
                enemyQueue.Add(new Enemy(rnd.Next(1, maxHealth + 1), startPos));
            }

            while (enemyQueue.Count > 0)
            {
                tick++;
                enemyQueue = runTick(enemyQueue, path, tick, numEnemies);
            }

        }
        static List<Enemy> runTick(List<Enemy> enemies, HashSet<(int x, int y)> path, int tick, int numEnemies)
        {
            // tick enemies
            int spacing = 3;

            for (int i = 0; i < tick / spacing - (numEnemies - enemies.Count) && i < numEnemies; i++) // staggers starting times
            {
                if (i >= enemies.Count) { continue; }
                if (!enemies[i].move(path))
                {
                    // reached end of path
                    enemies.RemoveAt(i);
                    lives--;
                    printUI();
                }
            }
            Thread.Sleep(150);

            //tick towers
            for (int i = 0; i < towers.Count; i++)
            {
                enemies = towers[i].fireWeapon(enemies);
            }
            Thread.Sleep(150);
            return enemies;
        }
    }
}
