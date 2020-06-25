using Katie_Rosario_Blog.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace Katie_Rosario_Blog.Controllers
{
    [RequireHttps]

    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            int pageSize = 3; //display 3 blog posts at a time on this page
            int pageNumber = (page ?? 1);
            var listPosts = IndexSearch(searchStr);
            return View(listPosts.Where(b => b.Published).OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize));
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            //ViewBag.Message = "Your contact page.";
            EmailModel model = new EmailModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
         public async Task<ActionResult> Contact(EmailModel model)
         {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold>({1})</p><p>Message:</p><p>{2}</p>";

                    var from = $"Katie Rosario <{WebConfigurationManager.AppSettings["emailfrom"]}>";

                    model.Body = "This is a message from your portfolio site. The name and the email of the contacting person is above.";

                    var email = new MailMessage(from, WebConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = "Portfolio Contact Email",
                        Body = string.Format(body, model.FromName, model.FromEmail, model.Body),
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);

                    return View(new EmailModel());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }

            }
                return View(model);
         }
    }
}