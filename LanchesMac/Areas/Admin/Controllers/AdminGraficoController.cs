using LanchesMac.Areas.Admin.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminGraficoController : Controller
    {
        private readonly GraficoVendasService _graficoVendas;

        public AdminGraficoController(GraficoVendasService graficoVendas)
        {
            _graficoVendas = graficoVendas ?? throw new ArgumentNullException(nameof(graficoVendas));
        }
        
        public JsonResult VendasLanches(int dias)
        {
            var lanchesVendasTotais = _graficoVendas.GetVendasLanche(dias);
            return Json(lanchesVendasTotais);
        }

        [HttpGet]
        public IActionResult Index(int dias)
        {
            return View();
        }

        [HttpGet]
        public IActionResult VendasMensal(int dias)
        {
            return View();
        }

        [HttpGet]
        public IActionResult VendasSemanal(int dias)
        {
            return View();
        }
    }
}
