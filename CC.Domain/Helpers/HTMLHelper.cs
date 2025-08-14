using CC.Domain.Dtos;
using System.Text;

namespace CC.Domain.Helpers;

public class HTMLHelper
{
    public static string GenerateScheduleHtml(List<ScheduleDto> schedules)
    {
        var dayMapping = new Dictionary<DayOfWeek, string>
    {
        { DayOfWeek.Monday, "Lunes" },
        { DayOfWeek.Tuesday, "Martes" },
        { DayOfWeek.Wednesday, "Miércoles" },
        { DayOfWeek.Thursday, "Jueves" },
        { DayOfWeek.Friday, "Viernes" },
        { DayOfWeek.Saturday, "Sábado" },
        { DayOfWeek.Sunday, "Domingo" }
    };

        var schedulesByDay = schedules
            .GroupBy(s => s.Date.DayOfWeek)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(s => s.StartTime).ToList()
            );

        var maxRowsPerDay = schedulesByDay.Values.DefaultIfEmpty(new List<ScheduleDto>()).Max(list => list.Count);
        var sb = new StringBuilder();

        sb.Append(@"Hola " + schedules[0].UserFullName);
        sb.Append("</br></br>");

        sb.Append(@"<table border='1' cellpadding='8' cellspacing='0' style='
        border-collapse: collapse;
        width: 100%;
        text-align: center;
        font-family: Arial, sans-serif;
        box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        border-radius: 8px;
        overflow: hidden;'>");

        sb.Append("<thead><tr>");
        foreach (var day in dayMapping.Values)
        {
            sb.Append($"<th colspan='3'>{day}</th>");
        }
        sb.Append("</tr><tr>");
        foreach (var _ in dayMapping.Values)
        {
            sb.Append("<th>Hora Inicio</th><th>Hora Fin</th><th>Puesto de trabajo</th>");
        }
        sb.Append("</tr></thead><tbody>");

        for (int row = 0; row < maxRowsPerDay; row++)
        {
            sb.Append("<tr>");
            foreach (var dayOfWeek in dayMapping.Keys)
            {
                if (schedulesByDay.TryGetValue(dayOfWeek, out var daySchedules) && row < daySchedules.Count)
                {
                    var schedule = daySchedules[row];

                    bool isLicenseOrSpecialSchedule = (schedule.StartTime == TimeSpan.Zero || schedule.StartTime == default(TimeSpan))
                                                     && (schedule.EndTime == TimeSpan.Zero || schedule.EndTime == default(TimeSpan))
                                                     && !string.IsNullOrWhiteSpace(schedule.Observation);

                    if (isLicenseOrSpecialSchedule)
                    {
                        if (row == 0)
                            sb.Append($"<td colspan='3' rowspan='{maxRowsPerDay}' style='font-style:italic; padding: 8px; border: 1px solid #ddd; text-align: center; vertical-align: middle;'>{schedule.Observation}</td>");
                    }
                    else
                    {
                        var isLast = row == daySchedules.Count - 1;
                        var endTime = isLast && !string.IsNullOrWhiteSpace(schedule.Observation)
                            ? schedule.Observation
                            : schedule.EndTime?.ToString(@"hh\:mm");

                        sb.Append($"<td style='padding: 8px; border: 1px solid #ddd;'>{schedule.StartTime:hh\\:mm}</td>");
                        sb.Append($"<td style='padding: 8px; border: 1px solid #ddd;'>{endTime}</td>");
                        sb.Append($"<td style='padding: 8px; border: 1px solid #ddd;'>{schedule.WorkstationName}</td>");
                    }
                }
                else
                {
                    if (row == 0)
                        sb.Append($"<td colspan='3' rowspan='{maxRowsPerDay}' style='font-style:italic; padding: 8px; border: 1px solid #ddd; text-align: center; vertical-align: middle;'>No programado</td>");
                }
            }
            sb.Append("</tr>");
        }

        sb.Append("</tbody></table>");
        sb.Append("</br></br>");

        sb.Append(@"Feliz dia !!");
        return sb.ToString();
    }
}