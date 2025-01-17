using UserService.Domain.Interfaces.Entities.Role;
using UserService.Domain.Keycloak;

namespace UserService.Domain.Extensions;

/// <summary>
///     Class for control keycloak attributes in requests
/// </summary>
public static class KeycloakExtensions
{
    /// <summary>
    ///     Add one role to attributes
    /// </summary>
    /// <param name="keycloakAttributes">KeycloakAttributes class</param>
    /// <param name="key">Name of current attribute</param>
    /// <param name="roleName">Name of Role</param>
    /// <returns>this KeycloakAttributes class</returns>
    public static KeycloakAttributes AddRole(this KeycloakAttributes keycloakAttributes, string key,
        string roleName)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(roleName);

        keycloakAttributes.TryGetValue(key, out var value);

        if (value is not HashSet<string> existingRoles) existingRoles = [];

        existingRoles.Add(roleName);
        keycloakAttributes[key] = existingRoles;

        return keycloakAttributes;
    }

    /// <summary>
    ///     Add multiple roles to attributes
    /// </summary>
    /// <param name="keycloakAttributes">KeycloakAttributes class</param>
    /// <param name="key">Name of current attribute</param>
    /// <param name="roleNames">String names of roles</param>
    /// <returns>this KeycloakAttributes class</returns>
    public static KeycloakAttributes AddRoles(this KeycloakAttributes keycloakAttributes, string key,
        params string[] roleNames)
    {
        foreach (var roleName in roleNames) AddRole(keycloakAttributes, key, roleName);

        return keycloakAttributes;
    }

    /// <summary>
    ///     Add multiple roles to attributes
    /// </summary>
    /// <param name="keycloakAttributes">KeycloakAttributes class</param>
    /// <param name="key">Name of current attribute</param>
    /// <param name="roles">Collection of Roles (or roles' name providers)</param>
    /// <returns>this KeycloakAttributes class</returns>
    public static KeycloakAttributes AddRoles(this KeycloakAttributes keycloakAttributes, string key,
        IEnumerable<IRoleNameProvider> roles)
    {
        var stringRoles = roles.Select(role => role.GetRoleName()).ToArray();

        return AddRoles(keycloakAttributes, key, stringRoles);
    }

    /// <summary>
    ///     Add UserId to attributes
    /// </summary>
    /// <param name="keycloakAttributes">KeycloakAttributes class</param>
    /// <param name="key">Name of current attribute</param>
    /// <param name="userId">Id of user</param>
    /// <returns>this KeycloakAttributes class</returns>
    public static KeycloakAttributes AddUserId(this KeycloakAttributes keycloakAttributes, string key,
        long userId)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(userId);

        keycloakAttributes[key] = userId.ToString();
        return keycloakAttributes;
    }

    /// <summary>
    ///     Add password credential to attributes
    /// </summary>
    /// <param name="credentials">List of KeycloakCredentials</param>
    /// <param name="password">User's password (not hashed)</param>
    /// <returns>this KeycloakAttributes class</returns>
    public static List<KeycloakCredentials> AddPassword(this List<KeycloakCredentials> credentials, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        const string passwordType = "password";
        credentials.Add(new KeycloakCredentials
        {
            Type = passwordType,
            Value = password
        });

        return credentials;
    }
}