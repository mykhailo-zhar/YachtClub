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
				//Удаление таблиц
				dbContext.Database.ExecuteSqlRaw(@"
drop table if exists 
  	Position, YachtType, ContractType, MaterialType, YachtLeaseType,
  	Staff, Event, Client, Seller,
	Material, Yacht,
	MaterialLease, YachtTest, Repair, ExtradationRequest, YachtLease,
	Contract, Review,
	Winner, Staff_Position, Yacht_Crew, Repair_Men, Review_Contract, Review_Yacht, Review_Captain, Position_YachtType, Position_Equivalent
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
                //Персонал
                dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Staff (
	ID     			serial     	NOT Null	Primary Key,
	Name     		varchar   	NOT Null,
	Surname     	varchar   	NOT Null,
	BirthDate    	date    	NOT Null	check(extract( year from age( cast(BirthDate  as date))) >= 18),
	Sex      	 	SEX     	NOT Null,  
	Email			Mail		NOT Null	unique,
	Phone			PhoneNumber	Not Null	unique,
	HiringDate		timestamp   Not Null	check(BirthDate < HiringDate) DEFAULT CURRENT_TIMESTAMP
);
				");

				//Клиент
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Client(
  	ID    			serial    	Not Null	Primary Key,
  	Name      		varchar    	Not Null,
  	Surname      	varchar    	Not Null,
  	BirthDate    	date    	Not Null,
	Sex				Sex			Not Null,	
  	Email      		Mail    	Not Null	unique,
	Phone			PhoneNumber	Not Null	unique
);
				");

				//Событие
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Event(
	ID			serial		Not Null	Primary Key,
	Name		varchar		Not Null,
	StartDate	date		Not Null,
	EndDate		date		check(StartDate <= EndDate ),
	Duration	date		Not Null	check(StartDate <= Duration),
	Status		varchar		Not Null	Default 'Created',

	unique(Name,StartDate)
);
				");

                /*
				YachtOwner - это тоже клиент, только у которого есть яхты
				CREATE TABLE YachtOwner(
					ID			serial		Not Null	Primary Key,
					Name		varchar		Not Null,
					Surname		varchar		Not Null,
					BirthDate	date		Not Null,
					Email		Mail		Not Null	unique,
					Phone		PhoneNumber	Not Null	unique,
					Sex			Sex			Not Null
				);
				*/
                #endregion

                #region Блок зависимых таблиц

                #region Поколение 1
                //Человек на должности
                dbContext.Database.ExecuteSqlRaw(@"
Create TABLE Staff_Position(
	ID				serial		Not Null	Primary Key,
	StaffID			int			Not Null
	References 	Staff(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	PositionID		int			Not Null
	References 	Position(ID)	
	On Update Cascade	
	On Delete Cascade,
	StartDate		date		Not Null,
	EndDate			date		check(StartDate <= EndDate),
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
	ID				Serial		Not Null	Primary Key,
	Name			varchar		Not Null,
	Status			varchar		Not Null,
	Rentable		bool		Not Null	Default TRUE,
	TypeID			int			Not Null
	References 	YachtType(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	YachtOwnerID	int			Not Null
	References 	Client(ID)	
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
	StartDate				date		Not Null,
	DeliveryDate			date		check(StartDate <= DeliveryDate)
);
				");

				//Тестовый заплыв яхты
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE YachtTest(
	ID			serial		Not Null	Primary Key,
	Date		date		Not Null,
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
	ID			serial		Not Null	Primary Key,
	StartDate	date		Not Null,
	EndDate		date		check(StartDate <= EndDate ),
	Duration	date		Not Null	check(StartDate <= Duration),
	Status		varchar		Not Null	Default 'New',
	Personnel	int			Not Null	check(Personnel > 0)	Default 1,
	YachtID		int			Not Null
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
	
	StartDate	date		Not Null,
	EndDate		date		check(StartDate <= EndDate),
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
	
	StartDate	date		Not Null,
	EndDate		date		check(StartDate <= EndDate),
	Duration	date		Not Null	check(StartDate <= Duration),
	Status		varchar		Not Null
);
				");
				
				//Договор на аренду места
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE YachtLease(
	ID				serial		Not Null	Primary Key,
	StartDate		date		Not Null,
	EndDate			date		check(StartDate <= EndDate ),
	Duration		date		Not Null	check(StartDate <= Duration),
	OverallPrice	My_Money	Not Null,
	YachtID			int			Not Null
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
	References 	Client(ID)	
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

	StartDate		date		Not Null,
	EndDate			date		check(StartDate <= EndDate ),
	Duration		date		Not Null	check(StartDate <= Duration),
	Specials		text		Not Null,
	Status			varchar		Not Null,
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
	References 	Client(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	ContractID		int			Not Null
	References 	Contract(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	Date			date		Not Null,
	Text			text		Not Null,
	Rate			int 		Not Null 	check(Rate > 0 AND Rate <= 5)
);

				");


				#endregion

				#endregion

				#region Справочные таблицы
				//Справочная таблица ремонтников
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Repair_Men(
	RepairID	int			Not Null
	References 	Repair(ID)	
	On Update Cascade	
	On Delete Cascade,
	
	StaffID		int			Not Null	
	References 	Staff_Position(ID)	
	On Update Cascade	
	On Delete Cascade,
	Primary Key(RepairID, StaffID)
);
				");

/*				//Обзоры на контракт
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Review_Contract(
	ReviewID	int			Not Null
	References 	Review(ID)	
	On Update Cascade	
	On Delete Cascade,

	ContractID	int			Not Null
	References 	Contract(ID)	
	On Update Cascade	
	On Delete Cascade,

	Primary Key ( ReviewID, ContractID )
);
				");*/

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
	PositionID	int			Not Null
	References 	Position(ID)	
	On Update Cascade	
	On Delete Cascade,

	YachtTypeID	int				Not Null
	References 	YachtType(ID)	
	On Update Cascade	
	On Delete Cascade,

	Primary Key ( PositionID, YachtTypeID )
);
				");
				
				//Эквиваленты должностей
				dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE Position_Equivalent(
	PositionID				int				Not Null
	References 	Position(ID)	
	On Update Cascade	
	On Delete Cascade,

	PositionEquivalentID	int				Not Null
	References 	Position(ID)	
	On Update Cascade	
	On Delete Cascade,

	Primary Key ( PositionID, PositionEquivalentID )
);
				");


				#endregion

				#endregion
			}

			#endregion
		}
		public static void SeedWithData(DataContext dbContext)
        {
			dbContext.Database.EnsureCreated();
			if (dbContext.Position.Count() == 0)
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
                               ('Shipboy', 				500.0),
                               ('Hired Captain',			0.0),
                ('Hired Boatswain',			2500.0),
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
insert into staff (name, surname, sex, BirthDate, HiringDate, email, phone)
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
				");

				//Покупатели
				dbContext.Database.ExecuteSqlRaw($@"
insert into client(name, surname, sex, birthdate, email, phone)
values
('Yachtclub', 	'',				'Other', 	'07-01-2019',		'yacht_club@gmail.com',  '+380983334590'	),
('Alexei', 		'Britov', 		'Male', 	'02-03-1947', 		'a_brit@gmail.com',		 '+380986769990'	),
('Melnik', 		'Baranov', 		'Male', 	'05-04-1967', 		'mebar@mail.ru',		 '+380986888990'	),
('Dmitriy', 	'Bideshev', 	'Male', 	'15-01-1989', 		'biDeshev777@gmail.com', '+380986868990'	),
('Gomel', 		'Bogdanov', 	'Male', 	'16-10-1963', 		'kosolov@gmail.com',	 '+380986765560'	),
('Jamala', 		'Nebinarna', 	'Female', 	'23-09-1954', 		'j_4354@gmail.com',		 '+380986768991'	),
('Tatiana'	, 	'Sparklovna', 	'Female', 	'07-01-2001', 		't.sparkle@gmail.com',	 '+380982334566'	),
('Christina', 	'Hrizaleva',  	'Female',	'08-05-2001',	 	'chrysalis@gmail.com',	 '+380986766778'	),
('Dimetrius', 	'Zhbanov',  	'Male',		'30-11-2000',	 	'd-zhb11@gmail.com',	 '+380943222990'	);
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

Update event set status='Ended'
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
('Alpha',			'Online',		1, 7),
('Storm',			'Canceled',		1, 1),
('Adelaida',		'Online',		4, 1),
('Latnyk',			'Online',		5, 8),
('Storm',			'Online',		7, 9),
('Infernal Rage',	'Online',		4, 1),
('Hello Kitty',		'Online',		5, 9),
('Beda',			'Online',		4, 1),
('Moby Dick',		'Online',		1, 8)
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
('17-01-2019', '19-04-2019',	'19-04-2019',	'Cancel',	3, 2),
('19-04-2019', '25-04-2019',	'25-04-2019',	'Cancel',	3, 2),
('21-06-2019', '24-07-2019',	'24-07-2019',	'Ended',	3, 3),
('21-06-2019', '23-07-2019',	'23-07-2019',	'Ended',	3, 5),
('12-10-2021', null,	'14-01-2022',	'Waiting for materials',	3,	3),
('10-10-2021', null,	'14-01-2022',	'Waiting for materials',	3,	6)
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
('01-07-2019', '20-07-2019', '02-07-2019',  'Done', 2, 3, 16, 3),
('24-06-2019', '10-07-2019', '24-07-2019',  'Done', 10, 4, 4, 4),
('12-10-2021',		   null, '12-12-2021',  'Awaits', 1, 10, 16, 5)
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
(1, 4),
(2, 4),
(2, 16),
(3, 16),
(4, 4),
(4, 16),
(5, 16);
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
('10-02-2019','11-02-2019', '11-02-2019', 'Done', 'No specials', 1500.0, 1, 2, 3),
('17-02-2019','17-02-2019', '17-02-2019', 'Done','No specials', 400.0, 2, 1, 4),
('18-02-2019','18-03-2019', '18-03-2019', 'Done','Long journey', 120000.0, 3, 3, 5),
('13-02-2019', '17-02-2019', '17-02-2019', 'Done', 'No specials', 8000.0, 4, 4, 4 ),
('25-04-2019', '21-06-2019', '21-06-2019', 'Done', 'Long journey', 400000.0, 3, 6, 5),
('07-01-2020', '21-02-2020', '21-02-2020', 'Done', 'Long journey', 350000.0, 3, 6, 5),
('01-07-2020', '05-08-2020', '05-08-2020', 'Done', 'Long journey', 200000.0, 3, 5, 5),
('09-05-2021', '09-05-2021', '09-05-2021', 'Done', 'No specials', 1000.0, 4, 7, 5),
('09-10-2021', null, '10-10-2022', 'Done', 'Long journey', 5000, 3, 4, 3),
('25-12-2021', null, '10-10-2022', 'Done', 'No specials', 2000, 4, 7, 4),
('27-12-2021', null, '10-10-2022', 'Done', 'No specials', 5000, 5, 3, 5)
;
				");

				#endregion

				#region Четвёртое поколение

				//Отзывы
				dbContext.Database.ExecuteSqlRaw($@"
insert into Review(ClientId, ContractID, date, text, rate)
values
( 4, 4, '25-02-2019', 'Некогда больше не поеду на этой яхте', 1 ) ,
( 3, 6, '25-02-2019', 'Превосходный капитан', 5 ) ,
( 4, 8, '19-05-2021', 'Как же хорошо', 5 ) 
;
				");

				#endregion
				#endregion

			}
        }
    }
}
