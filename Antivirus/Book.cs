using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antivirus
{
    [Table("Books")]
    public class Book
    {
        [Key]
        public System.Guid IdBook { get; set; }
        public string Name { get; set; }

        public string Author { get; set; }
        public string Phrase { get; set; }
        public string Razdel { get; set; }
        public string Hashes { get; set; }
    }
}
