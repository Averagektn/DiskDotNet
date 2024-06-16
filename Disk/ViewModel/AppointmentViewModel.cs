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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Localization = Disk.Properties.Langs.Appointment.AppointmentLocalization;

namespace Disk.ViewModel
{
    public class AppointmentViewModel(ModalNavigationStore modalNavigationStore, ISessionRepository sessionRepository)
        : ObserverViewModel
    {
        public bool IsNewAppointment { get; set; }
        public Patient Patient { get; set; } = AppointmentSession.Patient;
        public Doctor Doctor { get; set; } = AppSession.Doctor;
        public Session? SelectedSession { get; set; }
        public ObservableCollection<Session> Sessions { get; set; }
            = new(sessionRepository.GetSessionsWithResultsByAppointment(AppointmentSession.Appointment.Id).ToList());
        public ObservableCollection<PathToTarget> PathsToTargets { get; set; } = [];

        public ICommand StartSessionCommand
            => new Command(_ => modalNavigationStore.SetViewModel<StartSessionViewModel>(vm => vm.OnSessionOver += Update, canClose: true));
        public ICommand SessionSelectedCommand => new Command(SessionSelected);
        public ICommand ExportToExcelCommand => new Command(ExportToExcel);

        private void FillExcel(XLWorkbook workbook)
        {

            foreach (var session in Sessions)
            {
                var worksheet = workbook.Worksheets.Add();

                worksheet.Cell(1, 1).Value = "Date";
                worksheet.Cell(1, 2).Value = "Map";

                worksheet.Cell(2, 1).Value = session.DateTime;
                worksheet.Cell(2, 2).Value = session.Map;

                worksheet.Cell(4, 1).Value = "Dispersion";
                worksheet.Cell(4, 2).Value = "Deviation";
                worksheet.Cell(4, 3).Value = "MathExp";
                worksheet.Cell(4, 4).Value = "Score";

                var sres = session.SessionResult;
                worksheet.Cell(5, 1).Value = sres?.Dispersion;
                worksheet.Cell(5, 2).Value = sres?.Deviation;
                worksheet.Cell(5, 3).Value = sres?.MathExp;
                worksheet.Cell(5, 4).Value = sres?.Score;

                worksheet.Cell(7, 1).Value = "TargetNum";
                worksheet.Cell(7, 2).Value = "AngleDistance";
                worksheet.Cell(7, 3).Value = "Time";
                worksheet.Cell(7, 4).Value = "AngleSpeed";
                worksheet.Cell(7, 5).Value = "ApproachSpeed";

                var ptts = session.PathToTargets;
                int pttRow = 8;
                int pathCol = 7;
                foreach (var ptt in ptts)
                {
                    int pathRow = 1;
                    worksheet.Cell(pttRow, 1).Value = ptt.TargetNum;
                    worksheet.Cell(pttRow, 2).Value = ptt.AngleDistance;
                    worksheet.Cell(pttRow, 3).Value = ptt.Time;
                    worksheet.Cell(pttRow, 4).Value = ptt.AngleSpeed;
                    worksheet.Cell(pttRow, 5).Value = ptt.ApproachSpeed;
                    pttRow++;

                    var pathList = JsonConvert.DeserializeObject<List<Point2D<float>>>(ptt.CoordinatesJson)!;
                    worksheet.Cell(pathRow++, pathCol).Value = $"Target: {ptt.TargetNum + 1}";
                    worksheet.Cell(pathRow, pathCol).Value = "X";
                    worksheet.Cell(pathRow, pathCol + 1).Value = "Y";
                    worksheet.Cell(pathRow - 1, pathCol + 2).Value = "Profile projection";
                    worksheet.Cell(pathRow - 1, pathCol + 3).Value = "Frontal direction";
                    worksheet.Cell(pathRow - 1, pathCol + 4).Value = "Frontal left foot";
                    worksheet.Cell(pathRow++, pathCol + 5).Value = "Frontal right foot";


                    foreach (var point in pathList)
                    {
                        worksheet.Cell(pathRow, pathCol).Value = point.X;
                        worksheet.Cell(pathRow, pathCol + 1).Value = point.Y;

                        // profile
                        worksheet.Cell(pathRow, pathCol + 2).Value = 90.0f + point.Y;

                        // frontal
                        // left
                        if (point.X >= 0.0f)
                        {
                            worksheet.Cell(pathRow, pathCol + 3).Value = "<-";
                            // left foot
                            worksheet.Cell(pathRow, pathCol + 4).Value = 90.0f + point.X;
                            // right foot
                            worksheet.Cell(pathRow, pathCol + 5).Value = 90.0f - point.X;
                        }
                        // right
                        else
                        {
                            worksheet.Cell(pathRow, pathCol + 3).Value = "->";
                            // left foot
                            worksheet.Cell(pathRow, pathCol + 4).Value = 90.0f - point.X;
                            // right foot
                            worksheet.Cell(pathRow, pathCol + 5).Value = 90.0f + point.X;
                        }

                        pathRow++;
                    }

                    pathCol += 7;
                }

                new List<IXLRange>()
                {
                    worksheet.Range(1, 1, 1, 2),
                    worksheet.Range(4, 1, 4, 4),
                    worksheet.Range(7, 1, 7, 5)
                }
                .ForEach(header =>
                {
                    header.Style.Font.Bold = true;
                    header.Style.Fill.BackgroundColor = XLColor.LightGray;
                });

                _ = worksheet.Columns().AdjustToContents();
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
                workbook.SaveAs(filePath);

                try
                {
                    _ = Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show($"{Localization.SaveFailed}: {ex.Message}");
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