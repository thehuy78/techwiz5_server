using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWizWebApp.Domain
{
    public class VariantAttribute
    {
        [Key]
        public int id { get; set; }
        public int variantid { get; set; }
        public Variant? variant { get; set; }
        public string attributetype { get; set; }
        public int priority { get; set; }
        public string? note { get; set; }
        public string attributevalue { get; set; }
    }
}
