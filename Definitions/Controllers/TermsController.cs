using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Definitions;
using Microsoft.AspNet.Identity;

namespace Definitions.Controllers
{
    [Authorize]
    public class TermsController : Controller
    {
        private Entities db = new Entities();

        // GET: Terms
        public ActionResult Index(string sortOrder, string searchString)
        {
            IQueryable<Terms> terms;
            if (String.IsNullOrEmpty(searchString))
            {
                terms = db.Terms.Include(t => t.AspNetUsers);
            }
            else
            {
                terms = db.Terms
                    .Include(t => t.AspNetUsers)
                    .Include(t => t.ExtraTerms)
                    .Where(t => t.Value.Contains(searchString) || t.Description.Contains(searchString) || t.ExtraTerms.Any(e => e.Value.Contains(searchString)));
            }
            return View(terms.ToList());
        }

        // GET: Terms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Terms terms = db.Terms.Find(id);
            if (terms == null)
            {
                return HttpNotFound();
            }
            return View(terms);
        }

        // GET: Terms/Create
        public ActionResult Create()
        {
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: Terms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Value,Description")] Terms terms)
        {
            if (ModelState.IsValid)
            {
                terms.AspNetUserId = User.Identity.GetUserId();
                db.Terms.Add(terms);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email", terms.AspNetUserId);
            return View(terms);
        }


        // GET: Terms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Terms terms = db.Terms.Find(id);
            if (terms == null)
            {
                return HttpNotFound();
            }
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email", terms.AspNetUserId);
            return View(terms);
        }

        // POST: Terms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Value,Description")] Terms terms)
        {
            if (ModelState.IsValid)
            {
                terms.AspNetUserId = User.Identity.GetUserId();
                db.Entry(terms).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email", terms.AspNetUserId);
            return View(terms);
        }

        // GET: Terms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Terms terms = db.Terms.Find(id);
            if (terms == null)
            {
                return HttpNotFound();
            }
            return View(terms);
        }

        // POST: Terms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Terms terms = db.Terms.Find(id);
            Terms check = db.Terms.Include(t => t.ExtraTerms).Where(t => t.Id == id).FirstOrDefault();

            if(check != null && check.ExtraTerms != null)
            {
                foreach(var ex in check.ExtraTerms.ToList())
                {
                    db.ExtraTerms.Remove(ex);
                }
                db.SaveChanges();
            }
            db.Terms.Remove(terms);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CreateExtra(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Terms terms = db.Terms.Find(id);
            if (terms == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        [HttpPost, ActionName("CreateExtra")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateExtra(int? id, [Bind(Include = "Id,Value")] ExtraTerms extraTerm)
        {
            if(ModelState.IsValid)
            {
                extraTerm.TermsId = (int)id;
                db.ExtraTerms.Add(extraTerm);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = id });
            }
            return View();
        }

        public ActionResult DeleteExtra(int id)
        {
            ExtraTerms terms = db.ExtraTerms.Find(id);
            db.ExtraTerms.Remove(terms);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = terms.TermsId });
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
