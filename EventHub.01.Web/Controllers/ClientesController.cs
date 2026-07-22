using System.Threading.Tasks;
using System.Web.Mvc;
using EventHub._02.Bussines.DTOs;
using EventHub._02.Bussines.Services;
using EventHub._03.Data;

namespace EventHub._01.Web.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly IClienteService _clienteService;
        private readonly IEventoService _eventoService;

        public ClientesController()
        {
            var context = new EventHubContext();
            _clienteService = new ClienteService(context);
            _eventoService = new EventoService(context);
        }

        public async Task<ActionResult> Index(string search)
        {
            var clientes = await _clienteService.GetAllAsync();

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                clientes = clientes.FindAll(c =>
                    c.Nombre.ToLower().Contains(s) ||
                    c.Ruc.Contains(s) ||
                    c.Email.ToLower().Contains(s));
            }

            ViewBag.Search = search;
            return View(clientes);
        }

        public async Task<ActionResult> Details(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null) return HttpNotFound();

            var eventos = await _eventoService.GetAllAsync();
            eventos = eventos.FindAll(e => e.ClienteNombre == cliente.Nombre);

            ViewBag.Cliente = cliente;
            ViewBag.Eventos = eventos;
            return View(cliente);
        }

        public ActionResult Create()
        {
            return View(new ClienteDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ClienteDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = GetUserId();
            await _clienteService.CreateAsync(model, userId);

            TempData["SuccessMessage"] = "Cliente creado exitosamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAjax(ClienteDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos. Verifique los campos obligatorios." });

            try
            {
                var userId = GetUserId();
                await _clienteService.CreateAsync(model, userId);
                return Json(new { success = true, clienteId = model.Id, nombre = model.Nombre });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = "Error al crear el cliente: " + ex.Message });
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null) return HttpNotFound();
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ClienteDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _clienteService.UpdateAsync(model);
            TempData["SuccessMessage"] = "Cliente actualizado exitosamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAjax(ClienteDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos. Verifique los campos obligatorios." });

            try
            {
                await _clienteService.UpdateAsync(model);
                return Json(new { success = true, nombre = model.Nombre });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = "Error al actualizar el cliente: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ToggleEstado(int id)
        {
            await _clienteService.ToggleEstadoAsync(id);
            TempData["SuccessMessage"] = "Estado del cliente actualizado.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ToggleEstadoAjax(int id)
        {
            try
            {
                var result = await _clienteService.ToggleEstadoAsync(id);
                if (!result)
                    return Json(new { success = false, message = "Cliente no encontrado." });

                var cliente = await _clienteService.GetByIdAsync(id);
                return Json(new { success = true, estado = cliente.Estado });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Desactivar(int id)
        {
            await _clienteService.DeactivateAsync(id);
            TempData["SuccessMessage"] = "Cliente desactivado.";
            return RedirectToAction("Index");
        }

        private int GetUserId()
        {
            var data = System.Web.Security.FormsAuthentication.Decrypt(
                Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName]?.Value ?? "")?.UserData;
            if (!string.IsNullOrEmpty(data))
            {
                var parts = data.Split('|');
                if (parts.Length > 0 && int.TryParse(parts[0], out var id))
                    return id;
            }
            return 0;
        }
    }
}
