using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductShop.DTOs
{
    public class ExportUsersRootDTO
    {
        public int UsersCount { get; set; }

        public IEnumerable<UserDTO> Users { get; set; }
    }
}
