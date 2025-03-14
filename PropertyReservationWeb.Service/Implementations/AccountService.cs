using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.Domain.Helpers;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Domain.Response;
using PropertyReservationWeb.Domain.ViewModels.Account;
using PropertyReservationWeb.Service.Interfaces;

namespace PropertyReservationWeb.Service.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IJwtProvider _jwtprovider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(
            IBaseRepository<User> userRepository,
            IJwtProvider jwtprovider,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _jwtprovider = jwtprovider;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IBaseResponse<string>> Login(LoginViewModel model)
        {
            try
            {
                var user = await _userRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Email == model.Email);

                if (user == null)
                {
                    return new BaseResponse<string>
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.UserNotFound
                    };
                }

                if (user.Password != HashPasswordHelper.HashPassword(model.Password!))
                {
                    return new BaseResponse<string>
                    {
                        Description = "Неверный пароль или логин",
                        StatusCode = StatusCode.Unauthorized
                    };
                }

                var token = _jwtprovider.GenerateJwtToken(user);

                return new BaseResponse<string>
                {
                    Data = token,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<string>
                {
                    Description = "Ошибка сервера: " + ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<string>> Register(RegisterViewModel model)
        {
            try
            {
                var existingUser = await _userRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Email == model.Email);

                if (existingUser != null)
                {
                    return new BaseResponse<string>
                    {
                        Description = "Пользователь с таким email уже существует"
                    };
                }

                await _userRepository.Create(new User
                {
                    Email = model.Email!,
                    Password = HashPasswordHelper.HashPassword(model.Password!),
                    Role = Role.User,
                    DateOfRegistration = DateTime.Now
                });

                return new BaseResponse<string>
                {
                    Description = "Пользователь успешно зарегистрирован",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<string>
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<bool>> Logout(string email)
        {
            try
            {
                var user = await _userRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Email == email);

                if (user == null)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.UserNotFound,
                        Description = "Пользователь не найден"
                    };
                }

                var context = _httpContextAccessor.HttpContext;

                if (context == null || !context.User.Identity?.IsAuthenticated == true)
                {
                    return new BaseResponse<bool>
                    {
                        Data = false,
                        StatusCode = StatusCode.Unauthorized,
                        Description = "Вы не авторизованы"
                    };
                }

                context.Response.Cookies.Delete("my-cookies");

                return new BaseResponse<bool>
                {
                    Data = true,
                    StatusCode = StatusCode.OK,
                    Description = "Выход выполнен"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<bool>> ChangePassword(ChangePasswordViewModel model, string email)
        {
            try
            {
                var user = await _userRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Email == email);

                if (user == null)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.UserNotFound,
                        Description = "Пользователь не найден"
                    };
                }

                if(user.Password!= HashPasswordHelper.HashPassword(model.OldPassword!))
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.ErorPassword,
                        Description = "Старый пароль указан не верно"
                    };
                }

                user.Password = HashPasswordHelper.HashPassword(model.NewPassword!);
                await _userRepository.Update(user);

                return new BaseResponse<bool>
                {
                    Data = true,
                    StatusCode = StatusCode.OK,
                    Description = "Пароль успешно изменен"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
