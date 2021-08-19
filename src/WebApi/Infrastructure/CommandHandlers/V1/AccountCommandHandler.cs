using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Apis.V1;
using WebApi.Domain.Exceptions;
using WebApi.Domain.Users;
using WebApi.Infrastructure.Authentication.Jwt;
using WebApi.Infrastructure.Data;

namespace WebApi.Infrastructure.CommandHandlers.V1
{
    public class AccountLoginCommandHandler : IRequestHandler<LoginCommand, LoginDto>
    {
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;

        public AccountLoginCommandHandler(
            IJwtTokenGenerator tokenGenerator,
            ApplicationDbContext context,
            SignInManager<User> signInManager)
        {
            _tokenGenerator = tokenGenerator;
            _context = context;
            _signInManager = signInManager;
        }

        public async Task<LoginDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, false);
            if (result == SignInResult.Failed)
                throw new DomainException("Invalid email or password.");

            var query = await (from user in _context.Users
                               join department in _context.Departments on user.DepartmentId equals department.Id
                               where user.Email == request.Email
                               select new { user, department }).FirstOrDefaultAsync(cancellationToken);

            var token = _tokenGenerator.Generate(query.user, query.department.Type.ToString());

            return new LoginDto(token);
        }
    }

    public class AccountPasswordChangeCommandHandler : IRequestHandler<PasswordChangeCommand>
    {
        private readonly UserManager<User> _userManager;

        public AccountPasswordChangeCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Unit> Handle(PasswordChangeCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new DomainException("User could not be found");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
                return Unit.Value;

            throw new DomainException(string.Join(".", result.Errors.Select(e => e.Description)));
        }
    }

    public class UserNotificationSettingsUpdateCommandHandler : IRequestHandler<UserNotificationSettingsUpdateCommand>
    {
        private readonly ApplicationDbContext _context;

        public UserNotificationSettingsUpdateCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UserNotificationSettingsUpdateCommand request, CancellationToken cancellationToken)
        {
            var notificationSettings = await _context.UserNotificationSettings.FindAsync(request.UserId);

            notificationSettings ??= new UserNotificationSetting { UserId = request.UserId };
            notificationSettings.MutedDepartmentsNotificationIds = request.MutedDepartmentIds;

            if (_context.Entry(notificationSettings).State == EntityState.Detached)
                _context.UserNotificationSettings.Add(notificationSettings);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }

    public class UserNotificationSeenUpdateCommandHandler : IRequestHandler<UserNotificationSeenUpdateCommand>
    {
        private readonly ApplicationDbContext _context;

        public UserNotificationSeenUpdateCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UserNotificationSeenUpdateCommand request, CancellationToken cancellationToken)
        {
            UserNotification notification = new()
            {
                UserId = request.UserId,
                NotificationId = request.NotificationId,
                IsSeen = true
            };

            _context.Entry(notification).State = EntityState.Modified;
            _context.Entry(notification).Property(p => p.IsSeen).IsModified = true;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }

    public class UserDepartmentUpdateCommandHandler : IRequestHandler<UserDepartmentUpdateCommand, string>
    {
        private readonly ApplicationDbContext _context;

        public UserDepartmentUpdateCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(UserDepartmentUpdateCommand request, CancellationToken cancellationToken)
        {
            var newDepartment = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Type == request.DepartmentType, cancellationToken);
            if (newDepartment == null)
                throw new DomainException("Department could not be found.");

            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                throw new DomainException("User could not be found.");

            var oldDepartment = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == user.DepartmentId, cancellationToken);

            user.DepartmentId = newDepartment.Id;
            await _context.SaveChangesAsync(cancellationToken);

            return $"Department of user {user.Email} changed from {oldDepartment.Name} to {newDepartment.Name}";
        }
    }
}