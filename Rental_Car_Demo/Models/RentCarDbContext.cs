using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Rental_Car_Demo.Models;

public partial class RentCarDbContext : DbContext
{
    public RentCarDbContext()
    {
    }

    public RentCarDbContext(DbContextOptions<RentCarDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdditionalFunction> AdditionalFunctions { get; set; }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<CarBrand> CarBrands { get; set; }

    public virtual DbSet<CarColor> CarColors { get; set; }

    public virtual DbSet<CarDocument> CarDocuments { get; set; }

    public virtual DbSet<CarModel> CarModels { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<TermOfUse> TermOfUses { get; set; }

    public virtual DbSet<TokenInfor> TokenInfors { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Ward> Wards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=(local);database=RentCarDB;uid=sa;pwd=123;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdditionalFunction>(entity =>
        {
            entity.HasKey(e => e.FucntionId).HasName("PK__Addition__1A8C7CAFFB540B6A");

            entity.Property(e => e.FucntionId).HasColumnName("fucntionId");
            entity.Property(e => e.Bluetooth).HasColumnName("bluetooth");
            entity.Property(e => e.Camera).HasColumnName("camera");
            entity.Property(e => e.ChildLock).HasColumnName("childLock");
            entity.Property(e => e.ChildSeat).HasColumnName("childSeat");
            entity.Property(e => e.Dvd).HasColumnName("dvd");
            entity.Property(e => e.Gps).HasColumnName("gps");
            entity.Property(e => e.SunRoof).HasColumnName("sunRoof");
            entity.Property(e => e.Usb).HasColumnName("usb");
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__Address__26A111AD0792DC70");

            entity.ToTable("Address");

            entity.Property(e => e.AddressId).HasColumnName("addressId");
            entity.Property(e => e.CityId).HasColumnName("cityId");
            entity.Property(e => e.DistrictId).HasColumnName("districtId");
            entity.Property(e => e.HouseNumberStreet).HasColumnName("houseNumber_Street");
            entity.Property(e => e.WardId).HasColumnName("wardId");

            entity.HasOne(d => d.City).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK__Address__cityId__3F466844");

            entity.HasOne(d => d.District).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK__Address__distric__403A8C7D");

            entity.HasOne(d => d.Ward).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.WardId)
                .HasConstraintName("FK__Address__wardId__412EB0B6");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingNo).HasName("PK__Booking__C6D06266135626D7");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingNo).HasColumnName("bookingNo");
            entity.Property(e => e.CarId).HasColumnName("carId");
            entity.Property(e => e.DriverId).HasColumnName("driverId");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.PaymentMethod).HasColumnName("paymentMethod");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Car).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__Booking__carId__656C112C");

            entity.HasOne(d => d.Driver).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.DriverId)
                .HasConstraintName("FK__Booking__driverI__66603565");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Booking__userId__6477ECF3");
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("PK__Car__1436F17475DA3CED");

            entity.ToTable("Car");

            entity.Property(e => e.CarId).HasColumnName("carId");
            entity.Property(e => e.AddressId).HasColumnName("addressId");
            entity.Property(e => e.BackImage)
                .HasMaxLength(100)
                .HasColumnName("backImage");
            entity.Property(e => e.BasePrice)
                .HasColumnType("money")
                .HasColumnName("basePrice");
            entity.Property(e => e.BrandId).HasColumnName("brandId");
            entity.Property(e => e.ColorId).HasColumnName("colorId");
            entity.Property(e => e.Deposit)
                .HasColumnType("money")
                .HasColumnName("deposit");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DocumentId).HasColumnName("documentId");
            entity.Property(e => e.FrontbackImage)
                .HasMaxLength(100)
                .HasColumnName("frontbackImage");
            entity.Property(e => e.FucntionId).HasColumnName("fucntionId");
            entity.Property(e => e.FuelConsumption).HasColumnName("fuelConsumption");
            entity.Property(e => e.FuelType).HasColumnName("fuelType");
            entity.Property(e => e.LeftImage)
                .HasMaxLength(100)
                .HasColumnName("leftImage");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("licensePlate");
            entity.Property(e => e.Mileage).HasColumnName("mileage");
            entity.Property(e => e.ModelId).HasColumnName("modelId");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.ProductionYear).HasColumnName("productionYear");
            entity.Property(e => e.RightImage)
                .HasMaxLength(100)
                .HasColumnName("rightImage");
            entity.Property(e => e.Seats).HasColumnName("seats");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TermId).HasColumnName("termId");
            entity.Property(e => e.TransmissionType).HasColumnName("transmissionType");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Address).WithMany(p => p.Cars)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Car__addressId__5AEE82B9");

            entity.HasOne(d => d.Brand).WithMany(p => p.Cars)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Car__brandId__5812160E");

            entity.HasOne(d => d.Color).WithMany(p => p.Cars)
                .HasForeignKey(d => d.ColorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Car__colorId__59FA5E80");

            entity.HasOne(d => d.Document).WithMany(p => p.Cars)
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Car__documentId__5BE2A6F2");

            entity.HasOne(d => d.Fucntion).WithMany(p => p.Cars)
                .HasForeignKey(d => d.FucntionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Car__fucntionId__5DCAEF64");

            entity.HasOne(d => d.Model).WithMany(p => p.Cars)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Car__modelId__59063A47");

            entity.HasOne(d => d.Term).WithMany(p => p.Cars)
                .HasForeignKey(d => d.TermId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Car__termId__5CD6CB2B");

            entity.HasOne(d => d.User).WithMany(p => p.Cars)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Car__userId__571DF1D5");
        });

        modelBuilder.Entity<CarBrand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__CarBrand__06B7729910ABB342");

            entity.ToTable("CarBrand");

            entity.Property(e => e.BrandId).HasColumnName("brandId");
            entity.Property(e => e.BrandName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("brandName");
        });

        modelBuilder.Entity<CarColor>(entity =>
        {
            entity.HasKey(e => e.ColorId).HasName("PK__CarColor__70A64FDDBE2A991C");

            entity.ToTable("CarColor");

            entity.Property(e => e.ColorId).HasColumnName("colorId");
            entity.Property(e => e.ColorName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("colorName");
        });

        modelBuilder.Entity<CarDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__CarDocum__EFAAAD853F97C2B3");

            entity.ToTable("CarDocument");

            entity.Property(e => e.DocumentId).HasColumnName("documentId");
            entity.Property(e => e.Certificate)
                .HasMaxLength(100)
                .HasColumnName("certificate");
            entity.Property(e => e.Insurance)
                .HasMaxLength(100)
                .HasColumnName("insurance");
            entity.Property(e => e.Registration)
                .HasMaxLength(100)
                .HasColumnName("registration");
        });

        modelBuilder.Entity<CarModel>(entity =>
        {
            entity.HasKey(e => e.ModelId).HasName("PK__CarModel__0215CC59B978372D");

            entity.ToTable("CarModel");

            entity.Property(e => e.ModelId).HasColumnName("modelId");
            entity.Property(e => e.BrandId).HasColumnName("brandId");
            entity.Property(e => e.ModelName)
                .HasMaxLength(100)
                .HasColumnName("modelName");

            entity.HasOne(d => d.Brand).WithMany(p => p.CarModels)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK__CarModel__brandI__5070F446");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__City__B4BEB95E79136206");

            entity.ToTable("City");

            entity.Property(e => e.CityId).HasColumnName("cityId");
            entity.Property(e => e.CityProvince).HasColumnName("city_province");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.DistrictId).HasName("PK__District__2BAEF71030FD0F76");

            entity.ToTable("District");

            entity.Property(e => e.DistrictId).HasColumnName("districtId");
            entity.Property(e => e.CityId).HasColumnName("cityId");
            entity.Property(e => e.DistrictName).HasColumnName("districtName");

            entity.HasOne(d => d.City).WithMany(p => p.Districts)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK__District__cityId__398D8EEE");
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.DriverId).HasName("PK__Driver__F1532DF2CD7F1554");

            entity.ToTable("Driver");

            entity.HasIndex(e => e.Email, "UQ__Driver__AB6E6164B4794A47").IsUnique();

            entity.Property(e => e.DriverId).HasColumnName("driverId");
            entity.Property(e => e.AddressId).HasColumnName("addressId");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.DrivingLicense)
                .HasMaxLength(100)
                .HasColumnName("driving_license");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IsDifferent)
                .HasDefaultValue(false)
                .HasColumnName("isDifferent");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.NationalId).HasColumnName("nationalId");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__2613FD240591B4B9");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("feedbackId");
            entity.Property(e => e.BookingNo).HasColumnName("bookingNo");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Ratings).HasColumnName("ratings");

            entity.HasOne(d => d.BookingNoNavigation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.BookingNo)
                .HasConstraintName("FK__Feedback__bookin__693CA210");
        });

        modelBuilder.Entity<TermOfUse>(entity =>
        {
            entity.HasKey(e => e.TermId).HasName("PK__TermOfUs__90C2BD1ED260B63B");

            entity.ToTable("TermOfUse");

            entity.Property(e => e.TermId).HasColumnName("termId");
            entity.Property(e => e.NoFoodInCar).HasColumnName("noFoodInCar");
            entity.Property(e => e.NoPet).HasColumnName("noPet");
            entity.Property(e => e.NoSmoking).HasColumnName("noSmoking");
            entity.Property(e => e.Specify).HasColumnName("specify");
        });

        modelBuilder.Entity<TokenInfor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TokenInf__3214EC070F867887");

            entity.Property(e => e.Token).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("userId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__CB9A1CFFE94A6677");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__AB6E6164AACEC99D").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.AddressId).HasColumnName("addressId");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.DrivingLicense)
                .HasMaxLength(100)
                .HasColumnName("driving_license");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.NationalId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nationalId");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.RememberMe).HasColumnName("rememberMe");
            entity.Property(e => e.Role)
                .HasDefaultValue(false)
                .HasColumnName("role");
            entity.Property(e => e.Wallet)
                .HasColumnType("money")
                .HasColumnName("wallet");

            entity.HasOne(d => d.Address).WithMany(p => p.Users)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK__User__addressId__47DBAE45");
        });

        modelBuilder.Entity<Ward>(entity =>
        {
            entity.HasKey(e => e.WardId).HasName("PK__Ward__A14E2C10613CB6BD");

            entity.ToTable("Ward");

            entity.Property(e => e.WardId).HasColumnName("wardId");
            entity.Property(e => e.DistrictId).HasColumnName("districtId");
            entity.Property(e => e.WardName).HasColumnName("wardName");

            entity.HasOne(d => d.District).WithMany(p => p.Wards)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK__Ward__districtId__3C69FB99");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
