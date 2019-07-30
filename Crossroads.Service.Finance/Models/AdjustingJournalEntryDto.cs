using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class AdjustingJournalEntryDto
    {
        //[Journal_Entry_ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
        //[Created_Date] DATETIME NOT NULL,
        //[Sent_To_GL_Date]
        //DATETIME NULL,

        //[GL_Account_Number] NVARCHAR(20) NOT NULL,
        //[Amount] MONEY NOT NULL,
        //[Adjustment] NVARCHAR(75) NOT NULL,
        //[Description] NVARCHAR(500),
        //[Donation_Distribution_ID] INT FOREIGN KEY REFERENCES[Donation_Distributions]([Donation_Distribution_ID]),
        //[Domain_ID] INT NOT NULL CONSTRAINT[DF_cr_Adjusting_Journal_Entries_Domain_ID] DEFAULT((1))

        //[JsonProperty(PropertyName = "contactId")]
        //public int ContactId { get; set; }

        [JsonProperty(PropertyName = "journalEntryId")]
        public int JournalEntryId { get; set; }
    }
}
