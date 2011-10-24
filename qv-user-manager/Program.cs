﻿using System;
using System.Collections.Generic;
using System.Linq;
using NDesk.Options;

namespace qv_user_manager
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var list = "";
            var add = "";
            var remove = "";
            var docs = "";
            var prefix = "";
            var version = false;
            var help = false;

            try
            {
                var p = new OptionSet {
                    { "l|list=", "List CALs or usernames to console [{CAL|DMS}]", v => list = v.ToLower() },
                    { "a|add=", "Add users or assign CALs [{CAL|DMS}]", v => add = v.ToLower() },
                    { "r|remove=", "Remove specified users or inactive CALs [{CAL|DMS}]", v => remove = v.ToLower() },
                    { "d|document=", "QlikView document(s) to perform actions on", v => docs = v.ToLower() },
                    { "p|prefix=", "Use specified prefix for all users and CALs", v => prefix = v },
                    { "V|version", "Show version information", v => version = v != null },
                    { "?|h|help", "Show usage information", v => help = v != null },
                };

                p.Parse(args);

                if (help || args.Length == 0)
                {
                    Console.WriteLine("Usage: qv-user-manager [options]");
                    Console.WriteLine("Handles QlikView CALs and DMS user authorizations.");
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                    p.WriteOptionDescriptions(Console.Out);
                    Console.WriteLine();
                    Console.WriteLine("Options can be in the form -option, /option or --long-option");
                    return;
                }

                if (version)
                {
                    Console.WriteLine("qv-user-manager 20111024\n");
                    Console.WriteLine("This program comes with ABSOLUTELY NO WARRANTY.");
                    Console.WriteLine("This is free software, and you are welcome to redistribute it");
                    Console.WriteLine("under certain conditions.\n");
                    Console.WriteLine("Code: git clone git://github.com/braathen/qv-user-manager.git");
                    Console.WriteLine("Home: <https://github.com/braathen/qv-user-manager>");
                    Console.WriteLine("Bugs: <https://github.com/braathen/qv-user-manager/issues>");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            // Split list of multiple documents
            var documents = new List<string>(docs.Split(new[] { ';', ',', '|', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));

            // Remove possible duplicate documents
            documents = documents.Distinct().ToList();

            var users = new List<string>();

            // Look for console redirection or piped data
            try
            {
                var isKeyAvailable = Console.KeyAvailable;
            }
            catch (InvalidOperationException)
            {
                string s;
                while ((s = Console.ReadLine()) != null)
                {
                    users.Add(prefix + s.Trim());
                }
            }

            // Remove possible duplicate users
            users = users.Distinct().ToList();

            switch (remove)
            {
                case "dms":
                    RemoveDms(documents, users);
                    break;
                case "cal":
                    RemoveCals();
                    break;
            }

            switch (add)
            {
                case "dms":
                    AddDms(documents, users);
                    break;
                case "cal":
                    AddCals(documents, users);
                    break;
            }

            switch (list)
            {
                case "dms":
                    ListDms(documents);
                    break;
                case "cal":
                    ListCals();
                    break;
            }
        }
    }
}
