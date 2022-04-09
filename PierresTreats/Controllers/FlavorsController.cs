using PierresTreats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace PierresTreats.Controllers
{
  [Authorize]
  public class FlavorsController : Controller
  {
    private readonly PierresTreatsContext _db;

    public FlavorsController(PierresTreatsContext db)
    {
      _db = db;
    }

    [AllowAnonymous]
    public ActionResult Index()
    {
      List<Flavor> model = _db.Flavors.OrderBy(x => x.Type).ToList();
      return View(model);
    }

    public ActionResult Create()
    {
      return View();
    }
    
    [HttpPost]
    public ActionResult Create(Flavor flavor)
    {
      ViewBag.ErrorMessage = "";
      bool isUnique = true;
      List<Flavor> flavorList = _db.Flavors.ToList();
      foreach(Flavor iteration in flavorList)
      {
        if (flavor.Type == iteration.Type) 
        {
          isUnique = false;
          ModelState.AddModelError("DuplicateFlavor", iteration.Type + " already exists");
          return View();
        }
      }
      if (isUnique)
      {
      _db.Flavors.Add(flavor);
      _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisFlavor = _db.Flavors
          .Include(flavor => flavor.JoinEntities)
          .ThenInclude(join => join.Treat)
          .FirstOrDefault(flavor => flavor.FlavorId == id);
      return View(thisFlavor);
    }
    public ActionResult Edit(int id)
    {
      var thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == id);
      return View(thisFlavor);
    }

    [HttpPost]
    public ActionResult Edit(Flavor flavor)
    {
      _db.Entry(flavor).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == id);
      return View(thisFlavor);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == id);
      _db.Flavors.Remove(thisFlavor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}