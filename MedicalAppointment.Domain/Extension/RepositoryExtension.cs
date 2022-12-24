using Microsoft.Extensions.Configuration;

namespace MedicalAppointment.Domain.Extension; 
public static class RepositoryExtension {
    /*
     * para ele saber que essa classe e uam extensão de configuration 
     * precisamos utilizar o this para ele funcionar
     */
    public static string GetConnection(this IConfiguration configuration) {
        var connDataBaase = configuration.GetConnectionString("DefaultConnection");
        return connDataBaase;
    }
    public static string GetNameDataBase(this IConfiguration configuration) {
        var nameDataBaase = configuration.GetConnectionString("nameDataBase");
        return nameDataBaase;
    }
    public static string GetConnectionFull(this IConfiguration configuration) {
        return $"{configuration.GetConnection()}Database={configuration.GetNameDataBase()}";
    }
}
