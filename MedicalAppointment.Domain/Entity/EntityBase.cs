using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointment.Domain.Entity;
    public class EntityBase {
        public int Id { get; set; }
        public DateTime Creat_At { get; set; }
        public DateTime Update_At { get; set; }

    }

