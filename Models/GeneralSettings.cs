namespace HrBackend.Models;

public class GeneralSettings
{
    public int Id { get; set; } = 1; //Singleton row
    public CalculationMethod OvertimeCalculationMethod { get; set; }
    [Range(0,double.MaxValue)]
    public decimal OvertimeValue { get; set; }

    public CalculationMethod DeductionCalculationMethod { get; set; }
    [Range(0, double.MaxValue)]
    public decimal DeductionValue { get; set; }

    //Weekly of days
    public DayOfWeek WeeklyOfDay1 { get; set; }
    public DayOfWeek? WeeklyOfDay2 { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
