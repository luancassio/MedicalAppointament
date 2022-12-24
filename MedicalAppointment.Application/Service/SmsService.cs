using MedicalAppointment.Application.Email;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointment.Application.Service
{
    public class SmsService
    {

        public void SendSms(string _content)
        {
            var apiInstance = new TransactionalSMSApi();
            string sender = "senderName";
            string recipient = "+5547992890127";
            string content = _content;
            SendTransacSms.TypeEnum type = SendTransacSms.TypeEnum.Transactional;
            string tag = "testTag";
            string webUrl = "https://example.com/notifyUrl";

            try
            {
                var sendTransacSms = new SendTransacSms(sender, recipient, content, type, tag, webUrl);
                SendSms result = apiInstance.SendTransacSms(sendTransacSms);
                Debug.WriteLine(result.ToJson());
                Console.WriteLine(result.ToJson());
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
    }
}
