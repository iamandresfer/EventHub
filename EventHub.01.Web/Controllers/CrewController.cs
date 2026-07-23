using System;
using System.Linq;
using System.Web.Mvc;
using EventHub._02.Bussines.DTOs;
using EventHub._02.Bussines.Services;
using EventHub._03.Data;

namespace EventHub._01.Web.Controllers
{
    [Authorize]
    public class CrewController : Controller
    {
        private readonly ICrewService _crewService;

        public CrewController()
        {
            _crewService = new CrewService();
        }

        public ActionResult Index(int eventoId)
        {
            var context = new EventHubContext();
            var evento = context.Eventos.Find(eventoId);
            if (evento == null) return HttpNotFound();

            ViewBag.EventoId = eventoId;
            ViewBag.EventoNombre = evento.Nombre;

            var crew = _crewService.ObtenerCrewPorEvento(eventoId);
            return View(crew);
        }

        [HttpPost]
        public ActionResult CrearCrewAjax(CrewOperadorFormDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos" });

            try
            {
                var result = _crewService.CrearCrew(model);
                return Json(new { success = true, crew = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ActualizarCrewAjax(CrewOperadorFormDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos" });

            try
            {
                var result = _crewService.ActualizarCrew(model);
                if (result == null)
                    return Json(new { success = false, message = "Operador no encontrado" });

                return Json(new { success = true, crew = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult EliminarCrewAjax(int id)
        {
            try
            {
                var result = _crewService.EliminarCrew(id);
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
                var result = _crewService.ToggleEstado(id);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult ObtenerCrewEventoAjax(int eventoId)
        {
            try
            {
                var crew = _crewService.ObtenerCrewPorEvento(eventoId);
                return Json(new { success = true, crew = crew }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
