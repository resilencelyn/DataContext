using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    public class DbTable
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
