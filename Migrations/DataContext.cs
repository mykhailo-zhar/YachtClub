using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Project.Migrations
{
    public partial class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<Contract> Contract { get; set; }
        public virtual DbSet<Contracttype> Contracttype { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<Extradationrequest> Extradationrequest { get; set; }
        public virtual DbSet<Material> Material { get; set; }
        public virtual DbSet<Materiallease> Materiallease { get; set; }
        public virtual DbSet<Materialtype> Materialtype { get; set; }
        public virtual DbSet<Position> Position { get; set; }
        public virtual DbSet<PositionEquivalent> PositionEquivalent { get; set; }
        public virtual DbSet<PositionYachttype> PositionYachttype { get; set; }
        public virtual DbSet<Repair> Repair { get; set; }
        public virtual DbSet<RepairMen> RepairMen { get; set; }
        public virtual DbSet<Review> Review { get; set; }
        public virtual DbSet<ReviewCaptain> ReviewCaptain { get; set; }
        public virtual DbSet<ReviewContract> ReviewContract { get; set; }
        public virtual DbSet<ReviewYacht> ReviewYacht { get; set; }
        public virtual DbSet<Seller> Seller { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<StaffPosition> StaffPosition { get; set; }
        public virtual DbSet<Winner> Winner { get; set; }
        public virtual DbSet<Yacht> Yacht { get; set; }
        public virtual DbSet<YachtCrew> YachtCrew { get; set; }
        public virtual DbSet<Yachtlease> Yachtlease { get; set; }
        public virtual DbSet<Yachtleasetype> Yachtleasetype { get; set; }
        public virtual DbSet<Yachttest> Yachttest { get; set; }
        public virtual DbSet<Yachttype> Yachttype { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=YachtClub;Username=postgres;Password=111");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("client");

                entity.HasIndex(e => e.Email)
                    .HasName("client_email_key")
                    .IsUnique();

                entity.HasIndex(e => e.Phone)
                    .HasName("client_phone_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Birthdate)
                    .HasColumnName("birthdate")
                    .HasColumnType("date");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasColumnType("character varying");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnName("phone")
                    .HasColumnType("character varying");

                entity.Property(e => e.Sex)
                    .IsRequired()
                    .HasColumnName("sex")
                    .HasColumnType("character varying");

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasColumnName("surname")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.ToTable("contract");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Clientid).HasColumnName("clientid");

                entity.Property(e => e.Contracttypeid).HasColumnName("contracttypeid");

                entity.Property(e => e.Duration)
                    .HasColumnName("duration")
                    .HasColumnType("date");

                entity.Property(e => e.Enddate)
                    .HasColumnName("enddate")
                    .HasColumnType("date");

                entity.Property(e => e.Overallprice)
                    .HasColumnName("overallprice")
                    .HasColumnType("numeric");

                entity.Property(e => e.Specials)
                    .IsRequired()
                    .HasColumnName("specials");

                entity.Property(e => e.Startdate)
                    .HasColumnName("startdate")
                    .HasColumnType("date");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasColumnType("character varying");

                entity.Property(e => e.Yachtwithcrewid).HasColumnName("yachtwithcrewid");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Contract)
                    .HasForeignKey(d => d.Clientid)
                    .HasConstraintName("contract_clientid_fkey");

                entity.HasOne(d => d.Contracttype)
                    .WithMany(p => p.Contract)
                    .HasForeignKey(d => d.Contracttypeid)
                    .HasConstraintName("contract_contracttypeid_fkey");

                entity.HasOne(d => d.Yachtwithcrew)
                    .WithMany(p => p.Contract)
                    .HasForeignKey(d => d.Yachtwithcrewid)
                    .HasConstraintName("contract_yachtwithcrewid_fkey");
            });

            modelBuilder.Entity<Contracttype>(entity =>
            {
                entity.ToTable("contracttype");

                entity.HasIndex(e => e.Name)
                    .HasName("contracttype_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("numeric");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("event");

                entity.HasIndex(e => new { e.Name, e.Startdate })
                    .HasName("event_name_startdate_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Enddate)
                    .HasColumnName("enddate")
                    .HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Startdate)
                    .HasColumnName("startdate")
                    .HasColumnType("date");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<Extradationrequest>(entity =>
            {
                entity.ToTable("extradationrequest");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.Duration)
                    .HasColumnName("duration")
                    .HasColumnType("date");

                entity.Property(e => e.Enddate)
                    .HasColumnName("enddate")
                    .HasColumnType("date");

                entity.Property(e => e.Material).HasColumnName("material");

                entity.Property(e => e.Repairid).HasColumnName("repairid");

                entity.Property(e => e.Staffid).HasColumnName("staffid");

                entity.Property(e => e.Startdate)
                    .HasColumnName("startdate")
                    .HasColumnType("date");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasColumnType("character varying");

                entity.HasOne(d => d.MaterialNavigation)
                    .WithMany(p => p.Extradationrequest)
                    .HasForeignKey(d => d.Material)
                    .HasConstraintName("extradationrequest_material_fkey");

                entity.HasOne(d => d.Repair)
                    .WithMany(p => p.Extradationrequest)
                    .HasForeignKey(d => d.Repairid)
                    .HasConstraintName("extradationrequest_repairid_fkey");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.Extradationrequest)
                    .HasForeignKey(d => d.Staffid)
                    .HasConstraintName("extradationrequest_staffid_fkey");
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.ToTable("material");

                entity.HasIndex(e => e.Name)
                    .HasName("material_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Typeid).HasColumnName("typeid");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Material)
                    .HasForeignKey(d => d.Typeid)
                    .HasConstraintName("material_typeid_fkey");
            });

            modelBuilder.Entity<Materiallease>(entity =>
            {
                entity.ToTable("materiallease");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.Deliverydate)
                    .HasColumnName("deliverydate")
                    .HasColumnType("date");

                entity.Property(e => e.Material).HasColumnName("material");

                entity.Property(e => e.Overallprice)
                    .HasColumnName("overallprice")
                    .HasColumnType("numeric");

                entity.Property(e => e.Priceperunit)
                    .HasColumnName("priceperunit")
                    .HasColumnType("numeric");

                entity.Property(e => e.Seller).HasColumnName("seller");

                entity.Property(e => e.Startdate)
                    .HasColumnName("startdate")
                    .HasColumnType("date");

                entity.HasOne(d => d.MaterialNavigation)
                    .WithMany(p => p.Materiallease)
                    .HasForeignKey(d => d.Material)
                    .HasConstraintName("materiallease_material_fkey");

                entity.HasOne(d => d.SellerNavigation)
                    .WithMany(p => p.Materiallease)
                    .HasForeignKey(d => d.Seller)
                    .HasConstraintName("materiallease_seller_fkey");
            });

            modelBuilder.Entity<Materialtype>(entity =>
            {
                entity.ToTable("materialtype");

                entity.HasIndex(e => e.Name)
                    .HasName("materialtype_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.ToTable("position");

                entity.HasIndex(e => e.Name)
                    .HasName("position_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Salary)
                    .HasColumnName("salary")
                    .HasColumnType("numeric");
            });

            modelBuilder.Entity<PositionEquivalent>(entity =>
            {
                entity.HasKey(e => new { e.Positionid, e.Positionequivalentid })
                    .HasName("position_equivalent_pkey");

                entity.ToTable("position_equivalent");

                entity.Property(e => e.Positionid).HasColumnName("positionid");

                entity.Property(e => e.Positionequivalentid).HasColumnName("positionequivalentid");

                entity.HasOne(d => d.Positionequivalent)
                    .WithMany(p => p.PositionEquivalentPositionequivalent)
                    .HasForeignKey(d => d.Positionequivalentid)
                    .HasConstraintName("position_equivalent_positionequivalentid_fkey");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.PositionEquivalentPosition)
                    .HasForeignKey(d => d.Positionid)
                    .HasConstraintName("position_equivalent_positionid_fkey");
            });

            modelBuilder.Entity<PositionYachttype>(entity =>
            {
                entity.HasKey(e => new { e.Positionid, e.Yachttypeid })
                    .HasName("position_yachttype_pkey");

                entity.ToTable("position_yachttype");

                entity.Property(e => e.Positionid).HasColumnName("positionid");

                entity.Property(e => e.Yachttypeid).HasColumnName("yachttypeid");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.PositionYachttype)
                    .HasForeignKey(d => d.Positionid)
                    .HasConstraintName("position_yachttype_positionid_fkey");

                entity.HasOne(d => d.Yachttype)
                    .WithMany(p => p.PositionYachttype)
                    .HasForeignKey(d => d.Yachttypeid)
                    .HasConstraintName("position_yachttype_yachttypeid_fkey");
            });

            modelBuilder.Entity<Repair>(entity =>
            {
                entity.ToTable("repair");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Duration)
                    .HasColumnName("duration")
                    .HasColumnType("date");

                entity.Property(e => e.Enddate)
                    .HasColumnName("enddate")
                    .HasColumnType("date");

                entity.Property(e => e.Personnel).HasColumnName("personnel");

                entity.Property(e => e.Startdate)
                    .HasColumnName("startdate")
                    .HasColumnType("date");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasColumnType("character varying");

                entity.Property(e => e.Yachtid).HasColumnName("yachtid");

                entity.HasOne(d => d.Yacht)
                    .WithMany(p => p.Repair)
                    .HasForeignKey(d => d.Yachtid)
                    .HasConstraintName("repair_yachtid_fkey");
            });

            modelBuilder.Entity<RepairMen>(entity =>
            {
                entity.ToTable("repair_men");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Repairid).HasColumnName("repairid");

                entity.Property(e => e.Staffid).HasColumnName("staffid");

                entity.HasOne(d => d.Repair)
                    .WithMany(p => p.RepairMen)
                    .HasForeignKey(d => d.Repairid)
                    .HasConstraintName("repair_men_repairid_fkey");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.RepairMen)
                    .HasForeignKey(d => d.Staffid)
                    .HasConstraintName("repair_men_staffid_fkey");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("review");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Clientid).HasColumnName("clientid");

                entity.Property(e => e.Contractid).HasColumnName("contractid");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.Rate).HasColumnName("rate");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnName("text");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Review)
                    .HasForeignKey(d => d.Clientid)
                    .HasConstraintName("review_clientid_fkey");

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.Review)
                    .HasForeignKey(d => d.Contractid)
                    .HasConstraintName("review_contractid_fkey");
            });

            modelBuilder.Entity<ReviewCaptain>(entity =>
            {
                entity.HasKey(e => new { e.Reviewid, e.Captainid })
                    .HasName("review_captain_pkey");

                entity.ToTable("review_captain");

                entity.Property(e => e.Reviewid).HasColumnName("reviewid");

                entity.Property(e => e.Captainid).HasColumnName("captainid");

                entity.HasOne(d => d.Captain)
                    .WithMany(p => p.ReviewCaptain)
                    .HasForeignKey(d => d.Captainid)
                    .HasConstraintName("review_captain_captainid_fkey");

                entity.HasOne(d => d.Review)
                    .WithMany(p => p.ReviewCaptain)
                    .HasForeignKey(d => d.Reviewid)
                    .HasConstraintName("review_captain_reviewid_fkey");
            });

            modelBuilder.Entity<ReviewContract>(entity =>
            {
                entity.HasKey(e => new { e.Reviewid, e.Contractid })
                    .HasName("review_contract_pkey");

                entity.ToTable("review_contract");

                entity.Property(e => e.Reviewid).HasColumnName("reviewid");

                entity.Property(e => e.Contractid).HasColumnName("contractid");

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.ReviewContract)
                    .HasForeignKey(d => d.Contractid)
                    .HasConstraintName("review_contract_contractid_fkey");

                entity.HasOne(d => d.Review)
                    .WithMany(p => p.ReviewContract)
                    .HasForeignKey(d => d.Reviewid)
                    .HasConstraintName("review_contract_reviewid_fkey");
            });

            modelBuilder.Entity<ReviewYacht>(entity =>
            {
                entity.HasKey(e => new { e.Reviewid, e.Yachtid })
                    .HasName("review_yacht_pkey");

                entity.ToTable("review_yacht");

                entity.Property(e => e.Reviewid).HasColumnName("reviewid");

                entity.Property(e => e.Yachtid).HasColumnName("yachtid");

                entity.HasOne(d => d.Review)
                    .WithMany(p => p.ReviewYacht)
                    .HasForeignKey(d => d.Reviewid)
                    .HasConstraintName("review_yacht_reviewid_fkey");

                entity.HasOne(d => d.Yacht)
                    .WithMany(p => p.ReviewYacht)
                    .HasForeignKey(d => d.Yachtid)
                    .HasConstraintName("review_yacht_yachtid_fkey");
            });

            modelBuilder.Entity<Seller>(entity =>
            {
                entity.ToTable("seller");

                entity.HasIndex(e => e.Name)
                    .HasName("seller_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.ToTable("staff");

                entity.HasIndex(e => e.Email)
                    .HasName("staff_email_key")
                    .IsUnique();

                entity.HasIndex(e => e.Phone)
                    .HasName("staff_phone_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Birthdate)
                    .HasColumnName("birthdate")
                    .HasColumnType("date");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasColumnType("character varying");

                entity.Property(e => e.Hiringdate)
                    .HasColumnName("hiringdate")
                    .HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnName("phone")
                    .HasColumnType("character varying");

                entity.Property(e => e.Sex)
                    .IsRequired()
                    .HasColumnName("sex")
                    .HasColumnType("character varying");

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasColumnName("surname")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<StaffPosition>(entity =>
            {
                entity.ToTable("staff_position");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Enddate)
                    .HasColumnName("enddate")
                    .HasColumnType("date");

                entity.Property(e => e.Positionid).HasColumnName("positionid");

                entity.Property(e => e.Staffid).HasColumnName("staffid");

                entity.Property(e => e.Startdate)
                    .HasColumnName("startdate")
                    .HasColumnType("date");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.StaffPosition)
                    .HasForeignKey(d => d.Positionid)
                    .HasConstraintName("staff_position_positionid_fkey");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.StaffPosition)
                    .HasForeignKey(d => d.Staffid)
                    .HasConstraintName("staff_position_staffid_fkey");
            });

            modelBuilder.Entity<Winner>(entity =>
            {
                entity.HasKey(e => new { e.Eventid, e.Yachtid })
                    .HasName("winner_pkey");

                entity.ToTable("winner");

                entity.Property(e => e.Eventid).HasColumnName("eventid");

                entity.Property(e => e.Yachtid).HasColumnName("yachtid");

                entity.Property(e => e.Place).HasColumnName("place");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Winner)
                    .HasForeignKey(d => d.Eventid)
                    .HasConstraintName("winner_eventid_fkey");

                entity.HasOne(d => d.Yacht)
                    .WithMany(p => p.Winner)
                    .HasForeignKey(d => d.Yachtid)
                    .HasConstraintName("winner_yachtid_fkey");
            });

            modelBuilder.Entity<Yacht>(entity =>
            {
                entity.ToTable("yacht");

                entity.HasIndex(e => new { e.Name, e.Typeid })
                    .HasName("yacht_name_typeid_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Rentable).HasColumnName("rentable");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasColumnType("character varying");

                entity.Property(e => e.Typeid).HasColumnName("typeid");

                entity.Property(e => e.Yachtownerid).HasColumnName("yachtownerid");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Yacht)
                    .HasForeignKey(d => d.Typeid)
                    .HasConstraintName("yacht_typeid_fkey");

                entity.HasOne(d => d.Yachtowner)
                    .WithMany(p => p.Yacht)
                    .HasForeignKey(d => d.Yachtownerid)
                    .HasConstraintName("yacht_yachtownerid_fkey");
            });

            modelBuilder.Entity<YachtCrew>(entity =>
            {
                entity.ToTable("yacht_crew");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Crewid).HasColumnName("crewid");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Enddate)
                    .HasColumnName("enddate")
                    .HasColumnType("date");

                entity.Property(e => e.Startdate)
                    .HasColumnName("startdate")
                    .HasColumnType("date");

                entity.Property(e => e.Yachtid).HasColumnName("yachtid");

                entity.HasOne(d => d.Crew)
                    .WithMany(p => p.YachtCrew)
                    .HasForeignKey(d => d.Crewid)
                    .HasConstraintName("yacht_crew_crewid_fkey");

                entity.HasOne(d => d.Yacht)
                    .WithMany(p => p.YachtCrew)
                    .HasForeignKey(d => d.Yachtid)
                    .HasConstraintName("yacht_crew_yachtid_fkey");
            });

            modelBuilder.Entity<Yachtlease>(entity =>
            {
                entity.ToTable("yachtlease");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Duration)
                    .HasColumnName("duration")
                    .HasColumnType("date");

                entity.Property(e => e.Enddate)
                    .HasColumnName("enddate")
                    .HasColumnType("date");

                entity.Property(e => e.Overallprice)
                    .HasColumnName("overallprice")
                    .HasColumnType("numeric");

                entity.Property(e => e.Startdate)
                    .HasColumnName("startdate")
                    .HasColumnType("date");

                entity.Property(e => e.Yachtid).HasColumnName("yachtid");

                entity.Property(e => e.Yachtleasetypeid).HasColumnName("yachtleasetypeid");

                entity.HasOne(d => d.Yacht)
                    .WithMany(p => p.Yachtlease)
                    .HasForeignKey(d => d.Yachtid)
                    .HasConstraintName("yachtlease_yachtid_fkey");

                entity.HasOne(d => d.Yachtleasetype)
                    .WithMany(p => p.Yachtlease)
                    .HasForeignKey(d => d.Yachtleasetypeid)
                    .HasConstraintName("yachtlease_yachtleasetypeid_fkey");
            });

            modelBuilder.Entity<Yachtleasetype>(entity =>
            {
                entity.ToTable("yachtleasetype");

                entity.HasIndex(e => e.Name)
                    .HasName("yachtleasetype_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("numeric");
            });

            modelBuilder.Entity<Yachttest>(entity =>
            {
                entity.ToTable("yachttest");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.Results)
                    .IsRequired()
                    .HasColumnName("results");

                entity.Property(e => e.Staffid).HasColumnName("staffid");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasColumnType("character varying");

                entity.Property(e => e.Yachtid).HasColumnName("yachtid");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.Yachttest)
                    .HasForeignKey(d => d.Staffid)
                    .HasConstraintName("yachttest_staffid_fkey");

                entity.HasOne(d => d.Yacht)
                    .WithMany(p => p.Yachttest)
                    .HasForeignKey(d => d.Yachtid)
                    .HasConstraintName("yachttest_yachtid_fkey");
            });

            modelBuilder.Entity<Yachttype>(entity =>
            {
                entity.ToTable("yachttype");

                entity.HasIndex(e => e.Name)
                    .HasName("yachttype_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Capacity).HasColumnName("capacity");

                entity.Property(e => e.Class)
                    .IsRequired()
                    .HasColumnName("class")
                    .HasColumnType("character varying");

                entity.Property(e => e.Crewcapacity).HasColumnName("crewcapacity");

                entity.Property(e => e.Frame)
                    .IsRequired()
                    .HasColumnName("frame")
                    .HasColumnType("character varying");

                entity.Property(e => e.Goal)
                    .IsRequired()
                    .HasColumnName("goal")
                    .HasColumnType("character varying");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Sails).HasColumnName("sails");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
