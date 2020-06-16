using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WebApi.Models;
using WebApi.Providers;
using WebApi.Results;
using Core;
using System.Linq;
using System.Linq.Expressions;

namespace WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private UserManager _userManager;
        private IUserRepository _UserRepo;
        private ApplicationRoleManager _roleManager;
        //private readonly IResourceManager _resManager;
        //private IResourceManager _resManager;
        //private ICodeTableManager _CodetableManager;

        public AccountController(IUserRepository UserRepo)
        {
            _UserRepo = UserRepo;
        }

        public AccountController(UserManager userManager,
                 ApplicationRoleManager roleManager,
                 ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            AccessTokenFormat = accessTokenFormat;
            //ResManager = resManager;
        }

        public UserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<UserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public IHttpActionResult Register(RegisterBindingModel model)
        {
            //string strHostPath = ControllerContext.Request.RequestUri.AbsoluteUri.Replace(ControllerContext.Request.RequestUri.AbsolutePath, "/exact/");

            string ErrMsg = "Error: ";
            string SuccessMsg = "Success : User registration done successfully and pending for approval!!";

            if (null != model && model.Password.Trim().ToUpper() != model.ConfirmPassword.Trim().ToUpper())
            {
                ErrMsg += string.Join(";", "Password and Confirmation password doesn't match !!");
                return Ok(ErrMsg);
            }

            if (!ModelState.IsValid)
            {
                ErrMsg += string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                return Ok(ErrMsg);
            }

            var now = DateTime.Now;

            var user = new UserSnapshot()
            {
                UserName = model.UserName,
                Email = model.Email,
                Name = model.Name,
                Gender = model.Gender,
                PhoneNumber = model.Phone,
                Age = model.Age,
                Password = model.Password,
                IsActive = true,
                IsOwner = model.IsOwner,
                CreatedBy = 1,
                CreatedOn = now,
                LastUpdatedBy= 1,
                LastUpdatedOn= now
            };
            try
            {
                Expression<Func<UserSnapshot, bool>> expr1 = s => s.UserName == user.UserName && s.Password == user.Password && s.IsActive == true;
                //Checking if existing user
                bool IsUserExists = _UserRepo.GetFilteredUserDetails(expr1).Count() > 0;

                if (IsUserExists)
                {
                    ErrMsg += string.Join(";", "User already exists !!");
                    return Ok(ErrMsg);
                }
                else
                {
                    bool res = _UserRepo.AddUser(user, true);
                    //send cofirmation email
                    if (res)
                        return Ok(SuccessMsg);
                    else
                    {
                        ErrMsg += string.Join(";", "Some error occurred. Please contact administrator !!");
                        return Ok(ErrMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg += string.Join(";", ex.Message);
                return Ok(ErrMsg);
            }
        }

        //// GET api/Account/UserInfo
        //[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        //[Route("UserInfo")]
        //public UserInfoViewModel GetUserInfo()
        //{
        //    ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

        //    return new UserInfoViewModel
        //    {
        //        Email = User.Identity.GetUserName(),
        //        HasRegistered = externalLogin == null,
        //        LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
        //    };
        //}

        //// POST api/Account/Logout
        //[Route("Logout")]
        //public IHttpActionResult Logout()
        //{
        //    Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
        //    Authentication.SignOut(OAuthDefaults.AuthenticationType);
        //    return Ok();
        //}

        //// GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        //[Route("ManageInfo")]
        //public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        //{
        //    IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

        //    if (user == null)
        //    {
        //        return null;
        //    }

        //    List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

        //    foreach (IdentityUserLogin linkedAccount in user.Logins)
        //    {
        //        logins.Add(new UserLoginInfoViewModel
        //        {
        //            LoginProvider = linkedAccount.LoginProvider,
        //            ProviderKey = linkedAccount.ProviderKey
        //        });
        //    }

        //    if (user.PasswordHash != null)
        //    {
        //        logins.Add(new UserLoginInfoViewModel
        //        {
        //            LoginProvider = LocalLoginProvider,
        //            ProviderKey = user.UserName,
        //        });
        //    }

        //    return new ManageInfoViewModel
        //    {
        //        LocalLoginProvider = LocalLoginProvider,
        //        Email = user.UserName,
        //        Logins = logins,
        //        ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
        //    };
        //}

        //// POST api/Account/ChangePassword
        //[Route("ChangePassword")]
        //public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        //{
        //    string ErrMsg = "Error: ";
        //    string SuccessMsg = "Success : Your password has been successfully updated!! Please logIn with new password!!";
        //    string strHostPath = ControllerContext.Request.RequestUri.AbsoluteUri.Replace(ControllerContext.Request.RequestUri.AbsolutePath, "/exact/");

        //    if (!ModelState.IsValid)
        //    {
        //        ErrMsg += string.Join("; ", ModelState.Values
        //                                        .SelectMany(x => x.Errors)
        //                                        .Select(x => x.ErrorMessage));
        //        return Ok(ErrMsg);
        //    }

        //    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
        //    if (result.Succeeded)
        //    {
        //        User _user = await UserManager.FindByNameAsync(model.UserName);
        //        if (_user != null)
        //        {
        //            SendResetPasswordSuccessMail(_user, strHostPath);
        //        }
        //        else
        //        {
        //            ErrMsg += "Password reset process has been failed.";
        //            return Ok(ErrMsg);
        //        }
        //    }
        //    if (!result.Succeeded)
        //    {
        //        ErrMsg += string.Join("; ", result.Errors);
        //        return Ok(ErrMsg);
        //        //return GetErrorResult(result);
        //    }
        //    return Ok(SuccessMsg);
        //}

        //// POST api/Account/SetPassword
        //[Route("SetPassword")]
        //public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

        //    if (!result.Succeeded)
        //    {
        //        return GetErrorResult(result);
        //    }

        //    return Ok();
        //}

        //// POST api/Account/AddExternalLogin
        //[Route("AddExternalLogin")]
        //public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

        //    AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

        //    if (ticket == null || ticket.Identity == null || (ticket.Properties != null
        //        && ticket.Properties.ExpiresUtc.HasValue
        //        && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
        //    {
        //        return BadRequest("External login failure.");
        //    }

        //    ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

        //    if (externalData == null)
        //    {
        //        return BadRequest("The external login is already associated with an account.");
        //    }

        //    IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
        //        new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

        //    if (!result.Succeeded)
        //    {
        //        return GetErrorResult(result);
        //    }

        //    return Ok();
        //}

        //// POST api/Account/RemoveLogin
        //[Route("RemoveLogin")]
        //public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    IdentityResult result;

        //    if (model.LoginProvider == LocalLoginProvider)
        //    {
        //        result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
        //    }
        //    else
        //    {
        //        result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
        //            new UserLoginInfo(model.LoginProvider, model.ProviderKey));
        //    }

        //    if (!result.Succeeded)
        //    {
        //        return GetErrorResult(result);
        //    }

        //    return Ok();
        //}

        //// GET api/Account/ExternalLogin
        //[OverrideAuthentication]
        //[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        //[AllowAnonymous]
        //[Route("ExternalLogin", Name = "ExternalLogin")]
        //public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        //{
        //    if (error != null)
        //    {
        //        return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
        //    }

        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return new ChallengeResult(provider, this);
        //    }

        //    ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

        //    if (externalLogin == null)
        //    {
        //        return InternalServerError();
        //    }

        //    if (externalLogin.LoginProvider != provider)
        //    {
        //        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //        return new ChallengeResult(provider, this);
        //    }

        //    User user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
        //        externalLogin.ProviderKey));

        //    bool hasRegistered = user != null;

        //    if (hasRegistered)
        //    {
        //        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

        //        ClaimsIdentity oAuthIdentity = await this.GenerateUserIdentityAsync(user, UserManager,
        //           OAuthDefaults.AuthenticationType);
        //        ClaimsIdentity cookieIdentity = await this.GenerateUserIdentityAsync(user, UserManager,
        //            CookieAuthenticationDefaults.AuthenticationType);
        //        List<Claim> roles = oAuthIdentity.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

        //        AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName,
        //            user.FirstName, user.LastName, Newtonsoft.Json.JsonConvert.SerializeObject(roles.Select(x => x.Value)));
        //        Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
        //    }
        //    else
        //    {
        //        IEnumerable<Claim> claims = externalLogin.GetClaims();
        //        ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
        //        Authentication.SignIn(identity);
        //    }

        //    return Ok();
        //}

        //// GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        //[AllowAnonymous]
        //[Route("ExternalLogins")]
        //public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        //{
        //    IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
        //    List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

        //    string state;

        //    if (generateState)
        //    {
        //        const int strengthInBits = 256;
        //        state = RandomOAuthStateGenerator.Generate(strengthInBits);
        //    }
        //    else
        //    {
        //        state = null;
        //    }

        //    foreach (AuthenticationDescription description in descriptions)
        //    {
        //        ExternalLoginViewModel login = new ExternalLoginViewModel
        //        {
        //            Name = description.Caption,
        //            Url = Url.Route("ExternalLogin", new
        //            {
        //                provider = description.AuthenticationType,
        //                response_type = "token",
        //                client_id = Startup.PublicClientId,
        //                redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
        //                state = state
        //            }),
        //            State = state
        //        };
        //        logins.Add(login);
        //    }

        //    return logins;
        //}



        //[AllowAnonymous]
        //[Route("SendEmailResetPwdLink")]
        //[HttpPost]
        //public async Task<IHttpActionResult> SendEmailResetPwdLink(RegisterBindingModel model)
        //{
        //    string strHostPath = ControllerContext.Request.RequestUri.AbsoluteUri.Replace(ControllerContext.Request.RequestUri.AbsolutePath, "/exact/");
        //    string successMessage = "";

        //    try
        //    {
        //        if (model != null)
        //        {
        //            User _user = await UserManager.FindByNameAsync(model.UserName);
        //            if (_user != null)
        //            {
        //                SendPasswordResetLinkToMail(_user, strHostPath);
        //            }
        //            else
        //            {
        //                return Ok("ERROR");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //    return Ok(successMessage);
        //}

        //[AllowAnonymous]
        //[Route("ResetPassword")]
        //[HttpPost]
        //public async Task<IHttpActionResult> ResetPassword(ResetPasswordBindingModel model)
        //{
        //    string ErrMsg = "Error: ";
        //    string SuccessMsg = "Success : Your password has been successfully updated!!";
        //    string strHostPath = ControllerContext.Request.RequestUri.AbsoluteUri.Replace(ControllerContext.Request.RequestUri.AbsolutePath, "/exact/");

        //    try
        //    {
        //        if (model != null)
        //        {
        //            if (!ModelState.IsValid)
        //            {
        //                ErrMsg += string.Join("; ", ModelState.Values
        //                                        .SelectMany(x => x.Errors)
        //                                        .Select(x => x.ErrorMessage));
        //                return Ok(ErrMsg);
        //            }

        //            User _user = await UserManager.FindByNameAsync(model.UserName);

        //            if (_user != null)
        //            {
        //                var code = await UserManager.GeneratePasswordResetTokenAsync(_user.Id);
        //                var resetResult = await UserManager.ResetPasswordAsync(_user.Id, code, model.Password);
        //                if (resetResult.Succeeded == true)
        //                {
        //                    SendResetPasswordSuccessMail(_user, strHostPath);
        //                }
        //                else
        //                {
        //                    ErrMsg += "Password reset process has been failed.";
        //                    return Ok(ErrMsg);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //    return Ok(SuccessMsg);
        //}

        //[AllowAnonymous]
        //[Route("DecryptUserName")]
        //[HttpPost]
        //public IHttpActionResult DecryptUserName(ResetPasswordBindingModel model)
        //{

        //    string UserName = string.Empty;
        //    try
        //    {
        //        //string passPhrase = "Pas5pr@se";        // can be any string
        //        //string saltValue = "s@1tValue";        // can be any string
        //        //string hashAlgorithm = "SHA1";             // can be "MD5"
        //        //int passwordIterations = 2;                // can be any number
        //        //string initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes
        //        //int keySize = 256;                // can be 192 or 128
        //        //string PwdtoDecrypt = model.UserName.Replace("$^","==").Substring(0,model.UserName.Length - 3);
        //        //if (model != null)
        //        //{
        //        //    decryptedUserName = EncryptionUtility.Decrypt(PwdtoDecrypt, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);
        //        //}

        //        if (model != null && !string.IsNullOrWhiteSpace(model.UserName))
        //        {
        //            string userID = model.UserName.Substring(0, model.UserName.Length - 1);
        //            User _user = UserManager.FindById(userID);
        //            UserName = _user.UserName;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //    return Ok(UserName);
        //}

        //// POST api/Account/RegisterExternal
        //[OverrideAuthentication]
        //[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        //[Route("RegisterExternal")]
        //public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var info = await Authentication.GetExternalLoginInfoAsync();
        //    if (info == null)
        //    {
        //        return InternalServerError();
        //    }

        //    var user = new User() { UserName = model.Email, Email = model.Email };

        //    IdentityResult result = await UserManager.CreateAsync(user);
        //    if (!result.Succeeded)
        //    {
        //        return GetErrorResult(result);
        //    }

        //    result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //    if (!result.Succeeded)
        //    {
        //        return GetErrorResult(result);
        //    }
        //    return Ok();
        //}

        //[AllowAnonymous]
        //[Route("CreateRole")]
        //public async Task<IHttpActionResult> Create(RoleViewModel roleViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Initialize ApplicationRole instead of IdentityRole:
        //        var role = new ApplicationRole(roleViewModel.Name);
        //        var roleresult = await RoleManager.CreateAsync(role);
        //        if (roleresult.Succeeded)
        //        {
        //            return Ok();
        //        }
        //    }
        //    return BadRequest();
        //}

        //[HttpPost]
        //public async Task<IHttpActionResult> Edit(RoleViewModel roleModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var role = await RoleManager.FindByIdAsync(roleModel.Id);
        //        role.Name = roleModel.Name;

        //        // Update the new Description property:
        //        role.Description = roleModel.Description;
        //        await RoleManager.UpdateAsync(role);
        //        return Ok();
        //    }
        //    return BadRequest();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && _userManager != null)
        //    {
        //        _userManager.Dispose();
        //        _userManager = null;
        //    }

        //    base.Dispose(disposing);
        //}

        //[AllowAnonymous]
        //[Route("getUserRoles")]
        //[HttpPost]
        //public IHttpActionResult getUserRoles()
        //{
        //    var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new UnitOfWork()));
        //    var roles = roleManager.Roles.Select(a => new EnumerationModel
        //    {
        //        PositionNameID = a.Id.ToString(),
        //        PositionName = a.Name
        //    }).Where(r => r.PositionName.ToUpper().Trim() != "ADMIN").OrderBy(o => o.PositionName).ToList();
        //    return Ok(roles);
        //}

        //[AllowAnonymous]
        //[Route("getUserRolesBasedOnSignum")]
        //[HttpPost]
        //public IHttpActionResult getUserRolesBasedOnSignum(RegisterBindingModel model)
        //{
        //    IEnumerable<EnumerationModel> roles = null;
        //    var resourceInfo = _resManager.GetResourceIno(model.UserName);
        //    var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new UnitOfWork()));
        //    if (resourceInfo != null && resourceInfo.Manager.Trim().ToUpper() == "Y")
        //    {
        //        roles = roleManager.Roles.Select(a => new EnumerationModel
        //        {
        //            PositionNameID = a.Id.ToString(),
        //            PositionName = a.Name
        //        }).Where(r => r.PositionName.Trim().ToUpper() == "RESOURCE MANAGER").ToList();
        //    }
        //    if (resourceInfo != null && resourceInfo.Manager.Trim().ToUpper() != "Y")
        //    {
        //        roles = roleManager.Roles.Select(a => new EnumerationModel
        //        {
        //            PositionNameID = a.Id.ToString(),
        //            PositionName = a.Name
        //        }).Where(r => r.PositionName.Trim().ToUpper() == "READ ONLY").ToList();
        //    }
        //    return Ok(roles);
        //}

        //[Route("getAllUserRoles")]
        //[HttpPost]
        //public IHttpActionResult getAllUserRoles()
        //{
        //    var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new UnitOfWork()));
        //    var roles = roleManager.Roles.OrderBy(o => o.RoleRank).ThenBy(o => o.Name)
        //                .Select(a => new UserRoles
        //                {
        //                    Id = a.Id.ToString(),
        //                    Name = a.Name,
        //                    Rank = a.RoleRank.ToString()
        //                }).ToList();

        //    return Ok(roles);
        //}

        //[Route("getUserCustomer")]
        //[HttpPost]
        //public IHttpActionResult getUserCustomer()
        //{
        //    IEnumerable<CodeTableViewDTO> allCustomer = null;
        //    allCustomer = _CodetableManager.GetAllCustomers();

        //    List<comboBindingModel> userCustomer = (allCustomer == null
        //                                           ? new List<comboBindingModel>()
        //                                           : allCustomer
        //                                             .OrderBy(o => o.Value)
        //                                             .Select(s => new comboBindingModel
        //                                             {
        //                                                 ID = s.CodeTableId.ToString(),
        //                                                 Name = s.Value,
        //                                                 Description = s.Description
        //                                             })
        //                                             .ToList());

        //    return Ok(userCustomer);
        //}


        ////private IQueryable<UserModel> getAllUsers() //string SIGNUM
        ////{
        ////    IQueryable<UserModel> allUsers = null;

        ////    IdentityDbContext context = new IdentityDbContext("name=ROCKContext");
        ////    var userCollection = context.Users.ToList();
        ////    var allUsrs = new UserManager<User>(new UserStore<User>(new UnitOfWork()));
        ////    //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new UnitOfWork()));
        ////    var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new UnitOfWork()));


        ////    //var roles = roleManager.Roles.ToList();

        ////    allUsers = (from aspNetUser in allUsrs.Users.ToList()
        ////                join aspNetUserRoles in userCollection on aspNetUser.Id equals aspNetUserRoles.Id
        ////                //where (SIGNUM.Length <= 0) || (SIGNUM.Length > 0 && aspNetUser.UserName.Trim().ToUpper().Equals(SIGNUM.Trim().ToUpper()))
        ////                orderby aspNetUser.ApprovalStatus   //orderby aspNetUser.IsApproved
        ////                select new UserModel() {
        ////                      Id = aspNetUser.Id,
        ////                      UserName = aspNetUser.UserName,
        ////                      EmailId = aspNetUser.Email,
        ////                      ApprovalStatus = aspNetUser.ApprovalStatus, //IsApproved = aspNetUser.IsApproved,
        ////                      FirstName = aspNetUser.FirstName,
        ////                      LastName = aspNetUser.LastName,
        ////                      Role = (from usrRoles in aspNetUserRoles.Roles.ToList()
        ////                              join aspRoles in roleManager.Roles.ToList() on usrRoles.RoleId equals aspRoles.Id
        ////                              //join aspRoles in roles on usrRoles.RoleId equals aspRoles.Id
        ////                              orderby aspRoles.RoleRank
        ////                              select new UserRoles()
        ////                              {
        ////                                  Id = aspRoles.Id,
        ////                                  Name = aspRoles.Name,
        ////                                  Rank = aspRoles.RoleRank.ToString()
        ////                              }).ToList()
        ////                }).AsQueryable();

        ////    return allUsers;
        ////}



        //#region Helpers

        //private IAuthenticationManager Authentication
        //{
        //    get { return Request.GetOwinContext().Authentication; }
        //}

        //private IHttpActionResult GetErrorResult(IdentityResult result)
        //{
        //    if (result == null)
        //    {
        //        return InternalServerError();
        //    }

        //    if (!result.Succeeded)
        //    {
        //        if (result.Errors != null)
        //        {
        //            foreach (string error in result.Errors)
        //            {
        //                ModelState.AddModelError("", error);
        //            }
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            // No ModelState errors are available to send, so just return an empty BadRequest.
        //            return BadRequest();
        //        }

        //        return BadRequest(ModelState);
        //    }

        //    return null;
        //}

        //private class ExternalLoginData
        //{
        //    public string LoginProvider { get; set; }
        //    public string ProviderKey { get; set; }
        //    public string UserName { get; set; }

        //    public IList<Claim> GetClaims()
        //    {
        //        IList<Claim> claims = new List<Claim>();
        //        claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

        //        if (UserName != null)
        //        {
        //            claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
        //        }

        //        return claims;
        //    }

        //    public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
        //    {
        //        if (identity == null)
        //        {
        //            return null;
        //        }

        //        Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

        //        if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
        //            || String.IsNullOrEmpty(providerKeyClaim.Value))
        //        {
        //            return null;
        //        }

        //        if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
        //        {
        //            return null;
        //        }

        //        return new ExternalLoginData
        //        {
        //            LoginProvider = providerKeyClaim.Issuer,
        //            ProviderKey = providerKeyClaim.Value,
        //            UserName = identity.FindFirstValue(ClaimTypes.Name)
        //        };
        //    }
        //}

        //private static class RandomOAuthStateGenerator
        //{
        //    private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

        //    public static string Generate(int strengthInBits)
        //    {
        //        const int bitsPerByte = 8;

        //        if (strengthInBits % bitsPerByte != 0)
        //        {
        //            throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
        //        }

        //        int strengthInBytes = strengthInBits / bitsPerByte;

        //        byte[] data = new byte[strengthInBytes];
        //        _random.GetBytes(data);
        //        return HttpServerUtility.UrlTokenEncode(data);
        //    }
        //}

        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(User user, UserManager<User> manager, string authenticationType)
        //{
        //    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        //    var userIdentity = await manager.CreateIdentityAsync(user, authenticationType);
        //    // Add custom user claims here
        //    return userIdentity;
        //}

        //#endregion

        //private void SendPasswordResetLinkToMail(User model, string strHostPath)
        //{
        //    //string PwdResetLink = ConfigurationManager.AppSettings["PwdResetLink"] ?? "#";
        //    string PwdResetLink = strHostPath + "ResetPassword";

        //    try
        //    {
        //        //string passPhrase = "Pas5pr@se";        // can be any string
        //        //string saltValue = "s@1tValue";        // can be any string
        //        //string hashAlgorithm = "SHA1";             // can be "MD5"
        //        //int passwordIterations = 2;                // can be any number
        //        //string initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes
        //        //int keySize = 256;                // can be 192 or 128

        //        EMailer email = new EMailer();
        //        email.MailSubject = "NOVO [EXACT] | Password Reset Request | " + model.FirstName + " " + model.LastName;

        //        string body = string.Empty;
        //        using (StreamReader reader = new StreamReader(HostingEnvironment.MapPath("~/Templates/PasswordResetRequest.html")))
        //        {
        //            body = reader.ReadToEnd();
        //        }
        //        var deriveBytes = new Rfc2898DeriveBytes(model.UserName, 8);
        //        body = body.Replace("{FirstName}", model.FirstName);
        //        body = body.Replace("{LastName}", model.LastName);
        //        //string EncryptedPwd = EncryptionUtility.Encrypt(model.UserName, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize).Replace("==","$^")+"XT";                
        //        body = body.Replace("{ID}", model.Id);
        //        body = body.Replace("{destination}", PwdResetLink);
        //        email.MailBody = body;
        //        email.IsHTML = true;
        //        email.MailToList = new List<string>() { model.Email };
        //        //string decrypt = EncryptionUtility.Decrypt("hutKq9MQf6CpWVVCH5xJfg", passPhrase, saltValue, hashAlgorithm, 1, initVector, keySize);
        //        email.Send();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private void SendResetPasswordSuccessMail(User model, string strHostPath)
        //{
        //    try
        //    {
        //        EMailer email = new EMailer();
        //        email.MailSubject = "NOVO [EXACT] | Password Reset Complete | " + model.FirstName + " " + model.LastName;
        //        string body = string.Empty;
        //        //string appLoginURL = ConfigurationManager.AppSettings["eXactLogin"] ?? "#";
        //        string appLoginURL = strHostPath + "login";

        //        using (StreamReader reader = new StreamReader(HostingEnvironment.MapPath("~/Templates/PasswordResetSuccessful.html")))
        //        {
        //            body = reader.ReadToEnd();
        //        }
        //        body = body.Replace("{FirstName}", model.FirstName);
        //        body = body.Replace("{LastName}", model.LastName);
        //        body = body.Replace("{destination}", appLoginURL);
        //        email.MailBody = body;
        //        email.IsHTML = true;
        //        email.MailToList = new List<string>() { model.Email };
        //        email.Send();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}




    }
}
