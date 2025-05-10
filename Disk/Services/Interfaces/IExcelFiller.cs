using Disk.Entities;

namespace Disk.Services.Interfaces;

public interface IExcelFiller
{
    void ExportToExcel(Session session, List<Attempt> attempts, Patient patient, Map map);
}
