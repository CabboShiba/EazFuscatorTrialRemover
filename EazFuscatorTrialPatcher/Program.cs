using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace EazFuscatorTrialRemover
{
    internal class Program
    {
        private static string File = null;
        private static string ExpiryDate = null;
        static void Main(string[] args)
        {
            Console.Title = "EazFuscatorTrialRemover 7 Days Trial Remover by Cabbo - https://github.com/CabboShiba";
            try
            {
                File = args[0];
            }
            catch (Exception ex)
            {
                Log("Please use Drag&Drop. Press enter to leave...");
                Console.ReadLine();
                Process.GetCurrentProcess().Kill();
            }
            try
            {
                object[] parameters = null;
                var assembly = Assembly.LoadFile(Path.GetFullPath(File));
                var paraminfo = assembly.EntryPoint.GetParameters();
                parameters = new object[paraminfo.Length];
                Harmony patch = new Harmony("EazFuscatorTrialRemover_https://github.com/CabboShiba");
                patch.PatchAll(Assembly.GetExecutingAssembly());
                assembly.EntryPoint.Invoke(null, parameters);
            }
            catch (Exception ex)
            {
                Log($"Could not load {File}\n{ex.Message}");
            }
            Console.ReadLine();
        }

        [HarmonyPatch(typeof(System.DateTime), nameof(System.DateTime.Parse), new[] { typeof(string), typeof(IFormatProvider), typeof(DateTimeStyles)})]
        class GetexpiryDate
        {
            static bool Prefix(string s, IFormatProvider provider, DateTimeStyles styles)
            {
                ExpiryDate = s;
                Log($"Expiry date: {DateTime.Parse(ExpiryDate)}");
                return true;
            }
        }
        [HarmonyPatch(typeof(System.DateTime), nameof(System.DateTime.UtcNow), MethodType.Getter)]
        class FixUtc
        {
            static bool Prefix(ref DateTime __result)
            {
                try
                {
                    DateTime ParseTime = DateTime.Parse(ExpiryDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                    __result = ParseTime.AddDays(-1);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Log($"Fixed date. New date: {__result}");
                    Console.ResetColor();
                    return false;

                }
                catch (Exception ex)
                {
                    Log($"Could not execute patch.\nError: {ex.Message}");
                }
                return true;
            }
        }
        public static void Log(string Data)
        {
            string Log = $"[EazTrialRemover] {Data}";
            Console.WriteLine(Log);
        }
    }
}
