using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointment.Application.Auth.Cryptography; 
public class EncryptPassword {
    private readonly string _password;
    public EncryptPassword(string password) {
        _password = password;
    }
    public string Encrypt(string password) {
        var passwordWIthKey = $"{password}{_password}";
        var bytes = Encoding.UTF8.GetBytes(passwordWIthKey);
        var sha512 = SHA512.Create();
        byte[] hashBytes = sha512.ComputeHash(bytes);
        return StringBytes(hashBytes);
    }
    private static string  StringBytes(byte[] bytes) {
        var stringBuilder = new StringBuilder();
        foreach(byte bt in bytes) {
            var hex = bt.ToString("x2");
            stringBuilder.Append(hex);
        }
        return stringBuilder.ToString();
    }
}
