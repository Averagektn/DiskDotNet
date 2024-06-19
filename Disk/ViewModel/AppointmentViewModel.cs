﻿using ClosedXML.Excel;
using Disk.Data.Impl;
using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Sessions;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.Win32;
using Newtonsoft.Json;
using Serilog;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Localization = Disk.Properties.Langs.Appointment.AppointmentLocalization;

namespace Disk.ViewModel
{
    public class AppointmentViewModel(ModalNavigationStore modalNavigationStore, ISessionRepository sessionRepository, IMapRepository mapRepository)
        : ObserverViewModel
    {
        public bool IsNewAppointment { get; set; }
        public Patient Patient { get; set; } = AppointmentSession.Patient;
        public Doctor Doctor { get; set; } = AppSession.Doctor;
        public Session? SelectedSession { get; set; }
        public ObservableCollection<Session> Sessions { get; set; }
            = new(sessionRepository.GetSessionsWithResultsByAppointment(AppointmentSession.Appointment.Id));
        public ObservableCollection<PathToTarget> PathsToTargets { get; set; } = [];

        public ICommand StartSessionCommand
            => new Command(_ => modalNavigationStore.SetViewModel<StartSessionViewModel>(vm => vm.OnSessionOver += Update, canClose: true));
        public ICommand SessionSelectedCommand => new Command(SessionSelected);
        public ICommand ExportToExcelCommand => new Command(ExportToExcel);

        private const int ColsPerPath = 8;

        private void FillExcel(XLWorkbook workbook)
        {

            foreach (var session in Sessions)
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
                worksheet.Cell(5, 1).Value = sres?.Dispersion;
                worksheet.Cell(5, 2).Value = sres?.Deviation;
                worksheet.Cell(5, 3).Value = sres?.MathExp;
                worksheet.Cell(5, 4).Value = sres?.Score;

                worksheet.Cell(7, 1).Value = Localization.TargetNum;
                worksheet.Cell(7, 2).Value = Localization.AngleDistance;
                worksheet.Cell(7, 3).Value = Localization.Time;
                worksheet.Cell(7, 4).Value = Localization.AngleSpeed;
                worksheet.Cell(7, 5).Value = Localization.ApproachSpeed;

                var pits = session.PathInTargets;
                const int pathCol = 7;

                FillPtts(worksheet, session, pathCol);
                FillPits(worksheet, session, pathCol + ColsPerPath);

                new List<IXLRange>()
                {
                    worksheet.Range(1, 1, 1, 2),
                    worksheet.Range(4, 1, 4, 4),
                    worksheet.Range(7, 1, 7, 5),
                    worksheet.Range(1, pathCol, 2, ColsPerPath * (session.PathToTargets.Count + pits.Count)),
                }
                .ForEach(header =>
                {
                    header.Style.Font.Bold = true;
                    header.Style.Fill.BackgroundColor = XLColor.LightGray;
                });
                worksheet.Range(3, pathCol, 3, ColsPerPath * (session.PathToTargets.Count + pits.Count)).Style.Fill.BackgroundColor = XLColor.LightSlateGray;
                worksheet.Range(3, pathCol, 3, ColsPerPath * (session.PathToTargets.Count + pits.Count)).Style.Font.Bold = true;

                _ = worksheet.Columns().AdjustToContents().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
        }

        private static void FillPtts(IXLWorksheet worksheet, Session session, int pathCol)
        {
            var ptts = session.PathToTargets;
            var mapCenters = JsonConvert.DeserializeObject<List<Point2D<float>>>(session.MapNavigation.CoordinatesJson)!;

            foreach (var ptt in ptts)
            {
                worksheet.Cell(8, 1).Value = ptt.TargetNum;
                worksheet.Cell(8, 2).Value = ptt.AngleDistance;
                worksheet.Cell(8, 3).Value = ptt.Time;
                worksheet.Cell(8, 4).Value = ptt.AngleSpeed;
                worksheet.Cell(8, 5).Value = ptt.ApproachSpeed;

                var pathList = JsonConvert.DeserializeObject<List<Point2D<float>>>(ptt.CoordinatesJson)!;

                FillPath(worksheet, session, pathCol, mapCenters, pathList, ptt.TargetNum);

                pathCol += ColsPerPath * 2;
            }
        }

        private static void FillPits(IXLWorksheet worksheet, Session session, int pathCol)
        {
            var pits = session.PathInTargets;
            var mapCenters = JsonConvert.DeserializeObject<List<Point2D<float>>>(session.MapNavigation.CoordinatesJson)!;

            foreach (var pit in pits)
            {
                var pathList = JsonConvert.DeserializeObject<List<Point2D<float>>>(pit.CoordinatesJson)!;

                FillPath(worksheet, session, pathCol, mapCenters, pathList, pit.TargetId);

                pathCol += ColsPerPath * 2;
            }
        }

        private static void FillPath(IXLWorksheet worksheet, Session session, int pathCol, List<Point2D<float>> mapCenters,
            List<Point2D<float>> path, long targetId)
        {
            int pathRow = 1;

            worksheet.Cell(pathRow, pathCol++).Value = $"{Localization.PathToTarget}";
            worksheet.Cell(pathRow++, pathCol).Value = $"{Localization.TargetNum}: {targetId + 1}";
            worksheet.Cell(pathRow + 1, pathCol - 1).Value = Localization.TargetCenter;

            worksheet.Cell(pathRow, pathCol).Value = "X";
            worksheet.Cell(pathRow + 1, pathCol).Value = (mapCenters[(int)targetId].X - 0.5f) * session.MaxXAngle * 2;

            worksheet.Cell(pathRow, pathCol + 1).Value = "Y";
            worksheet.Cell(pathRow + 1, pathCol + 1).Value = (mapCenters[(int)targetId].Y - 0.5f) * session.MaxYAngle * 2;

            worksheet.Cell(pathRow - 1, pathCol + 2).Value = Localization.ProfileProjection;
            worksheet.Cell(pathRow + 1, pathCol + 2).Value = 90.0f + ((mapCenters[(int)targetId].Y - 0.5f) * session.MaxYAngle * 2);

            worksheet.Cell(pathRow - 1, pathCol + 3).Value = Localization.FrontalProjection;

            worksheet.Cell(pathRow - 1, pathCol + 4).Value = Localization.FrontLeftFoot;
            worksheet.Cell(pathRow + 1, pathCol + 4).Value = 90.0f + ((mapCenters[(int)targetId].X - 0.5f) * session.MaxXAngle * 2);

            worksheet.Cell(pathRow - 1, pathCol + 5).Value = Localization.FrontRightFoot;
            worksheet.Cell(pathRow + 1, pathCol + 5).Value = 90.0f - ((mapCenters[(int)targetId].X - 0.5f) * session.MaxXAngle * 2);

            pathRow += 2;

            FillExcelWithPoints(worksheet, pathCol, pathRow, path);
        }

        private static void FillExcelWithPoints(IXLWorksheet worksheet, int pathCol, int pathRow, List<Point2D<float>> pathList)
        {
            foreach (var point in pathList)
            {
                worksheet.Cell(pathRow, pathCol).Value = point.X;
                worksheet.Cell(pathRow, pathCol + 1).Value = point.Y;

                // profile
                worksheet.Cell(pathRow, pathCol + 2).Value = 90.0f + point.Y;

                // frontal
                // left
                worksheet.Cell(pathRow, pathCol + 3).Value = point.X < 0.0f ? (XLCellValue)"<-" : (XLCellValue)"->";
                // left foot
                worksheet.Cell(pathRow, pathCol + 4).Value = 90.0f + point.X;
                // right foot
                worksheet.Cell(pathRow, pathCol + 5).Value = 90.0f - point.X;

                pathRow++;
            }
        }

        private void ExportToExcel(object? obj)
        {
            using var workbook = new XLWorkbook();

            FillExcel(workbook);

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx",
                Title = Localization.ExportToExcel,
                FileName = $"{AppointmentSession.Patient.Surname} {AppointmentSession.Patient.Name} {AppointmentSession.Patient.Patronymic}.xlsx"
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

        private void SessionSelected(object? obj)
        {
            PathsToTargets.Clear();

            foreach (var pathToTarget in SelectedSession!.PathToTargets)
            {
                PathsToTargets.Add(pathToTarget);
            }
        }

        private void Update()
        {
            Sessions.Clear();
            var sessions = sessionRepository.GetSessionsWithResultsByAppointment(AppointmentSession.Appointment.Id);

            foreach (var session in sessions)
            {
                Sessions.Add(session);
            }
        }
    }
}