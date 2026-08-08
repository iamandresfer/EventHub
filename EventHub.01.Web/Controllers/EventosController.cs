using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EventHub._02.Bussines.DTOs;
using EventHub._02.Bussines.Services;
using EventHub._03.Data;

namespace EventHub._01.Web.Controllers
{
    [Authorize]
    public class EventosController : Controller
    {
        private readonly IEventoService _eventoService;
        private readonly IClienteService _clienteService;
        private readonly IVenueService _venueService;
        private readonly ITipoEventoService _tipoEventoService;
        private readonly INotificacionService _notificacionService;
        private readonly IEmailService _emailService;

        public EventosController()
        {
            var context = new EventHubContext();
            _eventoService = new EventoService(context);
            _clienteService = new ClienteService(context);
            _venueService = new VenueService(context);
            _tipoEventoService = new TipoEventoService(context);
            _notificacionService = new NotificacionService();
            _emailService = new EmailService();
        }

        public async Task<ActionResult> Index(string search, string estado)
        {
            var eventos = await _eventoService.GetAllAsync();

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                eventos = eventos.FindAll(e =>
                    e.Nombre.ToLower().Contains(s) ||
                    e.Codigo.ToLower().Contains(s) ||
                    e.ClienteNombre.ToLower().Contains(s));
            }

            if (!string.IsNullOrEmpty(estado))
                eventos = eventos.FindAll(e => e.Estado == estado);

            ViewBag.Search = search;
            ViewBag.Estado = estado;
            return View(eventos);
        }

        public async Task<ActionResult> Details(int id)
        {
            var evento = await _eventoService.GetByIdAsync(id);
            if (evento == null) return HttpNotFound();

            var context = new EventHubContext();

            var tareas = context.Tareas.Where(t => t.EventoId == id).ToList();
            ViewBag.TareasTotales = tareas.Count;
            ViewBag.TareasCompletadas = tareas.Count(t => t.Estado == "Completado");
            ViewBag.TareasPendientes = tareas.Count(t => t.Estado != "Completado");

            var operadorService = new OperadorService();
            var crew = operadorService.GetPorEvento(id);
            ViewBag.CrewCount = crew.Count;
            ViewBag.CrewList = crew.Take(6).ToList();

            return View(evento);
        }

        public async Task<ActionResult> Create()
        {
            ViewBag.Clientes = new SelectList(await _clienteService.GetActiveAsync(), "Id", "Nombre");
            ViewBag.Venues = new SelectList(await _venueService.GetAvailableAsync(), "Id", "Nombre");
            ViewBag.TiposEvento = new SelectList(await _tipoEventoService.GetAllAsync(), "Id", "Nombre");
            return View(new EventoFormDto
            {
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today,
                HoraInicio = new TimeSpan(8, 0, 0),
                HoraFin = new TimeSpan(18, 0, 0)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EventoFormDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = new SelectList(await _clienteService.GetActiveAsync(), "Id", "Nombre", model.ClienteId);
                ViewBag.Venues = new SelectList(await _venueService.GetAvailableAsync(), "Id", "Nombre", model.VenueId);
                ViewBag.TiposEvento = new SelectList(await _tipoEventoService.GetAllAsync(), "Id", "Nombre", model.TipoEventoId);
                return View(model);
            }

            try
            {
                var userId = GetUserId();
                var result = await _eventoService.CreateAsync(model, userId);

                TempData["SuccessMessage"] = "Evento creado exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Create Evento] Error: {ex}");
                TempData["ErrorMessage"] = "Error al crear el evento: " + GetDeepestMessage(ex);
                ViewBag.Clientes = new SelectList(await _clienteService.GetActiveAsync(), "Id", "Nombre", model.ClienteId);
                ViewBag.Venues = new SelectList(await _venueService.GetAvailableAsync(), "Id", "Nombre", model.VenueId);
                ViewBag.TiposEvento = new SelectList(await _tipoEventoService.GetAllAsync(), "Id", "Nombre", model.TipoEventoId);
                return View(model);
            }
        }

        private static string GetDeepestMessage(Exception ex)
        {
            var current = ex;
            while (current.InnerException != null)
                current = current.InnerException;
            return current.Message;
        }

        public async Task<ActionResult> Edit(int id)
        {
            var evento = await _eventoService.GetByIdAsync(id);
            if (evento == null) return HttpNotFound();

            var model = new EventoFormDto
            {
                Id = evento.Id,
                Nombre = evento.Nombre,
                ClienteId = evento.ClienteId,
                VenueId = evento.VenueId,
                TipoEventoId = evento.TipoEventoId,
                FechaInicio = evento.FechaInicio,
                FechaFin = evento.FechaFin,
                HoraInicio = evento.HoraInicio,
                HoraFin = evento.HoraFin,
                PresupuestoEstimado = evento.PresupuestoEstimado,
                Descripcion = evento.Descripcion,
                CoverPhotoUrl = evento.CoverPhotoUrl
            };

            ViewBag.Clientes = new SelectList(await _clienteService.GetActiveAsync(), "Id", "Nombre", evento.ClienteId);
            ViewBag.Venues = new SelectList(await _venueService.GetAvailableAsync(), "Id", "Nombre", evento.VenueId);
            ViewBag.TiposEvento = new SelectList(await _tipoEventoService.GetAllAsync(), "Id", "Nombre", evento.TipoEventoId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, EventoFormDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = new SelectList(await _clienteService.GetActiveAsync(), "Id", "Nombre", model.ClienteId);
                ViewBag.Venues = new SelectList(await _venueService.GetAvailableAsync(), "Id", "Nombre", model.VenueId);
                ViewBag.TiposEvento = new SelectList(await _tipoEventoService.GetAllAsync(), "Id", "Nombre", model.TipoEventoId);
                return View(model);
            }

            if (Request.Files.Count > 0 && Request.Files[0] != null && Request.Files[0].ContentLength > 0)
            {
                var file = Request.Files[0];
                var uploadsDir = Server.MapPath("~/Content/uploads/eventos/");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);
                file.SaveAs(filePath);
                model.CoverPhotoUrl = "/Content/uploads/eventos/" + fileName;
            }

            var result = await _eventoService.UpdateAsync(id, model);
            if (result == null) return HttpNotFound();

            TempData["SuccessMessage"] = "Evento actualizado exitosamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CambiarEstado(int id, string estado)
        {
            await _eventoService.UpdateEstadoAsync(id, estado);
            TempData["SuccessMessage"] = $"Evento marcado como {estado}.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Eliminar(int id)
        {
            await _eventoService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Evento eliminado.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateVenueAjax(VenueDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos. Verifique los campos obligatorios." });

            try
            {
                var result = await _venueService.CreateAsync(model);
                return Json(new { success = true, venueId = result.Id, nombre = result.Nombre });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al crear el lugar: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTipoEventoAjax(TipoEventoDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos. Verifique los campos obligatorios." });

            try
            {
                var result = await _tipoEventoService.CreateAsync(model);
                return Json(new { success = true, tipoEventoId = result.Id, nombre = result.Nombre });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al crear el tipo de evento: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadCover(HttpPostedFileBase file)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                    return Json(new { success = false, message = "No se proporcionó archivo." });

                var ext = Path.GetExtension(file.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                if (Array.IndexOf(allowed, ext) < 0)
                    return Json(new { success = false, message = "Formato no válido. Use JPG, PNG o WebP." });

                if (file.ContentLength > 5 * 1024 * 1024)
                    return Json(new { success = false, message = "La imagen no puede superar 5MB." });

                var uploadDir = Server.MapPath("~/Content/uploads/eventos");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                var fileName = $"evt_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{ext}";
                var filePath = Path.Combine(uploadDir, fileName);
                file.SaveAs(filePath);

                var url = Url.Content("~/Content/uploads/eventos/" + fileName);
                return Json(new { success = true, url = url });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UploadCover] Error: {ex}");
                return Json(new { success = false, message = "Error al guardar la imagen: " + ex.Message });
            }
        }

        private int GetUserId()
        {
            try
            {
                var cookie = Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];
                if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                    return 0;

                var ticket = System.Web.Security.FormsAuthentication.Decrypt(cookie.Value);
                if (ticket == null || string.IsNullOrEmpty(ticket.UserData))
                    return 0;

                var parts = ticket.UserData.Split('|');
                if (parts.Length > 0 && int.TryParse(parts[0], out var id))
                    return id;
            }
            catch { }
            return 0;
        }

        // --- MÓDULO KANBAN (TAREAS) ---

        public async Task<ActionResult> Tareas(int id)
        {
            var evento = await _eventoService.GetByIdAsync(id);
            if (evento == null) return HttpNotFound();

            ViewBag.EventoId = id;
            ViewBag.EventoNombre = evento.Nombre;

            var context = new EventHubContext();
            var usuarios = context.Usuarios.Where(u => u.Estado).Select(u => new { u.Id, u.Nombre }).ToList();
            ViewBag.Usuarios = new SelectList(usuarios, "Id", "Nombre");

            var operadorService = new OperadorService();
            var operadores = operadorService.GetActivos();
            ViewBag.Crew = new SelectList(operadores, "Id", "Nombre");

            var tareaService = new TareaService();
            var tareas = tareaService.ObtenerTareasPorEvento(id);

            return View(tareas);
        }

        [HttpPost]
        public ActionResult CreateTareaAjax(TareaFormDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos" });

            try
            {
                var tareaService = new TareaService();
                var result = tareaService.CrearTarea(model, GetUserId());

                if (result.OperadorId.HasValue && !string.IsNullOrEmpty(result.OperadorEmail))
                {
                    var context = new EventHubContext();
                    var evento = context.Eventos.Find(model.EventoId);
                    var eventoNombre = evento?.Nombre ?? "Evento";

                    var mensaje = $"Se te asigno la tarea \"{result.Titulo}\" en el evento \"{eventoNombre}\".";
                    if (result.FechaLimite.HasValue)
                        mensaje += $" Fecha limite: {result.FechaLimite.Value.ToString("dd/MM/yyyy")}.";

                    _notificacionService.CrearYEnviar(
                        "TareaCreada",
                        mensaje,
                        result.OperadorEmail,
                        result.OperadorNombre,
                        model.EventoId,
                        result.Id,
                        _emailService,
                        eventoNombre,
                        result.Titulo
                    );
                }

                return Json(new { success = true, tarea = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult EditTareaAjax(TareaFormDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos" });

            try
            {
                var tareaService = new TareaService();
                var updated = tareaService.ActualizarTarea(model);
                if (updated == null) return Json(new { success = false, message = "Tarea no encontrada" });

                var context = new EventHubContext();
                if (updated.OperadorId.HasValue && !string.IsNullOrEmpty(updated.OperadorEmail))
                {
                    var evento = context.Eventos.Find(model.EventoId);
                    var eventoNombre = evento?.Nombre ?? "Evento";
                    var fechaStr = model.FechaLimite.HasValue ? model.FechaLimite.Value.ToString("dd/MM/yyyy") : "sin fecha limite";

                    _notificacionService.CrearYEnviar(
                        "FechaModificada",
                        $"Se modifico la tarea \"{updated.Titulo}\" en el evento \"{eventoNombre}\". Fecha limite: {fechaStr}.",
                        updated.OperadorEmail,
                        updated.OperadorNombre,
                        model.EventoId,
                        model.Id,
                        _emailService,
                        eventoNombre,
                        updated.Titulo
                    );
                }

                return Json(new { success = true, tarea = updated });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateTareaStatusAjax(int tareaId, string nuevoEstado, int nuevoOrden)
        {
            try
            {
                var tareaService = new TareaService();
                var result = tareaService.ActualizarEstado(tareaId, nuevoEstado, nuevoOrden);

                if (nuevoEstado == "Completado")
                {
                    var tarea = tareaService.ObtenerPorId(tareaId);
                    if (tarea != null && tarea.OperadorId.HasValue && !string.IsNullOrEmpty(tarea.OperadorEmail))
                    {
                        var context = new EventHubContext();
                        var evento = context.Eventos.Find(tarea.EventoId);
                        var eventoNombre = evento?.Nombre ?? "Evento";

                        _notificacionService.CrearYEnviar(
                            "TareaCompletada",
                            $"La tarea \"{tarea.Titulo}\" del evento \"{eventoNombre}\" ha sido completada.",
                            tarea.OperadorEmail,
                            tarea.OperadorNombre,
                            tarea.EventoId,
                            tareaId,
                            _emailService,
                            eventoNombre,
                            tarea.Titulo
                        );
                    }
                }

                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateTareaOrdenAjax(int tareaId, int nuevoOrden, string estado)
        {
            try
            {
                var tareaService = new TareaService();
                var result = tareaService.ActualizarOrden(tareaId, nuevoOrden, estado);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult EliminarTareaAjax(int id)
        {
            try
            {
                var tareaService = new TareaService();
                var result = tareaService.EliminarTarea(id);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
