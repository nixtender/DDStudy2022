using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entites
{
    public class Avatar : Attach
    {
        public virtual User User { get; set; }
    }
}
