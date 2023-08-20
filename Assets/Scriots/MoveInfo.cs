using Scriots;
using System.Collections.Generic;

namespace Assets.Scriots
{
    public class MoveInfo
    {
        public Player Player { get; set; }
        public Position Position { get; set; }
        public List<Position> Outflanked { get; set; }
    }
}
