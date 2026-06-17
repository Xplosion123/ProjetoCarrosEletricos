using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoCarros.Interfaces;
using ProjetoCarros.Models;

namespace ProjetoCarros.Controllers
{
    public class CarrosController : Controller
    {
        private readonly ICarroRepositorio _carroRepositorio;

        public CarrosController(ICarroRepositorio carroRepositorio)
        {
            _carroRepositorio = carroRepositorio;
        }

        //paginas publicas 
        public IActionResult Index()
        {
            var carros = _carroRepositorio.ListarTodos();
            return View(carros); //passa a lista para a view
        }
        public IActionResult CarrosHibridos()
        {
            //Filtra só os hibridos no c# mesmo
            var hibridos = _carroRepositorio.ListarTodos()
                .Where(c => c.Categoria == "PHEV" || c.Categoria == "HEV" || c.Categoria == "MHEV")
                .ToList();
            return View(hibridos);
        }
        public IActionResult CarrosEletricos()
        {
            var eletricos = _carroRepositorio.ListarTodos()
                .Where(c => c.Categoria == "BEV" || c.Categoria == "FCEV")
                .ToList();
            return View(eletricos);
        }

        //Area que apenas os adms vão poder gerenciar

        [Authorize]
        public IActionResult Gerenciar()
        {
            //verifica manualmente se é adm 
            if (User.FindFirst("NivelAcesso")?.Value != "Admin")
                return RedirectToAction("AcessoNegado");

            var carros = _carroRepositorio.ListarTodos();
                return View(carros);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Cria() => View();

        [Authorize]
        [HttpPost]
        public IActionResult Criar( Carro model, IFormFile? imagem)
        {
            if (User.FindFirst("NivelAcesso")?.Value != "Admin")
                return RedirectToAction("AcessoNegado");

            if (!ModelState.IsValid)
                
                return View(model);

            _carroRepositorio.Criar(model, imagem);
            TempData["Sucesso"] = "Carro cadstrado com sucesso!";
            return RedirectToAction("Gerenciar");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Editar(int id)
        {
            if (User.FindFirst("NivelAcesso")?.Value != "Admin")
                return RedirectToAction("AcessoNegado");

            var carro = _carroRepositorio.BuscarPorId(id);
            if (carro == null)
                return NotFound();
            return View(carro);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Carro model, IFormFile? imagem)
        {
            if (User.FindFirst("NivelAcesso")?.Value != "Admin")
                return RedirectToAction("AcessoNegado");

            if (!ModelState.IsValid) 
                return View(model);

            _carroRepositorio.Editar(model, imagem);
            TempData["Sucesso"] = "Carro atualizado com sucesso!";
            return RedirectToAction("Gerenciar");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deletar(int id)
        {
            if (User.FindFirst("NivelAcesso")?.Value != "Admin")
                return RedirectToAction("AcessoNegado");

            _carroRepositorio.Deletar(id);
            TempData["Sucesso"] = "Carro Removido.";
            return RedirectToAction("Gerenciar");
        }

        public IActionResult AcessoNegado() => View();

        public IActionResult TodosModelos()
        {
            return View();
        }
        public IActionResult Novos()
        {
            return View();
        }
        public IActionResult SemiNovos()
        {
            return View();
        }
    }
}
