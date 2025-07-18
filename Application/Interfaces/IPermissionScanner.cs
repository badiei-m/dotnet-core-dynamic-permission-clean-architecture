namespace Application.Interfaces;

public interface IPermissionScanner
{
    IEnumerable<string> GetPermissionNames();
}