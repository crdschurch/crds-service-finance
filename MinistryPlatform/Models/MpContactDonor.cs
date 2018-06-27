using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Serialization;

namespace MinistryPlatform.Models
{
    /// <summary>
    /// Represents a MP Contact and potentially a Donor associated to the contact.  
    /// This could be for a registered User who has given online, or a Guest giver 
    /// who has previously given online, or for someone who has given cash or checks 
    /// directly without ever logging in.  There are various properties on this object
    /// that can be referenced to determine the state of the MpContactDonor.
    /// </summary>
    public class MpContactDonor
    {
        public int ContactId { get; set; }
        public int DonorId { get; set; }
        public string StatementFreq { get; set; }
        public string StatementType { get; set; }
        public int StatementTypeId { get; set; }
        public string StatementMethod { get; set; }
        public DateTime SetupDate { get; set; }
        public string ProcessorId { get; set; }
        public string Email { get; set; }
        public bool RegisteredUser { get; set; }
        public MpContactDetails Details { get; set; }
        public MpDonorAccount Account { get; set; }


        /// <summary>
        /// Returns true if this MpContactDonor represents an existing MP Contact, false if not.
        /// </summary>
        public bool ExistingContact { get { return (ContactId > 0); } }

        /// <summary>
        /// Returns true if this MpContactDonor represents an existing MP Donor, false if not.
        /// An existing Donor implies that there is an existing Contact.
        /// </summary>
        public bool ExistingDonor { get { return (ExistingContact && DonorId > 0); } }

        /// <summary>
        /// Returns true if this MpContactDonor has a record setup at the payment processor, false if not.
        /// </summary>
        public bool HasPaymentProcessorRecord { get { return (!String.IsNullOrWhiteSpace(ProcessorId)); } }

        public bool HasDetails { get { return (Details != null); } }

        public bool HasAccount { get { return (Account != null); } }
    }

    public class MpContactDetails
    {
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public MpPostalAddress Address { get; set; }
        public int HouseholdId { get; set; }

        public bool HasAddress { get { return (Address != null); } }
    }
}
