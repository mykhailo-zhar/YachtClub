using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Project.Models;

namespace Project.Migrations
{
    public partial class DataContext : DbContext
    {
        private IHttpContextAccessor Context { get; }
        private HttpContext HttpContext => Context.HttpContext;
        private ClaimsPrincipal User => HttpContext.User;
        public DataContext(DbContextOptions<DataContext> options, IHttpContextAccessor context)
            : base(options)
        {
            Context = context;
        }
        #region DBSets       
        //public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Availableresources> Availableresources { get; set; }
        public virtual DbSet<Busyyacht> Busyyacht { get; set; }
        public virtual DbSet<Contract> Contract { get; set; }
        public virtual DbSet<Contracttype> Contracttype { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<Extradationrequest> Extradationrequest { get; set; }
        public virtual DbSet<Material> Material { get; set; }
        public virtual DbSet<Materiallease> Materiallease { get; set; }
        public virtual DbSet<Materialtype> Materialtype { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<Position> Position { get; set; }
        public virtual DbSet<PositionYachttype> PositionYachttype { get; set; }
        public virtual DbSet<Readytocontract> Readytocontract { get; set; }
        public virtual DbSet<Repair> Repair { get; set; }
        public virtual DbSet<RepairMen> RepairMen { get; set; }
        public virtual DbSet<RepairStaff> RepairStaff { get; set; }
        public virtual DbSet<Review> Review { get; set; }
        public virtual DbSet<ReviewCaptain> ReviewCaptain { get; set; }
        public virtual DbSet<ReviewYacht> ReviewYacht { get; set; }
        public virtual DbSet<Seller> Seller { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<StaffPosition> StaffPosition { get; set; }
        public virtual DbSet<Winner> Winner { get; set; }
        public virtual DbSet<Yacht> Yacht { get; set; }
        public virtual DbSet<YachtCrew> YachtCrew { get; set; }
        public virtual DbSet<YachtCrewPosition> YachtCrewPosition { get; set; }
        public virtual DbSet<Yachtincontract> Yachtincontract { get; set; }
        public virtual DbSet<Yachtinevent> Yachtinevent { get; set; }
        public virtual DbSet<Yachtinrepair> Yachtinrepair { get; set; }
        public virtual DbSet<Yachtlease> Yachtlease { get; set; }
        public virtual DbSet<Yachtleasestatus> Yachtleasestatus { get; set; }
        public virtual DbSet<Yachtleasetype> Yachtleasetype { get; set; }
        public virtual DbSet<Yachttest> Yachttest { get; set; }
        public virtual DbSet<Yachttype> Yachttype { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var Role = User.FindFirst(ClaimsIdentity.DefaultRoleClaimType)?.Value;

                string Password = User.FindFirst("Password")?.Value ?? "";

                if (Role == null)
                {
                    Role = RolesReadonly.Guest;
                    Password = "1111";
                }
                else
                {
                    Role = GetLowerName(Role) + "_" + GetLowerName(User.Identity.Name);
                }
                optionsBuilder.UseNpgsql($"Host=localhost;Port=5432;ConnectionIdleLifetime=30;Database=YachtClub;Username={Role};Password={Password}");
            }
        }

        #region Functions

        [DbFunction("materialmetric", "public")]
        public string MaterialMetric(int MaterialID) => throw new NotImplementedException();

        [DbFunction("yachtsstatus", "public")]
        public string YachtsStatus(int Yachtid) => throw new NotImplementedException();

        [DbFunction("leadrepairman", "public")]
        public bool LeadRepairMan(int RepID, int RepairManID) => throw new NotImplementedException();

        [DbFunction("isstaff", "public")]
        public bool IsStaff(int PersonId) => throw new NotImplementedException();
        [DbFunction("isinterm", "public")]
        public bool IsInTerm(DateTime Start1, DateTime Start2, DateTime End2, DateTime End1) => throw new NotImplementedException();

        [DbFunction("hasrepairman", "public")]
        public bool HasRepairMan(int RepID, int RepairManID) => throw new NotImplementedException();
        [DbFunction("countactivecrew", "public")]
        public int CountActiveCrew(int Yachtid) => throw new NotImplementedException();
        [DbFunction("rolebyname", "public")]
        public int CountRoleByName(string email) => throw new NotImplementedException();


        public IQueryable<YachtCrew> YachtCrewByEvent(int eventid) => YachtCrew.FromSqlRaw($"select * from  YachtCrewByEvent({eventid})");
        public IQueryable<YachtCrew> CaptainByYachtid(int yachtid) => YachtCrew.FromSqlRaw($"select * from  CaptainByYachtid({yachtid})");
        #region Поиск и аналитика

        public IQueryable<MaterialAnalyticsViewModel> MaterialAnalytics(
            string MaterialName,
            string TypeName,
            DateTime From,
            DateTime To) =>
    Set<MaterialAnalyticsViewModel>()
            .FromSqlRaw(
        $"select * from MaterialAnalytics(" +
        $"'{MaterialName}', " +
        $"'{TypeName}', " +
        $"'{From.Day}-{From.Month}-{From.Year}'," +
        $"'{To.Day}-{To.Month}-{To.Year}'" +
        $")"
        );

        public IQueryable<ContractAnalyticsViewModel> ContractAnalytics(
            string Name, 
            string Surname,
            string Phone,
            string Email,
            string YName,
            string YType,
            DateTime From, 
            DateTime To) =>
    Set<ContractAnalyticsViewModel>()
            .FromSqlRaw(
        $"select * from ContractAnalytics(" +
        $"'{Name}'," +
        $"'{Surname}'," +
        $"'{Phone}', " +
        $"'{Email}'," +
        $"'{YName}'," +
        $"'{YType}'," +
        $"'{From.Day}-{From.Month}-{From.Year}'," +
        $"'{To.Day}-{To.Month}-{To.Year}'" +
        $")");

        public IQueryable<ContractAnalyticsViewModel> YachtleaseAnalytics(
            string Name, 
            string Surname,
            string Phone,
            string Email,
            string YName,
            string YType,
            DateTime From, 
            DateTime To) =>
    Set<ContractAnalyticsViewModel>()
            .FromSqlRaw(
        $"select * from YachtleaseAnalytics(" +
        $"'{Name}'," +
        $"'{Surname}'," +
        $"'{Phone}', " +
        $"'{Email}'," +
        $"'{YName}'," +
        $"'{YType}'," +
        $"'{From.Day}-{From.Month}-{From.Year}'," +
        $"'{To.Day}-{To.Month}-{To.Year}'" +
        $")");

        public IQueryable<CrewSearchModel> CrewSearch(
            string Name, 
            string Surname,
            string Phone,
            string Email,
            string Position,
            string YName,
            string YType,
              bool Active) =>
    Set<CrewSearchModel>()
            .FromSqlRaw(
        $"select * from YCView(" +
        $"'{Name}'," +
        $"'{Surname}'," +
        $"'{Phone}', " +
        $"'{Email}'," +
        $"'{Position}'," +
        $"'{YName}'," +
        $"'{YType}'," +
        $" {Active}" +
        $")");

        public IQueryable<StaffSearchModel> StaffSearch(
            string Name, 
            string Surname,
            string Phone,
            string Email,
            string Position,
              bool Active) =>
    Set<StaffSearchModel>()
            .FromSqlRaw(
        $"select * from SPView(" +
        $"'{Name}'," +
        $"'{Surname}'," +
        $"'{Phone}', " +
        $"'{Email}'," +
        $"'{Position}'," +
        $" {Active}" +
        $")");
        public IQueryable<RepairSearchModel> RepairSearch(
            string Name, 
            string Surname,
            string Phone,
            string Email,
            string YName,
            string YType,
              bool Active) =>
    Set<RepairSearchModel>()
            .FromSqlRaw(
        $"select * from RepairView(" +
        $"'{Name}'," +
        $"'{Surname}'," +
        $"'{Phone}', " +
        $"'{Email}'," +
        $"'{YName}'," +
        $"'{YType}'," +
        $" {Active}" +
        $")");
        #endregion

        #endregion
        #region Procedures
        public void Ping(string login, string role, string password) => Database.ExecuteSqlInterpolated($"call tryconnect({login},{role},{password});");
        public void DeleteProfile() => Database.ExecuteSqlInterpolated($"call removeallexistingroles_r({User.Identity.Name});");

        #endregion

        public string GetMyRole(string Role) => "my_" + RegexExtension.ReplaceAll(Role, new Regex("[ !\"#$%&'()*+,./:;<=>?@\\^_`{|}~]"), "_").ToLower();

        public string GetLowerName(string Name) => RegexExtension.ReplaceAll(Name, new Regex("[ !\"#$%&'()*+,./:;<=>?@\\^_`{|}~]"), "_").ToLower();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction(() => MaterialMetric(default));
            modelBuilder.HasDbFunction(() => YachtsStatus(default));
            modelBuilder.HasDbFunction(() => IsStaff(default));
            /*            modelBuilder.HasDbFunction(() => GetMyRole(default));
                        modelBuilder.HasDbFunction(() => GetLowerName(default));*/
            modelBuilder.HasDbFunction(() => LeadRepairMan(default, default));
            modelBuilder.HasDbFunction(() => HasRepairMan(default, default));
            modelBuilder.HasDbFunction(() => IsInTerm(default, default, default, default));
            modelBuilder.HasDbFunction(() => CountActiveCrew(default));
            modelBuilder.HasDbFunction(() => CountRoleByName(default));
            //modelBuilder.HasDbFunction(() => YachtCrewByEvent(default));

            /* modelBuilder.Entity<Account>(entity => {
                 entity.HasIndex(e => e.Login)
                       .HasName("account_login_key")
                       .IsUnique();

                 entity.HasOne(d => d.User)
                      .WithMany(p => p.Account)
                      .HasForeignKey(d => d.Userid)
                      .HasConstraintName("account_userid_fkey");
             });*/

            modelBuilder.Entity<MaterialAnalyticsViewModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<ContractAnalyticsViewModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<StaffSearchModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<CrewSearchModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<RepairSearchModel>().HasNoKey().ToView(null);

            modelBuilder.Entity<Availableresources>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Busyyacht>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.HasOne(d => d.Captaininyacht)
                    .WithMany(p => p.Contract)
                    .HasForeignKey(d => d.Captaininyachtid)
                    .HasConstraintName("contract_captaininyachtid_fkey");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Contract)
                    .HasForeignKey(d => d.Clientid)
                    .HasConstraintName("contract_clientid_fkey");

                entity.HasOne(d => d.Contracttype)
                    .WithMany(p => p.Contract)
                    .HasForeignKey(d => d.Contracttypeid)
                    .HasConstraintName("contract_contracttypeid_fkey");
            });

            modelBuilder.Entity<Contracttype>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("contracttype_name_key")
                    .IsUnique();

                entity.Property(e => e.Description).HasDefaultValueSql("' '::text");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasIndex(e => new { e.Name, e.Startdate })
                    .HasName("event_name_startdate_key")
                    .IsUnique();

                entity.Property(e => e.Canhavewinners).HasDefaultValueSql("true");

                entity.Property(e => e.Description).HasDefaultValueSql("' '::text");

                entity.Property(e => e.Userrate).HasDefaultValueSql("0");
            });

            modelBuilder.Entity<Extradationrequest>(entity =>
            {
                entity.Property(e => e.Description).HasDefaultValueSql("' '::text");

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
                entity.HasIndex(e => e.Name)
                    .HasName("material_name_key")
                    .IsUnique();

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Material)
                    .HasForeignKey(d => d.Typeid)
                    .HasConstraintName("material_typeid_fkey");
            });

            modelBuilder.Entity<Materiallease>(entity =>
            {
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
                entity.HasIndex(e => e.Name)
                    .HasName("materialtype_name_key")
                    .IsUnique();

                entity.Property(e => e.Description).HasDefaultValueSql("' '::text");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("person_email_key")
                    .IsUnique();

                entity.HasIndex(e => e.Phone)
                    .HasName("person_phone_key")
                    .IsUnique();

                entity.Property(e => e.Registrydate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("position_name_key")
                    .IsUnique();
            });

            modelBuilder.Entity<PositionYachttype>(entity =>
            {
                entity.HasKey(e => new { e.Positionid, e.Yachttypeid })
                    .HasName("position_yachttype_pkey");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.PositionYachttype)
                    .HasForeignKey(d => d.Positionid)
                    .HasConstraintName("position_yachttype_positionid_fkey");

                entity.HasOne(d => d.Yachttype)
                    .WithMany(p => p.PositionYachttype)
                    .HasForeignKey(d => d.Yachttypeid)
                    .HasConstraintName("position_yachttype_yachttypeid_fkey");
            });

            modelBuilder.Entity<Readytocontract>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Repair>(entity =>
            {
                entity.Property(e => e.Description).HasDefaultValueSql("' '::text");

                entity.Property(e => e.Duration).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Enddate).HasDefaultValueSql("NULL::timestamp without time zone");

                entity.Property(e => e.Personnel).HasDefaultValueSql("1");

                entity.Property(e => e.Startdate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Status).HasDefaultValueSql("'Created'::character varying");

                entity.HasOne(d => d.Yacht)
                    .WithMany(p => p.Repair)
                    .HasForeignKey(d => d.Yachtid)
                    .HasConstraintName("repair_yachtid_fkey");
            });

            modelBuilder.Entity<RepairMen>(entity =>
            {
                entity.HasIndex(e => new { e.Repairid, e.Staffid })
                    .HasName("repair_men_repairid_staffid_key")
                    .IsUnique();

                entity.HasOne(d => d.Repair)
                    .WithMany(p => p.RepairMen)
                    .HasForeignKey(d => d.Repairid)
                    .HasConstraintName("repair_men_repairid_fkey");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.RepairMen)
                    .HasForeignKey(d => d.Staffid)
                    .HasConstraintName("repair_men_staffid_fkey");
            });

            modelBuilder.Entity<RepairStaff>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.Property(e => e.Public).HasDefaultValueSql("true");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Review)
                    .HasForeignKey(d => d.Clientid)
                    .HasConstraintName("review_clientid_fkey");
            });

            modelBuilder.Entity<ReviewCaptain>(entity =>
            {
                entity.HasKey(e => new { e.Reviewid, e.Captainid })
                    .HasName("review_captain_pkey");

                entity.HasOne(d => d.Captain)
                    .WithMany(p => p.ReviewCaptain)
                    .HasForeignKey(d => d.Captainid)
                    .HasConstraintName("review_captain_captainid_fkey");

                entity.HasOne(d => d.Review)
                    .WithMany(p => p.ReviewCaptain)
                    .HasForeignKey(d => d.Reviewid)
                    .HasConstraintName("review_captain_reviewid_fkey");
            });

            modelBuilder.Entity<ReviewYacht>(entity =>
            {
                entity.HasKey(e => new { e.Reviewid, e.Yachtid })
                    .HasName("review_yacht_pkey");

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
                entity.HasIndex(e => e.Name)
                    .HasName("seller_name_key")
                    .IsUnique();

                entity.Property(e => e.Description).HasDefaultValueSql("' '::text");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<StaffPosition>(entity =>
            {
                entity.Property(e => e.Description).HasDefaultValueSql("' '::text");

                entity.Property(e => e.Salary).HasDefaultValueSql("0");

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
                entity.HasIndex(e => new { e.Name, e.Typeid })
                    .HasName("yacht_name_typeid_key")
                    .IsUnique();

                entity.Property(e => e.Description).HasDefaultValueSql("' '::text");

                entity.Property(e => e.Registrydate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Rentable).HasDefaultValueSql("true");

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
                entity.Property(e => e.Description).HasDefaultValueSql("' '::text");

                entity.HasOne(d => d.Crew)
                    .WithMany(p => p.YachtCrew)
                    .HasForeignKey(d => d.Crewid)
                    .HasConstraintName("yacht_crew_crewid_fkey");

                entity.HasOne(d => d.Yacht)
                    .WithMany(p => p.YachtCrew)
                    .HasForeignKey(d => d.Yachtid)
                    .HasConstraintName("yacht_crew_yachtid_fkey");
            });

            modelBuilder.Entity<YachtCrewPosition>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Yachtincontract>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Yachtinevent>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Yachtinrepair>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Yachtlease>(entity =>
            {
                entity.Property(e => e.Specials).HasDefaultValueSql("' '::text");

                entity.HasOne(d => d.Yacht)
                    .WithMany(p => p.Yachtlease)
                    .HasForeignKey(d => d.Yachtid)
                    .HasConstraintName("yachtlease_yachtid_fkey");

                entity.HasOne(d => d.Yachtleasetype)
                    .WithMany(p => p.Yachtlease)
                    .HasForeignKey(d => d.Yachtleasetypeid)
                    .HasConstraintName("yachtlease_yachtleasetypeid_fkey");
            });

            modelBuilder.Entity<Yachtleasestatus>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Yachtleasetype>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("yachtleasetype_name_key")
                    .IsUnique();

                entity.Property(e => e.Description).HasDefaultValueSql("' '::text");
            });

            modelBuilder.Entity<Yachttest>(entity =>
            {
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
                entity.HasIndex(e => e.Name)
                    .HasName("yachttype_name_key")
                    .IsUnique();

                entity.Property(e => e.Description).HasDefaultValueSql("' '::text");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
