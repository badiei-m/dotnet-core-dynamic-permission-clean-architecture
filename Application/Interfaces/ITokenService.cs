using Domain.Entities.System;

namespace Application.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}