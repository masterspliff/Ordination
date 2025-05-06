namespace ordination_test;
using System;
using System.Collections.Generic;

public class UserManager
{
    private Dictionary<string, string> users = new Dictionary<string, string>();
    private Dictionary<string, int> roles = new Dictionary<string, int>();

    public void AddUser(string u, string p, int r)
    {
        users[u] = p;
        roles[u] = r;
    }

    public bool Auth(string u, string p)
    {
        if (users.ContainsKey(u))
        {
            if (users[u] == p)
            {
                Console.WriteLine("User authenticated.");
                return true;
            }
        }
        return false;
    }

    public bool CanAccess(string u, string res)
    {
        if (!roles.ContainsKey(u))
            return false;

        int role = roles[u];

        if (res == "admin")
        {
            return role == 1;
        }
        else if (res == "editor")
        {
            return role == 1 || role == 2;
        }
        else
        {
            return true;
        }
    }
}