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

    public virtual DbSet<BookingInfo> BookingInfos { get; set; }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<CarBrand> CarBrands { get; set; }

    public virtual DbSet<CarColor> CarColors { get; set; }

    public virtual DbSet<CarDocument> CarDocuments { get; set; }

    public virtual DbSet<CarModel> CarModels { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<ContentOnScreen> ContentOnScreens { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<TermOfUse> TermOfUses { get; set; }

    public virtual DbSet<TokenInfor> TokenInfors { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<Ward> Wards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
            
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdditionalFunction>(entity =>
        {
            entity.HasKey(e => e.FucntionId).HasName("PK__Addition__1A8C7CAFDADED689");

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
            entity.HasKey(e => e.AddressId).HasName("PK__Address__26A111ADD1C55C0F");

            entity.ToTable("Address");

            entity.Property(e => e.AddressId).HasColumnName("addressId");
            entity.Property(e => e.CityId).HasColumnName("cityId");
            entity.Property(e => e.DistrictId).HasColumnName("districtId");
            entity.Property(e => e.HouseNumberStreet).HasColumnName("houseNumber_Street");
            entity.Property(e => e.WardId).HasColumnName("wardId");

            entity.HasOne(d => d.City).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK__Address__cityId__5BE2A6F2");

            entity.HasOne(d => d.District).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK__Address__distric__5CD6CB2B");

            entity.HasOne(d => d.Ward).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.WardId)
                .HasConstraintName("FK__Address__wardId__5DCAEF64");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingNo).HasName("PK__Booking__C6D062664D5C5E12");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingNo).HasColumnName("bookingNo");
            entity.Property(e => e.BookingInfoId).HasColumnName("bookingInfoId");
            entity.Property(e => e.CarId).HasColumnName("carId");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("endDate");
            entity.Property(e => e.PaymentMethod).HasColumnName("paymentMethod");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("startDate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.BookingInfo).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BookingInfoId)
                .HasConstraintName("FK_Booking_BookingInfo");

            entity.HasOne(d => d.Car).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__Booking__carId__5EBF139D");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Booking__userId__5FB337D6");
        });

        modelBuilder.Entity<BookingInfo>(entity =>
        {
            entity.HasKey(e => e.BookingInfoId).HasName("PK__BookingI__1460802B228FBD92");

            entity.ToTable("BookingInfo");

            entity.Property(e => e.BookingInfoId).HasColumnName("bookingInfoId");
            entity.Property(e => e.DriverAddressId).HasColumnName("driverAddressId");
            entity.Property(e => e.DriverDob).HasColumnName("driverDob");
            entity.Property(e => e.DriverDrivingLicense)
                .HasMaxLength(100)
                .HasColumnName("driverDriving_license");
            entity.Property(e => e.DriverEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("driverEmail");
            entity.Property(e => e.DriverName)
                .HasMaxLength(100)
                .HasColumnName("driverName");
            entity.Property(e => e.DriverNationalId).HasMaxLength(20).HasColumnName("driverNationalId");
            entity.Property(e => e.DriverPhone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("driverPhone");
            entity.Property(e => e.IsDifferent).HasColumnName("isDifferent");
            entity.Property(e => e.RenterAddressId).HasColumnName("renterAddressId");
            entity.Property(e => e.RenterDob).HasColumnName("renterDob");
            entity.Property(e => e.RenterDrivingLicense)
                .HasMaxLength(100)
                .HasColumnName("renterDriving_license");
            entity.Property(e => e.RenterEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("renterEmail");
            entity.Property(e => e.RenterName)
                .HasMaxLength(100)
                .HasColumnName("renterName");
            entity.Property(e => e.RenterNationalId).HasMaxLength(20).HasColumnName("renterNationalId");
            entity.Property(e => e.RenterPhone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("renterPhone");

            entity.HasOne(d => d.DriverAddress).WithMany(p => p.BookingInfoDriverAddresses)
                .HasForeignKey(d => d.DriverAddressId)
                .HasConstraintName("FK_BookingInfo_driverAddressId");

            entity.HasOne(d => d.RenterAddress).WithMany(p => p.BookingInfoRenterAddresses)
                .HasForeignKey(d => d.RenterAddressId)
                .HasConstraintName("FK_BookingInfo_renterAddressId");
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("PK__Car__1436F17478C9ABC7");

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
            entity.Property(e => e.FrontImage)
                .HasMaxLength(100)
                .HasColumnName("frontImage");
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
                .HasConstraintName("FK__Car__addressId__6383C8BA");

            entity.HasOne(d => d.Brand).WithMany(p => p.Cars)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Car__brandId__6477ECF3");

            entity.HasOne(d => d.Color).WithMany(p => p.Cars)
                .HasForeignKey(d => d.ColorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Car__colorId__656C112C");

            entity.HasOne(d => d.Document).WithMany(p => p.Cars)
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Car__documentId__66603565");

            entity.HasOne(d => d.Fucntion).WithMany(p => p.Cars)
                .HasForeignKey(d => d.FucntionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Car__fucntionId__6754599E");

            entity.HasOne(d => d.Model).WithMany(p => p.Cars)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Car__modelId__68487DD7");

            entity.HasOne(d => d.Term).WithMany(p => p.Cars)
                .HasForeignKey(d => d.TermId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Car__termId__693CA210");

            entity.HasOne(d => d.User).WithMany(p => p.Cars)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Car__userId__6A30C649");
        });

        modelBuilder.Entity<CarBrand>(entity =>
        {
            entity.HasKey(e => e.BrandId);

            entity.ToTable("CarBrand");

            entity.Property(e => e.BrandId)
                .ValueGeneratedNever()
                .HasColumnName("brandId");
            entity.Property(e => e.BrandLogo)
                .HasMaxLength(100)
                .HasColumnName("brandLogo");
            entity.Property(e => e.BrandName)
                .HasMaxLength(255)
                .HasColumnName("brandName");
        });

        modelBuilder.Entity<CarColor>(entity =>
        {
            entity.HasKey(e => e.ColorId).HasName("PK__CarColor__70A64FDDBE2EA484");

            entity.ToTable("CarColor");

            entity.Property(e => e.ColorId)
                .ValueGeneratedNever()
                .HasColumnName("colorId");
            entity.Property(e => e.ColorName)
                .HasMaxLength(255)
                .HasColumnName("colorName");
        });

        modelBuilder.Entity<CarDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__CarDocum__EFAAAD8570073C2C");

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
            entity.HasKey(e => e.ModelId).HasName("PK__CarModel__0215CC599459BD5F");

            entity.ToTable("CarModel");

            entity.Property(e => e.ModelId)
                .ValueGeneratedNever()
                .HasColumnName("modelId");
            entity.Property(e => e.BrandId).HasColumnName("brandId");
            entity.Property(e => e.ModelName)
                .HasMaxLength(255)
                .HasColumnName("modelName");

            entity.HasOne(d => d.Brand).WithMany(p => p.CarModels)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK__CarModel__brandI__123EB7A3");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__City__B4BEB95E086FE329");

            entity.ToTable("City");

            entity.Property(e => e.CityId)
                .ValueGeneratedNever()
                .HasColumnName("cityId");
            entity.Property(e => e.CityProvince)
                .HasMaxLength(255)
                .HasColumnName("city_province");
        });

        modelBuilder.Entity<ContentOnScreen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContentO__3213E83F1C1EDBDD");

            entity.ToTable("ContentOnScreen");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Image)
                .HasMaxLength(200)
                .HasColumnName("image");
            entity.Property(e => e.Screen)
                .HasMaxLength(200)
                .HasColumnName("screen");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.DistrictId).HasName("PK__District__2BAEF7101C78F861");

            entity.ToTable("District");

            entity.Property(e => e.DistrictId)
                .ValueGeneratedNever()
                .HasColumnName("districtId");
            entity.Property(e => e.CityId).HasColumnName("cityId");
            entity.Property(e => e.DistrictName)
                .HasMaxLength(255)
                .HasColumnName("districtName");

            entity.HasOne(d => d.City).WithMany(p => p.Districts)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK__District__cityId__398D8EEE");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__2613FD24AE891FE4");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("feedbackId");
            entity.Property(e => e.BookingNo).HasColumnName("bookingNo");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Ratings).HasColumnName("ratings");

            entity.HasOne(d => d.BookingNoNavigation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.BookingNo)
                .HasConstraintName("FK__Feedback__bookin__6D0D32F4");
        });

        modelBuilder.Entity<TermOfUse>(entity =>
        {
            entity.HasKey(e => e.TermId).HasName("PK__TermOfUs__90C2BD1E17E0779E");

            entity.ToTable("TermOfUse");

            entity.Property(e => e.TermId).HasColumnName("termId");
            entity.Property(e => e.NoFoodInCar).HasColumnName("noFoodInCar");
            entity.Property(e => e.NoPet).HasColumnName("noPet");
            entity.Property(e => e.NoSmoking).HasColumnName("noSmoking");
            entity.Property(e => e.Specify).HasColumnName("specify");
        });

        modelBuilder.Entity<TokenInfor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TokenInf__3214EC07FF33997A");

            entity.Property(e => e.Token).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("userId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__CB9A1CFF6F4144DA");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__AB6E6164B5E4A99C").IsUnique();

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
                .HasMaxLength(100)
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
                .HasConstraintName("FK__User__addressId__6E01572D");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.WalletId).HasName("PK__Wallet__3785C870B730F9D3");

            entity.ToTable("Wallet");

            entity.Property(e => e.WalletId).HasColumnName("walletId");
            entity.Property(e => e.Amount)
                .HasMaxLength(64)
                .HasColumnName("amount");
            entity.Property(e => e.BookingNo).HasColumnName("bookingNo");
            entity.Property(e => e.CarName).HasColumnName("carName");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Note)
                .IsUnicode(false)
                .HasColumnName("note");
            entity.Property(e => e.Type)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.BookingNoNavigation).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.BookingNo)
                .HasConstraintName("FK_Wallet_Booking");

            entity.HasOne(d => d.User).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wallet_User");
        });

        modelBuilder.Entity<Ward>(entity =>
        {
            entity.HasKey(e => e.WardId).HasName("PK__Ward__A14E2C1086D1130C");

            entity.ToTable("Ward");

            entity.Property(e => e.WardId)
                .ValueGeneratedNever()
                .HasColumnName("wardId");
            entity.Property(e => e.DistrictId).HasColumnName("districtId");
            entity.Property(e => e.WardName)
                .HasMaxLength(255)
                .HasColumnName("wardName");

            entity.HasOne(d => d.District).WithMany(p => p.Wards)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK__Ward__districtId__49C3F6B7");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
