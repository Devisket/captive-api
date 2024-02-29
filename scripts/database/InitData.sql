use captive_db;

set FOREIGN_KEY_CHECKS = 0;

	truncate table bank_branchs;
	truncate table banks_info;
	truncate table check_orders;
	truncate table batch_files;
	truncate table order_files;
	truncate table order_file_logs;
	truncate table form_checks;
	truncate table check_inventory; 
	truncate table product_configuration;
	truncate table product_type;
	truncate table order_file_configuration;
set FOREIGN_KEY_CHECKS = 1;


/*BANK INFO*/
insert into banks_info 
	(BankName,BankDescription,CreatedDate, ShortName)
values('SECURITY BANK', NULL, NOW(), "STBC");

insert into bank_branchs 
	(BranchName, BranchAddress1, BranchAddress2, BranchAddress3, BRSTNCode, BankId)
values 
('MainBranch','#78 Sangandaan','Quezon City','','310140045',1),
('Branch-Valenzuela','#65 Dalandanan','Valenzuela City','','010140073',1),
('Branch-Pasig','Unit 32 St. Lukes Bldg.','#90 Bagong Silang','Pasig City','010140196',1),
('Branch-Makati','56 Floor Armetis Bldg.', '#78 Bel-air', 'Makati','010140565',1),
('Branch-Cavite','#66 Mabuhat Cavite','','','010141409',1),
('Branch-Caloocan','Unit23 Apartment Bldg.','21 Somewhere ','#Caloocan City','030140118',1),
('Branch-Malolos','#42 Daan Malolos','','','300140013',1),
('Branch-Laguna','#2 Sta Rosa Laguna','','','060140052',1),
('Branch-Davao','Unit 21 Bldg.' ,'#2 Sta. Ignacio', 'Davao City', '010140109',1);

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
values
	('REGULAR', 1),
	('MANAGER CHECK', 1),
	('CHECKRIGHT WITH VOUCHER', 1),
	('CHECKRIGHT WITHOUT VOUCHER', 1),
	('CHECKONE', 1),
	('CHECKPOWER', 1);
	
/*INSERT PRODUCT CONFIGURATION*/	
insert into product_configuration 
	(OrderFileConfigurationId, ProductTypeId)
values
	(1,1),
	(1,2),
	(1,3),
	(1,4),
	(2,1),
	(3,5),
	(4,6);

/*FORM_CHECKS*/
insert into form_checks
	(CheckType,FormType,Description,Quantity,BankId,ProductTypeId, FileInitial  )
values
	("A","05","PERSONAL (50)", 50, 1, 1, 'P'),
	("B","16","COMMERCIAL (100)", 100, 1, 1, 'C'),
	("B","20","MANAGER'S CHECK (MC) (50)", 50, 1, 2, 'M'),
	("F","26","COMMERCIAL - CHECKRIGHT WITH VOUCHER (100)", 100, 1, 3, 'C'),
	("F","27","COMMERCIAL - CHECKRIGHT WITHOUT VOUCHER (100)", 100, 1, 4, 'C'),
	("F","25","PERSONAL (25)", 25, 1, 5, 'P'),
	("F","26","COMMERCIAL (50)", 50, 1, 5, 'C'),
	("E","23","PERSONAL (25)", 25, 1, 6, 'P'),
	("E","22","COMMERCIAL (50)", 50, 1, 6, 'C');
	
	
	
