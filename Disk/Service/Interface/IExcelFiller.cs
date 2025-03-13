using Disk.Entities;

namespace Disk.Service.Interface;

public interface IExcelFiller
{
    void ExportToExcel(Appointment appointment, List<Session> sessions, Patient patient, Map map);
}
