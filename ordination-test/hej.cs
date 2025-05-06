namespace ordination_test;
using System;
using System.Collections.Generic;

/// <summary>
/// Håndterer brugeradministration og adgangskontrol
/// </summary>
public class UserManager
{
    // Rolle-enum for bedre type-sikkerhed og IDE-support
    public enum Role
    {
        Administrator = 1,
        Editor = 2,
        User = 3
    }

    // Ressource-konstanter for bedre læsbarhed
    public static class Resources
    {
        public const string Admin = "admin";
        public const string Editor = "editor";
    }

    private Dictionary<string, string> users = new Dictionary<string, string>();
    private Dictionary<string, Role> roles = new Dictionary<string, Role>();

    /// <summary>
    /// Tilføjer en ny bruger med angivet brugernavn, adgangskode og rolle
    /// </summary>
    /// <param name="username">Brugerens brugernavn</param>
    /// <param name="password">Brugerens adgangskode</param>
    /// <param name="role">Brugerens rolle</param>
    /// <exception cref="ArgumentException">Hvis brugernavn eller adgangskode er null eller tom</exception>
    public void AddUser(string username, string password, Role role)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentException("Brugernavn må ikke være tomt", nameof(username));
        
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Adgangskode må ikke være tom", nameof(password));

        users[username] = password;
        roles[username] = role;
    }

    /// <summary>
    /// Autentificerer en bruger med brugernavn og adgangskode
    /// </summary>
    /// <param name="username">Brugerens brugernavn</param>
    /// <param name="password">Brugerens adgangskode</param>
    /// <returns>True hvis autentifikation lykkedes, ellers false</returns>
    public bool Authenticate(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return false;

        if (users.TryGetValue(username, out string storedPassword))
        {
            return storedPassword == password;
        }
        
        return false;
    }

    /// <summary>
    /// Kontrollerer om en bruger har adgang til en bestemt ressource
    /// </summary>
    /// <param name="username">Brugerens brugernavn</param>
    /// <param name="resource">Ressourcen der ønskes adgang til</param>
    /// <returns>True hvis brugeren har adgang, ellers false</returns>
    public bool CanAccess(string username, string resource)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(resource))
            return false;

        if (!roles.TryGetValue(username, out Role role))
            return false;

        if (resource == Resources.Admin)
        {
            return role == Role.Administrator;
        }
        else if (resource == Resources.Editor)
        {
            return role == Role.Administrator || role == Role.Editor;
        }
        else
        {
            return true; // Alle brugere har adgang til andre ressourcer
        }
    }
}

/// <summary>
/// Test klasse for UserManager
/// </summary>
public class UserManagerTests
{
    /// <summary>
    /// Tester autentifikation med korrekte og forkerte oplysninger
    /// </summary>
    public static void TestAuthentication()
    {
        UserManager manager = new UserManager();
        manager.AddUser("admin", "admin123", UserManager.Role.Administrator);
        
        // Test med korrekte oplysninger
        bool success = manager.Authenticate("admin", "admin123");
        Console.WriteLine($"Authentication with correct credentials: {success}");
        
        // Test med forkert adgangskode
        bool failure = manager.Authenticate("admin", "wrongpass");
        Console.WriteLine($"Authentication with wrong password: {failure}");
        
        // Test med ikke-eksisterende bruger
        bool nonExistingUser = manager.Authenticate("unknown", "pass");
        Console.WriteLine($"Authentication with non-existing user: {nonExistingUser}");
    }
    
    /// <summary>
    /// Tester adgangskontrol for forskellige roller og ressourcer
    /// </summary>
    public static void TestAccessControl()
    {
        UserManager manager = new UserManager();
        manager.AddUser("admin", "pass1", UserManager.Role.Administrator);
        manager.AddUser("editor", "pass2", UserManager.Role.Editor);
        manager.AddUser("user", "pass3", UserManager.Role.User);
    }
    
    /// <summary>
    /// Kør alle tests
    /// </summary>
    public static void RunAllTests()
    {
        TestAuthentication();
        
        TestAccessControl();
    }
}
