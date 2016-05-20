using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearcheyData.Entities
{
    public class TagRelation
    {
        public int Id { get; set; }
        public virtual Tag Tag1 { get; set; }
        public virtual Tag Tag2 { get; set; }

        public double Relation { get; set; }

    }
}
