using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tower_defence
{
    public class Enemy
    {
        private (int x, int y) pos;
        private int health;
        private HashSet<(int, int)> visited = new HashSet<(int, int)>();
        public Enemy(int health, (int, int) pos)
        {
            this.health = health;
            this.pos = pos;
            visited.Add(pos);
        }
        public bool move(HashSet<(int x, int y)> path)
        {
            unprintEnemy();

            // find next position
            foreach (var pathPos in path)
            {
                if (Math.Abs(pathPos.x - pos.x) + Math.Abs(pathPos.y - pos.y) == 1 && !visited.Contains(pathPos))
                {
                    pos = pathPos;
                    visited.Add(pathPos);
                    break;
                }
            }
            // check if enemy is at the end of the path
            if (visited.Count == path.Count)
            {
                // enemy has reached the end of the path
                return false;
            }
            else
            {
                printEnemy();
            }
            return true;
        }
        public void printEnemy()
        {
            Console.SetCursorPosition(pos.y * 2, pos.x);
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write(health);
        }
        public void unprintEnemy()
        {
            Console.SetCursorPosition(pos.y * 2, pos.x);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write('X');
        }
        public (int, int) getPosition()
        {
            return pos;
        }
        public int getHealth()
        {
            return health;
        }
        public HashSet<(int, int)> getPath()
        {
            return visited;
        }
        public void loseHealth(int damage)
        {
            health -= damage;
            //Program.printHighlight(pos.y * 2, pos.x, health.ToString());
        }
    }
}
