using AutoMapper;
using MinistryPlatform.Adjustments;
using System;
using System.Collections.Generic;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;

namespace Crossroads.Service.Finance.Services.Exports
{
    public class ExportService: IExportService
    {
        private readonly IAdjustmentRepository _adjustmentRepository;
        private readonly IMapper _mapper;

        public ExportService(IAdjustmentRepository adjustmentRepository, IMapper mapper)
        {
            _adjustmentRepository = adjustmentRepository;
            _mapper = mapper;
        }

        public void CreateJournalEntries()
        {
            // get adjustments that are not exported
            var yesterday = DateTime.Now.AddDays(-1);
            var startDate = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day);
            var endDate = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);
            var mpAdjustingJournalEntries = _adjustmentRepository.GetAdjustmentsByDate(startDate, endDate);

            // segment by account, then sort by donation date
            Dictionary<string, Dictionary<int, JournalEntryDto>> journalEntryDtos = new Dictionary<string, Dictionary<int, JournalEntryDto>>();


            // mark adjustments

            // create journal entries
        }
    }
}
