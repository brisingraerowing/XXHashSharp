using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;
using Mono.Security.Cryptography;
using System.Security.Cryptography;
using XXHashSharp;
using System.IO;
using System.Reflection;

namespace HashCodeGen
{
    class Program
    {

        static string _(string msgid) { return msgid; } // Stub localization function.

        static OptionSet options;

        static void Main(string[] args)
        {

            options = new OptionSet();

            HashAlgorithm algorithm = null;
            bool isFileInput = false;
            bool showHelp = false;
            Encoding encoding = Encoding.Default;

            options.Add("a|algorithm=", _("Select the algorithm to use."), (s) => { algorithm = GetAlgorithm(s); });
            options.Add("f|file", _("The input is a file."), (s) => { isFileInput = true; });
            options.Add("s|string", _("The input is a string (default)"), (s) => { isFileInput = false; });
            options.Add("e|encoding", _("Valid only with string input.\nThe encoding used to get the bytes from the string.\nCan be either an encoding name or a codepage."), (s) => { encoding = GetEncoding(s); });
            options.Add("h|help", _("Display this help."), (s) => { showHelp = true; });
            

            IEnumerable<string> remaining = null;

            try
            {
                remaining = options.Parse(args);
            }
            catch (OptionException optEx)
            {
                Console.Error.WriteLine(_("ERROR: {0}: Option {1}"), optEx.Message, optEx.OptionName);
                Console.WriteLine();
                Usage();
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(_("ERROR: {0}"), ex.Message);
                if(ex.InnerException != null)
                {
                    Console.Error.WriteLine("\t{0}", ex.InnerException.Message);
                }
                Console.WriteLine();
                Usage();
                Environment.Exit(1);
            }

            if (showHelp)
            {
                Usage();
                Environment.Exit(0);
            }

            if(algorithm == null)
            {
                Console.Error.WriteLine(_("ERROR: Algorithm required."));
                Console.WriteLine();
                Usage();
                Environment.Exit(1);
            }

            if(!remaining.Any())
            {
                Console.Error.WriteLine(_("ERROR: {0} expected."), isFileInput ? _("file") : _("string"));
                Console.WriteLine();
                Usage();
                Environment.Exit(1);
            }

            byte[] input = null;

            if(isFileInput)
            {
                input = File.ReadAllBytes(remaining.First());
            }
            else
            {
                input = encoding.GetBytes(remaining.First());
            }

            byte[] hash = algorithm.ComputeHash(input);

            Console.WriteLine(BitConverter.ToString(hash).Replace("-", ""));

        }

        static Encoding GetEncoding(string input)
        {
            Encoding enc = null;
            int tmp;
            if(!int.TryParse(input, out tmp))
            {
                enc = Encoding.GetEncoding(input);
            }
            else
            {
                enc = Encoding.GetEncoding(tmp);
            }
            return enc;
        }

        static HashAlgorithm GetAlgorithm(string name)
        {
            HashAlgorithm alg = null;

            switch(name.ToLowerInvariant())
            {
                case "md2":
                    alg = new MD2Managed();
                    break;
                case "md4":
                    alg = new MD4Managed();
                    break;
                case "md5":
                    alg = new MD5CryptoServiceProvider();
                    break;
                case "sha1":
                    alg = new SHA1CryptoServiceProvider();
                    break;
                case "sha224":
                    alg = new SHA224Managed();
                    break;
                case "sha256":
                    alg = new SHA256CryptoServiceProvider();
                    break;
                case "sha384":
                    alg = new SHA384CryptoServiceProvider();
                    break;
                case "sha512":
                    alg = new SHA512CryptoServiceProvider();
                    break;
                case "xxhash32":
                    alg = new XXHash32();
                    break;
                case "xxhash64":
                    alg = new XXHash64();
                    break;
                default:
                    throw new ArgumentException(_("Invalid algorithm."));
            }
            return alg;
        }

        static void Usage()
        {
            Console.WriteLine("hashcode - Generate hash codes for files and strings.");
            Console.WriteLine();
            Console.WriteLine("Usage: hashcode -a ALGORITHM [-f|-s] <FILE_OR_STRING>");
            Console.WriteLine();
            Console.WriteLine("Options: ");
            options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
            Console.WriteLine("The available algorithms are: ");
            Console.WriteLine("\tMD2");
            Console.WriteLine("\tMD4");
            Console.WriteLine("\tMD5");
            Console.WriteLine("\tSHA1");
            Console.WriteLine("\tSHA224");
            Console.WriteLine("\tSHA256");
            Console.WriteLine("\tSHA384");
            Console.WriteLine("\tSHA512");
            Console.WriteLine("\tXXHASH32");
            Console.WriteLine("\tXXHASH64");
            Console.WriteLine();
            Console.WriteLine("The algorithms are case insensitive.");
            Console.WriteLine();
            Console.WriteLine("SECURITY WARNING: ");
            Console.WriteLine("The XXHASH32 and XXHASH64 algorithms are not cryptographically secure!");
            Console.WriteLine("DO NOT use them in cryptographic contexts (like hashing a password)!");
        }
    }
}
