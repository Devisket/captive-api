use captive_db;

set FOREIGN_KEY_CHECKS = 0;

	truncate table bank_branchs;
	truncate table banks_info;
	truncate table order_file_configuration; 

set FOREIGN_KEY_CHECKS = 1;


/*BANK INFO*/
insert into banks_info 
	(BankName,BankDescription,CreatedDate)
values('SECURITY BANK', NULL, NOW());

insert into bank_branchs 
	(BranchName, BranchAddress, BRSTNCode, BankId)
values 
('MainBranch','#78 Sangandaan Quezon','310140045',1),
('Branch-Valenzuela','#65 Dalandanan Valenzuela','310140045',1),
('Branch-Pasig','#90 Bagong Silang Pasig','310140045',1),
('Branch-Makati','#78 Bel-air Makati','310140045',1),
('Branch-Cavite','#66 Mabuhat Cavite','310140045',1),
('Branch-Caloocan','#21 Somewhere Caloocan','310140045',1),
('Branch-Malolos','#42 Daan Malolos','310140045',1),
('Branch-Laguna','#2 Sta Rosa. Laguna','310140045',1);

/*ORDER FILE CONFIGURATION*/

insert into 
	order_file_configuration (Name, ConfigurationData)
values
	('CPTIVE','{\"checkType\":{\"pos\":1,\"maxChar\":1},\"brstn\":{\"pos\":2,\"maxChar\":9},\"voidChar\":{\"pos\":11,\"maxChar\":1},\"accountNumber\":{\"pos\":12,\"maxChar\":12},\"accountName\":{\"pos\":24,\"maxChar\":56},\"concode\":{\"pos\":80,\"maxChar\":1},\"formType\":{\"pos\":81,\"maxChar\":2},\"quantity\":{\"pos\":83,\"maxChar\":2},\"deliverTo\":{\"pos\":85,\"maxChar\":9}}');
