using Microsoft.EntityFrameworkCore;
using System.Linq;
using Project.Migrations;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using Project.Models;

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
	Winner, Staff_Position, Yacht_Crew, Repair_Men, Review_Contract, Review_Yacht, Review_Captain, Position_YachtType, Position_Equivalent, Account
Cascade;

				");

                //Удаление доменов
                dbContext.Database.ExecuteSqlRaw(@"
drop domain if exists Sex, My_Money, Mail, Phonenumber cascade
;
				");
                #endregion

                #region Создание доменов
                //Создание доменов
                dbContext.Database.ExecuteSqlRaw(@"
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
  ID    			serial    		Primary Key,
  Name   			varchar   		Not Null  	Unique,
  CrewPosition		boolean			Not Null	Default false,
  DeclineDelete		Boolean			Not Null	Default false,
  DeclineAccess		Boolean			Not Null	Default false,
  Salary        	My_Money    	Not Null
);
				");

                //Тип материала
                dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE MaterialType(
  ID		serial     Primary Key,
  Name		varchar    Not Null		Unique,
  Metric	varchar,
  Description	text	Default ' '
);
				");

                //Тип яхты
                dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE YachtType(
  	ID        			serial     	Primary Key,
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
  ID    		serial    	Primary Key,
  Name     	  	varchar   	Not Null	Unique,
  Price    	  	My_Money   	Not Null,	
  Description	text	Default ' '
);
				");

                //Тип аренды яхты
                dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE YachtLeaseType(
  ID    		serial    	Primary Key,
  Name     	  	varchar   	Not Null	Unique,
  Price    	  	My_Money   	Not Null,
  StaffOnly		bool		Not Null	Default FALSE,
  Description	text	Default ' '
);
				");

                //Тип продавца
                dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Seller (
	ID			serial		primary key,
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
  	ID    			serial    	Primary Key,
  	Name      		varchar    	Not Null,
  	Surname      	varchar    	Not Null,
  	BirthDate    	timestamp(2)   Not Null		check(extract(year from age(BirthDate)) >= 18),
	Sex				varchar		Not Null,	
  	Email      		Mail    	Not Null	unique,
	Phone			PhoneNumber	Not Null	unique,
    StaffOnly		Boolean		Not Null	Default false,
    StaffOrigin		Boolean		Not Null	Default false,
    DeclineDelete	Boolean		Not Null	Default false,
	RegistryDate	timestamp(2)	Not Null	check(BirthDate < RegistryDate)	Default current_timestamp
);
				");

                //Событие
                dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Event(
	ID			serial		Primary Key,
	Name		varchar		Not Null,
	StartDate	timestamp(2)		Not Null,
	EndDate		timestamp(2)		check(StartDate <= EndDate ),
	Duration	timestamp(2)		Not Null	check(StartDate <= Duration),
	Description		text		Default ' ',
	UserRate		int			Default 0,
	CanHaveWinners	boolean		not null	Default true
);
				");

                #endregion


                #region Блок зависимых таблиц

                #region Поколение 1
                //Человек на должности
                dbContext.Database.ExecuteSqlRaw(@"
Create TABLE Staff_Position(
	ID				serial		Primary Key,
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
	Salary        	My_Money			Default 0,
	Description		Text		Default ' '
);
				");

                //Материал
                dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Material(
	ID		serial		Primary Key,
	Name	varchar		Not Null	Unique,
	Metric	varchar     /*Default NULL*/,
	TypeID	int			Not Null
	References 	MaterialType(ID)	
	On Update Cascade	
	On Delete Cascade
);
				");

                //Яхта
                dbContext.Database.ExecuteSqlRaw(@"
Create TABLE Yacht(
	ID					Serial		Primary Key,
	Name				varchar		Not Null,
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
	ID				serial		Primary Key,
	Material		int			Not Null	
	References 	Material(ID)	
	On Update Cascade,
	
	Seller 			int			Not Null
	References Seller(ID)
	On Update Cascade,
	
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
	ID			serial		Primary Key,
	Date		timestamp(2)		Not Null,
	Results		text		Default ' ',
	YachtID		int			Not Null
	References 	Yacht(ID)	
	On Update Cascade,
	
	StaffID		int			Not Null
	References 	Staff_Position(ID)	
	On Update Cascade
);
				");

                //Ремонт
                dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Repair(
	ID			serial			Primary Key,
	StartDate	timestamp(2)		Not Null	Default current_timestamp,
	EndDate		timestamp(2)		check(StartDate <= EndDate )	Default null,
	Duration	timestamp(2)		Not Null	check(StartDate <= Duration)	Default current_timestamp,
	Status		varchar			Not Null	check(Status in ('Created','Waits', 'Canceled', 'In Progress','Done'))	Default 'Created',
	Personnel	int				Not Null	check(Personnel > 0)	Default 1,
	Description	text			Default ' ',
	YachtID		int				Not Null
	References 	Yacht(ID)	
	On Update Cascade
);
				");

                //Команда судна
                dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Yacht_Crew(
	ID			serial		Primary Key,
	YachtID		int			Not Null
	References 	Yacht(ID)	
	On Update Cascade,
	
	CrewID		int			Not Null
	References 	Staff_Position(ID)	
	On Update Cascade,
	
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
	On Update Cascade,
	Place		int		check(Place > 0),
	
	Primary Key (EventID, YachtID)
);
				");

                //Заявка на выдачу
                dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE ExtradationRequest(
	ID			serial		Primary Key,
	Count		int			Not Null	check(Count > 0),

	Material	int			Not Null	
	References 	Material(ID)	
	On Update Cascade,
	
	StaffID		int			Not Null
	References 	Staff_Position(ID)	
	On Update Cascade,
	
	RepairID	int			Not Null
	References 	Repair(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	StartDate	timestamp(2)		Not Null,
	EndDate		timestamp(2)		check(StartDate <= EndDate),
	Duration	timestamp(2)		Not Null	check(StartDate <= Duration),
	Description text			Not Null	Default ' ',
	Status		varchar			Not Null	check(Status in ('Created', 'Canceled', 'Waits', 'Done'))	 Default 'Created'
);
				");

                //Договор на аренду места
                dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE YachtLease(
	ID				serial			Primary Key,
	StartDate		timestamp(2)		Not Null,
	EndDate			timestamp(2)		check(StartDate <= EndDate ),
	Duration		timestamp(2)		Not Null	check(StartDate <= Duration),
	OverallPrice	My_Money		Not Null,
	Specials		text			Default ' ',
	Paid			bool			Not Null	Default False,
	YachtID			int				Not Null
	References 	Yacht(ID)	
	On Update Cascade	
	On Delete Cascade,

	YachtLeaseTypeID	int			Not Null
	References 	YachtLeaseType(ID)	
	On Update Cascade
);
				");

                #endregion

                #region Поколение 3

                //Контракт
                dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Contract(
	ID				serial		Primary Key,
	ClientID		int			Not Null
	References 	Person(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	ContractTypeID	int			Not Null
	References 	ContractType(ID)	
	On Update Cascade,
	
	CaptainInYachtID	int			Not Null
	References 	Yacht_Crew(ID)	
	On Update Cascade,

	StartDate		timestamp(2)		Not Null,
	EndDate			timestamp(2)		check(StartDate <= EndDate ),
	Duration		timestamp(2)		Not Null	check(StartDate <= Duration),
	Specials		text		Default ' ',
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
	Text			text		Default ' ',
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
	ID			serial		Primary Key,	

	RepairID	int			Not Null
	References 	Repair(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	StaffID		int			Not Null	
	References 	Staff_Position(ID)	
	On Update Cascade,

	Unique(RepairID, StaffID)
);
				");

                //Обзоры на яхту
                dbContext.Database.ExecuteSqlRaw(@"
/*CREATE TABLE Review_Yacht(
	ReviewID	int			Not Null
	References 	Review(ID)	
	On Update Cascade	
	On Delete Cascade,

	YachtID	int				Not Null
	References 	Yacht(ID)	
	On Update Cascade	
	On Delete Cascade,

	Primary Key ( ReviewID, YachtID )
);*/
				");

                //Обзоры на капитана
                dbContext.Database.ExecuteSqlRaw(@"
/*CREATE TABLE Review_Captain(
	ReviewID	int			Not Null
	References 	Review(ID)	
	On Update Cascade	
	On Delete Cascade,

	CaptainID	int			Not Null
	References 	Person(ID)	
	On Update Cascade	
	On Delete Cascade,

	Primary Key ( ReviewID, CaptainID )
);*/
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

/*				//Account
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Account(
	ID				serial		Not Null	Primary Key,
	Login			text		Not Null	Unique,
	Password		varchar		Not Null,
	UserID			int			Not Null
	References 	Person(ID)	
	On Update Cascade	
	On Delete Cascade
);
				");*/
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
                               ('Shipboy', 				    0.0),
                               ('DB Admin', 				4000.0),
                               ('Fired', 					0.0);

								update Position set CrewPosition = true where id in (5, 6, 7, 8);
								update Position set DeclineDelete = true where not id in (6, 7, 8, 9);
								update Position set DeclineAccess = true where id in (10);
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
update Person set staffonly = true where not id in (1);
update Person set stafforigin = true;

insert into Person (name, surname, sex, BirthDate, RegistryDate, email, phone)
values
('Yachtclub', 	'',				'Other', 	'07-01-2000',	'08-01-2019',	'yacht_club@gmail.com',  '+380983334590'	),
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
('None', 				100.0),9
('Fired', 				0.0);10
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
('23-01-2019',	'15-02-2019', 6, 10),
('12-02-2019',	null, 7, 5),
('15-02-2019',	null, 8, 6),
('13-02-2019',	null, 9, 5),
('15-02-2019',	'13-03-2019', 10, 7),
('15-02-2019',	null, 6, 5),
('13-03-2019',	null, 10, 8),
('20-03-2019',	null, 11, 6),
('23-03-2019',	null, 12, 7),
('23-03-2019',	null, 13, 4),
('24-03-2019',	null, 14, 5), 
('24-03-2019',	null, 15, 5), 
('24-04-2019',	null, 16, 5),
('24-04-2019',	null, 17, 5),
('24-04-2019',	'27-04-2019', 18, 5),
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

insert into Yacht(Name, TypeId, YachtOwnerID)
values
('Alpha',					1, 1),
('Storm',					1, 20),
('Adelaida',				4, 20),
('Latnyk',					5, 14),
('Storm',					7, 15),
('Infernal Rage',			4, 20),
('Hello Kitty',				5, 15),
('Beda',					4, 20),
('Moby Dick',				1, 14)
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
('01-07-2019', '20-07-2019', '02-07-2019',  'Canceled', 2, 3, 16, 3),
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

update YachtLease set paid = true;
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

insert into Contract(Startdate, enddate, duration, specials, averallprice, ClientID, ContractTypeID, CaptainInYachtID)
values 
('10-02-2019','11-02-2019', '11-02-2019', 'No specials', 1500.0, 20, 2, 3),
('17-02-2019','17-02-2019', '17-02-2019', 'No specials', 400.0, 21, 1, 4),
('18-02-2019','18-03-2019', '18-03-2019', 'Long journey', 120000.0, 22, 3, 5),
('13-02-2019', '17-02-2019', '17-02-2019', 'No specials', 8000.0, 23, 4, 4 ),
('25-04-2019', '21-06-2019', '21-06-2019',  'Long journey', 400000.0, 22, 6, 5),
('07-01-2020', '21-02-2020', '21-02-2020',  'Long journey', 350000.0, 22, 6, 5),
('01-07-2020', '05-08-2020', '05-08-2020',  'Long journey', 200000.0, 22, 5, 5),
('09-05-2021', '09-05-2021', '09-05-2021',  'No specials', 1000.0, 1, 4, 5),
('09-10-2021', null, '10-10-2022', 'Long journey', 5000, 22, 4, 3),
('25-12-2021', null, '10-10-2022', 'No specials', 2000, 23, 7, 6),
('27-12-2021', null, '10-10-2022', 'No specials', 5000, 24, 3, 5)
;

update Contract set paid = true where enddate is null;
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

		public static void SeedWithProcedure(DataContext dbContext)
        {
            #region Функции
			//Функции

            //Доступные материалы
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function MaterialMetric(
	material_id bigint
)
	returns varchar
as $$
declare 
	rec RECORD;
begin 
	select m.metric m1, t.metric m2 into rec
	from material as m join materialtype as t on m.typeid = t.id
	where m.id = material_id and m.id = material_id;
	
	if(rec.m1 ~ '^[\t\n\r\f\v]*$' or rec.m1 is null) then
		if(rec.m2 ~ '^[\t\n\r\f\v]*$' or rec.m2 is null) then
			return '';
		else
			return rec.m2;
		end if;
	else
		return  rec.m1;
	end if;
end;
$$ language plpgsql
SECURITY DEFINER;		
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
		return ( select count(er.id) from extradationrequest as er where er.repairid = repair_id) = 
		( select count(id) from extradationrequest as er where er.repairid = repair_id and (er.status in ('Canceled', 'Done')));
	else 
		return exists ( select * from extradationrequest as er where er.repairid = repair_id and er.status not in ('Canceled', 'Done'));
	end if;
end;
$$ language plpgsql
SECURITY DEFINER;
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
$$ language plpgsql
SECURITY DEFINER;
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
$$ language plpgsql
SECURITY DEFINER;
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
		   	from contract c join yacht_crew ywc on c.CaptainInYachtID = ywc.id 
			where ywc.yachtid = yid and c.enddate is null
		   ;
end; 
$$ language plpgsql
SECURITY DEFINER;
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
$$ language plpgsql
SECURITY DEFINER;
				");  

			//Статус яхты
            dbContext.Database.ExecuteSqlRaw(@"
create or replace FUNCTION YachtsStatus(
Yachtid int ) 
RETURNS varchar
as $$
declare
rec RECORD;
begin
	select * into rec from busyyacht where Yachtid = id;
	if(rec.val) then 
		if(rec.r) then 
			return 'В ремонте';
		elsif(rec.e) then
			return 'Учавствует в событии';
		elsif(rec.c) then
			return 'Выполняет контракт';
		elseif(rec.filled) then
			return 'Готова принимать контракты';
		else
			return 'Свободна';
		end if;
	else 
		return 'Недействительна';
	end if;
end;
$$ language plpgsql
SECURITY DEFINER;

				");	
			//Входит ли в таблицу персонала данная персона
            dbContext.Database.ExecuteSqlRaw(@"
create or replace FUNCTION IsStaff(
PersonID int ) 
RETURNS bool
as $$
declare
rec RECORD;
begin
	return exists (select * from staff where id = PersonId);
end;
$$ language plpgsql
SECURITY DEFINER;
				");
            //Current_Timestamp::timestamp
            dbContext.Database.ExecuteSqlRaw(@"
			create or replace function CurStmp() returns timestamp 
			as $$
			begin return current_timestamp::timestamp(2); end;
			$$ language plpgsql
			SECURITY DEFINER;
				");             
			//Ремонтники
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function LeadRepairMan (RepID int, RepairManID int)
	returns boolean
as $$
declare 
rec RECORD;
begin 	
		return (select s.staffid from Repair_men r 
				 join Staff_position s on r.staffid = s.id
				 where r.repairid = RepID limit 1 ) = RepairManID;
end;
$$ language plpgsql
SECURITY DEFINER;

create or replace function HasRepairMan (RepID int, RepairManID int)
	returns boolean
as $$
declare 
rec RECORD;
begin 	
		return exists (select s.staffid from Repair_men r 
				 join Staff_position s on r.staffid = s.id
				 where r.repairid = RepID and s.staffid = RepairManID);
end;
$$ language plpgsql
SECURITY DEFINER;
				"); 
			//AUTOGRANTER
            dbContext.Database.ExecuteSqlRaw(@"
create or replace FUNCTION AUTOGRANTER(
TAB varchar,
SCHEM varchar,
operation varchar,
USSR varchar ) 
RETURNS VOID
as $$
declare
rec RECORD;
begin
	for rec in 
	SELECT column_name
  FROM information_schema.columns
 WHERE table_schema = SCHEM
   AND table_name   = TAB
   loop 
   		EXECUTE 'GRANT ' || operation || '(' || rec.column_name || ') ON ' || TAB || ' TO ' || USSR  || ';';
   end loop;
   return;
end;
$$ language plpgsql;
				");	

			//Роль по текущему пользователю
            dbContext.Database.ExecuteSqlRaw(@"
/*create or replace function RoleByName (
	username varchar
)
	returns varchar
as $$
declare 
rec RECORD;
begin 	
		select role.rolname into rec from pg_auth_members m 
		join pg_authid role on m.roleid = role.oid
		join pg_authid member on m.member = member.oid
		where member.rolname = username;
return rec.rolname;
end;
$$ language plpgsql;*/

create or replace function rolebyname (
	_email varchar
)
	returns int
as $$
begin 	
	return (select count(*) from pg_roles p where p.rolname like '%' || lowerusername(_email) || '%');
end; 
$$ language plpgsql
SECURITY DEFINER;

				");
			//Опознавание черт
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function LowerUserName (
	username varchar
)
	returns varchar
as $$
begin 	
return lower(regexp_replace (username, '[ [:punct:]]', '_','g'));
end;
$$ language plpgsql
SECURITY DEFINER;

create or replace function MyRoleName (
	posname varchar
)
	returns varchar
as $$
begin 	
return 'my_' || lower(regexp_replace (posname, '[ [:punct:]]', '_','g'));
end;
$$ language plpgsql
SECURITY DEFINER;
				");	
			//Активное количество персонала
            dbContext.Database.ExecuteSqlRaw(@"
CREATE or replace Function CountActiveCrew (_yachtid int)
	Returns int
as $$
Begin
	Return ( select count(*) from yacht_crew where yachtid = _yachtid and enddate is null );
END;
$$ language plpgsql;
				");
			//Капитан по событию
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function YachtCrewByEvent (
	_eventid int
)
	returns setof Yacht_crew
as $$
begin 	
	return query 
	select * from yacht_crew where id in (	
		select distinct yc.id from yacht_crew yc 
		join winner w on w.yachtid = yc.yachtid 
		join event e on w.eventid = e.id and e.id = _eventid
		join staff_position sp on yc.crewid = sp.id 
		join position p on p.id = sp.positionid and p.name = 'Captain'
		where isinterm(yc.startdate, e.startdate, coalesce(e.enddate, e.startdate + interval '1 day' ),coalesce(yc.enddate, e.startdate + interval '1 day' ))
	);
end; 
$$ language plpgsql
SECURITY DEFINER;
				");	
			//Текущий капитан яхты
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function CaptainByYachtid (
	_yachtid int
)
	returns setof Yacht_crew
as $$
begin 	
	return query 
	select * from yacht_crew where id in (	
		select distinct yc.id from yacht_crew yc 
		join staff_position sp on yc.crewid = sp.id 
		join position p on p.id = sp.positionid and p.name = 'Captain'
		where yc.enddate is null and yc.yachtid = _yachtid
		Limit 1
	);
end; 
$$ language plpgsql
SECURITY DEFINER;
				");	
			
			//Аналитика по материалам
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function MaterialAnalytics (
	_name text,
	_type text,
	_from timestamp,
	_to timestamp
)
	returns Table(mname varchar, mtype varchar, avg_money decimal, sum_money decimal,
				 remains bigint, all_mat bigint, mlid bigint, erid bigint, metric varchar
				 )
as $$
begin 	
	return query 
	with available as (
		select * from extradationrequest er 
		where er.Status = 'Done' and isinterm(er.startdate,  _from, _to , coalesce(er.enddate, current_date) )
	)
	select distinct 
	m.name mname,
	t.name mtype, 
	round( avg(ml.overallprice) over (partition by m.id), 2 ) avg_money, 
	round( sum(ml.overallprice) over (partition by m.id), 2 ) sum_money,
	cast( sum(coalesce(a.count, 0)) over (partition by m.id) as bigint ) remains,
	cast( sum(ml.count) over (partition by m.id) as bigint )all_mat, 
	count(ml.id) over (partition by m.id) mlid,
	count(a.count) over (partition by m.id) erid,
	materialmetric(m.id) metric
	
	from materiallease ml 
	join material m on m.id = ml.material and m.name like '%' || _name || '%'
	join materialtype t on t.id = m.typeid and t.name like '%' || _type || '%'
	left join available a on a.material = m.id 
	where isinterm(ml.startdate, _from, _to, ml.deliverydate) and ml.deliverydate is not null
	order by mname desc
	;
end; 
$$ language plpgsql
SECURITY DEFINER;
				");
            //Аналитика по контрактам
            dbContext.Database.ExecuteSqlRaw(@"
			create or replace function ContractAnalytics (
				_name text,
				_surname text,
				_phone text,
				_email text,
				_yname text,
				_ytype text,
				_from timestamp,
				_to timestamp
			)
				returns Table(avg_money decimal, sum_money decimal, count bigint )
			as $$
			begin 	
				return query 
				select distinct
				round( avg(c.averallprice) over (), 2 ) avg_money, 
				round( sum(c.averallprice) over (), 2 ) sum_money, 
				count(c.id) over ()
				from contract c 
				join person p on c.clientid = p.id and (
					p.phone like '%' || _phone || '%'
					and
					p.email like '%' || _email || '%'
					and 
					p.name like '%' || _name || '%'
					and
					p.surname like '%' || _surname || '%'
					)
				join yacht_crew yc on yc.id = c.captaininyachtid 
				join yacht y on  yc.yachtid = y.id and y.name like '%' || _yname || '%' 
				join yachttype yt on y.typeid = yt.id and yt.name like '%' || _ytype || '%' 
				where 
				isinterm(c.startdate, _from, _to, coalesce(c.enddate, current_date) ) and c.paid;
			end; 

			$$ language plpgsql
			SECURITY DEFINER;
							");

			//Поиск по контрактам
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function ContractSearch (
				_name text,
				_surname text,
				_phone text,
				_email text,
				_cname text,
				_csurname text,
				_cphone text,
				_cemail text,
				_yname text,
				_ytype text,
				_ctype text,
				_ispaid boolean,
				_from timestamp,
				_to timestamp
			)
				returns Table(_id text,
							  _start timestamp(2), 
							  _end timestamp(2), 
							  _fend timestamp(2),
							  _yinfo text,
							  _clinfo text,
							  _capinfo text,
							  _specials text,
							  _averall decimal,
							  _ctname varchar,
							  _ctprice decimal)
			as $$
			begin 	
				return query 
				select distinct
				lpad(c.id || '', 8, '0'),
				c.startdate, 
				c.duration, 
				c.enddate,
				y.name || ' ( ' || yt.name || ' )',
				p.name || ' ' || p.surname || ' ' || p.email || ' ' || p.phone,
				cap.name || ' ' || cap.surname || ' ' || cap.email || ' ' || cap.phone,
				c.specials, 
				round( c.averallprice, 2 ),
				ct.name, 
				round( ct.price, 2 )
				from contract c 
				join person p on c.clientid = p.id and (
					p.phone like '%' || _phone || '%'
					and
					p.email like '%' || _email || '%'
					and 
					p.name like '%' || _name || '%'
					and
					p.surname like '%' || _surname || '%'
					)
				join yacht_crew yc on yc.id = c.captaininyachtid 
				join staff_position sp on yc.crewid = sp.id
				join person cap on cap.id = sp.staffid and (
					cap.phone like '%' || _cphone || '%'
					and
					cap.email like '%' || _cemail || '%'
					and 
					cap.name like '%' || _cname || '%'
					and
					cap.surname like '%' || _csurname || '%'
					)
				join contracttype ct on c.contracttypeid = ct.id and ct.name like '%' || _ctype || '%' 
				join yacht y on yc.yachtid = y.id and y.name like '%' || _yname || '%' 
				join yachttype yt on y.typeid = yt.id and yt.name like '%' || _ytype || '%' 
				where 
				isinterm(c.startdate, _from, _to, coalesce(c.enddate, current_date) ) and (c.paid = _ispaid);
			end; 
			
			$$ language plpgsql
			SECURITY DEFINER;
			
							");
			//Поиск по контрактам по аренде
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function YachtleaseSearch(
	_name text,
	_surname text,
	_phone text,
	_email text,
	_yname text,
	_ytype text,
	_ylname text,
	_ispaid boolean,
	_from timestamp,
	_to timestamp
)
	returns Table(_id text,_start timestamp(2), _end timestamp(2), _fend timestamp(2), _yinfo text, _clinfo text, _capinfo text,
							  _specials text, _averall decimal, _ctname varchar, _ctprice decimal)
as $$
begin 	
	return query 
	select distinct
				lpad(yl.id || '', 8, '0'),
				yl.startdate, 
				yl.duration, 
				yl.enddate,
				y.name || ' ( ' || yt.name || ' )',
				p.name || ' ' || p.surname || ' ' || p.email || ' ' || p.phone,
				'',
				yl.specials, 
				round( yl.overallprice, 2 ),
				ylc.name, 
				round( ylc.price, 2 )
	from yachtlease yl 
	join yacht y on yl.yachtid = y.id and y.name like '%' || _yname || '%' 
	join yachttype yt on y.typeid = yt.id and yt.name like '%' || _ytype || '%' 
	join person p on y.yachtownerid = p.id and (
		p.phone like '%' || _phone || '%'
		and
		p.email like '%' || _email || '%'
		and 
		p.name like '%' || _name || '%'
		and
		p.surname like '%' || _surname || '%'
	)
	join yachtleasetype ylc on ylc.id = yl.yachtleasetypeid and ylc.name like '%' || _ylname || '%' 
	where 
	isinterm(yl.startdate, _from, _to, coalesce(yl.enddate, current_date) ) and _ispaid = yl.paid;
end; 
$$ language plpgsql
SECURITY DEFINER;	
							");

            //Аналитика по контрактам на аренду
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function YachtleaseAnalytics (
	_name text,
	_surname text,
	_phone text,
	_email text,
	_yname text,
	_ytype text,
	_from timestamp,
	_to timestamp
)
	returns Table(avg_money decimal, sum_money decimal, count bigint )
as $$
begin 	
	return query 
	select distinct
	round( avg(yl.overallprice) over (), 2 ) avg_money, 
	round( sum(yl.overallprice) over (), 2 ) sum_money, 
	count(yl.id) over ()
	from yachtlease yl 
	join yacht y on yl.yachtid = y.id and y.name like '%' || _yname || '%' 
	join yachttype yt on y.typeid = yt.id and yt.name like '%' || _ytype || '%' 
	join person p on y.yachtownerid = p.id and (
		p.phone like '%' || _phone || '%'
		and
		p.email like '%' || _email || '%'
		and 
		p.name like '%' || _name || '%'
		and
		p.surname like '%' || _surname || '%'
	)
	where 
	isinterm(yl.startdate, _from, _to, coalesce(yl.enddate, current_date) ) and yl.paid;
end; 
$$ language plpgsql
SECURITY DEFINER;
				");
			//Поиск должностей
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function SPView (
	_name text,
	_surname text,
	_phone text,
	_email text,
	_pname text,
	_active bool
)
	returns Table(p_name varchar, p_surname varchar, p_phone varchar, p_email varchar, p_position varchar, p_startdate timestamp, p_enddate timestamp )
as $$
begin 	
	return query 
	select distinct
	p.name p_name,
	p.surname p_surname,
	cast(p.phone as varchar) p_phone,
	cast(p.email as varchar) p_email,
	po.name p_position,
	sp.startdate p_startdate,
	sp.enddate p_enddate
	from staff_position sp 
	join person p on sp.staffid = p.id and (
		p.phone like '%' || _phone || '%'
		and
		p.email like '%' || _email || '%'
		and 
		p.name like '%' || _name || '%'
		and
		p.surname like '%' || _surname || '%'
	)
	join position po on po.id = sp.positionid and po.name like '%' || _pname || '%'
	where (sp.enddate is not null and not _active) or (sp.enddate is null and _active);
end; 
$$ language plpgsql
SECURITY DEFINER;

				");
			//Поиск экипажей
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function YCView (
	_name text,
	_surname text,
	_phone text,
	_email text,
	_pname text,
	_yname text,
	_ytype text,
	_active bool
)
	returns Table(p_name varchar, p_surname varchar, p_phone varchar, p_email varchar, p_position varchar,
				  yachtname varchar, yachttype varchar, p_startdate timestamp, p_enddate timestamp )
as $$
begin 	
	return query 
	select 
	p.name p_name,
	p.surname p_surname,
	cast(p.phone as varchar) p_phone,
	cast(p.email as varchar) p_email,
	po.name p_position,
	y.name yachtname,
	yt.name yachttype,
	yc.startdate p_startdate,
	yc.enddate p_enddate
	from yacht_crew yc
	join yacht y on yc.yachtid = y.id and y.name like '%' || _yname || '%' 
	join yachttype yt on y.typeid = yt.id and yt.name like '%' || _ytype || '%' 
	join staff_position sp on yc.crewid = sp.id
	join person p on sp.staffid = p.id and (
		p.phone like '%' || _phone || '%'
		and
		p.email like '%' || _email || '%'
		and 
		p.name like '%' || _name || '%'
		and
		p.surname like '%' || _surname || '%'
	)
	join position po on po.id = sp.positionid and po.name like '%' || _pname || '%'
	where (yc.enddate is not null and not _active) or (yc.enddate is null and _active);
end; 
$$ language plpgsql
SECURITY DEFINER;
				");
			//Поиск ремонтов
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function RepairView (
	_name text,
	_surname text,
	_phone text,
	_email text,
	_yname text,
	_ytype text,
	_active bool
)
	returns Table(p_name varchar, p_surname varchar, p_phone varchar, p_email varchar, 
				  yachtname varchar, yachttype varchar, p_startdate timestamp, p_enddate timestamp )
as $$
begin 	
	return query 
	select 
	p.name p_name,
	p.surname p_surname,
	cast(p.phone as varchar) p_phone,
	cast(p.email as varchar) p_email,
	y.name yachtname,
	yt.name yachttype,
	r.startdate p_startdate,
	r.enddate p_enddate
	from repair_men rm 
	join repair r on rm.repairid = r.id
	join yacht y on r.yachtid = y.id and y.name like '%' || _yname || '%' 
	join yachttype yt on y.typeid = yt.id and yt.name like '%' || _ytype || '%' 
	join staff_position sp on rm.staffid = sp.id
	join person p on sp.staffid = p.id and (
		p.phone like '%' || _phone || '%'
		and
		p.email like '%' || _email || '%'
		and 
		p.name like '%' || _name || '%'
		and
		p.surname like '%' || _surname || '%'
	)
	where (r.enddate is not null and not _active) or (r.enddate is null and _active);
end; 
$$ language plpgsql
SECURITY DEFINER;
				");
			
			//Тесты яхт у яхты
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function YachttestInfo(
	_yid int
)
	returns Table(_test text, _who text, _date timestamp(2))
as $$
begin 	
	return query 
	select 
				yt.results, 
				p.name || ' ' || p.surname || ' ' || p.email || ' ' || p.phone,
				yt.date
	from yachttest yt 
	join staff_position sp on sp.id = yt.staffid and yt.yachtid = _yid
	join person p on sp.staffid = p.id
	order by yt.date desc;
end; 
$$ language plpgsql
SECURITY DEFINER;	
				");
			
			//Ремонты у яхты
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function RepairInfo(
	_yid int
)
	returns Table(_text text,
				  _status varchar,
				  _who text,
				  _start timestamp(2),
				  _end timestamp(2),
				  _fend timestamp(2)
				 )
as $$
begin 	
	return query 
	select 
				r.description, 
				r.status,
				p.name || ' ' || p.surname || ' ' || p.email || ' ' || p.phone,
				r.startdate,
				r.duration,
				r.enddate
	from repair r
	join staff_position sp on sp.id = ( select staffid from repair_men where repairid = 1 limit 1 )
	join person p on sp.staffid = p.id
	where r.yachtid = _yid and status <> 'Canceled'
	order by coalesce(r.enddate,curstmp()) desc;
end; 
$$ language plpgsql
SECURITY DEFINER;	
				");

			#endregion

			#region Процедуры
			//Капитан должен быть
			dbContext.Database.ExecuteSqlRaw(@"
CREATE OR REPLACE Procedure AddCaptainToYachttype(
YTid int
)
AS $$
begin
	insert into position_yachttype(positionid,yachttypeid,count) values ((select id from position where name = 'Captain'), YTId, 1);
end;
$$ language plpgsql
SECURITY DEFINER;
				");

			#region Хлам
			//ДАйте мне новый аккаунт пожалуйста
			/*			dbContext.Database.ExecuteSqlRaw(@"
			CREATE OR REPLACE Procedure CreateAccountByPO(
			PersonID int,
			_password varchar
			)
			AS $$
			declare
			rec record;
			begin
				execute 'select email from person where id = ' || PersonID || ' ;' into rec;
				insert into account(login, password, UserID) 
				values
				(rec.email, _password, PersonID);
			end;
			$$ language plpgsql;
							");*/
			#endregion

			//Новый ремонт, новый ремонтник
			dbContext.Database.ExecuteSqlRaw(@"
CREATE OR REPLACE Procedure PopulateRepair_Men(
_RepairID int,
_PersonID int
)
AS $$
declare
rec record;
begin
	execute 'select * from repair_staff where staffid = ' || _PersonID || ' and enddate is null ;' into rec;
		
	insert into repair_men(repairid, staffid) 
	values
	(_RepairID, rec.id);
end;
$$ language plpgsql
SECURITY DEFINER;

				");

			//Выдача новых активных ролей
			dbContext.Database.ExecuteSqlRaw(@"
CREATE OR REPLACE Procedure PopulateAllValidSP(
_email varchar, 
_pass varchar
)
AS $$
declare
rec record;
username varchar;
_email_ varchar;
begin
	_email_ := LowerUserName(_email);
	for rec in select po.name, p.email from 
	staff_position sp join person p on sp.staffid = p.id
	join position po on po.id = sp.positionid 
	where p.email = _email and sp.enddate is null
	Loop
		username := LowerUserName(rec.name) || '_' || _email_;
		if(not exists (select rolname from pg_roles where rolname = username)) then
			execute 'CREATE ROLE ' || username || ' WITH LOGIN PASSWORD ''' || _pass || ''';';
			execute 'GRANT ' || MyRoleName(rec.name) || ' TO ' || username || ' ;';
		end if;
	end loop;
	execute 'CREATE ROLE client_' || _email_ || ' WITH LOGIN PASSWORD ''' || _pass || ''';';
	execute 'GRANT my_client TO client_' || _email_  || ' ;';
end;
$$ language plpgsql
SECURITY DEFINER;
				");	
			//Выдача новых активных ролей Защищено
			dbContext.Database.ExecuteSqlRaw(@"
CREATE OR REPLACE Procedure PopulateAllValidSP_r(
_email varchar, 
_pass varchar
)
AS $$
begin
	call exec_by_rolecreator('call PopulateAllValidSP('''''|| _email ||''''','''''|| _pass ||''''');');
end;
$$ language plpgsql
SECURITY DEFINER;
				");


			//Обновление пароля активных ролей
			dbContext.Database.ExecuteSqlRaw(@"
CREATE OR REPLACE Procedure updateexistingroles(
_email varchar, 
_bpass varchar
)
AS $$
declare
rec record;
username varchar;
_email_ varchar;
begin
	_email_ := LowerUserName(_email);
	for rec in select rolname from pg_roles where rolname like '%' || _email_ ||'%'
	Loop
		execute 'ALTER ROLE ' || rec.rolname ||' WITH PASSWORD ''' || _bpass || ''' ;';
	end loop;
end;
$$ language plpgsql;
				");
			//Обновление пароля активных ролей Защищено
			dbContext.Database.ExecuteSqlRaw(@"
CREATE OR REPLACE Procedure updateexistingroles_r(
_email varchar, 
_bpass varchar
)
AS $$
begin
	call exec_by_rolecreator('call updateexistingroles('''''|| _email ||''''','''''|| _bpass ||''''');');
end;
$$ language plpgsql;

				");

			//Удаление активных ролей
			dbContext.Database.ExecuteSqlRaw(@"

CREATE OR REPLACE Procedure RemoveAllExistingRoles(
_email varchar
)
AS $$
declare
rec record;
username varchar;
begin
	for rec in select rolname from pg_roles where 
		rolname LIKE '%' || LowerUserName(_email) || '%'
	Loop
		execute 'DROP ROLE IF EXISTS '|| rec.rolname ||' ;';
	end loop;
end;
$$ language plpgsql;

				");
			//Удаление активных ролей Защищено
			dbContext.Database.ExecuteSqlRaw(@"

CREATE OR REPLACE Procedure RemoveAllExistingRoles_R(
_email varchar
)
AS $$
begin
	call exec_by_rolecreator('call removeallexistingroles('''''|| _email ||''''');');
end;
$$ language plpgsql;

				");

			//Удаление должности
			dbContext.Database.ExecuteSqlRaw(@"

CREATE OR REPLACE Procedure RemoveExistingRole(
_position varchar
)
AS $$
declare
rec record;
username varchar;
begin
	execute 'DROP ROLE IF EXISTS '|| _position ||' ;';
end;
$$ language plpgsql;

				");
			//Удаление должности
			dbContext.Database.ExecuteSqlRaw(@"

CREATE OR REPLACE Procedure RemoveExistingRole_R(
_position varchar
)
AS $$
begin
	call exec_by_rolecreator('call RemoveExistingRole('''''|| _position ||''''');');
end;
$$ language plpgsql;

				");

			//Переименование должности
			dbContext.Database.ExecuteSqlRaw(@"

CREATE OR REPLACE Procedure RenameRole(
_oldname varchar,
_newname varchar
)
AS $$
declare
rec record;
username varchar;
begin
	execute 'ALTER ROLE '|| _oldname ||' RENAME TO '|| _newname ||';';	
end;
$$ language plpgsql;


				");
			//Переименование должности
			dbContext.Database.ExecuteSqlRaw(@"

CREATE OR REPLACE Procedure RenameRole_R(
_oldname varchar,
_newname varchar
)
AS $$
begin
	call exec_by_rolecreator('call RenameRole('''''|| _oldname ||''''','''''|| _newname ||''''');');
end;
$$ language plpgsql;

				");

			//Пинг
			dbContext.Database.ExecuteSqlRaw(@"

CREATE OR REPLACE procedure TryConnect(
_email varchar, 
_role varchar,
_pass varchar
)
AS $$
declare
begin
	execute 'SELECT dblink_connect(''session__11'',''dbname=YachtClub user='|| LowerUserName(_role) || '_' || LowerUserName(_email) ||' password='|| _pass ||''');';
	execute 'select dblink_disconnect(''session__11'');'; 
end;
$$ language plpgsql;


				");	

			//Добавить один аккаунт
			dbContext.Database.ExecuteSqlRaw(@"

CREATE OR REPLACE Procedure addnewacc(
_email varchar, 
_role int,
_pass varchar
)
AS $$
declare
rec RECORD;
usy varchar;
begin
	execute ' select name  from position where id = ' || _role ||  ' ;' into rec;
	usy :=  LowerUserName(rec.name) || '_' || LowerUserName(_email);
	
	execute 'CREATE ROLE ' || usy || ' WITH LOGIN PASSWORD ''' || _pass || ''';';
	execute 'GRANT ' || myrolename(rec.name) ||' TO ' || usy || ' ;';
end;
$$ language plpgsql;

CREATE OR REPLACE Procedure addnewacc(
_email varchar, 
_pass varchar
)
AS $$
declare
usy varchar;
begin
	usy :=  'client_' || LowerUserName(_email);
	
	execute 'CREATE ROLE ' || usy || ' WITH LOGIN PASSWORD ''' || _pass || ''';';
	execute 'GRANT my_client TO ' || usy  || ' ;';
end;
$$ language plpgsql;
				");
			//Добавить один аккаунт Защищено
			dbContext.Database.ExecuteSqlRaw(@"

CREATE OR REPLACE Procedure addnewacc_r(
_email varchar, 
_role int,
_pass varchar
)
AS $$
declare
begin
	call exec_by_rolecreator('call addnewacc('''''|| _email ||''''','|| _role ||','''''|| _pass ||''''');');
end;
$$ language plpgsql;

CREATE OR REPLACE Procedure addnewacc_r(
_email varchar, 
_pass varchar
)
AS $$
begin
	call exec_by_rolecreator('call addnewacc('''''|| _email ||''''','''''|| _pass ||''''');');
end;
$$ language plpgsql;
				");

			//Добавить новую должность
			dbContext.Database.ExecuteSqlRaw(@"

CREATE OR REPLACE Procedure addnewrole(
_user varchar, 
_role varchar
)
AS $$
begin
	execute 'CREATE ROLE ' || _user || ' ;';
	execute 'GRANT ' || _role ||' TO ' || _user  || ' ;';
end;
$$ language plpgsql;
				");
			//Добавить новую должность Защищено
			dbContext.Database.ExecuteSqlRaw(@"

CREATE OR REPLACE Procedure addnewrole_r(
_user varchar, 
_role varchar
)
AS $$
begin
	call exec_by_rolecreator('call addnewrole('''''|| _user ||''''','''''|| _role ||''''');');
end;
$$ language plpgsql;
				");

			//Запуск команды от имени
			dbContext.Database.ExecuteSqlRaw(@"
CREATE OR REPLACE Procedure exec_by_rolecreator(
command text
)
AS $$
declare
usy varchar;
begin
	execute 'SELECT dblink_connect(''___session__11_'',''dbname=YachtClub user=guest_rollercoaster password=jesuschrist'');';
	begin
	execute 'select dblink(''___session__11_'','''|| command || ''');';
	EXCEPTION WHEN OTHERS THEN
		execute 'select dblink_disconnect(''___session__11_'');'; 
		raise exception '%', SQLERRM using ERRCODE = SQLSTATE;
	end;
    execute 'select dblink_disconnect(''___session__11_'');'; 
end;
$$ language plpgsql;
				");
			#endregion

			#region Правила
			//Правила
			dbContext.Database.ExecuteSqlRaw(@"
CREATE OR REPLACE RULE R_PYT_Captain_NotU AS ON UPDATE TO Position_Yachttype
    WHERE old.positionid = (select id from position where name = 'Captain')
    DO INSTEAD NOTHING;
	
CREATE OR REPLACE RULE R_PYT_Captain_NotD AS ON DELETE TO Position_Yachttype
    WHERE old.positionid = (select id from position where name = 'Captain')
    DO INSTEAD NOTHING;
	
CREATE OR REPLACE RULE R_P_Captain_NotD AS ON DELETE TO Position
    WHERE old.declinedelete
    DO INSTEAD NOTHING;


				");
			//Стандартная дата
			dbContext.Database.ExecuteSqlRaw(@"
	/*create or replace rule StandartDate as
	on insert to review
	where new.date is null
	do instead insert into review(clientid, date, text, public, rate) 
	values (new.clientid, current_timestamp, new.text, new.public new.rate);*/
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
select distinct m.id material, coalesce(ar.count, 0) count, materialmetric(m.id) format  from 
	material as m left join counter as ar on m.id = ar.material
	order by m.id
);
				");

            //Товарищи ремонтники
            dbContext.Database.ExecuteSqlRaw(@"
Create or replace view Repair_Staff as (
select sp.id, sp.staffid, sp.positionid, sp.startdate, sp.enddate,sp.salary, sp.description from
staff_position as sp 
where sp.id in (select StaffPositionListByPosition('Repairman'))
	);
");

            //Персонал
            dbContext.Database.ExecuteSqlRaw(@"
Create or replace View Staff as (
		select * from person p where p.stafforigin
			
);

");

            //Морские волки
            dbContext.Database.ExecuteSqlRaw(@"
create or replace view yacht_crew_position as (
	select yc.id, yc.yachtid, yc.crewid, p.id positionid, p.name positionname, yc.startdate, yc.enddate from yacht_crew yc 
	join staff_position sp on yc.crewid = sp.id  
	join position p on sp.positionid = p.id
	);

");

            //Приключения яхт
            dbContext.Database.ExecuteSqlRaw(@"
			--Яхты с действительными контрактами на аренду
			create or replace view YachtLeaseStatus as (
				with openlease as (
					select * from yachtlease yl where yl.paid and yl.enddate is null and yl.startdate <= current_timestamp
				),
					yachtinopenlease as (
					select * from yacht where id in (select yachtid from openlease)
				)
				select *,
				case when id in (select id from yachtinopenlease) then 'Free'
					 else 'Invalid'
				end status
				from yacht
			);
			--Яхты, находящиеся в ремонте
			create or replace view YachtInRepair as (
				with openrepair as (
					select * from repair where enddate is null and startdate <= current_timestamp
				),
				yachtinrepair as (
					select * from yacht where id in (select yachtid from openrepair)
				)
				select *, 
				case when id in (select id from yachtinrepair) then 'Repairing'
					 else 'Free'
				end status
				from yacht
			);
			--Яхты, выполняющие контракт
			create or replace view YachtInContract as (
				with yachtincontract as (
					select yc.yachtid from yacht_crew yc join contract c on c.CaptainInYachtID = yc.id 
					where c.paid and c.startdate <= current_timestamp and c.enddate is null
				),
				yachtfaraway as (
					select * from yacht where yacht.id in ( select yachtid from yachtincontract )
				)
				select *, 
				case when id in (select id from yachtfaraway ) then 'Far Away'
					 else 'Free'
				end status
				from yacht order by id
			);
			--Яхты, учавствующие в событии
			create or replace view YachtInEvent as (
				with yachtincontract as (
					select w.yachtid from winner w join event e on e.id = w.eventid
					where e.startdate <= current_timestamp and e.enddate is null
				),
				yachtfaraway as (
					select * from yacht where yacht.id in ( select yachtid from yachtincontract )
				)
				select *, 
				case when id in (select id from yachtfaraway ) then 'Participating'
					 else 'Free'
				end status
				from yacht order by id
			);
			--Яхты у которых есть минимум экипажа
			create or replace view ReadyToContract as (
				with 
				--Яхты с перечнем должностей, которые должны на ней находится
				yachtpositions as (
					select distinct y.id, pyt.positionid pid, pyt.count from position_yachttype pyt 
					join yachttype yt on pyt.yachttypeid = yt.id 
					join yacht y on y.typeid = yt.id order by y.id
				),
				--Перечень должностей находящихся на яхте и количество человек, которые находятся на должности X
				crewpositions as (
					select distinct yc.yachtid id, p.id pid, count(p.id) over (partition by yc.yachtid, p.id) count 
					from yacht_crew yc join 
					staff_position sp on yc.crewid = sp.id 
					join position p on sp.positionid = p.id where yc.enddate is null
				)
				,
				--Проверка каждой из должностей, соответствует ли та необходимому минимуму
				neccessaryareincrew as (
					select id, pid, case when yp.count <= cp.count then true else false end hascrew
					from yachtpositions yp left join crewpositions cp using(id, pid)
				),
				allincrew as (
					select id, bool_and(hascrew) status from neccessaryareincrew group by id
				) 
				select *, 
				case when not (select status from allincrew a where a.id = y.id ) then 'Filling' else 'Filled' end 
				status
				from yacht y
			);
			
			Create or replace view BusyYacht as (
				with 
				InRepair as (
					select id, case when status = 'Free' then false else true end from YachtInRepair 
				),
				InContract as (
					select id, case when status = 'Free' then false else true end from YachtInContract 
				),
				InEvent as (
					select id, case when status = 'Free' then false else true end from YachtInEvent
				),
				IsValid as (
					select id, case when status = 'Free' then true else false end from YachtLeaseStatus
				),
				IsFilled as (
					select id, case when status = 'Filled' then true else false end from ReadyToContract
				)
				select id, r.case r, c.case c, e.case e, i.case val, f.case filled from yacht 
				join InRepair r using(id) 
				join InContract c using (id) 
				join InEvent e using(id) 
				join IsValid i using(id)
				join IsFilled f using(id)
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
			return new;
        ELSIF (TG_OP = 'INSERT') then
			if((select r.status from repair r where r.id = new.repairid) in ('Done', 'Canceled')) then
				raise exception 'Попытка создания заявки на закрытый ремонт';
			end if;
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
	if (not exists (select * from repair_men rm where new.Staffid = rm.staffid and rm.repairid = new.repairid) and TG_OP = 'INSERT') then
		raise exception 'Данный ремонтник не может оставить заявку на материалы';
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
			rec := old;
			if( not (old.startdate >= current_timestamp and new.startdate >= current_timestamp) ) then
				rec.startdate := new.startdate;	
			end if;
			if(rec <> old) then raise exception 'Изменены запрещенные поля';
			end if;

			if(old.personnel > new.personnel) then 
				raise exception 'Количество персонала может только увеличиваться';
			end if;

			if( new.duration < current_timestamp or new.duration is null) then new.duration = current_timestamp; end if;
			return new;
        ELSIF (TG_OP = 'INSERT') then
			if( new.startdate < current_timestamp or new.startdate is null) then new.startdate = current_timestamp; end if;
			if( new.duration < current_timestamp or new.duration is null) then new.duration = current_timestamp; end if;
			New.enddate = null;
			New.Status = 'Created';
			select * into rec from busyyacht b where b.id = new.yachtid;
			if(not rec.val) then raise exception 'Недействительные яхты не обслуживаются';
			elsif(rec.r) then raise exception 'Яхта уже ремонтируется';
			elsif(rec.e) then raise exception 'Дождитесь окончания события, чтобы отремонтировать яхту';
			elsif(rec.c) then raise exception 'Дождитесь возвращения яхты с контракта';
			end if;
            RETURN NEW;
		ELSIF (TG_OP = 'DELETE') THEN
			if(old.status in ('Done')) then 
				raise exception 'Попытка удаления закрытого ремонта';			
			end if;
			if(exists (select * from extradationrequest er where er.repairid = old.id and er.Status = 'Done')) then
				raise exception 'На ремонт уже были выданы материалы';
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
	if (old.status = 'Created' and new.Status not in ( 'Waits', 'In Progress' , 'Canceled', 'Created') ) then 
		raise exception 'Переход из статуса создан, может быть только в статус Ожидает, В процессе или Отменён';
	elsif(old.status = 'Created' and new.Status = 'Waits' and 
		 not CHECK_ER_FOR_REPS(false, new.id) ) then
		raise exception 'Переход в статус Ожидает невозможен, ведь отсутствуют активные заявки';
	/*ОЖИДАЕТ МАТЕРИАЛОВ -> (В ПРОЦЕССЕ ЛИБО ОТМЕНЁН)*/
	elsif (old.status = 'Waits' and new.Status not in ('In Progress' , 'Canceled', 'Waits') ) then
		raise exception 'Переход из статуса Ожидает, может быть только в статус В Процессе или Отменён';
	elsif(old.status = 'Waits' and new.Status = 'In Progress' and 
		 not CHECK_ER_FOR_REPS(true, new.id) ) then
		raise exception 'Переход в статус В Процессе невозможен, ведь отсутствуют закрытые заявки';
	/*В ПРОЦЕССЕ -> (ОЖИДАЕТ МАТЕРИАЛОВ ЛИБО ГОТОВ ЛИБО ОТМЕНЁН)*/
	elsif (old.status = 'In Progress' and new.Status not in ('Waits', 'Done', 'Canceled', 'In Progress') ) then
		raise exception 'Переход из статуса В Процессе, может быть только в статус Ожидает или Готов или Отменён';	
	elsif(old.status = 'In Progress' and new.Status = 'Waits' and 
		 not CHECK_ER_FOR_REPS(false, new.id) ) then
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

			new.registrydate = current_timestamp;
            RETURN NEW;
end;
$$ language plpgsql;	

create trigger ReadonlyConstraint
Before insert on Person
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
				raise exception 'Попытка изменения уволенного сотрудника';
			end if;
			if( (select name from position where id = old.positionid) = 'Fired' and old.enddate is null and new.enddate is null ) then
				raise exception 'Попытка изменения уволенного сотрудника';
			end if;
			if(new.enddate is not null) then 
				update Yacht_Crew set enddate = new.enddate where crewid = new.id;
				if( (select count(*) from staff_position sp where old.id = sp.id and sp.enddate is null) - 1 <= 0) then
					insert into staff_position(staffid, positionid, startdate) values
					(old.staffid, (select id from position where name = 'Fired'), curstmp());
				end if;
			end if;
			
        ELSIF (TG_OP = 'INSERT') then
			--Выставление стандартной даты
			new.startdate = current_timestamp; 
			--Проверка открытых записей на должность
			if( (select count(sp.id) from staff_position sp 
				 where sp.staffid = new.staffid and sp.positionid = new.positionid and sp.enddate is null) >= 1) then
				 raise exception 'Уже присутствует открытая запись человека на данной должности, закройте предыдущую и повторите попытку';
			end if;

			if( (select name from position where id = new.positionid) = 'Fired' ) then
				update staff_position set enddate = curstmp() where staffid = new.staffid and enddate is null;
			end if;

			if( exists(select * from staff_position sp 
					   join person p on sp.staffid = p.id and p.id = new.staffid
					   join position po on sp.positionid = po.id and po.name = 'Fired'
					   where sp.enddate is null
					  ) ) then 
					update staff_position set enddate = curstmp() where staffid = new.staffid and enddate is null; 
			end if;
			new.enddate = null;
		ELSIF (TG_OP = 'DELETE') then
			if(old.enddate is not null) then 
				raise exception 'Попытка удаления уволенного сотрудника';
			end if;
			return old;
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
        IF (TG_OP = 'INSERT') then
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
Before insert or delete on Repair_Men
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
        ELSIF (TG_OP = 'INSERT') then
			new.startdate = current_timestamp;
			new.enddate = null;
		ELSIF (TG_OP = 'DELETE') then 
			if(old.enddate is not null) then 
				raise exception 'Попытка удаления уволенного члена экипажа ';			
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

            //Триггер проверки экипажа
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function YC2 ()
	returns trigger
as $$
begin 	
		if(TG_OP = 'INSERT') then
			  if( exists 
				   (
						select * from staff_position sp join position p on sp.positionid = p.id where not p.crewposition
						and new.crewid = sp.id
				   ) 
				  ) then raise exception 'Вы добавляете не моряка на должность члена экипажа';
			  elsif ( exists (select * from staff_position sp where new.crewid = sp.id and sp.enddate is not null) ) then
				raise exception 'Попытка добавить переведённого на другую должность сотрудника';
			  elsif ( exists (select * from yacht_crew yc where yc.crewid = new.crewid and yc.enddate is null ) ) then
				raise exception 'Данный моряк уже служит на другой яхте';
			  elsif( exists (select * from yacht_crew_position y
					   where y.yachtid = new.yachtid
					   and y.enddate is null
						and positionname = 'Captain'
					   ) 
			   and exists ( select * from staff_position sp join position p on p.id = sp.positionid
							where sp.id = new.crewid and sp.enddate is null and p.name = 'Captain' )
			  
			  ) then
			   	raise exception 'На этой яхте уже есть капитан';
			  
			  end if;
		elsif(TG_OP = 'UPDATE') then
			  if ( (select b.c from busyyacht b where b.id = new.yachtid and new.enddate is not null ) ) then
				raise exception 'Невозможно перевести сотрудника с яхты во время выполнения контракта';
			  elsif ( (
					select b.e from busyyacht b join yacht_crew_position ycp on ycp.yachtid = b.id
					   where b.id = new.yachtid and ycp.crewid = new.crewid and new.enddate is not null 
						and ycp.positionname = 'Captain'
					  ) ) then
				raise 		exception 'Невозможно перевести капитана с яхты во время мероприятия';
			  end if;
		end if;
		RETURN new;
end;
$$ language plpgsql;	

create trigger CrewConstaraint
Before insert or update on Yacht_Crew
for each row execute function YC2 ();
				");

            //Триггер проверки количества экипажа
            dbContext.Database.ExecuteSqlRaw(@"
create or replace function YC3 ()
	returns trigger
as $$
declare 
rec RECORD;
begin 	
		select yt.crewcapacity cap into rec from yacht y join yachttype yt on y.typeid = yt.id where y.id = new.yachtid;		
		if( rec.cap - (
			select count(yc.id) from yacht_crew yc where yc.yachtid = new.yachtid and yc.enddate is null 
		) - 1 <= 0 ) then 
			 raise exception 'Данная яхта не поддерживает более % членов экипажа', rec.cap;
		end if;
		RETURN new;
end;
$$ language plpgsql;	

create trigger PersonnelChecker
Before insert on Yacht_Crew
for each row execute function YC3 ();
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
			rec := old;
			if( not (old.startdate >= current_timestamp and new.startdate >= current_timestamp)) then
				rec.startdate := new.startdate;
			end if;
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
			if( not (select canhavewinners from event e where e.id = new.eventid ) ) then 
				new.place := null;
			end if;
		ELSIF (TG_OP = 'INSERT') then
				select * into rec from event e where new.eventid = e.id ; 
				if(rec.enddate is not null) then raise exception 'Попытка участия в закрытом событии'; end if;
				if(not rec.canhavewinners) then 
					new.place := null;
				end if;
				
				select * into rec from busyyacht b where new.yachtid = b.id;
				if(not rec.val) then 
					raise exception 'Яхта недействительна и не может учавствовать в мероприятии';
				elsif( rec.r or rec.c) then
				  	raise exception 'Попытка участия в событии при ремонте/контракте';
				end if;

				if(not exists(select * from yacht_crew yc join yacht_crew_position yp using(id) 
							  where positionname = 'Captain' and yc.enddate is null and yc.yachtid = new.yachtid ) ) then
				    raise exception 'На яхте в текущий момент отсутствует действующий капитан';
				end if;
				
		ELSIF (TG_OP = 'DELETE') then 
			if( not exists ( select * from event e where e.id = old.eventid and e.enddate is null)) then 
				raise exception 'Попытка удаления учасника события вместе с событием, удалите пожалуйста всех учасников события';			
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
						rec := old;
						if(old.paid) then
								rec.startdate = new.startdate;
								rec.paid = new.paid;
						end if;
						if(rec <> old) 
							then raise exception 'Изменены запрещенные поля';
						end if;	
					ELSIF (TG_OP = 'INSERT') then
						if(exists (select * from yachtlease where yachtid = new.yachtid and enddate is null) ) then
							raise exception 'невозможно заключить новый контракт, если старый ещё не завершён';
						end if;
						new.enddate = null;
					ELSIF (TG_OP = 'DELETE') then 
						if(old.enddate is not null) then 
							raise exception 'Попытка удаления закрытого контракта на пирс';			
						end if;
						return old;
					END IF;

						if(new.startdate <= current_timestamp or new.startdate is null) then
							new.startdate = current_timestamp;
						end if;
						if(new.duration <= current_timestamp or new.duration is null) then
							new.duration = current_timestamp;
						end if;
						if(new.overallprice is null) then
							new.overallprice = (select ylt.price from yachtleasetype ylt where ylt.id = new.yachtleasetypeid) * 
								 abs( extract(day from new.startdate - new.duration ) );	
						end if;

					RETURN new;
			end;
			$$ language plpgsql;

			create trigger ReadonlyConstraint
			Before insert or update or delete on YachtLease
			for each row execute function YL1 ();
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
					 IF (TG_OP = 'INSERT') then
						 new.registrydate = current_timestamp;
					 END IF;
					 RETURN new;
			 end;
			 $$ language plpgsql;

			 create trigger ReadonlyConstraint
			 Before insert on Yacht
			 for each row execute function Y1 ();
							 ");
            #endregion

            #region Contract
            dbContext.Database.ExecuteSqlRaw(@"
			create or replace function C1()
             returns trigger
            as $$
            declare
            rec RECORD;
            begin

                 IF(TG_OP = 'UPDATE') THEN
						if(old.enddate is not null) then 
							raise exception 'Попытка изменения закрытого контракта';
						end if;
						rec := old;
						if(old.paid) then
								rec.startdate = new.startdate;
								rec.averallprice = new.averallprice;
								rec.paid = new.paid;
						end if;

						if(rec <> old) then 
							raise exception 'Изменены запрещённые поля';
						end if;

                 ELSIF(TG_OP = 'INSERT') then
				 		if( not ( 
							 select yp.positionname = 'Captain' from yacht_crew_position yp where yp.id = new.captaininyachtid
							   ) ) then
							   raise exception 'Невозможно создать новый контракт без капитана';
						end if;
						new.enddate = null;
                 ELSIF(TG_OP = 'DELETE') then
                     if (old.enddate is not null and old.paid) then
                          raise exception 'Попытка удаления закрытого контракта';
					 end if;
					 return old;
           		 END IF;
				 IF( not old.paid and new.paid) then
					    if( (select b.c from busyyacht b join yacht_crew yc on yc.yachtid = b.id 
							where yc.id = new.captaininyachtid
							) ) then
							raise exception 'Невозможно заключить новый контракт, если старый ещё не завершён';
						elsif( ( 
							select b.e from busyyacht b join yacht_crew yc on yc.yachtid = b.id 
							where yc.id = new.captaininyachtid
							   ) ) then
							   raise exception 'Невозможно заключить новый контракт, если яхта участвует в мероприятии';
						elsif( ( 
							select b.r from busyyacht b join yacht_crew yc on yc.yachtid = b.id 
							where yc.id = new.captaininyachtid
							   ) ) then
							   raise exception 'Невозможно заключить новый контракт, если яхта ремонтируется';
						elsif( ( 
							select not b.val from busyyacht b join yacht_crew yc on yc.yachtid = b.id 
							where yc.id = new.captaininyachtid
							   ) ) then
							   raise exception 'Невозможно заключить новый контракт, если яхта недействительна';
						elsif( ( 
							select not b.filled from busyyacht b join yacht_crew yc on yc.yachtid = b.id 
							where yc.id = new.captaininyachtid
							   ) ) then
							   raise exception 'Невозможно заключить новый контракт, если требуемый экипаж не собран';
						elsif( ( 
							select not y.rentable from yacht y join yacht_crew yc on yc.yachtid = y.id
							where yc.id = new.captaininyachtid
							   ) ) then
							   raise exception 'Невозможно заключить новый контракт, если яхта не арендуема';
						end if;
				 end if;
				 		if(new.specials is null) then
							new.specials = ' ';
						end if;
					    if(new.startdate <= current_timestamp or new.startdate is null) then
							new.startdate = current_timestamp;
						end if;
						if(new.duration <= current_timestamp or new.duration is null) then
							new.duration = current_timestamp;
						end if;
						if(new.averallprice is null ) then 
							new.averallprice = (select ct.price from contracttype ct where ct.id = new.contracttypeid) * 
							 abs( extract(day from new.startdate - new.duration ) + 1 );
						end if;
            RETURN new;
            end;
            $$ language plpgsql;

            create trigger ReadonlyConstraint
            Before insert or update or delete on Contract
            for each row execute function C1();
            ");
			#endregion

			#region Position_Yachttype
			//Триггер на добавление в таблицу допусков должностей.
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function PYT1 ()
	returns trigger
as $$
declare 
rec RECORD;
begin 	
		if( new.positionid not in (select id from position where crewposition) ) then
			raise exception 'Попытка добавить не морскую должность';
		end if;
	
		select yt.crewcapacity cap into rec from yachttype yt where yt.id = new.yachttypeid;		
		if( rec.cap - (
			select coalesce(sum(pyt.count),0) from position_yachttype pyt where pyt.yachttypeid = new.yachttypeid
		) - ( select coalesce( abs( new.count - sum(pyt.count)), new.count ) from position_yachttype pyt
			 where pyt.yachttypeid = new.yachttypeid and pyt.positionid = new.positionid)  < 0 ) then 
			 raise exception 'Данная тип яхты не поддерживает более % членов экипажа', rec.cap;
		end if;
		RETURN new;
end;
$$ language plpgsql;	

create trigger PersonnelChecker
Before insert or update on position_yachttype
for each row execute function PYT1 ();

				");


			//Триггер на добавление в PYT капитана
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function YT2 ()
	returns trigger
as $$
declare 
rec RECORD;
begin 	
		call AddCaptainToYachttype(new.ID);
		RETURN new;
end;
$$ language plpgsql;	

create trigger InsertCaptain
After insert on Yachttype
for each row execute function YT2 ();

				");
			#endregion


			//Аккаунты
			#region Accs
			dbContext.Database.ExecuteSqlRaw(@"
create or replace function POS1 ()
	returns trigger
as $$
declare 
rec RECORD;
rolename varchar;
begin 	

		IF(TG_OP = 'INSERT') then 
			rolename :=  MyRoleName(new.name); 
			execute 'SELECT count(rolname) FROM pg_roles WHERE rolname = ''' || rolename || ''';' into rec;
			if(rec.count = 0 ) then
			  execute 'call addnewrole_r('''|| rolename || ''', ''amy_user'' );';
			end if;
		end if;
		IF(TG_OP = 'UPDATE') then
			rolename :=  MyRoleName(old.name); 
			execute 'SELECT count(rolname) FROM pg_roles WHERE rolname = ''' || rolename || ''';' into rec;
			if(rec.count = 0 ) then
			   execute 'call addnewrole_r('''|| rolename || ''', ''amy_user'' );';
			end if;
			if(old.name <> new.name) then 
			 	execute 'call renamerole_r('''|| rolename || ''', '''|| MyRoleName(new.name) ||''' );';
			end if;
		end if;
		return new;
end;
$$ language plpgsql;	

create trigger InsertCaptain
After insert or update on Position
for each row execute function POS1 ();
				");

			dbContext.Database.ExecuteSqlRaw($@"
create or replace function SPosition1 ()
	returns trigger
as $$
declare 
usy RECORD;
rec RECORD;
username varchar;
begin 	
		IF(TG_OP = 'INSERT') then 
		
				select p.id, p.name, pe.email into usy from position p 
				join person pe on pe.id = new.staffid and p.id = new.positionid;
								
				username := LowerUserName(usy.name) || '_' || LowerUserName(usy.email);

				execute 'SELECT count(rolname) FROM pg_roles WHERE rolname = ''' || username || ''';' into rec;
		
			if(rec.count = 0 ) then
			  execute 'call addnewacc_r('''|| usy.email || ''', '|| usy.id ||', ''{Hash_Extension.StandartPassword}'' );';
			end if;
			
			return new;
		end if;
		IF(TG_OP = 'UPDATE') then
		
				select p.id, p.name, pe.email into usy from staff_position sp 
				join position p on p.id = sp.positionid 
				join person pe on pe.id = sp.staffid
				where sp.id = new.id;

				username := LowerUserName(usy.name) || '_' || LowerUserName(usy.email);

				execute 'SELECT count(rolname) FROM pg_roles WHERE rolname = '''
				|| username || ''';' into rec;
		
			if(rec.count = 0 and new.enddate is null and old.enddate is null  ) then
			    execute 'call addnewacc_r('''|| usy.email || ''', '|| usy.id ||', ''{Hash_Extension.StandartPassword}'' );';
			end if;
			if(new.enddate is not null) then 
				execute 'call removeexistingrole_r('''|| username ||''') ;';
			end if;
			return new;
		end if;
		IF(TG_OP = 'DELETE') then
				select p.id, p.name, pe.email into usy from staff_position sp 
				join position p on p.id = sp.positionid 
				join person pe on pe.id = sp.staffid
				where sp.id = old.id;

				username := LowerUserName(usy.name) || '_' || LowerUserName(usy.email);
				
				execute 'call removeexistingrole_r('''|| username ||''') ;';	
			return old;
		end if;
end;
$$ language plpgsql;

create trigger InsertAccount
Before insert or update or delete on Staff_Position
for each row execute function SPosition1 ();
				");
            #endregion

            #endregion

            #region SameTriggers
/*            void ParentRemoval(string Parent, string Child, string Property)
            {
				dbContext.Database.ExecuteSqlRaw($@"
		create or replace function {Parent}_{Child}()
			returns trigger
		as $$
		begin 	
				if(
					exists (select * from {Child} t0 join {Parent} t1 on t0.{Property} = t1.id where t1.id = old.id)
				) then
					raise exception 'Данный тип используется, поэтому удалить его нельзя';
				end if;
				return old;
		end;
		$$ language plpgsql;

		create trigger ParentRemover
		Before delete on {Parent}
		for each row execute function {Parent}_{Child}();
				");
			}
			 
			ParentRemoval(
				nameof(Contracttype),
				nameof(Contract),
				nameof(Contract.Contracttypeid)
				);
			ParentRemoval(
				nameof(Yachtleasetype),
				nameof(Yachtlease),
				nameof(Yachtlease.Yachtleasetypeid)
				);
			ParentRemoval(
				"Yacht_Crew",
				nameof(Contract),
				nameof(Contract.Captaininyachtid)
				);
			ParentRemoval(
				nameof(Yachttype),
				nameof(Yacht),
				nameof(Yacht.Typeid)
				);
			ParentRemoval(
				nameof(Position),
				"StaffPosition",
				nameof(StaffPosition.Positionid)
				);
			ParentRemoval(
				nameof(Material),
				nameof(Materiallease),
				nameof(Materiallease.Material)
				);
			ParentRemoval(
				nameof(Seller),
				nameof(Materiallease),
				nameof(Materiallease.Seller)
				);
			ParentRemoval(
				nameof(Materialtype),
				nameof(Material),
				nameof(Material.Typeid)
				);*/

			void DeclineDelete(string Parent)
			{
				dbContext.Database.ExecuteSqlRaw($@"
		create or replace function DeclineDelete_{Parent}()
			returns trigger
		as $$
		begin 	
				if( old.declinedelete ) then
					raise exception 'Запрещено удалять эту запись';
				end if;

				return old;
		end;
		$$ language plpgsql;

		create trigger DeclineDelete
		Before delete on {Parent}
		for each row execute function DeclineDelete_{Parent}();
				");
			}

			DeclineDelete(nameof(Person));
			//DeclineDelete(nameof(Position));

			dbContext.Yachttype.ToList().ForEach(
				p =>
                {
					dbContext.Database.ExecuteSqlRaw($"call AddCaptainToYachttype({p.Id});");
                }
				);
            #endregion
		}

		public static void SeedAccounts(DataContext dbContext)
        {
			var ps = Hash_Extension.StandartPassword;
			//Account персонала
			dbContext.Person.ToList().ForEach(p => {
			    dbContext.Database.ExecuteSqlRaw($"call removeallexistingroles('{p.Email}');");
				dbContext.Database.ExecuteSqlRaw($"call populateallvalidsp('{p.Email}','{ps}');");
			});

			

			#region GRANTS

			//Кладовщик
			dbContext.Database.ExecuteSqlRaw(@"
grant insert,update,delete,select on material, materiallease, materialtype, seller to my_storekeeper;
grant select on availableresources, repair, person, position, repair_staff to my_storekeeper;
grant select,update,delete on extradationrequest to my_storekeeper;

GRANT EXECUTE ON FUNCTION 
check_er_for_reps, leadrepairman, materialanalytics, materialmetric, 
staffpositionlistbyposition, staffpositionlistbypositionlist
TO my_storekeeper;

				");

			//Кадровик
			dbContext.Database.ExecuteSqlRaw(@"
grant insert,update,delete,select on staff, position, staff_position, yacht_crew, person to my_personell_officer;

grant select on yacht_crew, yacht, yachttype, busyyacht to my_personell_officer;

GRANT EXECUTE ON FUNCTION 
countactivecrew, spview, ycview
TO my_personell_officer;

GRANT EXECUTE ON PROCEDURE  addnewacc_r(_email varchar, _role int, _pass varchar),
addnewrole_r, removeexistingrole_r, renamerole_r TO my_personell_officer;
				");
			//Ремонтник
			dbContext.Database.ExecuteSqlRaw(@"
grant select on person, position, repair_staff, material,busyyacht, materialtype, availableresources to my_repairman;
grant insert,update,delete,select on yachttest, repair, repair_men, extradationrequest to my_repairman;

GRANT EXECUTE ON FUNCTION 
check_er_for_reps, hasrepairman, leadrepairman,
 materialmetric, staffpositionlistbypositionlist, repairview
TO my_repairman;

GRANT EXECUTE ON PROCEDURE populaterepair_men TO my_repairman;
				");
			//Владелец
			dbContext.Database.ExecuteSqlRaw(@"
grant insert, select, update, delete on yachttype, position_yachttype, yachtleasetype, contracttype, event, winner, yachtlease to my_owner;
grant select on yacht_crew, review, yacht_crew_position to my_owner;

GRANT EXECUTE ON FUNCTION 
contractanalytics, materialanalytics, materialmetric, yachtleaseanalytics, contractsearch, yachtleasesearch
TO my_owner;

GRANT EXECUTE ON PROCEDURE  addcaptaintoyachttype  TO my_owner;
				");	
			//Капитан
			dbContext.Database.ExecuteSqlRaw(@"
grant insert,update,delete,select on contract to my_captain;
grant select on yacht_crew_position to my_captain;

GRANT EXECUTE ON FUNCTION 
countactivecrew
TO my_captain;
				");
			//Стандартные типы данных
			dbContext.Database.ExecuteSqlRaw(@"
grant ALL PRIVILEGES on ALL SEQUENCES IN SCHEMA PUBLIC to amy_data_types;

grant select on material, materialtype, materiallease, extradationrequest to amy_materialist;
				");		
			//MyDefault
			dbContext.Database.ExecuteSqlRaw(@"
grant select on event, winner, yacht_crew, staff, staff_position, person, yacht, yachttype, yachtleasetype, contracttype , position, busyyacht to my_default;
grant insert on person to my_default;

GRANT EXECUTE ON PROCEDURE addnewacc_r(_email varchar,_pass varchar),
populateallvalidsp_r, removeallexistingroles_r, tryconnect, updateexistingroles_r TO my_default;

GRANT EXECUTE ON FUNCTION captainbyyachtid, 
curstmp, isinterm, isstaff, 
myrolename, rolebyname, yachtcrewbyevent, 
yachtsstatus TO my_default;
				");
			//Клиент
			dbContext.Database.ExecuteSqlRaw(@"
grant select on yachtlease, busyyacht, contract to my_client;
grant insert,update,delete on yacht to my_client;
grant insert, select, update on review to my_client;

GRANT EXECUTE ON FUNCTION yachttestinfo, repairinfo TO my_client;
				");

			#endregion
		}
    }
}
