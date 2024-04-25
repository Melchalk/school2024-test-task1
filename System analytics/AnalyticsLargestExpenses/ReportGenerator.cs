﻿using AnalyticsLargestExpenses.Interfaces;
using AnalyticsLargestExpenses.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AnalyticsLargestExpenses;

public class ReportGenerator : IReportGenerator
{
    private const string OUTPUT_FILE = "months.json";

    private readonly string _directoryRoot;
    private readonly string _inputFile;
    private readonly ILogger<ReportGenerator> _logger;

    private readonly IReportHandler _reportHandler;

    private readonly IJsonConverter<List<PurchaseDto>> _jsonConverterPurchase;
    private readonly IJsonConverter<ReportResponse> _jsonConverterResponse;

    public ReportGenerator(
        IReportHandler reportHandler,
        IJsonConverter<List<PurchaseDto>> jsonConverterPurchase,
        IJsonConverter<ReportResponse> jsonConverterResponse,
        ILogger<ReportGenerator> logger,
        IOptions<InputFileInfo> fileInfo)
    {
        _directoryRoot = new DirectoryInfo(@"..\..\..\").Parent!.FullName;
        _logger = logger;
        _reportHandler = reportHandler;
        _jsonConverterPurchase = jsonConverterPurchase;
        _jsonConverterResponse = jsonConverterResponse;
        _inputFile = fileInfo.Value.Name;
    }

    public string GetReportInJson()
    {
        var purchase = _jsonConverterPurchase
            .DeserializeJsonFromFile(Path.Combine(_directoryRoot, _inputFile));

        if (purchase is null)
        {
            _logger.LogError("The original data was null");
            throw new ArgumentNullException("The original data was null");
        }

        var report = _reportHandler.GetReport(purchase);

        string reportInJson = _jsonConverterResponse
            .SerializeJsonToFile(Path.Combine(_directoryRoot, OUTPUT_FILE), report);

        _logger.LogInformation($"Результат формирования отчета:\n{reportInJson}");

        return reportInJson;
    }
}
