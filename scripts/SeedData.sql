use CaptiveDB
go

DECLARE @bankId UNIQUEIDENTIFIER;

IF NOT EXISTS(SELECT 1 FROM banks_info where BankName = 'SampleBank')
BEGIN
	INSERT INTO banks_info 
		(Id, BankName, ShortName, BankDescription, CreatedDate) 
	VALUES
		(NEWID(), 'SampleBank', 'SB', 'This is a test bank', GETDATE())

	select @bankId = Id from banks_info where BankName = 'SampleBank'


	INSERT INTO bank_branchs
		(Id, BranchName, BRSTNCode, BranchAddress1, BranchAddress2, BranchStatus, BankInfoId)
	VALUES
		(NEWID(), 'Sample - MainBranch','1239', 'Quezon City', 'Sta Cruz Lapaz', 'Active', @bankId)

END;

