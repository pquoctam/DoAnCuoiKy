using Microsoft.EntityFrameworkCore;
using TexasChicken.Models;

public class TexasContext : DbContext
{
    public TexasContext(DbContextOptions<TexasContext> options) : base(options) { }

    public DbSet<KhachHang> KhachHang { get; set; }
    public DbSet<LoaiMon> LoaiMon { get; set; }

    public DbSet<ThucDon> ThucDon { get; set; }
    public DbSet<NguoiDung> NguoiDung { get; set; }
    public DbSet<ThanhPho> ThanhPho { get; set; }
    public DbSet<QuanHuyen> QuanHuyen { get; set; }
    public DbSet<NhaHang> NhaHang { get; set; }
    public DbSet<TinTuc> TinTuc { get; set; }
    public DbSet<Topping> Topping { get; set; }
    public DbSet<MonAn_Topping> MonAn_Topping { get; set; }

    public DbSet<Cart> Cart { get; set; }
    public DbSet<CartItem> CartItem { get; set; }
    public DbSet<CartItemTopping> CartItemTopping { get; set; }
    public virtual DbSet<DonHang> DonHang { get; set; }
    public virtual DbSet<ChiTietDonHang> ChiTietDonHang { get; set; }
    public virtual DbSet<ChiTietDonHangTopping> ChiTietDonHangTopping { get; set; }
    public DbSet<KhuyenMai> KhuyenMai { get; set; }
    public DbSet<MaGiamGia> MaGiamGia { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình khóa chính tổng hợp cho bảng trung gian
        modelBuilder.Entity<MonAn_Topping>()
            .HasKey(mt => new { mt.MaMon, mt.MaTopping });

        // Cấu hình quan hệ với bảng ThucDon
        modelBuilder.Entity<MonAn_Topping>()
            .HasOne(mt => mt.MonAn)
            .WithMany(m => m.MonAn_Topping)
            .HasForeignKey(mt => mt.MaMon);

        // Cấu hình quan hệ với bảng Topping
        modelBuilder.Entity<MonAn_Topping>()
            .HasOne(mt => mt.Topping)
            .WithMany(t => t.MonAn_Topping)
            .HasForeignKey(mt => mt.MaTopping);
    }
}
