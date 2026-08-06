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
        private readonly IOperadorService _operadorService;

        public CrewController()
        {
            _operadorService = new OperadorService();
        }

        public ActionResult Index(int eventoId)
        {
            var context = new EventHubContext();
            var evento = context.Eventos.Find(eventoId);
            if (evento == null) return HttpNotFound();

            ViewBag.EventoId = eventoId;
            ViewBag.EventoNombre = evento.Nombre;

            var operadores = _operadorService.GetPorEvento(eventoId);
            return View(operadores);
        }

        [HttpGet]
        public ActionResult Index()
        {
            var operadores = _operadorService.GetActivos();
            return View("IndexGlobal", operadores);
        }

        [HttpPost]
        public ActionResult CrearOperadorAjax(OperadorFormDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos" });

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
        public ActionResult AsignarAEventoAjax(int operadorId, int eventoId)
        {
            try
            {
                var context = new EventHubContext();
                var operador = context.Operadores.Find(operadorId);
                if (operador == null)
                    return Json(new { success = false, message = "Operador no encontrado" });

                operador.EventoId = eventoId;
                context.SaveChanges();
                return Json(new { success = true });
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

        [HttpGet]
        public ActionResult ObtenerCrewEventoAjax(int eventoId)
        {
            try
            {
                var operadores = _operadorService.GetPorEvento(eventoId);
                return Json(new { success = true, crew = operadores }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ObtenerOperadoresAjax()
        {
            try
            {
                var operadores = _operadorService.GetActivos();
                return Json(new { success = true, operadores = operadores }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
