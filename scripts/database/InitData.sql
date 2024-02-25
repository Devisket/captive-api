use captive_db;

set FOREIGN_KEY_CHECKS = 0;

	truncate table bank_branchs;
	truncate table banks_info;
	truncate table order_file_configuration;
	truncate table form_checks;
	truncate table check_inventory; 

set FOREIGN_KEY_CHECKS = 1;


/*BANK INFO*/
insert into banks_info 
	(BankName,BankDescription,CreatedDate)
values('SECURITY BANK', NULL, NOW());

insert into bank_branchs 
	(BranchName, BranchAddress1, BRSTNCode, BankId)
values 
('MainBranch','#78 Sangandaan Quezon','310140045',1),
('Branch-Valenzuela','#65 Dalandanan Valenzuela','010140073',1),
('Branch-Pasig','#90 Bagong Silang Pasig','010140196',1),
('Branch-Makati','#78 Bel-air Makati','010140565',1),
('Branch-Cavite','#66 Mabuhat Cavite','010141409',1),
('Branch-Caloocan','#21 Somewhere Caloocan','030140118',1),
('Branch-Malolos','#42 Daan Malolos','300140013',1),
('Branch-Laguna','#2 Sta Rosa Laguna','060140052',1),
('Branch-Davao','#2 Sta. Ignacio Davao','010140109',1);

/*ORDER FILE CONFIGURATION*/

insert into order_file_configuration 
	(Name, ConfigurationData, ConfigurationType  ,BankId)
values
	('CPTIVE','{\"checkType\":{\"pos\":1,\"maxChar\":1},\"brstn\":{\"pos\":2,\"maxChar\":9},\"voidChar\":{\"pos\":11,\"maxChar\":1},\"accountNumber\":{\"pos\":12,\"maxChar\":12},\"accountName\":{\"pos\":24,\"maxChar\":56},\"concode\":{\"pos\":80,\"maxChar\":1},\"formType\":{\"pos\":81,\"maxChar\":2},\"quantity\":{\"pos\":83,\"maxChar\":2},\"deliverTo\":{\"pos\":85,\"maxChar\":9}}','Text', 1),
	('YSECPT','{\"checkType\":{\"pos\":1,\"maxChar\":1},\"brstn\":{\"pos\":2,\"maxChar\":9},\"voidChar\":{\"pos\":11,\"maxChar\":1},\"accountNumber\":{\"pos\":12,\"maxChar\":12},\"accountName\":{\"pos\":24,\"maxChar\":56},\"concode\":{\"pos\":80,\"maxChar\":1},\"formType\":{\"pos\":81,\"maxChar\":2},\"quantity\":{\"pos\":83,\"maxChar\":2},\"deliverTo\":{\"pos\":85,\"maxChar\":9}}','Text', 1),
	('CONSOL','{\"checkType\":{\"pos\":1,\"maxChar\":1},\"brstn\":{\"pos\":2,\"maxChar\":9},\"voidChar\":{\"pos\":11,\"maxChar\":1},\"accountNumber\":{\"pos\":12,\"maxChar\":12},\"accountName\":{\"pos\":24,\"maxChar\":56},\"concode\":{\"pos\":80,\"maxChar\":1},\"formType\":{\"pos\":81,\"maxChar\":2},\"quantity\":{\"pos\":83,\"maxChar\":2},\"checkpower\":{\"pos\":85,\"maxChar\":3},\"deliverTo\":{\"pos\":88,\"maxChar\":9}}','Text', 1),
	('NOVABU','{\"checkType\":{\"pos\":1,\"maxChar\":1},\"brstn\":{\"pos\":2,\"maxChar\":9},\"voidChar\":{\"pos\":11,\"maxChar\":1},\"accountNumber\":{\"pos\":12,\"maxChar\":12},\"accountName\":{\"pos\":24,\"maxChar\":56},\"concode\":{\"pos\":80,\"maxChar\":1},\"formType\":{\"pos\":81,\"maxChar\":2},\"quantity\":{\"pos\":83,\"maxChar\":2},\"checkpower\":{\"pos\":85,\"maxChar\":3},\"deliverTo\":{\"pos\":88,\"maxChar\":9}}','Text', 1);
	


/*INSERT PRODUCT TYPE*/
insert into product_type 
	(ProductName, BankInfoId)
	('REGULAR', 1),
	('MANAGER CHECK', 1),
	('CHECKRIGHT WITH VOUCHER', 1),
	('CHECKRIGHT WITHOUT VOUCHER', 1),
	('CHECKONE', 1),
	('CHECKPOWER', 1),
	
	


/*FORM_CHECKS*/
insert into form_checks
	(CheckType,FormType,Description,Quantity,BankId, OrderFileConfigurationId)
values
	("A","05","REGULAR PERSONAL (50)", 50, 1, 1),
	("B","16","REGULAR COMMERCIAL (100)", 100, 1, 1),
	("B","20","MANAGER'S CHECK (MC) (50)", 50, 1, 1),
	("F","26","COMMERCIAL - CHECKRIGHT WITH VOUCHER (100)", 100, 1, 1),
	("F","27","COMMERCIAL - CHECKRIGHT WITHOUT VOUCHER (100)", 100, 1, 1),
	("A","05","REGULAR PERSONAL (50)", 50, 1, 2),
	("B","16","REGULAR COMMERCIAL (100)", 100, 1, 2),
	("F","25","PERSONAL (25)", 25, 1, 3),
	("F","26","COMMERCIAL (50)", 50, 1, 3),
	("E","23","PERSONAL (25)", 25, 1, 4),
	("E","22","COMMERCIAL (50)", 50, 1, 4);
	
	
	
