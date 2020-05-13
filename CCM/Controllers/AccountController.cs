using CCM.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CCM.Controllers
{
    [Authorize]
    [RequireHttps]
    public class AccountController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        // GET: /Account/Login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult Login(string returnUrl)
        {
            if(User !=null && User.Identity !=null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {

                var user = await UserManager.FindByNameAsync(model.Email);
                if (user != null)
                {


                    if (user.Role == "Liaison")
                    {
                        var isactive = _db.Liaisons.Where(x => x.Id == user.CCMid).Select(x => x.isActive).FirstOrDefault();
                        if (isactive == false)
                        {
                            ModelState.AddModelError("", "Account is blocked.");
                            return View(model);
                        }

                    }
                    if (user.Role == "Physician")
                    {
                        var isactive = _db.Physicians.Where(x => x.Id == user.CCMid).Select(x => x.isActive).FirstOrDefault();
                        if (isactive == false)
                        {
                            ModelState.AddModelError("", "Account is blocked.");
                            return View(model);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
                string Gender = "Male";
                switch (result)
                {
                    case SignInStatus.Success:
                        //var user = await UserManager.FindByNameAsync(model.Email);
                        if (user.Role == "Liaison")
                        {
                            Gender = _db.Liaisons.Where(x => x.Id == user.CCMid).Select(x => x.Gender).FirstOrDefault();

                            _db.LoginHistories.Add(new LoginHistory
                            {
                                UserId = user.Id,
                                LoginDateTime = DateTime.Now
                            });
                            _db.SaveChanges();
                            Session["LoggedID"] = _db.LoginHistories.Where(x => x.UserId == user.Id).ToList().OrderByDescending(x => x.Id).ToList().FirstOrDefault().Id;
                        }
                        
                        if(user.Role== "Sales")
                            Gender = _db.saleStaffs.Where(x => x.Id == user.CCMid).Select(x => x.Gender).FirstOrDefault();

                        Session["UserID"] = user.Id;
                        Session["Role"] = user.Role;
                        Session["Gender"] = Gender;

                        bool isTempPwdLS = await UserManager.CheckPasswordAsync(user, user.LastName.ToLower() + "#LS1013" /* + user.CCMid */);


                        //if (await UserManager.CheckPasswordAsync(user, "npi" + user.LastName + "#PG1013" /* + user.CCMid */ ) ||
                        //    await UserManager.CheckPasswordAsync(user, user.LastName.ToLower() + "#PA1013" /* + user.CCMid */ ) ||
                        //    await UserManager.CheckPasswordAsync(user, user.LastName.ToLower() + "#PH1013" /* + user.CCMid */ ) ||
                        //    await UserManager.CheckPasswordAsync(user, user.LastName.ToLower() + "#LS1013" /* + user.CCMid */ ))
                        //{


                        //    //return RedirectToAction("ChangePassword", "Manage");
                        //    return RedirectToAction("ChangePassword", "Manage");
                        //    //model.Password
                        //}


                        if (user.Role == "Patient")
                            return RedirectToAction("Details", "PatientPortal", new { patientId = user.CCMid });
                        if (user.Role == "QAQC")
                            return RedirectToAction("Index", "CcmStatus", new { status = "Clinical Sign-Off" });
                        if (user.Role == "Sales")
                            return RedirectToAction("TotalPatients", "Patient", new { status = "" });

                        return RedirectToAction("Index", "Home");
                        //return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);

            }
        }

        // GET: /Account/Register
        [Authorize(Roles = "Admin")]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = "Admin"
                };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, "Admin");

                    return View();
                }
                AddErrors(result);
            }

            return View(model);
        }

        // GET: /Account/ForgotPassword
        //[AllowAnonymous]
        [Authorize(Roles = "Admin")]
        public ActionResult ForgotPassword()
        {
            if (User.Identity.GetUserName() == "stanadmin@gmail.com")
            {
                return View();
            }
            else
                return RedirectToActionPermanent("Index", "Home");
           
        }

        // POST: /Account/ForgotPasswordbvcvcnbvnvb
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = await UserManager.FindByNameAsync(model.Email);
                //if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                //{
                //    // Don't reveal that the user does not exist or is not confirmed
                //    return View("ForgotPasswordConfirmation");
                //}
                ApplicationUser user1 = await UserManager.FindByNameAsync(model.Email);
                if (user1 != null)
                {
                    Random _rdm = new Random();
                     
                    var NewPassword = user1.LastName + _rdm.Next(10000, 99999);
                user1.PasswordHash = UserManager.PasswordHasher.HashPassword(NewPassword);
                    var result = await UserManager.UpdateAsync(user1);
                    if (!result.Succeeded)
                    {
                        //throw exception......
                    }
                    ViewBag.Message = NewPassword;
                    return View();
                }
                
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //Session["LoggedID"]
            if (Session["LoggedID"] != null)
            {
                int id = Convert.ToInt32(Session["LoggedID"]);
                var loginhistory = _db.LoginHistories.Where(x => x.Id == id).FirstOrDefault();
                if (loginhistory != null)
                {
                    loginhistory.LogOutDateTime = DateTime.Now;
                    _db.Entry(loginhistory).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon();
            Response.Cookies.Clear();
            return RedirectToAction("Index", "Home");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }


        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}