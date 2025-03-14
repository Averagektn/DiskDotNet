using Disk.Entities;

namespace Disk.Service.Interface;

public interface IExcelFiller
{
    void ExportToExcel(Session session, List<Attempt> attempts, Patient patient, Map map);
}
