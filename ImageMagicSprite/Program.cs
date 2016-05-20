
namespace ImageMagicSprite
{


    static class Program
    {


        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [System.STAThread]
        static void Main()
        {
            bool bShowWindow = false;

            if (bShowWindow)
            {
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                System.Windows.Forms.Application.Run(new Form1());
            }

            LocalizeSprites();
        }


        public static void LocalizeSprites()
        {
            string strContent = System.IO.File.ReadAllText(@"d:\stefan.steiger\documents\visual studio 2013\Projects\OpenIdSelector\OpenIdSelector\js\openid-en.js", System.Text.Encoding.UTF8);
            strContent = System.IO.File.ReadAllText(@"d:\stefan.steiger\documents\visual studio 2013\Projects\OpenIdSelector\OpenIdSelector\js\openid-de.js", System.Text.Encoding.UTF8);


            cRootObject localizations = TrySerialize(strContent);

            foreach (System.Collections.Generic.KeyValuePair<string, cProvider> provider in localizations.providers["providers_large"])
            {
                System.Console.WriteLine(provider.Key + ":");
                System.Console.WriteLine(provider.Value.label);
                System.Console.WriteLine(provider.Value.name);
                System.Console.WriteLine(provider.Value.url);
            }

            
            foreach (System.Collections.Generic.KeyValuePair<string, cProvider> provider in localizations.providers["providers_small"])
            {
                System.Console.WriteLine(provider.Key + ":");
                System.Console.WriteLine(provider.Value.label);
                System.Console.WriteLine(provider.Value.name);
                System.Console.WriteLine(provider.Value.url);
            }
            


            System.Console.WriteLine(localizations);
        }


        static string RemoveCstyleComments(string strInput)
        {
            string strPattern = @"/[*][\w\d\s]+[*]/";
            //strPattern = @"/\*.*?\*/"; // Doesn't work
            //strPattern = "/\\*.*?\\*/"; // Doesn't work
            //strPattern = @"/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/ "; // Doesn't work
            //strPattern = @"/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/ "; // Doesn't work

            // http://stackoverflow.com/questions/462843/improving-fixing-a-regex-for-c-style-block-comments
            strPattern = @"/\*(?>(?:(?>[^*]+)|\*(?!/))*)\*/";  // Works !

            string strOutput = System.Text.RegularExpressions.Regex.Replace(strInput, strPattern, string.Empty, System.Text.RegularExpressions.RegexOptions.Multiline);
            // System.Console.WriteLine(strOutput);
            return strOutput;
        } // End Function RemoveCstyleComments


        static string ReplaceVariables(string strInput)
        {
            string strPattern = @"var\s+providers_large(\s+)?=(\s+)?{(\s+)?";
            strInput = System.Text.RegularExpressions.Regex.Replace(strInput, strPattern, "\"providers_large\" : {" + System.Environment.NewLine, System.Text.RegularExpressions.RegexOptions.Multiline);

            strPattern = @"(\s+)?var\s+providers_small(\s+)?=(\s+)?{(\s+)?";
            strInput = System.Text.RegularExpressions.Regex.Replace(strInput, strPattern, ",   \"providers_small\" : {" + System.Environment.NewLine, System.Text.RegularExpressions.RegexOptions.Multiline);

            strPattern = @"}(\s+)?;(\s+)?";
            strInput = System.Text.RegularExpressions.Regex.Replace(strInput, strPattern, "}" + System.Environment.NewLine, System.Text.RegularExpressions.RegexOptions.Multiline);

            strPattern = @"$(\s+)?(\w+)(\s+)?:(\s+)?{";
            strInput = System.Text.RegularExpressions.Regex.Replace(strInput, strPattern, "\"$2\" : {", System.Text.RegularExpressions.RegexOptions.Multiline);

            strPattern = @"name(\s+)?:(\s+)?'";
            strInput = System.Text.RegularExpressions.Regex.Replace(strInput, strPattern, "\"name\" : '", System.Text.RegularExpressions.RegexOptions.Multiline);

            strPattern = @"url(\s+)?:(\s+)?'";
            strInput = System.Text.RegularExpressions.Regex.Replace(strInput, strPattern, "\"url\" : '", System.Text.RegularExpressions.RegexOptions.Multiline);

            strPattern = @"label(\s+)?:(\s+)?'";
            strInput = System.Text.RegularExpressions.Regex.Replace(strInput, strPattern, "\"label\" : '", System.Text.RegularExpressions.RegexOptions.Multiline);


            strInput = strInput.Replace("'", "\"");


            strPattern = "openid\\.locale.*";
            //strInput = System.Text.RegularExpressions.Regex.Replace(strInput, strPattern, "", System.Text.RegularExpressions.RegexOptions.Multiline);

            var ma = System.Text.RegularExpressions.Regex.Match(strInput, strPattern, System.Text.RegularExpressions.RegexOptions.Singleline);

            string[] localeConfigs = ma.Groups[0].Value.Replace("\r", "").Split(new char[] { '\n' }
                , System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < localeConfigs.Length; ++i)
            {
                int ind = localeConfigs[i].IndexOf("//");
                if(ind != -1)
                    localeConfigs[i] = localeConfigs[i].Substring(0, ind);

                ind = localeConfigs[i].IndexOf("=");
                if (ind != -1)
                    localeConfigs[i] = localeConfigs[i].Substring(0, ind) + " : "
                        + localeConfigs[i].Substring(ind+1);

                localeConfigs[i] = "," + localeConfigs[i].TrimStart().TrimEnd(new char[] { '\r', '\n', ' ', '\t', ';' }).Substring("openid.".Length);
            }

            string localeConfig = "\n,\"config\": {\n\"foo\": \"123\" \n" + string.Join("\n", localeConfigs) + "\n}\n";

            strInput = System.Text.RegularExpressions.Regex.Replace(strInput, strPattern, "", System.Text.RegularExpressions.RegexOptions.Singleline);
            strPattern = null;

            /*
            string[] astrTrailingComments = {
                             @"openid\.locale"
                            ,@"openid\.sprite"
                            ,@"openid\.demo_text"
                            ,@"openid\.signin_text"
                            ,@"openid\.image_title"
            };

            foreach (string strThisPattern in astrTrailingComments)
            {
                strInput = System.Text.RegularExpressions.Regex.Replace(strInput, strThisPattern + ".+", "", System.Text.RegularExpressions.RegexOptions.Multiline);
            } // Next strThisPattern
            */

            strInput = "{\n\"providers\": {" + strInput + "}" + localeConfig + "\n}\n";

            //Console.WriteLine(strInput);
            return strInput;
        } // End Function ReplaceVariables


        public class cProvider
        {
            public string name;
            public string label;
            public string url;
        }


        public class cProviders
        {
            public System.Collections.Generic.Dictionary<string, cProvider> providers_large;
            public System.Collections.Generic.Dictionary<string, cProvider> providers_small;
        }

        public class cConfig
        {
            public string foo;
            public string locale;
            public string sprite;
            public string demo_text;
            public string signin_text;
            public string image_title;
        }

        public class cRootObject
        {
            //public cProviders providers;
            public System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, cProvider>> providers;
            public cConfig config;
        }


        static cRootObject TrySerialize(string strInput)
        {
            strInput = RemoveCstyleComments(strInput);
            strInput = ReplaceVariables(strInput);

            cRootObject objScript = Newtonsoft.Json.JsonConvert.DeserializeObject<cRootObject>(strInput);
            

            /*
            foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.Dictionary<string, cProvider>> provider_group in objScript.providers)
            {
                System.Console.WriteLine(provider_group.Key + ":");
                foreach (System.Collections.Generic.KeyValuePair<string, cProvider> provider in provider_group.Value)
                {
                    System.Console.WriteLine(provider.Key + ":");
                    System.Console.WriteLine(provider.Value.label);
                    System.Console.WriteLine(provider.Value.name);
                    System.Console.WriteLine(provider.Value.url);
                }
            }
            */

            return objScript;
        } // End Function TrySerialize


    }


}
