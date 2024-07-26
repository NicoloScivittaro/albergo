using albergo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using albergo.DAO;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Albergo.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class ServiziController : Controller
    {
        private readonly IServizioAggiuntivoDao _servizioDao;
        private readonly ILogger<ServiziController> _logger;

        public ServiziController(IServizioAggiuntivoDao servizioDao, ILogger<ServiziController> logger)
        {
            _servizioDao = servizioDao;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var servizi = _servizioDao.GetAll();
                return View(servizi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving services.");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public IActionResult Details(int id)
        {
            var servizio = _servizioDao.GetById(id);
            if (servizio == null)
            {
                return NotFound();
            }
            return View(servizio);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Servizio servizio)
        {
            if (ModelState.IsValid)
            {
                _servizioDao.Add(servizio);
                return RedirectToAction(nameof(Index));
            }
            return View(servizio);
        }

        public IActionResult Edit(int id)
        {
            var servizio = _servizioDao.GetById(id);
            if (servizio == null)
            {
                return NotFound();
            }
            return View(servizio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Servizio servizio)
        {
            if (id != servizio.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _servizioDao.Update(servizio);
                return RedirectToAction(nameof(Index));
            }
            return View(servizio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _servizioDao.Delete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting service with ID {id}.", id);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
