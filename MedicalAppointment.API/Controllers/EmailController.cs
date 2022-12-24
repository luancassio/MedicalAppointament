using MedicalAppointment.Application.Email;
using MedicalAppointment.Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointment.API.Controllers; 
[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase {
	private readonly EmailService _emailSend;

	public EmailController(EmailService emailSend) {
		_emailSend = emailSend;
	}

    [HttpPost]
    public async Task<IActionResult> Send(EmailSend emailSend) {
        await _emailSend.SendEmailAsync(emailSend);
        return this.StatusCode(StatusCodes.Status200OK, 
            new { success = true, message = $"Email Enviado para {emailSend.ToAddress} com sucesso!" });
    }
}
