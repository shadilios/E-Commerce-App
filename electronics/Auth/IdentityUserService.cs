﻿using Electronics.Auth.Interfaces;
using Electronics.Auth.Model;
using Electronics.Auth.Model.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Electronics.Auth
{
    public class IdentityUserService : IUserService
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;

        public IdentityUserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> SignInMngr)
        {
            _userManager = userManager;
            _signInManager = SignInMngr;
        }

        public async Task<UserDTO> Register(RegisterDTO registerDTO, ModelStateDictionary modelstate)
        {
            if (registerDTO.Username == null || registerDTO.Password == null || registerDTO.Email == null)
            {
                return null;
            }

            var user = new ApplicationUser
            {
                UserName = registerDTO.Username,
                Email = registerDTO.Email
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
            {
                // here goes the roles specifications ... 
                
                
                return new UserDTO
                {
                    Username = user.UserName,
                };
            }

            //collect all the errors using the modelState
            foreach (var error in result.Errors)
            {
                var errorKey =
                    error.Code.Contains("Password") ? "Password Issue" :
                    error.Code.Contains("Email") ? "Email Issue" :
                    error.Code.Contains("UserName") ? nameof(registerDTO.Username) :
                    "";

                modelstate.AddModelError(errorKey, error.Description);
            }
            return null;
        }

        public async Task<UserDTO> Authenticate(string username, string password)
        {
            //to be fixed
            if (username == null || password == null)
            {
                return null;
            }
                var result = await _signInManager.PasswordSignInAsync(username, password, true, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(username);
                    return new UserDTO
                    {
                        Username = user.UserName
                    };
                }


            return null;
        }

        public async Task<UserDTO> GetUser(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            return new UserDTO
            {
                Username = user.UserName
            };
        }
    }
}
