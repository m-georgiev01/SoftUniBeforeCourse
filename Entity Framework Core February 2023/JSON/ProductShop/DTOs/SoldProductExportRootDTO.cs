using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductShop.DTOs
{
    public class SoldProductExportRootDTO
    {
        public int Count { get; set; }

        public IEnumerable<SoldProductExportDTO> Products { get; set; }
    }
}
