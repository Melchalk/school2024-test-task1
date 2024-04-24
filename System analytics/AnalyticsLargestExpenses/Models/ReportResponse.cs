﻿namespace AnalyticsLargestExpenses.Models;

/// <summary>
/// The class for generating a report on the most profitable months
/// </summary>
public class ReportResponse
{
    public required List<string> Months { get; set; }
}