using System;
using System.Collections.Generic;
using DOMAIN.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace DOMAIN;

public partial class TripnestDbContext : DbContext
{
    public TripnestDbContext()
    {
    }

    public TripnestDbContext(DbContextOptions<TripnestDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Amenities> Amenities { get; set; }

    public virtual DbSet<Auditlogs> Auditlogs { get; set; }

    public virtual DbSet<Bookingitems> Bookingitems { get; set; }

    public virtual DbSet<Bookings> Bookings { get; set; }

    public virtual DbSet<Companies> Companies { get; set; }

    public virtual DbSet<Companyemployees> Companyemployees { get; set; }

    public virtual DbSet<Companypermissions> Companypermissions { get; set; }

    public virtual DbSet<Companyrolepermissions> Companyrolepermissions { get; set; }

    public virtual DbSet<Companyroles> Companyroles { get; set; }

    public virtual DbSet<Embeddings> Embeddings { get; set; }

    public virtual DbSet<Itineraries> Itineraries { get; set; }

    public virtual DbSet<Messages> Messages { get; set; }

    public virtual DbSet<Notifications> Notifications { get; set; }

    public virtual DbSet<Payments> Payments { get; set; }

    public virtual DbSet<Properties> Properties { get; set; }

    public virtual DbSet<Propertyamenities> Propertyamenities { get; set; }

    public virtual DbSet<Propertyphotos> Propertyphotos { get; set; }

    public virtual DbSet<Refreshtokens> Refreshtokens { get; set; }

    public virtual DbSet<Reviews> Reviews { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }

    public virtual DbSet<Roomavailability> Roomavailability { get; set; }

    public virtual DbSet<Roomprices> Roomprices { get; set; }

    public virtual DbSet<Rooms> Rooms { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    public virtual DbSet<Waypoints> Waypoints { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;port=3306;database=tripnest;user=root;password=123456", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.44-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_vietnamese_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Amenities>(entity =>
        {
            entity.HasKey(e => e.AmenityId).HasName("PRIMARY");

            entity.ToTable("amenities");

            entity.HasIndex(e => e.NameVi, "uq_amenities_name_vi").IsUnique();

            entity.Property(e => e.AmenityId).HasColumnName("amenity_id");
            entity.Property(e => e.NameEn)
                .HasMaxLength(100)
                .HasColumnName("name_en");
            entity.Property(e => e.NameVi)
                .HasMaxLength(100)
                .HasColumnName("name_vi");
            entity.Property(e => e.Slug)
                .HasMaxLength(100)
                .HasColumnName("slug");
        });

        modelBuilder.Entity<Auditlogs>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PRIMARY");

            entity.ToTable("auditlogs");

            entity.HasIndex(e => e.PerformedBy, "idx_audit_performed_by");

            entity.Property(e => e.AuditId).HasColumnName("audit_id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.AfterJson)
                .HasColumnType("json")
                .HasColumnName("after_json");
            entity.Property(e => e.BeforeJson)
                .HasColumnType("json")
                .HasColumnName("before_json");
            entity.Property(e => e.EntityId)
                .HasMaxLength(255)
                .HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(100)
                .HasColumnName("entity_type");
            entity.Property(e => e.PerformedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("performed_at");
            entity.Property(e => e.PerformedBy).HasColumnName("performed_by");

            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.Auditlogs)
                .HasForeignKey(d => d.PerformedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_audit_user");
        });

        modelBuilder.Entity<Bookingitems>(entity =>
        {
            entity.HasKey(e => e.BookingItemId).HasName("PRIMARY");

            entity.ToTable("bookingitems");

            entity.HasIndex(e => e.BookingId, "idx_bookingitems_booking");

            entity.HasIndex(e => e.RoomId, "idx_bookingitems_room");

            entity.Property(e => e.BookingItemId).HasColumnName("booking_item_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.Nights).HasColumnName("nights");
            entity.Property(e => e.Price)
                .HasPrecision(12, 2)
                .HasColumnName("price");
            entity.Property(e => e.Qty)
                .HasDefaultValueSql("'1'")
                .HasColumnName("qty");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Subtotal)
                .HasPrecision(12, 2)
                .HasColumnName("subtotal");

            entity.HasOne(d => d.Booking).WithMany(p => p.Bookingitems)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("fk_bookingitems_booking");

            entity.HasOne(d => d.Room).WithMany(p => p.Bookingitems)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bookingitems_room");
        });

        modelBuilder.Entity<Bookings>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PRIMARY");

            entity.ToTable("bookings");

            entity.HasIndex(e => new { e.PropertyId, e.Status }, "idx_bookings_property_status");

            entity.HasIndex(e => e.UserId, "idx_bookings_user");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CheckinDate).HasColumnName("checkin_date");
            entity.Property(e => e.CheckoutDate).HasColumnName("checkout_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .HasDefaultValueSql("'USD'")
                .IsFixedLength()
                .HasColumnName("currency");
            entity.Property(e => e.GuestsCount)
                .HasDefaultValueSql("'1'")
                .HasColumnName("guests_count");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'Pending'")
                .HasColumnType("enum('Pending','Confirmed','Cancelled','Completed')")
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(12, 2)
                .HasColumnName("total_price");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Version)
                .HasDefaultValueSql("'1'")
                .HasColumnName("version");

            entity.HasOne(d => d.Property).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("fk_bookings_property");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_bookings_user");
        });

        modelBuilder.Entity<Companies>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PRIMARY");

            entity.ToTable("companies");

            entity.HasIndex(e => e.OwnerUserId, "idx_companies_owner");

            entity.HasIndex(e => e.Slug, "uq_companies_slug").IsUnique();

            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Address)
                .HasMaxLength(512)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.OwnerUserId).HasColumnName("owner_user_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.Slug).HasColumnName("slug");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.OwnerUser).WithMany(p => p.Companies)
                .HasForeignKey(d => d.OwnerUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_companies_owner");
        });

        modelBuilder.Entity<Companyemployees>(entity =>
        {
            entity.HasKey(e => e.CompanyEmployeeId).HasName("PRIMARY");

            entity.ToTable("companyemployees");

            entity.HasIndex(e => e.CompanyRoleId, "fk_companyemployees_companyrole");

            entity.HasIndex(e => e.CompanyId, "idx_companyemployees_company");

            entity.HasIndex(e => e.UserId, "idx_companyemployees_user");

            entity.HasIndex(e => new { e.CompanyId, e.UserId }, "uq_company_user").IsUnique();

            entity.Property(e => e.CompanyEmployeeId).HasColumnName("company_employee_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CompanyRoleId).HasColumnName("company_role_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("joined_at");
            entity.Property(e => e.LeftAt)
                .HasColumnType("datetime")
                .HasColumnName("left_at");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Company).WithMany(p => p.Companyemployees)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("fk_companyemployees_company");

            entity.HasOne(d => d.CompanyRole).WithMany(p => p.Companyemployees)
                .HasForeignKey(d => d.CompanyRoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_companyemployees_companyrole");

            entity.HasOne(d => d.User).WithMany(p => p.Companyemployees)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_companyemployees_user");
        });

        modelBuilder.Entity<Companypermissions>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PRIMARY");

            entity.ToTable("companypermissions");

            entity.HasIndex(e => e.CompanyId, "idx_companypermissions_company");

            entity.HasIndex(e => new { e.CompanyId, e.Name }, "uq_companypermission_company_name").IsUnique();

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");

            entity.HasOne(d => d.Company).WithMany(p => p.Companypermissions)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("fk_companypermissions_company");
        });

        modelBuilder.Entity<Companyrolepermissions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("companyrolepermissions");

            entity.HasIndex(e => e.PermissionId, "idx_crp_perm");

            entity.HasIndex(e => e.CompanyRoleId, "idx_crp_role");

            entity.HasIndex(e => new { e.CompanyRoleId, e.PermissionId }, "uq_crp").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyRoleId).HasColumnName("company_role_id");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");

            entity.HasOne(d => d.CompanyRole).WithMany(p => p.Companyrolepermissions)
                .HasForeignKey(d => d.CompanyRoleId)
                .HasConstraintName("fk_crp_role");

            entity.HasOne(d => d.Permission).WithMany(p => p.Companyrolepermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("fk_crp_permission");
        });

        modelBuilder.Entity<Companyroles>(entity =>
        {
            entity.HasKey(e => e.CompanyRoleId).HasName("PRIMARY");

            entity.ToTable("companyroles");

            entity.HasIndex(e => e.CompanyId, "idx_companyrole_company");

            entity.HasIndex(e => new { e.CompanyId, e.Name }, "uq_companyrole_company_name").IsUnique();

            entity.Property(e => e.CompanyRoleId).HasColumnName("company_role_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasOne(d => d.Company).WithMany(p => p.Companyroles)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("fk_companyrole_company");
        });

        modelBuilder.Entity<Embeddings>(entity =>
        {
            entity.HasKey(e => e.EmbeddingId).HasName("PRIMARY");

            entity.ToTable("embeddings");

            entity.HasIndex(e => new { e.ItemType, e.ItemId }, "idx_embeddings_item");

            entity.Property(e => e.EmbeddingId).HasColumnName("embedding_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ItemType)
                .HasMaxLength(50)
                .HasColumnName("item_type");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.VectorBlob).HasColumnName("vector_blob");
            entity.Property(e => e.VectorRef)
                .HasMaxLength(255)
                .HasColumnName("vector_ref");
        });

        modelBuilder.Entity<Itineraries>(entity =>
        {
            entity.HasKey(e => e.ItineraryId).HasName("PRIMARY");

            entity.ToTable("itineraries");

            entity.HasIndex(e => e.UserId, "idx_itineraries_user");

            entity.Property(e => e.ItineraryId).HasColumnName("itinerary_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Metadata)
                .HasColumnType("json")
                .HasColumnName("metadata");
            entity.Property(e => e.NameEn)
                .HasMaxLength(255)
                .HasColumnName("name_en");
            entity.Property(e => e.NameVi)
                .HasMaxLength(255)
                .HasColumnName("name_vi");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Itineraries)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_itineraries_user");
        });

        modelBuilder.Entity<Messages>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PRIMARY");

            entity.ToTable("messages");

            entity.HasIndex(e => e.FromUserId, "idx_messages_from");

            entity.HasIndex(e => e.PropertyId, "idx_messages_property");

            entity.HasIndex(e => e.ToUserId, "idx_messages_to");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FromUserId).HasColumnName("from_user_id");
            entity.Property(e => e.IsAi)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_ai");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.ToUserId).HasColumnName("to_user_id");

            entity.HasOne(d => d.FromUser).WithMany(p => p.MessagesFromUser)
                .HasForeignKey(d => d.FromUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_messages_from");

            entity.HasOne(d => d.Property).WithMany(p => p.Messages)
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_messages_property");

            entity.HasOne(d => d.ToUser).WithMany(p => p.MessagesToUser)
                .HasForeignKey(d => d.ToUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_messages_to");
        });

        modelBuilder.Entity<Notifications>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PRIMARY");

            entity.ToTable("notifications");

            entity.HasIndex(e => e.UserId, "idx_notifications_user");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_read");
            entity.Property(e => e.Payload)
                .HasColumnType("json")
                .HasColumnName("payload");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_notifications_user");
        });

        modelBuilder.Entity<Payments>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PRIMARY");

            entity.ToTable("payments");

            entity.HasIndex(e => e.BookingId, "idx_payments_booking");

            entity.HasIndex(e => e.ProviderRef, "idx_payments_provider_ref");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasPrecision(12, 2)
                .HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("currency");
            entity.Property(e => e.PaidAt)
                .HasColumnType("datetime")
                .HasColumnName("paid_at");
            entity.Property(e => e.Provider)
                .HasMaxLength(100)
                .HasColumnName("provider");
            entity.Property(e => e.ProviderRef).HasColumnName("provider_ref");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payments_booking");
        });

        modelBuilder.Entity<Properties>(entity =>
        {
            entity.HasKey(e => e.PropertyId).HasName("PRIMARY");

            entity.ToTable("properties");

            entity.HasIndex(e => new { e.TitleEn, e.DescriptionEn }, "ft_properties_title_description_en").HasAnnotation("MySql:FullTextIndex", true);

            entity.HasIndex(e => new { e.TitleVi, e.DescriptionVi }, "ft_properties_title_description_vi").HasAnnotation("MySql:FullTextIndex", true);

            entity.HasIndex(e => e.CompanyId, "idx_properties_company");

            entity.HasIndex(e => e.OwnerUserId, "idx_properties_owner");

            entity.HasIndex(e => e.PriceBase, "idx_properties_price");

            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.AddressFormatted)
                .HasMaxLength(512)
                .HasColumnName("address_formatted");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .HasDefaultValueSql("'USD'")
                .IsFixedLength()
                .HasColumnName("currency");
            entity.Property(e => e.DescriptionEn)
                .HasColumnType("text")
                .HasColumnName("description_en");
            entity.Property(e => e.DescriptionVi)
                .HasColumnType("text")
                .HasColumnName("description_vi");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.OwnerUserId).HasColumnName("owner_user_id");
            entity.Property(e => e.PriceBase)
                .HasPrecision(12, 2)
                .HasColumnName("price_base");
            entity.Property(e => e.PropertyType)
                .HasMaxLength(50)
                .HasColumnName("property_type");
            entity.Property(e => e.Province)
                .HasMaxLength(100)
                .HasColumnName("province");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'1'")
                .HasColumnName("status");
            entity.Property(e => e.Street)
                .HasMaxLength(255)
                .HasColumnName("street");
            entity.Property(e => e.TitleEn).HasColumnName("title_en");
            entity.Property(e => e.TitleVi).HasColumnName("title_vi");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Propertyamenities>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("propertyamenities");

            entity.HasIndex(e => e.AmenityId, "idx_pa_amenity");

            entity.HasIndex(e => e.PropertyId, "idx_pa_property");

            entity.HasIndex(e => new { e.PropertyId, e.AmenityId }, "uq_property_amenity").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AmenityId).HasColumnName("amenity_id");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");

            entity.HasOne(d => d.Amenity).WithMany(p => p.Propertyamenities)
                .HasForeignKey(d => d.AmenityId)
                .HasConstraintName("fk_pa_amenity");

            entity.HasOne(d => d.Property).WithMany(p => p.Propertyamenities)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("fk_pa_property");
        });

        modelBuilder.Entity<Propertyphotos>(entity =>
        {
            entity.HasKey(e => e.PhotoId).HasName("PRIMARY");

            entity.ToTable("propertyphotos");

            entity.HasIndex(e => e.PropertyId, "idx_photos_property");

            entity.Property(e => e.PhotoId).HasColumnName("photo_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Meta)
                .HasColumnType("json")
                .HasColumnName("meta");
            entity.Property(e => e.Order)
                .HasDefaultValueSql("'0'")
                .HasColumnName("order");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.Url)
                .HasMaxLength(1024)
                .HasColumnName("url");

            entity.HasOne(d => d.Property).WithMany(p => p.Propertyphotos)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("fk_photos_property");
        });

        modelBuilder.Entity<Refreshtokens>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PRIMARY");

            entity.ToTable("refreshtokens");

            entity.HasIndex(e => e.UserId, "idx_rt_user");

            entity.Property(e => e.TokenId).HasColumnName("token_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeviceInfo)
                .HasMaxLength(255)
                .HasColumnName("device_info");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime")
                .HasColumnName("expires_at");
            entity.Property(e => e.RevokedAt)
                .HasColumnType("datetime")
                .HasColumnName("revoked_at");
            entity.Property(e => e.TokenHash)
                .HasMaxLength(512)
                .HasColumnName("token_hash");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Refreshtokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_rt_user");
        });

        modelBuilder.Entity<Reviews>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PRIMARY");

            entity.ToTable("reviews");

            entity.HasIndex(e => e.PropertyId, "idx_reviews_property");

            entity.HasIndex(e => e.Rating, "idx_reviews_rating");

            entity.HasIndex(e => e.UserId, "idx_reviews_user");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Property).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("fk_reviews_property");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_reviews_user");
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "uq_roles_name").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Roomavailability>(entity =>
        {
            entity.HasKey(e => e.AvailabilityId).HasName("PRIMARY");

            entity.ToTable("roomavailability");

            entity.HasIndex(e => new { e.RoomId, e.Date }, "idx_roomavailability_room_date").IsUnique();

            entity.Property(e => e.AvailabilityId).HasColumnName("availability_id");
            entity.Property(e => e.AvailableCount).HasColumnName("available_count");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.RoomId).HasColumnName("room_id");

            entity.HasOne(d => d.Room).WithMany(p => p.Roomavailability)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("fk_roomavailability_room");
        });

        modelBuilder.Entity<Roomprices>(entity =>
        {
            entity.HasKey(e => e.RoomPriceId).HasName("PRIMARY");

            entity.ToTable("roomprices");

            entity.HasIndex(e => e.RoomId, "idx_roomprices_room");

            entity.Property(e => e.RoomPriceId).HasColumnName("room_price_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Price)
                .HasPrecision(12, 2)
                .HasColumnName("price");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.ValidFrom).HasColumnName("valid_from");
            entity.Property(e => e.ValidTo).HasColumnName("valid_to");

            entity.HasOne(d => d.Room).WithMany(p => p.Roomprices)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("fk_roomprices_room");
        });

        modelBuilder.Entity<Rooms>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PRIMARY");

            entity.ToTable("rooms");

            entity.HasIndex(e => e.PropertyId, "idx_rooms_property");

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.CancellationPolicy)
                .HasColumnType("json")
                .HasColumnName("cancellation_policy");
            entity.Property(e => e.Capacity)
                .HasDefaultValueSql("'1'")
                .HasColumnName("capacity");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.NameEn)
                .HasMaxLength(255)
                .HasColumnName("name_en");
            entity.Property(e => e.NameVi)
                .HasMaxLength(255)
                .HasColumnName("name_vi");
            entity.Property(e => e.PricePerNight)
                .HasPrecision(12, 2)
                .HasColumnName("price_per_night");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.Stock)
                .HasDefaultValueSql("'1'")
                .HasColumnName("stock");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Property).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("fk_rooms_property");
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.RoleId, "idx_users_role");

            entity.HasIndex(e => e.Email, "uq_users_email").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.Locale)
                .HasMaxLength(8)
                .HasDefaultValueSql("'vi'")
                .HasColumnName("locale");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(512)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.ProfilePhotoUrl)
                .HasMaxLength(1024)
                .HasColumnName("profile_photo_url");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_users_roles");
        });

        modelBuilder.Entity<Waypoints>(entity =>
        {
            entity.HasKey(e => e.WaypointId).HasName("PRIMARY");

            entity.ToTable("waypoints");

            entity.HasIndex(e => e.ItineraryId, "idx_waypoints_itinerary");

            entity.Property(e => e.WaypointId).HasColumnName("waypoint_id");
            entity.Property(e => e.Arrival)
                .HasColumnType("datetime")
                .HasColumnName("arrival");
            entity.Property(e => e.ItineraryId).HasColumnName("itinerary_id");
            entity.Property(e => e.Lat).HasColumnName("lat");
            entity.Property(e => e.Lng).HasColumnName("lng");
            entity.Property(e => e.Order)
                .HasDefaultValueSql("'0'")
                .HasColumnName("order");
            entity.Property(e => e.PlaceId)
                .HasMaxLength(255)
                .HasColumnName("place_id");

            entity.HasOne(d => d.Itinerary).WithMany(p => p.Waypoints)
                .HasForeignKey(d => d.ItineraryId)
                .HasConstraintName("fk_waypoints_itinerary");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
