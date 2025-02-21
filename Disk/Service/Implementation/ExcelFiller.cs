using ClosedXML.Excel;
using Disk.Data.Impl;
using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Service.Interface;
using Microsoft.Win32;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using System.Windows;
using Localization = Disk.Properties.Langs.Appointment.AppointmentLocalization;

namespace Disk.Service.Implementation;

public class ExcelFiller(IMapRepository mapRepository) : IExcelFiller
{
    private const int ColsPerPath = 7;

    public void ExportToExcel(IEnumerable<Session> sessions, Patient patient)
    {
        using var workbook = new XLWorkbook();

        FillExcel(workbook, sessions);

        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Excel Workbook|*.xlsx",
            Title = Localization.ExportToExcel,
            FileName = $"{patient.Surname} {patient.Name} {patient.Patronymic}.xlsx"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            string filePath = saveFileDialog.FileName;

            try
            {
                workbook.SaveAs(filePath);
                _ = Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"{Localization.SaveFailed}: {ex.Message}");
                Log.Error("Error saving excel file " + filePath);
            }
        }
    }

    private void FillExcel(XLWorkbook workbook, IEnumerable<Session> sessions)
    {
        foreach (var session in sessions)
        {
            var worksheet = workbook.Worksheets.Add();

            worksheet.Cell(1, 1).Value = Localization.DateTime;
            worksheet.Cell(1, 2).Value = Localization.Map;

            worksheet.Cell(2, 1).Value = session.DateTime;
            worksheet.Cell(2, 2).Value = mapRepository.GetById(session.Map).Name;

            worksheet.Cell(4, 1).Value = Localization.Dispersion;
            worksheet.Cell(4, 2).Value = Localization.Deviation;
            worksheet.Cell(4, 3).Value = Localization.MathExp;
            worksheet.Cell(4, 4).Value = Localization.Score;

            var sres = session.SessionResult;
            if (sres is not null)
            {
                SetFloatCell(worksheet, 5, 1, (float)sres.Dispersion);
                SetFloatCell(worksheet, 5, 2, (float)sres.Deviation);
                SetFloatCell(worksheet, 5, 3, (float)sres.MathExp);
                worksheet.Cell(5, 4).Value = sres.Score;
            }

            worksheet.Cell(7, 1).Value = Localization.TargetNum;
            worksheet.Cell(7, 2).Value = Localization.AngleDistance;
            worksheet.Cell(7, 3).Value = Localization.Time;
            worksheet.Cell(7, 4).Value = Localization.AngleSpeed;
            worksheet.Cell(7, 5).Value = Localization.ApproachSpeed;

            const int pathCol = 7;

            FillPtts(worksheet, session, pathCol);
            FillPits(worksheet, session, pathCol + ColsPerPath);

            new List<IXLRange>()
                {
                    worksheet.Range(1, 1, 1, 2),
                    worksheet.Range(4, 1, 4, 4),
                    worksheet.Range(7, 1, 7, 5),
                    worksheet.Range(1, pathCol, 2, (ColsPerPath * (session.PathToTargets.Count + session.PathInTargets.Count + 1)) - 2),
                }
            .ForEach(header =>
            {
                header.Style.Font.Bold = true;
                header.Style.Fill.BackgroundColor = XLColor.LightGray;
            });
            worksheet.Range(3, pathCol, 3, (ColsPerPath * (session.PathToTargets.Count + session.PathInTargets.Count + 1)) - 2).Style.Fill
                .BackgroundColor = XLColor.LightSlateGray;
            worksheet.Range(3, pathCol, 3, (ColsPerPath * (session.PathToTargets.Count + session.PathInTargets.Count + 1)) - 2).Style.Font
                .Bold = true;

            _ = worksheet.Columns().AdjustToContents().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }
    }

    private static void FillPtts(IXLWorksheet worksheet, Session session, int pathCol)
    {
        var ptts = session.PathToTargets;
        var mapCenters = JsonConvert.DeserializeObject<List<Point2D<float>>>(session.MapNavigation.CoordinatesJson)!;

        int pttRow = 8;
        foreach (var ptt in ptts)
        {
            worksheet.Cell(pttRow, 1).Value = ptt.TargetNum + 1;
            SetFloatCell(worksheet, pttRow, 2, (float)ptt.AngleDistance);
            SetFloatCell(worksheet, pttRow, 3, (float)ptt.Time);
            SetFloatCell(worksheet, pttRow, 4, (float)ptt.AngleSpeed);
            SetFloatCell(worksheet, pttRow++, 5, (float)ptt.ApproachSpeed);

            var pathList = JsonConvert.DeserializeObject<List<Point2D<float>>>(ptt.CoordinatesJson)!;

            worksheet.Cell(1, pathCol++).Value = $"{Localization.PathToTarget}";
            FillPath(worksheet, session, pathCol, mapCenters, pathList, ptt.TargetNum);

            pathCol += (ColsPerPath * 2) - 1;
        }
    }

    private static void FillPits(IXLWorksheet worksheet, Session session, int pathCol)
    {
        var pits = session.PathInTargets;
        var mapCenters = JsonConvert.DeserializeObject<List<Point2D<float>>>(session.MapNavigation.CoordinatesJson)!;

        foreach (var pit in pits)
        {
            var pathList = JsonConvert.DeserializeObject<List<Point2D<float>>>(pit.CoordinatesJson)!;

            worksheet.Cell(1, pathCol).Value = Localization.PathInTarget;
            worksheet.Cell(4, pathCol).Value = Localization.Precision;
            worksheet.Cell(5, pathCol).Style.NumberFormat.Format = "0.00";
            worksheet.Cell(5, pathCol++).Value = float.Round(pit.Precision, 3);
            FillPath(worksheet, session, pathCol, mapCenters, pathList, pit.TargetId);

            pathCol += (ColsPerPath * 2) - 1;
        }
    }

    private static void FillPath(IXLWorksheet worksheet, Session session, int pathCol, List<Point2D<float>> mapCenters,
        List<Point2D<float>> path, long targetId)
    {
        int pathRow = 1;

        worksheet.Cell(pathRow, pathCol).Value = $"{Localization.TargetNum}:";
        worksheet.Cell(pathRow++, pathCol + 1).Value = targetId + 1;
        worksheet.Cell(pathRow + 1, pathCol - 1).Value = Localization.TargetCenter;

        worksheet.Cell(pathRow, pathCol).Value = "X";
        SetFloatCell(worksheet, pathRow + 1, pathCol, (mapCenters[(int)targetId].X - 0.5f) * session.MaxXAngle * 2);

        worksheet.Cell(pathRow, pathCol + 1).Value = "Y";
        SetFloatCell(worksheet, pathRow + 1, pathCol + 1, (mapCenters[(int)targetId].Y - 0.5f) * session.MaxYAngle * 2);

        worksheet.Cell(pathRow - 1, pathCol + 2).Value = Localization.ProfileProjection;
        SetFloatCell(worksheet, pathRow + 1, pathCol + 2, 90.0f + ((mapCenters[(int)targetId].Y - 0.5f) * session.MaxYAngle * 2));

        worksheet.Cell(pathRow - 1, pathCol + 3).Value = Localization.FrontLeftFoot;
        SetFloatCell(worksheet, pathRow + 1, pathCol + 3, 90.0f + ((mapCenters[(int)targetId].X - 0.5f) * session.MaxXAngle * 2));

        worksheet.Cell(pathRow - 1, pathCol + 4).Value = Localization.FrontRightFoot;
        SetFloatCell(worksheet, pathRow + 1, pathCol + 4, 90.0f - ((mapCenters[(int)targetId].X - 0.5f) * session.MaxXAngle * 2));

        pathRow += 2;

        FillExcelWithPoints(worksheet, pathCol, pathRow, path);
    }

    private static void FillExcelWithPoints(IXLWorksheet worksheet, int pathCol, int pathRow, List<Point2D<float>> pathList)
    {
        foreach (var point in pathList)
        {
            SetFloatCell(worksheet, pathRow, pathCol, point.X);
            SetFloatCell(worksheet, pathRow, pathCol + 1, point.Y);

            // profile
            SetFloatCell(worksheet, pathRow, pathCol + 2, 90.0f + point.Y);

            // frontal
            // left foot
            SetFloatCell(worksheet, pathRow, pathCol + 3, 90.0f + point.X);
            // right foot
            SetFloatCell(worksheet, pathRow, pathCol + 4, 90.0f - point.X);

            pathRow++;
        }
    }

    private static void SetFloatCell(IXLWorksheet worksheet, int row, int column, float value)
    {
        worksheet.Cell(row, column).Style.NumberFormat.Format = "0.0";
        worksheet.Cell(row, column).Value = value;
    }
}
