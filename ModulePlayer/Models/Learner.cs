using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModulePlayer.Models
{
    public class Learner
    {
        public Learner()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
