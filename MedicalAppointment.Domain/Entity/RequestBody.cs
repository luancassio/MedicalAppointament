using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointment.Domain.Entity;

public class RequestBody
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public RequestBody(bool success, string message)
    {
        Success = success;
        Message = message;
    }

}
