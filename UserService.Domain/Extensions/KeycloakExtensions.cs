using UserService.Domain.Keycloak;

namespace UserService.Domain.Extensions;

public static class KeycloakExtensions
{
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

    public static KeycloakAttributes AddRoles(this KeycloakAttributes keycloakAttributes, string key,
        params string[] roleNames)
    {
        foreach (var roleName in roleNames) AddRole(keycloakAttributes, key, roleName);

        return keycloakAttributes;
    }

    public static KeycloakAttributes AddRoles(this KeycloakAttributes keycloakAttributes, string key,
        params object[] roles)
    {
        var stringRoles = roles.Select(role => role.ToString()!).ToArray();

        return AddRoles(keycloakAttributes, key, stringRoles);
    }

    public static KeycloakAttributes AddUserId(this KeycloakAttributes keycloakAttributes, string key,
        long userId)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(userId);

        keycloakAttributes[key] = userId.ToString();
        return keycloakAttributes;
    }

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