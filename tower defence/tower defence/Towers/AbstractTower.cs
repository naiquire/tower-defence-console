using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tower_defence.Towers
{
    public abstract class AbstractTower
    {
        protected char towerChar;
        protected ConsoleColor textColor;
        protected ConsoleColor BGColor;

        private int damage;
        private (int x, int y) pos;
        private int range;
        private int maxCooldown;
        private int cooldown;

        public AbstractTower(int damage, (int, int) pos, int range, int cooldown)
        {
            this.damage = damage;
            this.pos = pos;
            this.range = range;
            this.maxCooldown = cooldown;
            this.cooldown = cooldown;
        }
        public abstract (char, ConsoleColor, ConsoleColor) getTowerInfo();
        public void printTower()
        {
            Console.SetCursorPosition(pos.x * 2, pos.y);
            Console.BackgroundColor = BGColor;
            Console.ForegroundColor = textColor;
            Console.Write(towerChar);
            Console.ResetColor();
        }
        public List<Enemy> fireWeapon(List<Enemy> enemies)
        {
            cooldown--;
            if (cooldown <= 0)
            {
                foreach (Enemy enemy in enemies)
                {
                    // find an enemy in range
                    (int x, int y) enemyPos = enemy.getPosition();
                    if (Math.Abs(enemyPos.x - pos.y) + Math.Abs(enemyPos.y - pos.x) <= range)
                    {
                        // fire animation thing idk
                        enemy.loseHealth(damage);
                        if (enemy.getHealth() <= 0)
                        {
                            // enemy is dead
                            enemy.unprintEnemy();
                            enemies.Remove(enemy);
                            //enemy.loseHealth(-5);
                        }
                        cooldown = maxCooldown;
                        break;
                    }
                }
            }
            return enemies;
        }
    }
}
