public class GanToppingViewModel
{
    public int MaMon { get; set; }
    public string TenMon { get; set; }
    public List<ToppingItem> DanhSachTopping { get; set; }

}

public class ToppingItem
{
    public int MaTopping { get; set; }
    public string TenTopping { get; set; }
    public bool DuocChon { get; set; }

    
}
