using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Fanfiction.Models;
using System.Linq;

namespace Fanfiction.Controllers
{
    public class AdminController : Controller
    {

        UserManager<IdentityUser> _userManager;
        public AdminController(UserManager<IdentityUser> manager)
        {
            _userManager = manager;
        }
        public async Task<IActionResult> Accounts()
        {
            if (!HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Account", "Identity", new { id = "Login" });
            List<User> users = new List<User>();
            foreach (var item in _userManager.Users.ToList())
            {
                User u = new User { Email = item.Email, ID = item.Id, Name = item.UserName};
                if (await _userManager.IsInRoleAsync(item, "blocked") == true)
                {
                    u.Role = "blocked";
                }
                else
                {
                    if (await _userManager.IsInRoleAsync(item, "admin") == true)
                    {
                        u.Role = "admin";
                    }
                    else
                    {
                        u.Role = "user";
                    }
                }
                users.Add(u);
            }
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Action(List<string> AreChecked, IFormCollection form)
        {
            foreach (var item in AreChecked)
            { 
                IdentityUser user = await _userManager.FindByIdAsync(item);
                if (form.Keys.Contains("delete"))
                {
                    if (!(await _userManager.IsInRoleAsync(user, "admin"))){
                        await _userManager.DeleteAsync(user);
                    }

                }
                if (form.Keys.Contains("block"))
                {

                    if (await _userManager.IsInRoleAsync(user, "blocked"))
                    {
                        await _userManager.RemoveFromRoleAsync(user, "blocked");
                    }
                    else if (!(await _userManager.IsInRoleAsync(user, "admin")))
                    {
                        await _userManager.AddToRoleAsync(user, "blocked");
                    }
                }
                else if (form.Keys.Contains("change"))
                {
                    if (user.UserName != User.Identity.Name && !(await _userManager.IsInRoleAsync(user, "blocked")))
                        if (await _userManager.IsInRoleAsync(user, "admin"))
                        {
                            await _userManager.AddToRoleAsync(user, "user");
                            await _userManager.RemoveFromRoleAsync(user, "admin");
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, "admin");
                            if (await _userManager.IsInRoleAsync(user, "user"))
                                await _userManager.RemoveFromRoleAsync(user, "user");
                        }

                }
                
            }
            return RedirectToAction("Accounts", "Admin");
        }
    }
}
