using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Users.Commands
{
    using MediatR;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Results;
    using Meritocious.Core.Interfaces;
    using Meritocious.Infrastructure.Data.Repositories;
    using BCrypt.Net;
    using Microsoft.Extensions.Logging;
    using Meritocious.Core.Events;
    using Meritocious.Core.Extensions;
    using Meritocious.Common.DTOs.Auth;

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<UserProfileDto>>
    {
        private readonly UserRepository userRepository;
        private readonly IMediator mediator;
        private readonly ILogger<RegisterUserCommandHandler> logger;

        public RegisterUserCommandHandler(
            UserRepository userRepository,
            IMediator mediator,
            ILogger<RegisterUserCommandHandler> logger)
        {
            this.userRepository = userRepository;
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task<Result<UserProfileDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Check for existing username
            if (await userRepository.GetByUsernameAsync(request.Username) != null)
            {
                return Result.Failure<UserProfileDto>("Username is already taken");
            }

            // Check for existing email
            if (await userRepository.GetByEmailAsync(request.Email) != null)
            {
                return Result.Failure<UserProfileDto>("Email is already registered");
            }

            // Hash password
            string passwordHash = BCrypt.EnhancedHashPassword(request.Password);

            // Create user
            var user = User.Create(request.Username, request.Email, passwordHash);
            await userRepository.AddAsync(user);

            // Publish event
            await mediator.Publish(new UserRegisteredEvent(user.Id), cancellationToken);

            return Result.Success(user.ToDto());
        }
    }
}