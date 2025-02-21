using Disk.Entities;

namespace Disk.Service.Interface;

public interface IExcelFiller
{
    void ExportToExcel(IEnumerable<Session> sessions, Patient patient);
}
