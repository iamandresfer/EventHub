using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventHub._02.Bussines.DTOs;
using EventHub._02.Bussines.Services;
using EventHub._03.Data;

namespace EventHub._01.Web.Controllers
{
    [Authorize]
    public class OperadoresController : Controller
    {
        private readonly IOperadorService _operadorService;

        public OperadoresController()
        {
            _operadorService = new OperadorService();
        }

        public ActionResult Index(string search)
        {
            ViewBag.Search = search;
            var operadores = _operadorService.GetConEventos();

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                operadores = operadores.FindAll(o =>
                    o.Nombre.ToLower().Contains(s) ||
                    (!string.IsNullOrEmpty(o.Email) && o.Email.ToLower().Contains(s)) ||
                    (!string.IsNullOrEmpty(o.Cedula) && o.Cedula.Contains(s)) ||
                    (!string.IsNullOrEmpty(o.Rol) && o.Rol.ToLower().Contains(s)));
            }

            return View(operadores);
        }

        [HttpPost]
        public ActionResult CrearOperadorAjax(OperadorFormDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos. Verifica los campos obligatorios." });

            try
            {
                var result = _operadorService.Create(model);
                return Json(new { success = true, operador = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ActualizarOperadorAjax(OperadorFormDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos." });

            try
            {
                var result = _operadorService.Update(model);
                if (result == null)
                    return Json(new { success = false, message = "Operador no encontrado." });

                return Json(new { success = true, operador = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult EliminarOperadorAjax(int id)
        {
            try
            {
                var result = _operadorService.Delete(id);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ToggleEstadoAjax(int id)
        {
            try
            {
                var result = _operadorService.ToggleEstado(id);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult RemoverDeEventoAjax(int operadorId)
        {
            try
            {
                var result = _operadorService.RemoverDeEvento(operadorId);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UploadFotoAjax(HttpPostedFileBase file)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                    return Json(new { success = false, message = "No se proporcionó archivo." });

                var ext = System.IO.Path.GetExtension(file.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                if (Array.IndexOf(allowed, ext) < 0)
                    return Json(new { success = false, message = "Formato no válido. Use JPG, PNG o WebP." });

                if (file.ContentLength > 2 * 1024 * 1024)
                    return Json(new { success = false, message = "La imagen no puede superar 2MB." });

                var uploadDir = Server.MapPath("~/Content/uploads/operadores");
                if (!System.IO.Directory.Exists(uploadDir))
                    System.IO.Directory.CreateDirectory(uploadDir);

                var fileName = $"ope_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{ext}";
                var filePath = System.IO.Path.Combine(uploadDir, fileName);
                file.SaveAs(filePath);

                var url = Url.Content("~/Content/uploads/operadores/" + fileName);
                return Json(new { success = true, url = url });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al guardar la imagen: " + ex.Message });
            }
        }

        [HttpGet]
        public ActionResult ObtenerOperadoresAjax(string q = "")
        {
            try
            {
                var operadores = _operadorService.GetActivos();
                if (!string.IsNullOrEmpty(q))
                {
                    var lower = q.ToLower();
                    operadores = operadores.FindAll(o =>
                        o.Nombre.ToLower().Contains(lower) ||
                        (!string.IsNullOrEmpty(o.Rol) && o.Rol.ToLower().Contains(lower)));
                }
                return Json(new { success = true, operadores = operadores }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult MisTareas(string email)
        {
            if (string.IsNullOrEmpty(email))
                return View("MisTareas", new List<TareaDto>());

            var context = new EventHubContext();
            var operador = context.Operadores.FirstOrDefault(o => o.Email == email);
            if (operador == null)
                return View("MisTareas", new List<TareaDto>());

            var tareas = context.Tareas
                .Where(t => t.OperadorId == operador.Id)
                .OrderBy(t => t.Estado).ThenBy(t => t.Orden)
                .Select(t => new TareaDto
                {
                    Id = t.Id,
                    EventoId = t.EventoId,
                    Titulo = t.Titulo,
                    Descripcion = t.Descripcion,
                    Estado = t.Estado,
                    Categoria = t.Categoria,
                    FechaLimite = t.FechaLimite,
                    OperadorId = t.OperadorId,
                    OperadorNombre = t.Operador != null ? t.Operador.Nombre : null,
                    Orden = t.Orden
                })
                .ToList();

            ViewBag.Email = email;
            return View("MisTareas", tareas);
        }
    }
}
