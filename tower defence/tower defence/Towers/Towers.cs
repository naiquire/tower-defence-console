using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tower_defence.Towers
{
    public class Archer : AbstractTower
    {
        public Archer((int, int) pos) : base(1, pos, 3, 2)
        {
            towerChar = 'A';
            textColor = ConsoleColor.Green;
            BGColor = ConsoleColor.Black;

            printTower();
        }
        public override (char, ConsoleColor, ConsoleColor) getTowerInfo()
        {
            return (towerChar, textColor, BGColor);
        }
    }
    public class Cannon : AbstractTower
    {
        public Cannon((int, int) pos) : base(2, pos, 5, 4)
        {
            towerChar = 'C';
            textColor = ConsoleColor.Red;
            BGColor = ConsoleColor.Black;
        }
        public override (char, ConsoleColor, ConsoleColor) getTowerInfo()
        {
            return (towerChar, textColor, BGColor);
        }
    }
    public class Mage : AbstractTower
    {
        public Mage((int, int) pos) : base(3, pos, 7, 6)
        {
            towerChar = 'M';
            textColor = ConsoleColor.Magenta;
            BGColor = ConsoleColor.Black;
        }
        public override (char, ConsoleColor, ConsoleColor) getTowerInfo()
        {
            return (towerChar, textColor, BGColor);
        }
    }
    public class Ballista : AbstractTower
    {
        public Ballista((int, int) pos) : base(5, pos, 10, 10)
        {
            towerChar = 'B';
            textColor = ConsoleColor.Blue;
            BGColor = ConsoleColor.Black;
        }
        public override (char, ConsoleColor, ConsoleColor) getTowerInfo()
        {
            return (towerChar, textColor, BGColor);
        }
    }
}
