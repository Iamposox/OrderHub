using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities;
public class Role
{
    public Guid Id { get; set; }
    public string Rolename { get; set; } = default!;
    public string? RoleDescription { get; set; }
    public ICollection<User> Users { get; set;}
}
