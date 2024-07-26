using albergo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using albergo.DAO;
using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Albergo.DAO;

namespace albergo.Controllers
{
    [Authorize(Policy = "GeneralAccessPolicy")]
    public class ClienteController : Controller
    {
        private readonly IClienteDao _clienteDao;
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(IClienteDao clienteDao, ILogger<ClienteController> logger)
        {
            _clienteDao = clienteDao;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var clienti = _clienteDao.GetAll();
                return View(clienti);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei clienti.");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public IActionResult Details(int id)
        {
            try
            {
                var cliente = _clienteDao.GetById(id);
                if (cliente == null)
                {
                    return NotFound();
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei dettagli del cliente.");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _clienteDao.Add(cliente);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante la creazione del cliente.");
                    ModelState.AddModelError(string.Empty, "Errore durante la creazione del cliente. Riprova più tardi.");
                }
            }
            return View(cliente);
        }

        public IActionResult Edit(int id)
        {
            try
            {
                var cliente = _clienteDao.GetById(id);
                if (cliente == null)
                {
                    return NotFound();
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del cliente per la modifica.");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _clienteDao.Update(cliente);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante l'aggiornamento del cliente.");
                    ModelState.AddModelError(string.Empty, "Errore durante l'aggiornamento del cliente. Riprova più tardi.");
                }
            }
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _clienteDao.Delete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del cliente.");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}