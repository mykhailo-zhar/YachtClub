using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class PositionYachttype
    {
        public int Positionid { get; set; }
        public int Yachttypeid { get; set; }

        public virtual Position Position { get; set; }
        public virtual Yachttype Yachttype { get; set; }
    }
}
