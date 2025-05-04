using ClosedXML.Excel;
using Disk.Data.Impl;
using Disk.Entities;
using Disk.Service.Interface;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using Localization = Disk.Properties.Langs.ExcelFiller.ExcelFillerLocalization;

namespace Disk.Service.Implementation;

public class ExcelFiller : IExcelFiller
{
    private const int ColsPerPath = 7;

    public void ExportToExcel(Session session, List<Attempt> attempts, Patient patient, Map map)
    {
        using var workbook = new XLWorkbook();

        FillExcel(workbook, attempts, map);

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
                Log.Error($"Error saving excel file {ex}");
                throw;
            }
        }
    }

    private static void FillExcel(XLWorkbook workbook, List<Attempt> attempts, Map map)
    {
        foreach (var attempt in attempts)
        {
            var worksheet = workbook.Worksheets.Add();

            worksheet.Cell(1, 1).Value = Localization.DateTime;
            worksheet.Cell(1, 2).Value = Localization.Map;

            worksheet.Cell(2, 1).Value = attempt.DateTime;
            worksheet.Cell(2, 2).Value = map.Name;

            worksheet.Cell(4, 1).Value = $"{Localization.Deviation} X";
            worksheet.Cell(4, 2).Value = $"{Localization.Deviation} Y";
            worksheet.Cell(4, 3).Value = $"{Localization.MathExp} X";
            worksheet.Cell(4, 4).Value = $"{Localization.MathExp} Y";
            worksheet.Cell(4, 5).Value = $"{Localization.MaxAngle} (X;Y)";
            worksheet.Cell(4, 6).Value = $"{Localization.CursorRadius}";
            worksheet.Cell(4, 7).Value = $"{Localization.TargetRadius}";

            var sres = attempt.AttemptResult;
            if (sres is not null)
            {
                SetFloatCell(worksheet, 5, 1, (float)sres.DeviationX);
                SetFloatCell(worksheet, 5, 2, (float)sres.DeviationY);
                SetFloatCell(worksheet, 5, 3, (float)sres.MathExpX);
                SetFloatCell(worksheet, 5, 4, (float)sres.MathExpY);
                worksheet.Cell(5, 5).Value = $"{attempt.MaxXAngle:f2}; {attempt.MaxYAngle:f2}";
                SetFloatCell(worksheet, 5, 6, attempt.CursorRadius);
                SetFloatCell(worksheet, 5, 7, attempt.TargetRadius);
            }

            worksheet.Cell(8, 1).Value = Localization.TargetNum;
            worksheet.Cell(8, 2).Value = Localization.Time;
            worksheet.Cell(8, 3).Value = Localization.ApproachSpeed;
            worksheet.Cell(8, 4).Value = Localization.AverageSpeed;

            worksheet.Range(7, 5, 7, 7).Merge().Value = Localization.EllipseArea;
            worksheet.Cell(8, 5).Value = Localization.PathToTarget;
            worksheet.Cell(8, 6).Value = Localization.PathInTarget;
            worksheet.Cell(8, 7).Value = Localization.FullPath;

            worksheet.Range(7, 8, 7, 10).Merge().Value = Localization.ConvexHullArea;
            worksheet.Cell(8, 8).Value = Localization.PathToTarget;
            worksheet.Cell(8, 9).Value = Localization.PathInTarget;
            worksheet.Cell(8, 10).Value = Localization.FullPath;

            const int pathCol = 12;

            FillPtts(worksheet, attempt, pathCol, map);
            FillPits(worksheet, attempt, pathCol + ColsPerPath, map);

            new List<IXLRange>()
                {
                    worksheet.Range(firstCellRow: 1, firstCellColumn: 1, lastCellRow: 1, lastCellColumn: 2),
                    worksheet.Range(firstCellRow: 4, firstCellColumn: 1, lastCellRow: 4, lastCellColumn: 7),
                    worksheet.Range(firstCellRow: 7, firstCellColumn: 5, lastCellRow: 7, lastCellColumn: 10),
                    worksheet.Range(firstCellRow: 8, firstCellColumn: 1, lastCellRow: 8, lastCellColumn: 10),
                    worksheet.Range(firstCellRow: 1, firstCellColumn: pathCol, lastCellRow: 2,
                        lastCellColumn: (ColsPerPath * (attempt.PathToTargets.Count + attempt.PathInTargets.Count + 1)) + 3),
                }
            .ForEach(header =>
            {
                header.Style.Font.Bold = true;
                header.Style.Fill.BackgroundColor = XLColor.LightGray;
            });
            worksheet.Range(firstCellRow: 3, firstCellColumn: pathCol, lastCellRow: 2,
                lastCellColumn: (ColsPerPath * (attempt.PathToTargets.Count + attempt.PathInTargets.Count + 1)) + 3)
                .Style.Fill.BackgroundColor = XLColor.LightSlateGray;
            worksheet.Range(firstCellRow: 3, firstCellColumn: pathCol, lastCellRow: 2,
                lastCellColumn: (ColsPerPath * (attempt.PathToTargets.Count + attempt.PathInTargets.Count + 1)) + 3)
                .Style.Font.Bold = true;

            _ = worksheet
                .Columns()
                .AdjustToContents().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }
    }

    private static void FillPtts(IXLWorksheet worksheet, Attempt attempt, int pathCol, Map map)
    {
        var ptts = attempt.PathToTargets;
        var mapCenters = JsonConvert.DeserializeObject<List<Point2D<float>>>(map.CoordinatesJson)!;

        int pttRow = 9;
        foreach (var ptt in ptts)
        {
            worksheet.Cell(pttRow, 1).Value = ptt.TargetNum + 1;
            SetFloatCell(worksheet, pttRow, 2, (float)ptt.Time);
            SetFloatCell(worksheet, pttRow, 3, (float)ptt.ApproachSpeed);
            SetFloatCell(worksheet, pttRow, 4, (float)ptt.AverageSpeed);
            SetFloatCell(worksheet, pttRow, 5, (float)ptt.EllipseArea);
            SetFloatCell(worksheet, pttRow, 8, (float)ptt.ConvexHullArea);
            pttRow++;

            var pathList = JsonConvert.DeserializeObject<List<Point2D<float>>>(ptt.CoordinatesJson)!;

            worksheet.Cell(1, pathCol++).Value = $"{Localization.PathToTarget}";
            FillPath(worksheet, pathCol, mapCenters, pathList, ptt.TargetNum);

            pathCol += (ColsPerPath * 2) - 1;
        }
    }

    private static void FillPits(IXLWorksheet worksheet, Attempt attempt, int pathCol, Map map)
    {
        var pits = attempt.PathInTargets;
        var mapCenters = JsonConvert.DeserializeObject<List<Point2D<float>>>(map.CoordinatesJson)!;

        int pitRow = 9;
        foreach (var pit in pits)
        {
            var pathList = JsonConvert.DeserializeObject<List<Point2D<float>>>(pit.CoordinatesJson)!;

            SetFloatCell(worksheet, pitRow, 6, (float)pit.EllipseArea);
            SetFloatCell(worksheet, pitRow, 7, (float)pit.FullPathEllipseArea);
            SetFloatCell(worksheet, pitRow, 9, (float)pit.ConvexHullArea);
            SetFloatCell(worksheet, pitRow, 10, (float)pit.FullPathConvexHullArea);
            pitRow++;

            worksheet.Cell(1, pathCol).Value = Localization.PathInTarget;
            worksheet.Cell(4, pathCol).Value = Localization.Accuracy;
            worksheet.Cell(5, pathCol).Style.NumberFormat.Format = "0.00";
            worksheet.Cell(5, pathCol++).Value = double.Round(pit.Accuracy, 3);
            FillPath(worksheet, pathCol, mapCenters, pathList, pit.TargetId);

            pathCol += (ColsPerPath * 2) - 1;
        }
    }

    private static void FillPath(IXLWorksheet worksheet, int pathCol, List<Point2D<float>> mapCenters,
        List<Point2D<float>> path, long targetId)
    {
        int pathRow = 1;

        worksheet.Cell(pathRow, pathCol).Value = $"{Localization.TargetNum}:";
        worksheet.Cell(pathRow++, pathCol + 1).Value = targetId + 1;
        worksheet.Cell(pathRow + 1, pathCol - 1).Value = Localization.TargetCenter;

        worksheet.Cell(pathRow, pathCol).Value = "X";
        SetFloatCell(worksheet, pathRow + 1, pathCol, mapCenters[(int)targetId].X);

        worksheet.Cell(pathRow, pathCol + 1).Value = "Y";
        SetFloatCell(worksheet, pathRow + 1, pathCol + 1, mapCenters[(int)targetId].Y);

        worksheet.Cell(pathRow - 1, pathCol + 2).Value = Localization.ProfileProjection;
        SetFloatCell(worksheet, pathRow + 1, pathCol + 2, 90.0f + mapCenters[(int)targetId].Y);

        worksheet.Cell(pathRow - 1, pathCol + 3).Value = Localization.FrontLeftFoot;
        SetFloatCell(worksheet, pathRow + 1, pathCol + 3, 90.0f + mapCenters[(int)targetId].X);

        worksheet.Cell(pathRow - 1, pathCol + 4).Value = Localization.FrontRightFoot;
        SetFloatCell(worksheet, pathRow + 1, pathCol + 4, 90.0f - mapCenters[(int)targetId].X);

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
