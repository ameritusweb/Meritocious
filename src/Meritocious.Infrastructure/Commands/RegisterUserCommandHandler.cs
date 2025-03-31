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

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<User>>
    {
        private readonly UserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(
            UserRepository userRepository,
            IMediator mediator,
            ILogger<RegisterUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Result<User>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Check for existing username
            if (await _userRepository.GetByUsernameAsync(request.Username) != null)
                return Result.Failure<User>("Username is already taken");

            // Check for existing email
            if (await _userRepository.GetByEmailAsync(request.Email) != null)
                return Result.Failure<User>("Email is already registered");

            // Hash password
            string passwordHash = BCrypt.EnhancedHashPassword(request.Password);

            // Create user
            var user = User.Create(request.Username, request.Email, passwordHash);
            await _userRepository.AddAsync(user);

            // Publish event
            await _mediator.Publish(new UserRegisteredEvent(user.Id), cancellationToken);

            return Result.Success(user);
        }
    }
}