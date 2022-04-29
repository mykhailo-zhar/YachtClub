using Microsoft.EntityFrameworkCore;
using System.Linq;
using Project.Migrations;

namespace Project.Database
{
    public class SeedData
    {
		public static void RestartDatabase(DataContext dbContext, bool ToSeed = true)
        {
			#region Создание базы

			if (ToSeed)
			{
				#region Удаление старой базы и доменов
				//Удаление Виртуальных Поименованых Производных Таблиц
				dbContext.Database.ExecuteSqlRaw(@"
					drop view if exists  AvailableResources, repair_staff, staff ;
				");

				//Удаление таблиц
				dbContext.Database.ExecuteSqlRaw(@"
drop table if exists 
  	Position, YachtType, ContractType, MaterialType, YachtLeaseType,
  	Staff, Event, Client, Seller, Person,
	Material, Yacht,
	MaterialLease, YachtTest, Repair, ExtradationRequest, YachtLease,
	Contract, Review,
	Winner, Staff_Position, Yacht_Crew, Repair_Men, Review_Contract, Review_Yacht, Review_Captain, Position_YachtType, Position_Equivalent, HiredStaff
Cascade;

				");

				//Удаление доменов
				dbContext.Database.ExecuteSqlRaw(@"
drop domain if exists Sex, My_Money, Mail, Phonenumber
;
				");
                #endregion

                #region Создание доменов
				//Создание доменов
                dbContext.Database.ExecuteSqlRaw(@"
CREATE Domain SEX as varchar
CHECK (Value in ('Male','Female','Other'));

CREATE Domain My_Money as decimal
CHECK (Value >= 0.0);

CREATE Domain Mail as varchar
CHECK (Value ~ '^[A-Za-z0-9._%\-]+@[A-Za-z0-9]+[.][A-Za-z]+$');

CREATE DOMAIN PhoneNumber as varchar 
CHECK (Value ~ '^[+][0-9]{{12}}$');

set datestyle = 'iso, dmy'; 
				");

                #endregion

                #region Таблицы типов

				//Должность
                dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Position (
  ID    			serial    		Not Null  	Primary Key,
  Name   			varchar   		Not Null  	Unique,
  Salary        	My_Money    	Not Null
);
				");

				//Тип материала
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE MaterialType(
  ID		serial     Not Null		Primary Key,
  Name		varchar    Not Null		Unique,
  Metric	varchar    Default NULL,
  Description	text	Default ' '
);
				");

				//Тип яхты
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE YachtType(
  	ID        			serial     	Not Null		Primary Key,
  	Name        		varchar     Not Null		Unique,
	Frame				varchar		Not Null		check(Frame in ('Single','Twain', 'Ternary')),	
	Goal				varchar		Not Null		check(Goal in ('Sport','Cruise')),
	Class				varchar		Not Null		check(Class in ('Sport','Kiel','Schwertbott','Compromiss')),
  	CrewCapacity  		int       	Not Null    	check(CrewCapacity > 0),
  	Capacity     		int      	Not Null    	check(Capacity >  0),  
  	Sails        		int       	Not Null    	check(Sails >= 0),
	Description	text	Default ' '
);
				");

				//Тип контракта
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE ContractType(
  ID    		serial    	Not Null	Primary Key,
  Name     	  	varchar   	Not Null	Unique,
  Price    	  	My_Money   	Not Null,	
  Description	text	Default ' '
);
				");
				
				//Тип аренды яхты
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE YachtLeaseType(
  ID    		serial    	Not Null	Primary Key,
  Name     	  	varchar   	Not Null	Unique,
  Price    	  	My_Money   	Not Null,
  StaffOnly		bool		Not Null	Default FALSE,
  Description	text	Default ' '
);
				");
				
				//Тип продавца
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Seller (
	ID			serial		Not Null	primary key,
	Name		varchar		Not Null	unique,
	Description	text	Default ' '
);
				");


                #endregion

                #region Блок основных таблиц
				#region Блок независимых таблиц

				//Персона
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Person(
  	ID    			serial    	Not Null	Primary Key,
  	Name      		varchar    	Not Null,
  	Surname      	varchar    	Not Null,
  	BirthDate    	timestamp(2)   Not Null,
	Sex				Sex			Not Null,	
  	Email      		Mail    	Not Null	unique,
	Phone			PhoneNumber	Not Null	unique,
	RegistryDate	timestamp(2)	Not Null	check(BirthDate < RegistryDate)	Default current_timestamp
);
				");

				//Событие
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Event(
	ID			serial		Not Null	Primary Key,
	Name		varchar		Not Null,
	StartDate	timestamp(2)		Not Null,
	EndDate		timestamp(2)		check(StartDate <= EndDate ),
	Duration	timestamp(2)		Not Null	check(StartDate <= Duration),
	Description		text		Default ' ',
	UserRate		int			Default 0,
	CanHaveWinners	boolean		not null	Default true,

	unique(Name,StartDate)
);
				");

                #endregion
					
				
                #region Блок зависимых таблиц

                #region Поколение 1
                //Человек на должности
                dbContext.Database.ExecuteSqlRaw(@"
Create TABLE Staff_Position(
	ID				serial		Not Null	Primary Key,
	StaffID			int			Not Null
	References 	Person(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	PositionID		int			Not Null
	References 	Position(ID)	
	On Update Cascade	
	On Delete Cascade,
	StartDate		timestamp(2)		Not Null,
	EndDate			timestamp(2)		check(StartDate <= EndDate),
	Salary        	My_Money,
	Description		Text		Default ' '
);
				");

				//Материал
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Material(
	ID		serial		Not Null	Primary Key,
	Name	varchar		Not Null	Unique,
	Metric	varchar     Default NULL,
	TypeID	int			Not Null
	References 	MaterialType(ID)	
	On Update Cascade	
	On Delete Cascade
);
				");

				//Яхта
				dbContext.Database.ExecuteSqlRaw(@"
Create TABLE Yacht(
	ID					Serial		Not Null	Primary Key,
	Name				varchar		Not Null,
	Status				varchar		Not Null	check(Status in ('Valid', 'Invalid')),
	Rentable			bool		Not Null	Default TRUE,
	Registrydate		timestamp(2)	Not Null	Default current_timestamp,
	Description			text		Not Null	Default ' ',
	TypeID				int			Not Null
	References 	YachtType(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	YachtOwnerID	int			Not Null
	References 	Person(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	Unique(Name,TypeID)
);
				");
				#endregion

				#region Поколение 2
				//Договор на поставку материала
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE MaterialLease(
	ID				serial		Not Null	Primary Key,
	Material		int			Not Null	
	References 	Material(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	Seller 			int			Not Null
	References Seller(ID)
	On Update Cascade	
	On Delete Cascade,
	
	PricePerUnit			My_Money	Not Null,
	Count 					int			Not Null	check(Count > 0),
	OverallPrice			My_Money	Not Null	check(OverallPrice = Count * PricePerUnit),
	StartDate				timestamp(2)		Not Null,
	DeliveryDate			timestamp(2)		check(StartDate <= DeliveryDate)
);
				");

				//Тестовый заплыв яхты
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE YachtTest(
	ID			serial		Not Null	Primary Key,
	Date		timestamp(2)		Not Null,
	Results		text		Not Null,
	YachtID		int			Not Null
	References 	Yacht(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	StaffID		int			Not Null
	References 	Staff_Position(ID)	
	On Update Cascade	
	On Delete Cascade
);
				");

				//Ремонт
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Repair(
	ID			serial			Not Null	Primary Key,
	StartDate	timestamp(2)		Not Null	Default current_timestamp,
	EndDate		timestamp(2)		check(StartDate <= EndDate )	Default null,
	Duration	timestamp(2)		Not Null	check(StartDate <= Duration)	Default current_timestamp,
	Status		varchar			Not Null	check(Status in ('Created','Waits', 'Canceled', 'In Progress','Done'))	Default 'Created',
	Personnel	int				Not Null	check(Personnel > 0)	Default 1,
	Description	text			Not Null	Default ' ',
	YachtID		int				Not Null
	References 	Yacht(ID)	
	On Update Cascade	
	On Delete Cascade
);
				");
				
				//Команда судна
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Yacht_Crew(
	ID			serial		Not Null	Primary Key,
	YachtID		int			Not Null
	References 	Yacht(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	CrewID		int			Not Null
	References 	Staff_Position(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	StartDate	timestamp(2)		Not Null,
	EndDate		timestamp(2)		check(StartDate <= EndDate),
	Description text		Default ' '
);
				");
				
				//Список учавствовавших в мероприятии
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Winner(
	EventID		int		Not Null
	References 	Event(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	YachtID		int		Not Null
	References 	Yacht(ID)	
	On Update Cascade	
	On Delete Cascade,
	Place		int		check(Place > 0),
	
	Primary Key (EventID, YachtID)
);
				");
				
				//Заявка на выдачу
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE ExtradationRequest(
	ID			serial		Not Null	Primary Key,
	Count		int			Not Null	check(Count > 0),

	Material	int			Not Null	
	References 	Material(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	StaffID		int			Not Null
	References 	Staff_Position(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	RepairID	int			Not Null
	References 	Repair(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	StartDate	timestamp(2)		Not Null,
	EndDate		timestamp(2)		check(StartDate <= EndDate),
	Duration	timestamp(2)		Not Null	check(StartDate <= Duration),
	Description text			Not Null	Default ' ',
	Status		varchar			Not Null	check(Status in ('Created', 'Canceled', 'Waits', 'Done'))
);
				");
				
				//Договор на аренду места
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE YachtLease(
	ID				serial			Not Null	Primary Key,
	StartDate		timestamp(2)		Not Null,
	EndDate			timestamp(2)		check(StartDate <= EndDate ),
	Duration		timestamp(2)		Not Null	check(StartDate <= Duration),
	OverallPrice	My_Money		Not Null,
	Specials		text			Not Null	Default ' ',
	Paid			bool			Not Null	Default False,
	YachtID			int				Not Null
	References 	Yacht(ID)	
	On Update Cascade	
	On Delete Cascade,

	YachtLeaseTypeID	int			Not Null
	References 	YachtLeaseType(ID)	
	On Update Cascade	
	On Delete Cascade
);
				");

				#endregion

				#region Поколение 3

				//Контракт
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Contract(
	ID				serial		Not Null	Primary Key,
	ClientID		int			Not Null
	References 	Person(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	ContractTypeID	int			Not Null
	References 	ContractType(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	YachtWithCrewID	int			Not Null
	References 	Yacht_Crew(ID)	
	On Update Cascade	
	On Delete Cascade,

	StartDate		timestamp(2)		Not Null,
	EndDate			timestamp(2)		check(StartDate <= EndDate ),
	Duration		timestamp(2)		Not Null	check(StartDate <= Duration),
	Specials		text		Not Null,
	Status			varchar		Not Null,
	Paid			bool		Not Null	Default False,
	AverallPrice	My_Money	Not Null
);
				");

				#endregion

				#region Поколение 4

				//Отзыв
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Review(
	ID				serial		Not Null	Primary Key,
	ClientID		int			Not Null
	References 	Person(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	Date			timestamp(2)		Not Null,
	Text			text		Not Null,
	Public			bool		Not Null	Default true,
	UserRate		int			Not Null	Default 0,
	Rate			int 		Not Null 	check(Rate > 0 AND Rate <= 5)
);

				");


				#endregion

				#endregion

				#region Справочные таблицы
				//Справочная таблица ремонтников
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Repair_Men(
	ID			serial		Not Null	Primary Key,	

	RepairID	int			Not Null
	References 	Repair(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	StaffID		int			Not Null	
	References 	Staff_Position(ID)	
	On Update Cascade	
	On Delete Cascade,

	Unique(RepairID, StaffID)
);
				");

				//Обзоры на яхту
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Review_Yacht(
	ReviewID	int			Not Null
	References 	Review(ID)	
	On Update Cascade	
	On Delete Cascade,

	YachtID	int				Not Null
	References 	Yacht(ID)	
	On Update Cascade	
	On Delete Cascade,

	Primary Key ( ReviewID, YachtID )
);
				");

				//Обзоры на капитана
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Review_Captain(
	ReviewID	int			Not Null
	References 	Review(ID)	
	On Update Cascade	
	On Delete Cascade,

	CaptainID	int			Not Null
	References 	Staff_Position(ID)	
	On Update Cascade	
	On Delete Cascade,

	Primary Key ( ReviewID, CaptainID )
);
				");
				
				//Список должностей, обязательно присутствующих на яхте
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Position_YachtType(
	PositionID	int		Not Null
	References 	Position(ID)	
	On Update Cascade	
	On Delete Cascade,

	YachtTypeID	int		Not Null
	References 	YachtType(ID)	
	On Update Cascade	
	On Delete Cascade,

	Count		int		Not Null	check (Count > 0),

	Primary Key ( PositionID, YachtTypeID )
);
				");

				//Наёмный персонал
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE HiredStaff(
	Yacht_CrewID	int		Not Null
	References  Yacht_Crew(ID)	
	On Update Cascade	
	On Delete Cascade,

	ClientID	int		Not Null
	References 	Person(ID)	
	On Update Cascade	
	On Delete Cascade,

	Primary Key ( Yacht_CrewID, ClientID)
);
				");

				#endregion

				#endregion
			}

			#endregion
        }
		public static void SeedWithData(DataContext dbContext, bool Force = false)
        {
			dbContext.Database.EnsureCreated();
			if (dbContext.Position.Count() == 0 || false)
			{
                #region Типы
                //Должности
                dbContext.Database.ExecuteSqlRaw($@"
                               INSERT INTO Position(name,salary)
                               values
                               ('Owner', 					10000.0),
                               ('Personell Officer', 		5000.0),
                               ('Storekeeper', 			5000.0),
                               ('Repairman', 				1000.0),
                               ('Captain', 				3500.0),
                               ('Boatswain',				2500.0),
                               ('Cook', 					1500.0),
                               ('Shipboy', 				  0.0),
                               ('Hired Captain',			0.0),
                ('Hired Boatswain',			   0.0),
                               ('Hired Cook', 				0.0),
                               ('Hired Shipboy', 			0.0),
                               ('None', 					0.0),
                               ('Fired', 					0.0);
                           ");

				//Типы яхт
				dbContext.Database.ExecuteSqlRaw($@"
INSERT INTO YachtType(name,frame,goal,class,crewcapacity, capacity, sails)
values
('KA-6',	'Single', 	'Cruise', 	'Kiel', 		3, 3, 1),
('KA-5',	'Single', 	'Cruise', 	'Kiel', 		3, 2, 1),
('KA-4',	'Single', 	'Cruise', 	'Kiel', 		3, 1, 1),
('S-2',		'Twain', 	'Sport', 	'Sport', 		2, 1, 2),
('S-1',		'Single', 	'Sport', 	'Sport', 		2, 2, 0), 
('M-5',		'Single', 	'Cruise', 	'Schwertbott', 	2, 5, 0),
('M-4',		'Single', 	'Cruise', 	'Schwertbott', 	3, 5, 0)
;
				");

				//Типы материалов
				dbContext.Database.ExecuteSqlRaw($@"
INSERT INTO MaterialType(name)
values
('Fuel'),
('Parts'),
('Sails'),
('Engine'),
('Instruments');

UPDATE MaterialType SET Metric = 'kg' where Name = 'Fuel'
				");

				//Типы контрактов
				dbContext.Database.ExecuteSqlRaw($@"
Insert INTO ContractType(name,price)
values
('Standart'			, 400.0),
('Premium'			, 750.0),
('VIP'				, 1999.9),
('Party'			, 1500.0),
('Training Trip'	, 750.0),
('Romantic Holidays', 850.0),
('Family Trip'		, 1000.0);
				");

				//Типы договоров на аренду яхту
				dbContext.Database.ExecuteSqlRaw($@"
Insert INTO YachtLeaseType(name,price)
values
('Standart'				, 0.0),
('Premium'				, 500.0),
('VIP'					, 9999.9),
('StaffOnly'			, 0.0)
;

Update YachtLeaseType set StaffOnly = TRUE where Name = 'StaffOnly';
				");
                #endregion

                #region Блок независимых таблиц
                //Продавец
                dbContext.Database.ExecuteSqlRaw($@"
insert into seller(name)
values
('Umeco Inc.'),
('E(P)RON'),
('Engines and engines')
;
				");

				//Персонал
				dbContext.Database.ExecuteSqlRaw($@"
insert into Person (name, surname, sex, BirthDate, RegistryDate,email, phone)
values 
('Tatiana'	, 	'Sparklovna'	, 'Female'	, '07-01-2001', '07-01-2019', 't.sparkle@gmail.com',	'+380982334566'	),
('Anatoliy'	, 	'Egorov'		, 'Male'	, '08-01-2001', '07-01-2019', 'a.gorov@gmail.com',		'+380432343566'	),
('Daria'	, 	'Jinova'		, 'Female'	, '09-07-2001', '12-01-2019', 'djin@gmail.com',			'+380982354666'	),
('Valeriya'	, 	'Hlebova'		, 'Female'	, '10-11-2001', '13-01-2019', 'gdemoigleb@gmail.com', 	'+380912312366'	),
('Dmitriy'	, 	'Gabov'			, 'Male'	, '20-05-2001', '15-01-2019', 'jabytop@gmail.com',		'+380986786666'	),
('Vitaliy'	, 	'Hmuriy'		, 'Male'	, '10-09-2001', '20-01-2019', 'net@gmail.com',			'+380989994566'	),
('Denis'	, 	'Otsutstvuet'	, 'Female'	, '19-12-2001',	'12-02-2019', 'dotatop@gmail.com',		'+380984564566' ),
('Dasha'	, 	'Raduga'		, 'Female'	, '21-01-1997',	'13-02-2019', '20.cooler@gmail.com',	'+380988884566' ),
('Valeriy'	, 	'Makov'			, 'Male'	, '19-05-2001', '13-02-2019', 'gdemoihleb@gmail.com',	'+380967866566'	),
('Dmitriy'	, 	'Gabov'			, 'Male'	, '15-04-2001', '15-02-2019', 'j4byt0p@gmail.com', 		'+380982254566' ),
('Vitaliy'	, 	'Hmuriy'		, 'Male'	, '15-03-2001', '20-03-2019', 'da_da@gmail.com',		'+380923444466'	),
('Denis'	, 	'Bliznec'		, 'Male'	, '10-01-2002',	'23-03-2019', 'nashapo4ta@gmail.com',	'+380982376566' ),
('Denis'	, 	'Bliznec'		, 'Male'	, '10-01-2002',	'23-03-2019', 'denis2@gmail.com',		'+380982334565' ),
('Christina', 	'Hrizaleva'		, 'Female'	, '08-05-2001',	'24-03-2019', 'chrysalis@gmail.com',	'+380986766778' ),
('Dimetrius', 	'Zhbanov'		, 'Male'	, '30-11-2000',	'24-03-2019', 'd-zhb11@gmail.com',	 	'+380943222990'	),
('Nikola'	, 	'Krabova'		, 'Female'	, '15-04-2001', '24-04-2019', 'ja213t0p@gmail.com', 	'+380534434336' ),
('Jim'		, 	'Morzhov'		, 'Male'	, '15-03-2001', '24-04-2019', 'dafasda@gmail.com',		'+380925557666'	),
('Lina'		, 	'Krest'			, 'Female'	, '10-01-2002',	'25-04-2019', 'na42221da@gmail.com',	'+380982376896' ),
('Denis'	, 	'Smirk'			, 'Male'	, '10-01-2002',	'26-04-2019', 'den123s2@gmail.com',		'+380981343565' );

insert into Person (name, surname, sex, BirthDate, RegistryDate, email, phone)
values
('Yachtclub', 	'',				'Other', 	'07-01-2019',	'08-01-2019',	'yacht_club@gmail.com',  '+380983334590'	),
('Alexei', 		'Britov', 		'Male', 	'02-03-1947', 	'08-01-2019',	'a_brit@gmail.com',		 '+380986769990'	),
('Melnik', 		'Baranov', 		'Male', 	'05-04-1967', 	'08-01-2019',	'mebar@mail.ru',		 '+380986888990'	),
('Dmitriy', 	'Bideshev', 	'Male', 	'15-01-1989', 	'08-01-2019',	'biDeshev777@gmail.com', '+380986868990'	),
('Gomel', 		'Bogdanov', 	'Male', 	'16-10-1963', 	'08-01-2019',	'kosolov@gmail.com',	 '+380986765560'	),
('Jamala', 		'Nebinarna', 	'Female', 	'23-09-1954', 	'08-01-2019',	'j_4354@gmail.com',		 '+380986768991'	);
				");
				
				//События
				dbContext.Database.ExecuteSqlRaw($@"
insert into event(name, startdate, enddate, duration)
values 
('First YachtClub Event', 			'01-02-2019', 	'03-02-2019', 	'03-02-2019'),
('SpringRace #1', 					'10-05-2019', 	'10-05-2019', 	'10-05-2019'),
('International Regatta #456', 		'07-09-2019', 	'04-10-2019', 	'04-10-2019'),
('1st Anniversary Event',			'07-01-2020',	'10-01-2020', 	'12-01-2020'),
('SpringRace #2', 					'10-05-2020', 	'10-05-2020', 	'10-05-2020'),
('International Regatta	#457', 		'07-09-2019', 	'04-10-2019', 	'04-10-2019'),
('Anniversary Event',				'07-01-2021', 	'10-01-2021', 	'10-01-2021')
;
				");
                #endregion

                #region Зависимые таблицы

                #region Первое поколение
                //Материалы
                dbContext.Database.ExecuteSqlRaw($@"
/*('Fuel'),
('Parts'),
('Sails'),
('Engine'),
('Instruments')*/

insert into Material(name, TypeID, Metric)
values
('Diesel Fuel 1', 				1, NULL),
('Diesel Fuel 2', 				1, NULL),
('Carbon fiber for patching', 	2, 'm2'),
('Simple fiber for patching', 	2, 'm2');

insert into Material(name, TypeID)
values
('Small sails', 				3),
('Medium sails', 				3),
('Large sails', 				3),
('E-9565-IC', 					4),
('E-9555-IC', 					4),
('EE-10005-IC', 				4),
('Screwdriver',					5),
('Cutter',						5),
('Crowbar',						5)
;
				");
				//Персонал на должностях
				dbContext.Database.ExecuteSqlRaw($@"
/*
('Tatiana'	, 	'Sparklovna'	, 'Female'	, '07-01-2001', '07-01-2019', 't.sparkle@gmail.com',	'+380982334566'	),1
('Anatoliy'	, 	'Egorov'		, 'Male'	, '08-01-2001', '07-01-2019', 'a.gorov@gmail.com',		'+380435657566'	),2
('Daria'	, 	'Jinova'		, 'Female'	, '09-07-2001', '12-01-2019', 'djin@gmail.com',			'+380982354666'	),3
('Valeriya'	, 	'Hlebova'		, 'Female'	, '10-11-2001', '13-01-2019', 'gdemoigleb@gmail.com', 	'+380912312366'	),4
('Dmitriy'	, 	'Gabov'			, 'Male'	, '20-05-2001', '15-01-2019', 'jabytop@gmail.com',		'+380986786666'	),5
('Vitaliy'	, 	'Hmuriy'		, 'Male'	, '10-09-2001', '20-01-2019', 'net@gmail.com',			'+380982359567'	),6
('Denis'	, 	'Otsutstvuet'	, 'Male'	, '19-12-2001',	'12-02-2019', 'dotatop@gmail.com',		'+380984564566' ),7
('Dasha'	, 	'Raduga'		, 'Female'	, '21-01-1997',	'13-02-2019', '20.cooler@gmail.com',	'+380982972566' ),8
('Valeriya'	, 	'Makova'		, 'Female'	, '19-05-2001', '13-02-2019', 'gdemoihleb@gmail.com',	'+380967866566'	),9
('Dmitriy'	, 	'Gabov'			, 'Male'	, '15-04-2001', '15-02-2019', 'j4byt0p@gmail.com', 		'+380982334566' ),10
('Vitaliy'	, 	'Hmuriy'		, 'Male'	, '15-03-2001', '20-03-2019', 'da_da@gmail.com',		'+380923444466'	),11
('Denis'	, 	'Bliznec'		, 'Male'	, '10-01-2002',	'23-03-2019', 'nashapo4ta@gmail.com',	'+380982376566' ),12
('Denis'	, 	'Bliznec'		, 'Male'	, '10-01-2002',	'23-03-2019', 'denis2@gmail.com',		'+380982334565' ),13
('Christina', 	'Hrizaleva'		, 'Female'	, '08-05-2001',	'24-03-2019', 'chrysalis@gmail.com',	'+380986766778' ),14
('Dimetrius', 	'Zhbanov'		, 'Male'	, '30-11-2000',	'24-03-2019', 'd-zhb11@gmail.com',	 	'+380943222990'	),15
('Nikola'	, 	'Krabov'		, 'Male'	, '15-04-2001', '24-04-2019', 'ja213t0p@gmail.com', 	'+380534434336' ),16
('Jim'		, 	'Morzhov'		, 'Male'	, '15-03-2001', '24-04-2019', 'dafasda@gmail.com',		'+380925557666'	),17
('Lina'		, 	'Krest'			, 'Female'	, '10-01-2002',	'25-04-2019', 'na42221da@gmail.com',	'+380982376896' ),18
('Denis'	, 	'Smirk'			, 'Male'	, '10-01-2002',	'26-04-2019', 'den123s2@gmail.com',		'+380981343565' );19
*/
/*
('Owner', 				10000.0),1
('Personell Officer', 	5000.0),2
('Storekeeper', 		5000.0),3
('Repairman', 			1000.0),4
('Captain', 			3500.0),5
('Boatswain',			2500.0),6
('Cook', 				1500.0),7
('Shipboy', 			500.0),8
('Hired Captain',		0.0),9
('Hired Boatswain',		2500.0),10
('Hired Cook', 			1500.0),11
('Hired Shipboy', 		500.0),12
('None', 				100.0),13
('Fired', 				0.0);14
*/

insert into Staff_Position(Startdate, enddate, Staffid, positionid)
values
('07-01-2019',	null, 1, 1),
('07-01-2019',	null, 1, 5),
('07-01-2019',	null, 2, 3),
('12-01-2019',	null, 3, 2),
('13-01-2019',	null, 4, 4),
('15-01-2019',	null, 5, 5),
('15-01-2019',	'23-01-2019', 6, 6),
('23-01-2019',	'15-02-2019', 6, 14),
('12-02-2019',	null, 7, 5),
('15-02-2019',	null, 8, 6),
('13-02-2019',	null, 9, 5),
('15-02-2019',	'13-03-2019', 10, 7),
('15-02-2019',	null, 6, 5),
('13-03-2019',	null, 10, 8),
('20-03-2019',	null, 11, 6),
('23-03-2019',	null, 12, 7),
('23-03-2019',	null, 13, 4),
('24-03-2019',	null, 14, 9), 
('24-03-2019',	null, 15, 9), 
('24-04-2019',	null, 16, 5),
('24-04-2019',	null, 17, 5),
('24-04-2019',	'27-04-2019', 18, 9),
('27-04-2019',	null, 18, 6),
('27-04-2019',	null, 19, 4);
				");
				//Яхты
				dbContext.Database.ExecuteSqlRaw($@"
/*
('KA-6',	'Single', 	'Cruise', 	'Kiel', 		3, 3, 1),1
('KA-5',	'Single', 	'Cruise', 	'Kiel', 		3, 2, 1),2
('KA-4',	'Single', 	'Cruise', 	'Kiel', 		3, 1, 1),3
('S-2',		'Twain', 	'Sport', 	'Sport', 		2, 1, 2),4
('S-1',		'Single', 	'Sport', 	'Sport', 		2, 2, 0),5
('M-5',		'Single', 	'Cruise', 	'Schwertbott', 	2, 5, 0),6
('M-4',		'Single', 	'Cruise', 	'Schwertbott', 	3, 5, 0)7
*/

/*
('Yachtclub', 	'',				'Other', 	'07-01-2019',		'yacht_club@gmail.com',  '+380983334590'	),1
('Alexei', 		'Britov', 		'Male', 	'02-03-1947', 		'a_brit@gmail.com',		 '+380986769990'	),
('Melnik', 		'Baranov', 		'Male', 	'05-04-1967', 		'mebar@mail.ru',		 '+380986888990'	),
('Dmitriy', 	'Bideshev', 	'Male', 	'15-01-1989', 		'biDeshev777@gmail.com', '+380986868990'	),
('Gomel', 		'Bogdanov', 	'Male', 	'16-10-1963', 		'kosolov@gmail.com',	 '+380986765560'	),
('Jamala', 		'Nebinarna', 	'Female', 	'23-09-1954', 		'j_4354@gmail.com',		 '+380986768991'	),
('Tatiana'	, 	'Sparklovna', 	'Female', 	'07-01-2001', 		't.sparkle@gmail.com',	 '+380982334566'	),7
('Christina', 	'Hrizaleva',  	'Female',	'08-05-2001',	 	'chrysalis@gmail.com',	 '+380986766778'	),8
('Dimetrius', 	'Zhbanov',  	'Male',		'30-11-2000',	 	'd-zhb11@gmail.com',	 '+380943222990'	);9
*/

insert into Yacht(Name, Status, TypeId, YachtOwnerID)
values
('Alpha',			'Valid',		1, 1),
('Storm',			'Invalid',		1, 20),
('Adelaida',		'Valid',		4, 20),
('Latnyk',			'Valid',		5, 14),
('Storm',			'Valid',		7, 15),
('Infernal Rage',	'Valid',		4, 20),
('Hello Kitty',		'Valid',		5, 15),
('Beda',			'Valid',		4, 20),
('Moby Dick',		'Valid',		1, 14)
;
				");
				#endregion

				#region Второе поколение

				//Договоры на поставку материалов
				dbContext.Database.ExecuteSqlRaw($@"
/*
('Diesel Fuel 1', 				(select id from materialtype where name = 'Fuel')),1
('Diesel Fuel 2', 				(select id from materialtype where name = 'Fuel')),2
('Carbon fiber for patching', 	(select id from materialtype where name = 'Parts')),3
('Simple fiber for patching', 	(select id from materialtype where name = 'Parts')),4
('Small sails', 				(select id from materialtype where name = 'Sails')),5
('Medium sails', 				(select id from materialtype where name = 'Sails')),6
('Large sails', 				(select id from materialtype where name = 'Sails')),7
('E-9565-IC', 					(select id from materialtype where name = 'Engine')),8
('E-9555-IC', 					(select id from materialtype where name = 'Engine')),9
('EE-10005-IC', 				(select id from materialtype where name = 'Engine')),10
('Screwdriver',					(select id from materialtype where name = 'Instruments')),11
('Cutter',						(select id from materialtype where name = 'Instruments')),12
('Crowbar',						(select id from materialtype where name = 'Instruments'))13
*/

/*
('Umeco Inc.'),1
('E(P)RON'),2
('Engines and engines')3
*/

insert into MaterialLease(priceperunit, count, overallprice, StartDate,DeliveryDate, material, seller)
values
(30.0, 		100, 	3000.0,		'07-01-2019',	'07-01-2019',	1, 1),
(40.0, 		100, 	4000.0,		'07-01-2019',	'07-01-2019',	2, 1),
(100.0, 	10, 	1000.0,		'07-01-2019',	'07-01-2019',	4, 1),
(1000.0, 	10, 	10000.0,	'07-01-2019',	'07-01-2019',	6, 1),
(350.0, 	10, 	3500.0,		'07-01-2019',	'07-01-2019',	11, 1),
(150.0, 	10, 	1500.0,		'07-01-2019',	'07-01-2019',	12, 1),
(3500.0, 	1, 		3500.0,		'11-10-2021',	'11-10-2021',	10, 3)
;
				");

                //Экипаж
                dbContext.Database.ExecuteSqlRaw($@"
/*
('07-01-2019',	null, 1, 1),1
('07-01-2019',	null, 1, 5),2-
('07-01-2019',	null, 2, 3),3
('12-01-2019',	null, 3, 2),4
('13-01-2019',	null, 4, 4),5
('15-01-2019',	null, 5, 5),6-
('15-01-2019',	'23-01-2019', 6, 6),7-
('23-01-2019',	'15-02-2019', 6, 11),8
('12-02-2019',	null, 7, 5),9--
('15-02-2019',	null, 8, 6),10--
('13-02-2019',	null, 9, 5),11=
('15-02-2019',	'13-03-2019', 10, 6),12
('15-02-2019',	null, 6, 5),13\
('13-03-2019',	null, 10, 8),14\
('20-03-2019',	null, 11, 6),15=
('23-03-2019',	null, 12, 7),16\
('23-03-2019',	null, 13, 4),17
('24-03-2019',	null, 14, 9),18l
('24-03-2019',	null, 15, 9),19
('24-04-2019',	null, 16, 5),20q
('24-04-2019',	null, 17, 5),21
('24-04-2019',	'27-04-2019', 18, 9),22
('27-04-2019',	null, 18, 6),23q
('27-04-2019',	null, 19, 4);24
*/

/*
('KA-6',	'Single', 	'Cruise', 	'Kiel', 		3, 3, 1),1
('KA-5',	'Single', 	'Cruise', 	'Kiel', 		3, 2, 1),2
('KA-4',	'Single', 	'Cruise', 	'Kiel', 		3, 1, 1),3
('S-2',		'Twain', 	'Sport', 	'Sport', 		2, 1, 2),4
('S-1',		'Single', 	'Sport', 	'Sport', 		2, 2, 0),5
('M-5',		'Single', 	'Cruise', 	'Schwertbott', 	2, 5, 0),6
('M-4',		'Single', 	'Cruise', 	'Schwertbott', 	3, 5, 0)7
*/

/*
('Alpha', 'Online', 1, 2), 1
('Storm', 'Canceled', 1, 1), 2
('Adelaida', 'Online',4, 1), 3 
('Latnyk', 'Online', 5, 3),4
('Storm', 'Online', 7, 4),5
('Infernal Rage', 'Online', 4, 1),6
('Hello Kitty', 'Online', 5, 4),7
('Beda', 'Online', 4, 1),8
('Moby Dick', 'Online', 1, 3)9
*/

insert into Yacht_Crew(yachtid, crewid, startdate, enddate)
values
(1, 2, '07-01-2019', null),
(2, 6, '13-01-2019', '17-01-2019'),
(2, 7, '15-01-2019', '17-01-2019'),
(3, 6, '17-01-2019', '12-02-2019'),
(3, 7, '17-01-2019', '12-02-2019'),
(3, 9, '12-02-2019', null),
(3, 10, '15-02-2019', null),
(4, 11, '13-02-2019', null),
(4, 15, '20-03-2019', null),
(5, 13, '15-02-2019', null),
(5, 16, '23-03-2019', null),
(5, 14, '23-03-2019', null),
(6, 20, '24-04-2019', null),
(6, 23, '27-04-2019', null),
(7, 18, '24-03-2019', null);
				");

				//Тесты работоспособности яхт
                dbContext.Database.ExecuteSqlRaw($@"
/*
('Alpha', 'Online', 1, 2), 1
('Storm', 'Canceled', 1, 1), 2
('Adelaida', 'Online',4, 1), 3 
('Latnyk', 'Online', 5, 3),4
('Storm', 'Online', 7, 4),5
('Infernal Rage', 'Online', 4, 1),6
('Hello Kitty', 'Online', 5, 4),7
('Beda', 'Online', 4, 1),8
('Moby Dick', 'Online', 1, 3)9
*/

/*
('07-01-2019',	null, 1, 1),1
('07-01-2019',	null, 2, 3),2
('12-01-2019',	null, 3, 2),3
('13-01-2019',	null, 4, 4),4
('15-01-2019',	null, 5, 5),5
('15-01-2019',	'23-01-2019', 6, 6),6
('23-01-2019',	'15-02-2019', 6, 10),7
('12-02-2019',	null, 7, 5),8
('15-02-2019',	null, 8, 6),9
('13-02-2019',	null, 9, 5),10
('15-02-2019',	'13-03-2019', 10, 6),11
('15-02-2019',	null, 6, 5),12
('13-03-2019',	null, 10, 8),13
('20-03-2019',	null, 11, 6),14
('23-03-2019',	null, 12, 7),15
('23-03-2019',	null, 13, 4);16
*/

insert into YachtTest(date, results, yachtid, staffid)
values
('17-01-2019', 'Massive problems with maneuvers, engine, sails, etc', 2, 5),
('20-06-2019', 'Abnormalities in movement', 3, 5),
('20-06-2019', 'Abnormalities in movement', 3, 5),
('20-06-2019', 'Abnormalities in movement', 3, 17),
('20-06-2019', 'No problems', 4, 5),
('20-06-2019', 'No problems', 4, 5),
('21-06-2019', 'Minor problem with ships frame', 5, 5),
('23-07-2019', 'No problems', 5, 17),
('23-07-2019', 'No problems', 5, 17),
('24-07-2019', 'No problems', 3, 17),
('24-07-2019', 'No problems', 3, 17),
('11-10-2021', 'Massive problems with engine', 3, 5)
;
				");
				
				//Ремонты
                dbContext.Database.ExecuteSqlRaw($@"
insert into Repair(startdate, enddate, duration, status, personnel, yachtid)
values
('17-01-2019', '19-04-2019',	'19-04-2019',	'Canceled',	3, 2),
('19-04-2019', '25-04-2019',	'25-04-2019',	'Canceled',	3, 2),
('21-06-2019', '24-07-2019',	'24-07-2019',	'Done',	3, 3),
('21-06-2019', '23-07-2019',	'23-07-2019',	'Done',	3, 5),
('12-10-2021', null,	'14-01-2022',	'Waits',	3,	3),
('10-10-2021', null,	'14-01-2022',	'Waits',	3,	6)
;
				");
				
				//Запросы на выдачу материалов
                dbContext.Database.ExecuteSqlRaw($@"
/*
('Diesel Fuel 1', 				(select id from materialtype where name = 'Fuel')),1
('Diesel Fuel 2', 				(select id from materialtype where name = 'Fuel')),2
('Carbon fiber for patching', 	(select id from materialtype where name = 'Parts')),3
('Simple fiber for patching', 	(select id from materialtype where name = 'Parts')),4
('Small sails', 				(select id from materialtype where name = 'Sails')),5
('Medium sails', 				(select id from materialtype where name = 'Sails')),6
('Large sails', 				(select id from materialtype where name = 'Sails')),7
('E-9565-IC', 					(select id from materialtype where name = 'Engine')),8
('E-9555-IC', 					(select id from materialtype where name = 'Engine')),9
('EE-10005-IC', 				(select id from materialtype where name = 'Engine')),10
('Screwdriver',					(select id from materialtype where name = 'Instruments')),11
('Cutter',						(select id from materialtype where name = 'Instruments')),12
('Crowbar',						(select id from materialtype where name = 'Instruments'))13
*/

/*
(30.0, 		100, 	3000.0, '07-01-2019', 1, 1),1
(40.0, 		100, 	4000.0, '07-01-2019', 2, 1),2
(100.0, 	10, 	1000.0, '07-01-2019', 4, 1),3
(1000.0, 	10, 	10000.0, '07-01-2019', 6, 1),4
(350.0, 	10, 	3500.0, '07-01-2019', 11, 1),5
(150.0, 	10, 	1500.0, '07-01-2019', 12, 1),6
(3500.0, 	1, 		3500.0, '11-10-2021', 10, 3)7
*/

insert into ExtradationRequest(startdate, enddate, duration, status, count, material, staffid, repairid)
values
('17-01-2019', '19-04-2019', '17-02-2019',	'Canceled', 5, 1, 4, 1),
('17-01-2019', '19-04-2019', '17-02-2019',	'Canceled', 15, 3, 4, 1),
('17-01-2019', '19-04-2019', '17-02-2019',	'Canceled', 1, 8, 4, 1),
('17-01-2019', '19-04-2019', '17-02-2019',	'Canceled', 1, 6, 4, 1),
('19-01-2019', '25-04-2019', '25-02-2019',	'Canceled', 5, 1, 4, 2),
('19-01-2019', '25-04-2019', '25-02-2019',	'Canceled', 15, 3, 4, 2),
('19-01-2019', '25-04-2019', '25-02-2019',	'Canceled', 1, 8, 4, 2),
('19-01-2019', '25-04-2019', '25-02-2019',	'Canceled', 1, 6, 4, 2),
('01-07-2019', '20-07-2019', '02-07-2019',  'Waits', 2, 3, 16, 3),
('24-06-2019', '10-07-2019', '24-07-2019',  'Done', 10, 4, 4, 4),
('12-10-2021',		   null, '12-12-2021',  'Waits', 1, 10, 16, 5)
;
				");
				
				//Победитель
                dbContext.Database.ExecuteSqlRaw($@"
/*
('First YachtClub Event', 			'01-02-2019', 	'03-02-2019'),	1
('SpringRace #1', 					'10-05-2019', 	'10-05-2019'),	2
('International Regatta #456', 		'07-09-2019', 	'04-10-2019'),	3
('1st Anniversary Event',			'07-01-2020',	'10-01-2020'),	4
('SpringRace #2', 					'10-05-2020', 	'10-05-2020'),	5
('International Regatta	#457', 		'07-09-2019', 	'04-10-2019'),	6
('Anniversary Event',				'07-01-2021', 	'10-01-2021')	7
*/

insert into Winner(eventid, yachtid, place)
values
(1, 2, null),
(1, 3, null),
(1, 4, null),
(2, 3, 10),
(2, 1, 1),
(2, 4, 15),
(3, 3, 1),
(3, 4, 120),
(3, 5, 51),
(4, 3, null),
(4, 4, null),
(5, 3, null),
(5, 4, null),
(5, 5, null),
(6, 3, 21),
(6, 1, 1),
(6, 4, 56),
(6, 5, 8),
(7, 3, null),
(7, 5, null),
(7, 4, null);
				");
				
				//Договор на аренду яхты
                dbContext.Database.ExecuteSqlRaw($@"
/*Insert INTO YachtLeaseType(name,price)
values
('Standart'				, 0.0),1
('Premium'				, 500.0),2
('VIP'					, 9999.9),3
('StaffOnly'			, 0.0)4
;*/

insert into YachtLease(startdate, enddate, duration, overallprice, yachtid, yachtleasetypeid)
values
('07-01-2019', '25-04-2019',	'12-12-2122', 0, 1, 4),
('07-01-2019', null,			'12-12-2022', 9000, 2, 2),
('08-01-2019', '07-06-2019',	'12-12-2022', 7500, 3, 2 ),
('25-02-2019', '24-07-2019',	'12-12-2022', 10500, 4, 2),
('07-04-2019', null,			'12-12-2022', 12000, 6, 2),
('07-06-2019', '30-12-2020',	'31-03-2021', 25785, 3, 3),
('24-07-2019', '31-03-2021',	'31-03-2021', 37020, 4, 3),
('31-03-2021', null,			'12-12-2022', 30000, 4, 3),
('01-04-2021', null,			'12-12-2022', 40000, 5, 3)
;
				");

                #endregion

                #region Третье поколение
                //Ремонтники
                dbContext.Database.ExecuteSqlRaw($@"
insert into Repair_Men(RepairID, StaffID)
values
(1, 5),
(2, 5),
(2, 17),
(3, 17),
(4, 5),
(4, 17),
(5, 17);
				");

				//Контракты
                dbContext.Database.ExecuteSqlRaw($@"
/*
('Alexei', 		'Britov', 		'Male', 	'02-03-1947', 	'a_brit@gmail.com'),1
('Melnik', 		'Baranov', 		'Male', 	'05-04-1967', 	'mebar@mail.ru'),2
('Dmitriy', 	'Bideshev', 	'Male', 	'15-01-1989', 	'biDeshev777@gmail.com'),3
('Gomel', 		'Bogdanov', 	'Male', 	'16-10-1963', 	'kosolov@gmail.com'),4
('Jamala', 		'Nebinarna', 	'Female', 	'23-09-1954', 	'j_4354@gmail.com');5
*/

/*
('Standart'			, 400.0),1
('Premium'			, 750.0),2
('VIP'				, 1999.9),3
('Party'			, 1500.0),4
('Training Trip'	, 750.0),5
('Romantic Holidays', 850.0),6
('Family Trip'		, 1000.0);7

*/

/*
('07-01-2019', '25-04-2019', 0, 0, 0, 1),
('07-01-2019', null, null, 0, null, 2),
('08-01-2019', '07-06-2019', 150, 50, 7500, 3 ),
('25-02-2019', '24-07-2019', 150, 70, 10500, 4 ),
('07-06-2019', '30-12-2020', 573, 45, 25785, 3),
('24-07-2019', '31-03-2021', 617, 60, 37020, 4),
('31-03-2021', null, null, 60, null, 4)
*/

/*
('07-01-2019',	null, 1, 1),1
('07-01-2019',	null, 2, 3),2
('12-01-2019',	null, 3, 2),3
('13-01-2019',	null, 4, 4),4
('15-01-2019',	null, 5, 5),5
('15-01-2019',	'23-01-2019', 6, 6),6
('23-01-2019',	'15-02-2019', 6, 10),7
('12-02-2019',	null, 7, 5),8
('15-02-2019',	null, 8, 6),9
('13-02-2019',	null, 9, 5),10
('15-02-2019',	'13-03-2019', 10, 6),11
('15-02-2019',	null, 6, 5),12
('13-03-2019',	null, 10, 8),13
('20-03-2019',	null, 11, 6),14
('23-03-2019',	null, 12, 7),15
('23-03-2019',	null, 13, 4);16
*/
/*
('Alpha', 'Online', 1, 2), 1
('Storm', 'Canceled', 1, 1), 2
('Adelaida', 'Online',4, 1), 3 
('Latnyk', 'Online', 5, 3),4
('Storm', 'Online', 7, 4),5
('Infernal Rage', 'Online', 4, 1),6
('Hello Kitty', 'Online', 5, 4),7
('Beda', 'Online', 4, 1),8
('Moby Dick', 'Online', 1, 3)9
*/

insert into Contract(Startdate, enddate, duration, status, specials, averallprice, ClientID, ContractTypeID, YachtWithCrewID)
values 
('10-02-2019','11-02-2019', '11-02-2019', 'Done', 'No specials', 1500.0, 20, 2, 3),
('17-02-2019','17-02-2019', '17-02-2019', 'Done','No specials', 400.0, 21, 1, 4),
('18-02-2019','18-03-2019', '18-03-2019', 'Done','Long journey', 120000.0, 22, 3, 5),
('13-02-2019', '17-02-2019', '17-02-2019', 'Done', 'No specials', 8000.0, 23, 4, 4 ),
('25-04-2019', '21-06-2019', '21-06-2019', 'Done', 'Long journey', 400000.0, 22, 6, 5),
('07-01-2020', '21-02-2020', '21-02-2020', 'Done', 'Long journey', 350000.0, 22, 6, 5),
('01-07-2020', '05-08-2020', '05-08-2020', 'Done', 'Long journey', 200000.0, 22, 5, 5),
('09-05-2021', '09-05-2021', '09-05-2021', 'Done', 'No specials', 1000.0, 1, 4, 5),
('09-10-2021', null, '10-10-2022', 'Done', 'Long journey', 5000, 22, 4, 3),
('25-12-2021', null, '10-10-2022', 'Done', 'No specials', 2000, 23, 7, 4),
('27-12-2021', null, '10-10-2022', 'Done', 'No specials', 5000, 24, 3, 5)
;
				");

				#endregion

				#region Четвёртое поколение

				//Отзывы
				dbContext.Database.ExecuteSqlRaw($@"
insert into Review(ClientId,  date, text, rate, userrate)
values
( 4, '25-02-2019', 'Некогда больше не поеду на этой яхте', 1 , 0) ,
( 3, '25-02-2019', 'Превосходный капитан', 5 , 0) ,
( 4, '19-05-2021', 'Как же хорошо', 5 , 0) 
;
				");

				#endregion
				#endregion

			}
        }

		public static void SeedWithProcedure(DataContext dbContext )
        {
			#region Функции и процедурки
			//Доступные материалы
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function MaterialMetric(
	material_id bigint,
	count decimal
)
	returns varchar
as $$
declare 
	rec RECORD;
begin 
	select count, m.metric m1, t.metric m2 into rec
	from material as m join materialtype as t on m.typeid = t.id
	where m.id = material_id and m.id = material_id;
	
	if(rec.m1 ~ '^[\t\n\r\f\v]*$' or rec.m1 is null) then
		if(rec.m2 ~ '^[\t\n\r\f\v]*$' or rec.m2 is null) then
			return rec.count || '';
		else
			return rec.count || ' ' || rec.m2;
		end if;
	else
		return rec.count || ' ' || rec.m1;
	end if;
end;
$$ language plpgsql;	
				");
			//Проверка статусов заявок ремонта
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function CHECK_ER_FOR_REPS(
closed boolean,
repair_id int
)
returns boolean
as $$
begin 
	if(closed) then
		return not ( select count(er.id) from extradationrequest as er where er.repairid = repair_id) = 
		( select count(id) from extradationrequest as er where er.repairid = repair_id and (er.status in ('Canceled', 'Done')));
	else 
		return exists ( select * from extradationrequest as er where er.repairid = repair_id and er.status not in ('Canceled', 'Done'));
	end if;
end;
$$ language plpgsql;
				");
			//Действующие сотрудники
			dbContext.Database.ExecuteSqlRaw(@"
CREATE or replace Function StaffPositionListByPositionList (pos varchar[])
	Returns Table (staffid int)
as $$
Begin
	Return query 
		Select sf.id staffid from staff_position sf join position p on p.id = sf.positionid
		where p.name in (select unnest(pos)) and sf.enddate is null;
	
END;
$$ language plpgsql;

CREATE or replace Function StaffPositionListByPosition (pos varchar)
	Returns Table (staffid int)
as $$
Begin
	Return query select StaffPositionListByPositionList(Array[pos]);
END;
$$ language plpgsql;
				");
			//Пересечение дат
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function IsInTerm(
	_s1 timestamp,
	_s2 timestamp,
	_e2 timestamp,
	_e1 timestamp
) returns boolean
as $$
begin 	
	return _s1 <= _e2 and _s2 <= _e1;
end;
$$ language plpgsql;
				");
			//Активные контракты яхты
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function ActiveContractByYachtID (
	yid int
)
	returns table(cid int, startdate timestamp)
as $$
begin 	
	return query select c.id cid, c.startdate startdate
		   	from contract c join yacht_crew ywc on c.yachtwithcrewid = ywc.id 
			where ywc.yachtid = yid and c.enddate is null
		   ;
end; 
$$ language plpgsql;
				");	
			//Активные ремонты яхты
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function ActiveRepairByYachtID (
	yid int
)
	returns table(rid int, startdate timestamp)
as $$
begin 	
	return query select r.id cid, r.startdate
		   	from repair r 
			where r.yachtid = yid and r.enddate is null
		   ;
end; 
$$ language plpgsql;
				");
			//Current_Timestamp::timestamp
			dbContext.Database.ExecuteSqlRaw(@"
			create or replace function CurStmp() returns timestamp 
			as $$
			begin return current_timestamp::timestamp(2); end;
			$$ language plpgsql;
				");
			

			#endregion

			#region Views
			//Доступные материалы
			dbContext.Database.ExecuteSqlRaw(@"
create or replace view AvailableResources as (
with mlcount as(
select  ml.material, sum(ml.count) count from 
materiallease as ml
where ml.deliverydate is not null
group by ml.material order by ml.material
),
ercount as (
select  er.material, sum(er.count) count from 
extradationrequest as er
where er.enddate is not null and Status = 'Done'
group by er.material order by er.material
),
counter as
(
select m.material, coalesce(m.count, 0) - coalesce(e.count, 0) count from mlcount as m left join ercount as e on m.material = e.material
union
select e.material, coalesce(m.count, 0) - coalesce(e.count, 0) count from mlcount as m right join ercount as e on m.material = e.material
)
select distinct m.id material, coalesce(ar.count, 0) count, materialmetric(m.id, coalesce(ar.count, 0)) format  from 
	material as m left join counter as ar on m.id = ar.material
	order by m.id
);
				");

			//Товарищи ремонтники
			dbContext.Database.ExecuteSqlRaw(@"
Create or replace view Repair_Staff as (
select sp.id, sp.staffid, sp.positionid, sp.startdate, sp.enddate, sp.description from
staff_position as sp join position as p on sp.positionid = p.id 
where sp.id in (select StaffPositionListByPosition('Repairman'))
	);
");

			//Персонал
			dbContext.Database.ExecuteSqlRaw(@"
Create or replace View Staff as (
		select * from person where id in (
		select distinct sp.staffid from
		staff_position as sp
		)
);

");

			#endregion

			#region Triggers

			#region ExtradationRequest

			//Триггер проверки завершённых заявок
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function ER1()
	returns trigger
as $$
declare
	rec RECORD;
begin 	
		IF (TG_OP = 'UPDATE') THEN 
			if(old.status in ('Canceled','Done')) then 
				raise exception 'Попытка изменения закрытой заявки';			
			end if;
			rec := new;
			rec.duration := old.duration;
			rec.enddate := old.enddate;
			rec.personnel := old.personnel;
			rec.description := old.description;
			rec.status := old.status;
			if(rec <> old) then raise exception 'Изменены запрещенные поля';
			end if;
			if(new.personnel < old.personnel) then raise exception 'Количество персонала занимающегося ремонтом может только увеличиваться';
			end if;
			return new;
        ELSIF (TG_OP = 'INSERT') then
			New.startdate = current_timestamp;
			if( new.duration < current_timestamp or new.duration is null) then new.duration = current_timestamp; end if;
			New.enddate = null;
			New.Status = 'Created';
            RETURN NEW;
		ELSIF (TG_OP = 'DELETE') THEN
			if(old.status in ('Done')) then 
				raise exception 'Попытка удаления закрытой заявки';			
			end if;
			return old;
        END IF;
		return new;
end;
$$ language plpgsql;	

create trigger ReadonlyConstraint
Before insert or update or delete on ExtradationRequest 
for each row execute function ER1();
				");
			//Закрывать заявку, в случае если статус в Отменённых или Выполненных
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function ER2()
	returns trigger
as $$
begin 
	if (new.Status = 'Created' ) then 
		new.Status = 'Waits';
	end if;
	if (not exists (select * from repair_men rm where new.Staffid = rm.staffid and rm.repairid = new.repairid) ) then
		raise exception 'От имени данного сотрудника нельзя добавить или изменить заявку';
	end if;

	if(new.status in ('Canceled','Done') and new.enddate is null) then
		new.enddate = current_timestamp;			
	elsif(new.status not in ('Canceled','Done') and new.enddate is not null) then
		new.enddate = null;
	end if;
	return new;
end;
$$ language plpgsql;	

create trigger Closer
Before insert or update on ExtradationRequest 
for each row execute function ER2();
				");
			//Триггер на выдачу материалов при нехватке
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function ER3()
	returns trigger
as $$
begin 
	if ( exists 
				 (
					 select * from availableresources as ar
					 where New.material = ar.material and ar.count - New.count < 0 and New.Status = 'Done'
				 )
				) 
	then raise exception 'Не хватает материалов при попытке выдачи #%', New.ID;
	end if;
	return new;
end;
$$ language plpgsql;	

create trigger ExtradationAvailableMaterials
Before insert or update on ExtradationRequest 
for each row execute function ER3();
				");	
			//Триггер на добавление 
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function ER4()
	returns trigger
as $$
begin 
	if( exists (select * from repair r where r.id = new.repairid and r.status in ('Created','In Progress'))) then
	update Repair set Status='Waits' where id = new.repairid;
	end if;
	return new;
end;
$$ language plpgsql;	

create trigger UpdateRepairAfterInsert
After insert on ExtradationRequest 
for each row execute function ER4();
				");

			#endregion

			#region Repair

			//Триггер проверки завершённых заявок
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function REP1()
	returns trigger
as $$
declare
	rec RECORD;
begin 	
		IF (TG_OP = 'UPDATE') THEN 
			if(old.status in ('Canceled','Done')) then 
				raise exception 'Попытка изменения закрытого ремонта';			
			end if;
			rec := new;
			if(old.startdate > current_timestamp and new.startdate > current_timestamp) then
				rec.startdate := old.startdate;	
			end if;
			rec.duration := old.duration;
			rec.enddate := old.enddate;
			rec.personnel := old.personnel;
			rec.description := old.description;
			rec.status := old.status;
			if(rec <> old) then raise exception 'Изменены запрещенные поля';
			end if;
			return new;
        ELSIF (TG_OP = 'INSERT') then
			if( new.startdate < current_timestamp or new.startdate is null) then new.startdate = current_timestamp; end if;
			if( new.duration < current_timestamp or new.duration is null) then new.duration = current_timestamp; end if;
			New.enddate = null;
			New.Status = 'Created';
            RETURN NEW;
		ELSIF (TG_OP = 'DELETE') THEN
			if(old.status in ('Done')) then 
				raise exception 'Попытка удаления закрытой заявки';			
			end if;
			return old;
        END IF;
		return new;
end;
$$ language plpgsql;	

create trigger ReadonlyConstraint
Before insert or update or delete on Repair
for each row execute function REP1();
				");
			//Изменение статуса
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function REP2()
	returns trigger
as $$
begin 
	/*Создан -> (ОЖИДАЕТ МАТЕРИАЛОВ ЛИБО ОТМЕНЁН)*/
	if (old.status = 'Created' and new.Status not in ( 'Waits' , 'Canceled', 'Created') ) then 
		raise exception 'Переход из статуса создан, может быть только в статус Ожидает или Отменён';
	elsif(old.status = 'Created' and new.Status = 'Waits' and 
		 not CHECK_ER_FOR_RER(false, new.id) ) then
		raise exception 'Переход в статус Ожидает невозможен, ведь отсутствуют активные заявки';
	/*ОЖИДАЕТ МАТЕРИАЛОВ -> (В ПРОЦЕССЕ ЛИБО ОТМЕНЁН)*/
	elsif (old.status = 'Waits' and new.Status not in ('In Progress' , 'Canceled', 'Waits') ) then
		raise exception 'Переход из статуса Ожидает, может быть только в статус В Процессе или Отменён';
	elsif(old.status = 'Waits' and new.Status = 'In Progress' and 
		 not CHECK_ER_FOR_RER(true, new.id) ) then
		raise exception 'Переход в статус В Процессе невозможен, ведь отсутствуют закрытые заявки';
	/*В ПРОЦЕССЕ -> (ОЖИДАЕТ МАТЕРИАЛОВ ЛИБО ГОТОВ ЛИБО ОТМЕНЁН)*/
	elsif (old.status = 'In Progress' and new.Status not in ('Waits', 'Done', 'Canceled', 'In Progress') ) then
		raise exception 'Переход из статуса В Процессе, может быть только в статус Ожидает или Готов или Отменён';	
	elsif(old.status = 'In Progress' and new.Status = 'Waits' and 
		 not CHECK_ER_FOR_RER(false, new.id) ) then
		raise exception 'Переход в статус Ожидает невозможен, ведь отсутствуют активные заявки';
	end if;
	
	if(new.status in ('Canceled','Done') and new.enddate is null) then
			update ExtradationRequest set status = 'Canceled' 
			where repairid = new.id and status not in ('Canceled','Done');
		new.enddate = current_timestamp;			
	elsif(new.status not in ('Canceled','Done') and new.enddate is not null) then
		new.enddate = null;
	end if;
	return new;
end;
$$ language plpgsql;	


create trigger Closer
Before update on Repair
for each row execute function REP2();
				");

			#endregion

			#region Person

			//Триггер начальной проверки персон
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function P1()
	returns trigger
as $$
begin 	
		IF (TG_OP = 'UPDATE') THEN 
			if(old.registrydate <> new.registrydate) then 
				raise exception 'Изменены запрещённые поля';
			end if;
			return new;
        ELSIF (TG_OP = 'INSERT') then
			new.registrydate = current_timestamp;
            RETURN NEW;
        END IF;
		return new;
end;
$$ language plpgsql;	

create trigger ReadonlyConstraint
Before insert or update or delete on Person
for each row execute function P1();
				");

			#endregion

			#region Staff_Position

			//Триггер начальной проверки персон
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function SP1()
	returns trigger
as $$
declare 
rec RECORD;
begin 	
		IF (TG_OP = 'UPDATE') THEN 
			if(old.enddate is not null) then 
				raise exception 'Попытка изменения закрытой должности';
			end if;
			rec := new;
			rec.enddate = old.enddate;
			rec.description = old.description;
			rec.salary = old.salary;
			if(rec <> old) then 
				raise exception 'Изменены запрещённые поля';
			end if;
			
			if(new.enddate is not null) then 
				new.enddate = current_timestamp;
			end if;
			
			return new;
        ELSIF (TG_OP = 'INSERT') then
			--Выставление стандартной даты
			new.startdate = current_timestamp; 
			--Стандартная запрлата
			if(new.salary is null) then
				new.salary = (select p.salary from position p where p.id = new.positionid);
			end if;
			--Проверка открытых записей на должность
			if( (select count(sp.id) from staff_position sp 
				 where sp.staffid = new.staffid and sp.positionid = new.positionid and sp.enddate is null) >= 1) then
				 raise exception 'Уже присутствует открытая запись человека на данной должности, закройте предыдущую и повторите попытку';
			end if;
            RETURN NEW;
        END IF;
		return new;
end;
$$ language plpgsql;	

create trigger ReadonlyConstraint
Before insert or update or delete on Staff_Position
for each row execute function SP1();
				");

			#endregion

			#region Repair_Men

			//Триггер начальной проверки ремонтников на ремонте
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function RM1()
	returns trigger
as $$
declare 
rec RECORD;
pers int;
begin 	
		IF (TG_OP = 'UPDATE') THEN 
			raise exception 'Запрещено изменение данной сущности';
        ELSIF (TG_OP = 'INSERT') then
			pers = (select r.personnel from repair r where r.id = new.repairid) ;
			if ( (select r.status from repair r where r.id = new.repairid) in ('Canceled','Done') ) then 
				raise exception 'Отсутствует возможность добавить персонал на закрытый ремонт';
			elsif( (select count(*) from repair_men rm where rm.repairid = new.repairid ) >
			   pers ) then
			   raise exception 'Количество персонала на данном ремонте не должно превышать %', pers;
			end if;
			--Добавление залогиненого сотрудника
            RETURN new;
		ELSIF (TG_OP = 'DELETE') then 
			if ( ( select r.status from repair r where r.id = old.repairid) in ('Canceled','Done')  ) then 
				raise exception 'Отсутствует возможность убрать персонал с закрытого ремонта';
			end if;
			return old;
        END IF;
end;
$$ language plpgsql;	

create trigger ReadonlyConstraint
Before insert or update or delete on Repair_Men
for each row execute function RM1();
				");

			#endregion

			#region MaterialLease

			//Триггер начальной проверки контракта на выдачу материалов
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function ML1 ()
	returns trigger
as $$
declare 
rec RECORD;
pers int;
begin 	
		IF (TG_OP = 'UPDATE') THEN 
			if(old.deliverydate is not null) then 
				raise exception 'Попытка изменения закрытого контракта на материалы';			
			end if;
			rec := new;
			rec.deliverydate := old.deliverydate;
			rec.overallprice := old.overallprice;
			rec.count := old.count;
			rec.priceperunit := old.priceperunit;
			if(rec <> old) 
				then raise exception 'Изменены запрещенные поля';
			end if;	
        ELSIF (TG_OP = 'INSERT') then
			new.startdate = current_timestamp;
		ELSIF (TG_OP = 'DELETE') then 
			if(old.deliverydate is not null) then 
				raise exception 'Попытка удаления закрытого контракта на материалы';			
			end if;
			return old;
        END IF;
		new.overallprice = new.count * new.priceperunit;
		RETURN new;
end;
$$ language plpgsql;	

create trigger ReadonlyConstraint
Before insert or update or delete on MaterialLease
for each row execute function ML1 ();
				");

			#endregion

			#region Yacht_Crew

			//Триггер начальной проверки экипажа
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function YC1 ()
	returns trigger
as $$
declare 
rec RECORD;
pers int;
begin 	
		IF (TG_OP = 'UPDATE') THEN 
			if(old.enddate is not null) then 
				raise exception 'Попытка изменения уволеного члена экипажа';			
			end if;
			rec := new;
			rec.enddate := old.enddate;
			rec.description := old.description;
			if(rec <> old) 
				then raise exception 'Изменены запрещенные поля';
			end if;	
        ELSIF (TG_OP = 'INSERT') then
			new.startdate = current_timestamp;
		ELSIF (TG_OP = 'DELETE') then 
			if(old.deliverydate is not null) then 
				raise exception 'Попытка удаления закрытого контракта на материалы';			
			end if;
			return old;
        END IF;
		RETURN new;
end;
$$ language plpgsql;	

create trigger ReadonlyConstraint
Before insert or update or delete on Yacht_Crew
for each row execute function YC1 ();
				");

			#endregion
			
			#region Event

			//Триггер начальной проверки событий
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function E1 ()
	returns trigger
as $$
declare 
rec RECORD;
pers int;
begin 	
		IF (TG_OP = 'UPDATE') THEN 
			if(old.enddate is not null) then 
				raise exception 'Попытка изменения закрытого события';			
			end if;
			rec := new;
			if(new.startdate >= current_timestamp and old.startdate >= current_timestamp) then
				rec.startdate := old.startdate;
			end if;
			rec.enddate := old.enddate;
			rec.duration := old.duration;
			rec.description := old.description;
			if(rec <> old) 
				then raise exception 'Изменены запрещенные поля';
			end if;
		ELSIF (TG_OP = 'INSERT') then
			if(new.startdate <= current_timestamp or new.startdate is null) then
			new.startdate = current_timestamp;
			end if;
		ELSIF (TG_OP = 'DELETE') then 
			if(old.enddate is not null) then 
				raise exception 'Попытка удаления закрытого события';			
			end if;
			return old;
        END IF;
		RETURN new;
end;
$$ language plpgsql;	

create trigger ReadonlyConstraint
Before insert or update or delete on Event
for each row execute function E1 ();
				");

			#endregion			

			#region Winner

			//Триггер начальной проверки потенциальных победителей
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function W1 ()
	returns trigger
as $$
declare 
rec RECORD;
begin 	
		IF (TG_OP = 'UPDATE') THEN 
			if( not exists ( select * from event e where e.id = old.eventid and e.enddate is null) )then 
				raise exception 'Попытка изменения закрытого события';			
			end if;
			rec := new;
			rec.place := old.place;
			if(rec <> old) 
				then raise exception 'Изменены запрещенные поля';
			end if;	
			
			if( not (select canhavewinners from winner) ) then 
				new.place := null;
			end if;
		ELSIF (TG_OP = 'INSERT') then
				select * into rec from event e where new.eventid = e.id ; 
				if(rec.enddate is not null) then raise exception 'Попытка участия в закрытом событии'; end if;
				if( exists (
					select cid from ActiveContractByYachtID(new.yachtid) where IsInTerm(  rec.startdate, startdate, 
									  current_timestamp::timestamp, rec.duration  )
				) or exists
				   (
				   select rid from ActiveRepairByYachtID(new.yachtid) where IsInTerm(  rec.startdate, startdate, 
									  current_timestamp::timestamp,rec.duration )
				   ) 
				  ) then
				  raise exception 'Попытка участия в событии при ремонте/контракте';
				end if;
				if(not rec.canhavewinners) then 
					new.place := null;
				end if;
		ELSIF (TG_OP = 'DELETE') then 
			if( not exists ( select * from event e where e.id = old.eventid and e.enddate is null)) then 
				raise exception 'Попытка удаления закрытого события';			
			end if;
			return old;
        END IF;
		RETURN new;
end;
$$ language plpgsql;

create trigger ReadonlyConstraint
Before insert or update or delete on Winner
for each row execute function W1 ();
				");

            #endregion

            #region YachtLease
			//Триггер начальной проверки договоров на яхты
            dbContext.Database.ExecuteSqlRaw(@"
			create or replace function YL1 ()
				returns trigger
			as $$
			declare 
			rec RECORD;
			begin 	
					IF (TG_OP = 'UPDATE') THEN 
						if( old.enddate is not null )then 
							raise exception 'Попытка изменения закрытого договора на яхту';			
						end if;
						rec := new;
						if(not old.paid) then
							rec.paid = old.paid;
						end if;
						rec.enddate := old.enddate;
						rec.overallprice := old.overallprice;
						rec.specials := old.specials;
						if(rec <> old) 
							then raise exception 'Изменены запрещенные поля';
						end if;	
					ELSIF (TG_OP = 'INSERT') then
						if(exists (select * from yachtlease where yachtid = new.yachtid and enddate is null) ) then
							raise exception 'невозможно заключить новый контракт, если старый ещё не завершён';
						end if;
						if(new.startdate <= current_timestamp or new.startdate is null) then
							new.startdate = current_timestamp;
						end if;
						new.overallprice = (select ylt.price from yachtleasetype ylt where ylt.id = new.yachtleasetypeid) * 
							 abs( extract(day from new.startdate - new.duration ) );	 
					ELSIF (TG_OP = 'DELETE') then 
						if(old.enddate is not null) then 
							raise exception 'Попытка удаления закрытого контракта на пирс';			
						end if;
						return old;
					END IF;
					RETURN new;
			end;
			$$ language plpgsql;

			create trigger ReadonlyConstraint
			Before insert or update or delete on YachtLease
			for each row execute function YL1 ();
							");
			//Триггер начальной проверки изменение статусов яхт
            dbContext.Database.ExecuteSqlRaw(@"
			create or replace function YL2 ()
				returns trigger
			as $$
			declare 
			rec RECORD;
			begin 
				if(
					isinterm(new.startdate, CurStmp(), CurStmp(), coalesce(new.enddate, CurStmp())) and
					new.paid
				) then
					update Yacht set Status = 'Valid' where id = new.yachtid;
				end if;
				if(
					new.enddate is not null and new.enddate <= CurStmp()					
				) then
					update Yacht set Status = 'Invalid' where id = new.yachtid;
				end if;
				return new;
			end;
			$$ language plpgsql;

			create trigger StatusChecker
			After update or insert on YachtLease
			for each row execute function YL2 ();
							");
            #endregion

            #region Yacht
			//Триггер начальной проверки на яхты
            dbContext.Database.ExecuteSqlRaw(@"
			 create or replace function Y1 ()
				 returns trigger
			 as $$
			 declare 
			 rec RECORD;
			 begin 	
					 IF (TG_OP = 'UPDATE') THEN 
						 if(new.name <> old.name or
							new.registrydate <> old.registrydate or 
							new.typeid <> old.typeid
						   ) 
							 then raise exception 'Изменены запрещенные поля';
						 end if;	
					 ELSIF (TG_OP = 'INSERT') then
						 new.registrydate = current_timestamp;
						 new.status = 'Invalid';
					 END IF;
					 RETURN new;
			 end;
			 $$ language plpgsql;

			 create trigger ReadonlyConstraint
			 Before insert or update on Yacht
			 for each row execute function Y1 ();
							 "); 
			//Триггер проверки статуса яхт
			dbContext.Database.ExecuteSqlRaw(@"
			 create or replace function Y2 ()
				returns trigger
			as $$
			declare 
			rec RECORD;
			begin 	
				if (old.status = 'Invalid' and new.Status in ( 'Valid') 
					and not exists( 
						select * from yachtlease yl where yl.paid and new.id = yl.yachtid and
						isinterm(yl.startdate, CurStmp(), CurStmp(), coalesce(yl.enddate, CurStmp()))
					)
				   ) then 
				   raise exception 'Невозможно перейти из статуса Недействителен в статус Действителен, ведь
				   отсутствуют текущие оплаченные контракты';
				elsif(
					old.status = 'Valid' and new.Status in ( 'Invalid') and 
					not exists (
						select * from yachtlease yl where yl.paid and new.id = yl.yachtid and yl.enddate <= CurStmp()
					)
				) then 				
					raise exception 'Невозможно перейти из статуса Действителен в статус Недействителен, ведь
				  	присутствуют текущие оплаченные контракты';
				end if;
				return new;
			end;
			$$ language plpgsql;

			create trigger StatusChecker
			Before update on Yacht
			for each row execute function Y2 ();
							 ");
            #endregion

    //        #region Contract
    //        dbContext.Database.ExecuteSqlRaw(@"
			 //create or replace function  ()
				// returns trigger
			 //as $$
			 //declare 
			 //rec RECORD;
			 //begin 	
				//	 IF (TG_OP = 'UPDATE') THEN 

				//	 ELSIF (TG_OP = 'INSERT') then

				//	 ELSIF (TG_OP = 'DELETE') then 
				//		 if(old.enddate is not null) then 
				//			 raise exception 'Попытка удаления закрытого события';			
				//		 end if;
				//		 return old;
				//	 END IF;
				//	 RETURN new;
			 //end;
			 //$$ language plpgsql;

			 //create trigger ReadonlyConstraint
			 //Before insert or update or delete on Winner
			 //for each row execute function  ();
				//			 ");
    //        #endregion

            #endregion
        }
    }
}
