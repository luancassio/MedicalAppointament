using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointment.Domain.Entity;
    public class User : IdentityUser<string> {
    public string Name { get; set; }
    public string Password { get; set; }
    public DateTime Create_At { get; set; }
    public DateTime Update_At { get; set; }
    public bool isActive { get; set; }
}
