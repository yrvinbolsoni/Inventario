using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IntraGriegHomolog.Models;

namespace IntraGriegHomolog.Controllers.ComputersControl
{
    [Authorize(Roles = "TI")]
    public class EmpresaController : Controller
    {
        private DbIntra db = new DbIntra();

        // GET: Empresa
        public ActionResult Index()
        {
            var cAD_EMP = db.CAD_EMP.Include(c => c.CAD_LOCAL);
            return View(cAD_EMP.ToList());
        }

        // GET: Empresa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAD_EMP cAD_EMP = db.CAD_EMP.Find(id);
            if (cAD_EMP == null)
            {
                return HttpNotFound();
            }
            return View(cAD_EMP);
        }

        // GET: Empresa/Create
        public ActionResult Create()
        {
            ViewBag.local_id = new SelectList(db.CAD_LOCAL, "id", "descs");
            return View();
        }

        // POST: Empresa/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descs,cnpj,endereco,telefone,cep,local_id")] CAD_EMP cAD_EMP)
        {
            if (ModelState.IsValid)
            {
                db.CAD_EMP.Add(cAD_EMP);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.local_id = new SelectList(db.CAD_LOCAL, "id", "descs", cAD_EMP.local_id);
            return View(cAD_EMP);
        }

        // GET: Empresa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAD_EMP cAD_EMP = db.CAD_EMP.Find(id);
            if (cAD_EMP == null)
            {
                return HttpNotFound();
            }
            ViewBag.local_id = new SelectList(db.CAD_LOCAL, "id", "descs", cAD_EMP.local_id);
            return View(cAD_EMP);
        }

        // POST: Empresa/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descs,cnpj,endereco,telefone,cep,local_id")] CAD_EMP cAD_EMP)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cAD_EMP).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.local_id = new SelectList(db.CAD_LOCAL, "id", "descs", cAD_EMP.local_id);
            return View(cAD_EMP);
        }

        // GET: Empresa/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAD_EMP cAD_EMP = db.CAD_EMP.Find(id);
            if (cAD_EMP == null)
            {
                return HttpNotFound();
            }
            return View(cAD_EMP);
        }

        // POST: Empresa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CAD_EMP cAD_EMP = db.CAD_EMP.Find(id);
            db.CAD_EMP.Remove(cAD_EMP);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
