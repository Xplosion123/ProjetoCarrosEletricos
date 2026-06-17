using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using ProjetoCarros.Models;
using ProjetoCarros.Repositorio;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using ProjetoCarros.Interfaces;
namespace ProjetoCarros.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        public UsuarioController(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }
        [HttpGet]
        public IActionResult Logar() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logar(Usuario model)
        {
            if (!ModelState.IsValid) return View(model);
            var usuario = _usuarioRepositorio.Validar(model.Email, model.Senha);
            if (usuario != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nome),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim("NivelAcesso",usuario.Nivel),
                    new Claim("UsuarioId",usuario.Id.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties { IsPersistent = false });
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "E-mail ou senha Invalidos.");
            return View(model);
        }

        [HttpGet]
        public IActionResult CriarConta() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CriarConta(Usuario model)
        {
            if (!ModelState.IsValid) return View(model);
            _usuarioRepositorio.CriarConta(model);
            return RedirectToAction("Logar");
        }

        public async Task<IActionResult> Sair()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Logar");
        }

        [HttpGet]
        public  IActionResult DeletarConta()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletarConta(ConfirmarExclusaoViewModel model)
        {
            var usuarioIdClaim = User.FindFirst("UsuarioId");
            if (usuarioIdClaim == null)
            {
                return RedirectToAction("Logar");
            }
            int usuarioId = int.Parse(usuarioIdClaim.Value);
            var usuario = _usuarioRepositorio.BuscarPorId(usuarioId);
            if (usuario == null)
            {
                return RedirectToAction("Logar");
            }

            bool senhaValida = BCrypt.Net.BCrypt.Verify(model.Senha, usuario.Senha);
            if (!senhaValida)
            {
                ModelState.AddModelError("", "Senha Incorreta.");
                return View(model);
            }
            _usuarioRepositorio.DeletarConta(usuarioId);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Logar");
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Configuracoes()
        {
            return View();
        }
    }
}