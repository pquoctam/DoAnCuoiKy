using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TexasChicken.Models;

public class BaseController : Controller
{
    protected readonly TexasContext _context;

    public BaseController(TexasContext context)
    {
        _context = context;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewBag.DanhMuc = _context.LoaiMon.ToList();
        base.OnActionExecuting(context);
    }
}
