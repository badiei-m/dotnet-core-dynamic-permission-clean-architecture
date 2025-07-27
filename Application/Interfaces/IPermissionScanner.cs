using Application.DTOs;

namespace Application.Interfaces;

public interface IPermissionScanner
{
    IEnumerable<PermissionDto> GetPermissionNames();
}