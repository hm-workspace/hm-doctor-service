using System.Data;

namespace DoctorService.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection(string connectionName = "DefaultConnection");
}


