using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class PositionEquivalent
    {
        public int Positionid { get; set; }
        public int Positionequivalentid { get; set; }

        public virtual Position Position { get; set; }
        public virtual Position Positionequivalent { get; set; }
    }
}
