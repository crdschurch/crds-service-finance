
/*
 * this was used to export donations from MP to debug an issue with batches
 * this doesnt bring over any related data (contact, donor account) to keep it simple,
  * and also sets the donor_id to 1 since that is required
 */

DECLARE @batchId INT = 414878;

select CAST(1 as int) AS Donor_ID, Donation_Amount, Donation_Date, Payment_Type_ID, Donation_Status_ID, Transaction_Code, Domain_ID
from Donations
where Batch_ID = @batchId;
