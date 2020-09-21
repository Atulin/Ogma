using System.Collections.Generic;
using Ogma3.Data.DTOs;

namespace Ogma3.Pages.Shared
{
    public class UserCard
    {
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string Title { get; set; }
        public IEnumerable<RoleDTO> Roles { get; set; }
    }
}