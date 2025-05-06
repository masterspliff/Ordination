namespace ordination_test;
using System;
using System.Collections.Generic;

/// <summary>
/// Håndterer brugeradministration og adgangskontrol
/// </summary>
public class UserManager
{
    // Rolle-konstanter for bedre læsbarhed
    public static class Roles
    {
        public const int Administrator = 1;
        public const int Editor = 2;
        public const int User = 3;
    }

    // Ressource-konstanter for bedre læsbarhed
    public static class Resources
    {
        public const string Admin = "admin";
        public const string Editor = "editor";
    }

    private Dictionary<string, string> users = new Dictionary<string, string>();
    private Dictionary<string, int> roles = new Dictionary<string, int>();

    /// <summary>
    /// Tilføjer en ny bruger med angivet brugernavn, adgangskode og rolle
    /// </summary>
    /// <param name="username">Brugerens brugernavn</param>
    /// <param name="password">Brugerens adgangskode</param>
    /// <param name="role">Brugerens rolle (brug Roles-konstanter)</param>
    /// <exception cref="ArgumentException">Hvis brugernavn eller adgangskode er null eller tom</exception>
    public void AddUser(string username, string password, int role)
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

        if (!roles.TryGetValue(username, out int role))
            return false;

        if (resource == Resources.Admin)
        {
            return role == Roles.Administrator;
        }
        else if (resource == Resources.Editor)
        {
            return role == Roles.Administrator || role == Roles.Editor;
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
        manager.AddUser("admin", "admin123", UserManager.Roles.Administrator);
        
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
        manager.AddUser("admin", "pass1", UserManager.Roles.Administrator);
        manager.AddUser("editor", "pass2", UserManager.Roles.Editor);
        manager.AddUser("user", "pass3", UserManager.Roles.User);
        
        // Test administrator adgang
        Console.WriteLine($"Admin can access admin area: {manager.CanAccess("admin", UserManager.Resources.Admin)}");
        Console.WriteLine($"Admin can access editor area: {manager.CanAccess("admin", UserManager.Resources.Editor)}");
        
        // Test editor adgang
        Console.WriteLine($"Editor can access admin area: {manager.CanAccess("editor", UserManager.Resources.Admin)}");
        Console.WriteLine($"Editor can access editor area: {manager.CanAccess("editor", UserManager.Resources.Editor)}");
        
        // Test almindelig bruger adgang
        Console.WriteLine($"User can access admin area: {manager.CanAccess("user", UserManager.Resources.Admin)}");
        Console.WriteLine($"User can access editor area: {manager.CanAccess("user", UserManager.Resources.Editor)}");
        
        // Test adgang til andre ressourcer
        Console.WriteLine($"User can access public area: {manager.CanAccess("user", "public")}");
    }
    
    /// <summary>
    /// Kør alle tests
    /// </summary>
    public static void RunAllTests()
    {
        Console.WriteLine("Running authentication tests:");
        TestAuthentication();
        
        Console.WriteLine("\nRunning access control tests:");
        TestAccessControl();
    }
}
