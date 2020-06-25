using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Katie_Rosario_Blog.Helpers;
using Katie_Rosario_Blog.Models;
using PagedList;
using PagedList.Mvc;

namespace Katie_Rosario_Blog.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "Admin")]

    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);

            int pageSize = 8; //display 3 blog posts at a time on this page
            int pageNumber = (page ?? 1);

            return View(db.BlogPosts.OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize));

        }

        public IQueryable<BlogPost> IndexSearch(string searchStr)
        {

            IQueryable<BlogPost> result = null;
            if (searchStr != null)
            {
                result = db.BlogPosts.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) ||
                            p.Body.Contains(searchStr) ||
                            p.Comments.Any(c => c.Body.Contains(searchStr) ||
                                              c.Author.FirstName.Contains(searchStr) ||
                                              c.Author.LastName.Contains(searchStr) ||
                                              c.Author.DisplayName.Contains(searchStr) ||
                                              c.Author.Email.Contains(searchStr)));

            }
            else
            {
                result = db.BlogPosts.AsQueryable();
            }

            return result.OrderByDescending(p => p.Created);

        }

        // GET: BlogPosts/Details/5

        [AllowAnonymous]
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Slug,Abstract,Body,MediaUrl,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {

                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Oops, the Title cannot be empty!");
                    return View(blogPost);
                }
                if (db.BlogPosts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("", $"Oops, the {blogPost.Title} has been used before!");
                    return View(blogPost);
                }
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var justFileName = Path.GetFileNameWithoutExtension(image.FileName);
                    justFileName = StringUtilities.URLFriendly(justFileName);
                    justFileName = $"{justFileName}-{DateTime.Now.Ticks}";
                    justFileName = $"{justFileName}{Path.GetExtension(image.FileName)}";
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), justFileName));
                    blogPost.MediaUrl = "/Uploads/" + justFileName;
                }
                blogPost.Slug = Slug;
                blogPost.Created = DateTime.Now;
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }







        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Slug,Abstract,Body,MediaUrl,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var justFileName = Path.GetFileNameWithoutExtension(image.FileName);
                    justFileName = StringUtilities.URLFriendly(justFileName);
                    justFileName = $"{justFileName}-{DateTime.Now.Ticks}";
                    justFileName = $"{justFileName}{Path.GetExtension(image.FileName)}";

                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), justFileName));
                    blogPost.MediaUrl = "/Uploads/" + justFileName;
                }

                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (blogPost.Slug != Slug)
                {
                    if (string.IsNullOrEmpty(Slug))
                    {
                        ModelState.AddModelError("Title", "Oops, the Title cannot be empty!");
                        return View(blogPost);
                    }

                    if (db.BlogPosts.Any(p => p.Slug == Slug))
                    {
                        ModelState.AddModelError("", $"Oops, the {blogPost.Title} has been used before!");
                        return View(blogPost);
                    }

                    blogPost.Slug = Slug;
                    blogPost.Updated = DateTime.Now;
                }

                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
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
