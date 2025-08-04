using AutoMapper;
using FoodRoasterServer.Constants;
using FoodRoasterServer.Data;
using FoodRoasterServer.DTOs.Roaster;
using FoodRoasterServer.Exceptions.UserExceptions;
using FoodRoasterServer.Models;
using FoodRoasterServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace FoodRoasterServer.Services
{
    public class RoasterService : IRoasterService
    {
        private readonly IGenericRepository<UserMealRegistration> _genericRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        public RoasterService(IGenericRepository<UserMealRegistration> genericRepository, IMapper mapper, AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<RoasterRegisterResponseDTO>> RegisterToWeeklyRoaster(RoasterRegisterDTO dto)
        { 
            var userId = GetUserId();
            try
            {
                foreach (var meal in dto.Meals)
                {
                    var registration = new UserMealRegistration
                    {
                        UserId = userId,
                        FoodMenuId = meal.FoodMenuId,
                        RegisteredAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsVegChoice = meal.IsVegChoice
                    };
                    await _context.UserMealRegistrations.AddAsync(registration);
                }
                await _context.SaveChangesAsync();

                // Now retrieve the newly inserted records with FoodMenu loaded
                var userMealRegistrations = await _context.UserMealRegistrations
                    .Where(r => r.UserId == userId && dto.Meals.Select(m => m.FoodMenuId).Contains(r.FoodMenuId))
                    .Include(r => r.FoodMenu)
                        .ThenInclude(f => f.FoodItems)
                    .ToListAsync();
                var insertedRegistrations = _mapper.Map<List<RoasterRegisterResponseDTO>>(userMealRegistrations);
                return insertedRegistrations;
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UX_UserMealRegistration_UserId_FoodMenuId") == true)
            {
                throw new UseRegisteredForFoodException(AppMessages.Validation.UserAlreadyRegistered);
            }
        }

        public async Task<List<WeeklyUserRoasterDTO>> GetUserWeeklyRoaster(DateTime startDate, DateTime endDate)
        {
            var userId = GetUserId();
            var registrations = await _context.UserMealRegistrations
                  .Where(r => r.UserId == userId &&
                              r.FoodMenu.MenuDate >= startDate &&
                              r.FoodMenu.MenuDate <= endDate)
                  .Include(r => r.FoodMenu)
                      .ThenInclude(fm => fm.FoodItems)
                  .ToListAsync();

            var weekRoasterRegistrations =  _mapper.Map<List<WeeklyUserRoasterDTO>>(registrations);
            return await UpdateWeeklyRoasterResponseWithHolidays(weekRoasterRegistrations, startDate, endDate);
        }

        public async Task<List<FoodMenu>> GetWeeklyRoaster(DateTime startDate, DateTime endDate)
        {
            var foodMenus = await _context.FoodMenus
                  .Where(r =>
                              r.MenuDate >= startDate &&
                              r.MenuDate <= endDate)
                  .Include(r => r.FoodItems)
                  .ToListAsync();

                    if (!foodMenus.Any())
                    {
                        var holidays = await _context.Holidays
                            .Where(h => h.HolidayDate >= startDate && h.HolidayDate <= endDate)
                            .ToListAsync();

                        // Return a dummy FoodMenu entry for each holiday
                        var holidayMenus = holidays.Select(h => new FoodMenu
                        {
                            MenuDate = h.HolidayDate,
                            FoodItems = new List<FoodItem>(), // Empty since it's a holiday
                                                              // Optionally include a flag or comment field if you have one
                                                              // e.g., Reason = "Holiday"
                        }).ToList();

                        return holidayMenus;
                    }
            return foodMenus;
        }

        public async Task<WeeklyUserRoasterDTO> UpdateWeeklyRoaster(UpdateWeeklyRoasterDTO reqObj)
        {
            ValidateMenuDate(reqObj.MenuDate);
            return await HandleFoodMenuRegistration(reqObj);
        }
        public async Task RemoveWeeklyRoasterByDate(DeleteWeeklyRoasterDTO reqObj)
        {
            var targetDate = reqObj.MenuDate;
            var userId = GetUserId();
            var registrations = await _context.UserMealRegistrations
                .Include(r => r.FoodMenu)
                .Where(r => r.UserId ==userId &&
                            r.FoodMenu.MenuDate.Date == targetDate)
                .ToListAsync();

            if (registrations.Any())
            {
                _context.UserMealRegistrations.RemoveRange(registrations);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<List<DailyRoasterRegistrationStats>> GetRoasterWeeklyStats(DateTime startDate)
        {
            var endDate = startDate.AddDays(6);

            // Get all registrations in the week with related menu info
            var registrations = await _context.UserMealRegistrations
                .Include(r => r.FoodMenu)
                .Where(r => r.FoodMenu != null && r.FoodMenu.MenuDate >= startDate && r.FoodMenu.MenuDate <= endDate)
                .ToListAsync();

            // get holiday dates and reasons from Holiday table
            var holidays = await _context.Holidays
                .Where(h => h.HolidayDate >= startDate && h.HolidayDate <= endDate)
                .ToDictionaryAsync(h => h.HolidayDate.Date, h => h.Reason);

            var result = new List<DailyRoasterRegistrationStats>();

            for (int i = 0; i < 7; i++)
            {
                var currentDate = startDate.AddDays(i);

                var dayRegs = registrations
                    .Where(r => r.FoodMenu.MenuDate.Date == currentDate.Date)
                    .ToList();

                holidays.TryGetValue(currentDate.Date, out var holidayReason);

                result.Add(new DailyRoasterRegistrationStats
                {
                    Date = currentDate,
                    TotalRegistrations = dayRegs.Count,
                    VegCount = dayRegs.Count(r => r.IsVegChoice),
                    NonVegCount = dayRegs.Count(r => !r.IsVegChoice),
                    IsHoliday = holidayReason != null,
                    HolidayReason = holidayReason
                });
            }

            return result;

        }

        private int GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim);
        }

        private async Task<List<WeeklyUserRoasterDTO>> UpdateWeeklyRoasterResponseWithHolidays(List<WeeklyUserRoasterDTO> roasterList, DateTime startDate, DateTime endDate)
        {

            // Get holidays within the date range
            var holidays = await _context.Holidays
                .Where(h => h.HolidayDate >= startDate && h.HolidayDate <= endDate)
                .ToListAsync();

            // Add any missing days (no registration but might be a holiday)
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                // If user has not registered for this day
                if (!roasterList.Any(r => r.RegisteredAt.Date == date))
                {
                    var holiday = holidays.FirstOrDefault(h => h.HolidayDate.Date == date);
                    if (holiday != null)
                    {
                        roasterList.Add(new WeeklyUserRoasterDTO
                        {
                            RegisteredAt = date,
                            UpdatedAt = date,
                            IsVegChoice = false,
                            FoodMenuId = 0,
                            IsHoliday = holiday != null,
                            HolidayReason = holiday?.Reason
                        });
                    }
                }
                else
                {
                    // Update DTO with holiday info if it's also a holiday
                    var existing = roasterList.First(r => r.RegisteredAt.Date == date);
                    var holiday = holidays.FirstOrDefault(h => h.HolidayDate.Date == date);
                    if (holiday != null)
                    {
                        existing.IsHoliday = true;
                        existing.HolidayReason = holiday.Reason;
                    }
                }
            }
            // Sort by date ascending
            return roasterList.OrderBy(r => r.RegisteredAt).ToList();
        }

        public async Task<List<WeeklyUserRoasterDTO>> UpdateWeeklyRoaster(List<UpdateWeeklyRoasterDTO> reqObj)
        {
            var updatedList = new List<WeeklyUserRoasterDTO>();

            foreach (var item in reqObj)
            {
                ValidateMenuDate(item.MenuDate);

                var registrationResult = await HandleFoodMenuRegistration(item);
                if (registrationResult != null)
                {
                    updatedList.Add(registrationResult);
                }
            }

            return updatedList;
        }

        private void ValidateMenuDate(DateTime menuDate)
        {
            var today = DateTime.UtcNow.Date;

            int daysFromMonday = (int)today.DayOfWeek - (int)DayOfWeek.Monday;
            if (daysFromMonday < 0) daysFromMonday += 7;

            var startOfCurrentWeek = today.AddDays(-daysFromMonday);
            var endOfCurrentWeek = startOfCurrentWeek.AddDays(5);

            var startOfNextWeek = startOfCurrentWeek.AddDays(7);
            var endOfNextWeek = startOfNextWeek.AddDays(5);

            bool isValidWeek = (menuDate >= startOfCurrentWeek && menuDate <= endOfCurrentWeek) ||
                               (menuDate >= startOfNextWeek && menuDate <= endOfNextWeek);

            if (!isValidWeek)
                throw new ArgumentException(AppMessages.Validation.WeeklyRoasterUpdateValidation);
        }

        private async Task<WeeklyUserRoasterDTO?> HandleFoodMenuRegistration(UpdateWeeklyRoasterDTO item)
        {
            var menuDate = item.MenuDate.Date;
            var foodMenu = await _context.FoodMenus.FirstOrDefaultAsync(f => f.MenuDate.Date == menuDate);
            var userId = GetUserId();

            if (foodMenu != null)
            {
                var registration = await _context.UserMealRegistrations
                    .FirstOrDefaultAsync(r => r.FoodMenuId == foodMenu.Id && r.UserId == userId);

                if (registration != null)
                {
                    registration.IsVegChoice = item.IsVegChoice;
                    registration.UpdatedAt = DateTime.UtcNow;
                    _context.UserMealRegistrations.Update(registration);
                }
                else
                {
                    registration = new UserMealRegistration
                    {
                        FoodMenuId = foodMenu.Id,
                        UserId = userId,
                        IsVegChoice = item.IsVegChoice,
                        RegisteredAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _context.UserMealRegistrations.AddAsync(registration);
                }

                await _context.SaveChangesAsync();

                return new WeeklyUserRoasterDTO
                {
                    Id = registration.Id,
                    FoodMenuId = foodMenu.Id,
                    RegisteredAt = registration.RegisteredAt,
                    UpdatedAt = registration.UpdatedAt,
                    IsVegChoice = registration.IsVegChoice,
                    FoodMenu = foodMenu,
                    IsHoliday = false,
                    HolidayReason = null
                };
            }

            // Handle holidays
            var holiday = await _context.Holidays.FirstOrDefaultAsync(h => h.HolidayDate.Date == menuDate);
            if (holiday != null)
            {
                return new WeeklyUserRoasterDTO
                {
                    Id = 0,
                    FoodMenuId = 0,
                    RegisteredAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsVegChoice = item.IsVegChoice,
                    FoodMenu = null,
                    IsHoliday = true,
                    HolidayReason = holiday.Reason
                };
            }

            return null;
        }


        public (DateTime StartDate, DateTime EndDate) ValidateGivenPeriod(string period)
        {
            period = string.IsNullOrWhiteSpace(period) ? "current-week" : period.ToLower();
            DateTime startDate;
            DateTime endDate;
            DateTime today = DateTime.Today;

            switch (period)
            {
                case "current-week":
                    int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                    startDate = today.AddDays(-diff);
                    endDate = startDate.AddDays(5); // Only Monday to Saturday
                    break;

                case "previous-week":
                    int prevDiff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                    endDate = today.AddDays(-prevDiff - 1);
                    startDate = endDate.AddDays(-5); // Only 6 days
                    break;

                default:
                    throw new ArgumentException(AppMessages.Errors.InvalidRoasterQueryParams);
            }

            return (startDate, endDate);
        }


        public void ValidateForPassedDays(List<UpdateWeeklyRoasterDTO> reqBody)
        {
            DateTime today = DateTime.Today;
            DateTime now = DateTime.Now;
            var invalidDates = reqBody.Where(item =>
                item.MenuDate.Date < today || 
                (item.MenuDate.Date == today && now.TimeOfDay > new TimeSpan(9, 0, 0))
            ).ToList();

            if (invalidDates.Any()) throw new InvalidOperationException(AppMessages.Errors.InvalidDateModifications);
        }
    }
}
