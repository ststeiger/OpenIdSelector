

using System.Windows.Forms;


namespace ImageMagicSprite
{


    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
            ShellExecute("notepad.exe");
        }


        public static T GetPropertyValue<T>(object obj, string propertyName)
        {
            return (T) obj.GetType().InvokeMember(propertyName, System.Reflection.BindingFlags.GetProperty |
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public
                , null, obj, null
            );
        }


        public static void SetPropertyValue<T>(object obj, string propertyName, T val)
        {

            obj.GetType().InvokeMember(propertyName, System.Reflection.BindingFlags.SetProperty | 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public
                , null, obj, new object[] { val }
            );

        }


        public static void ShellExecute(string cmd)
        {

            System.Type t = System.Type.GetTypeFromProgID("WScript.Shell");
            // dynamic shell = System.Activator.CreateInstance(t);
            // dynamic exec = shell.Exec(cmd);

            object shell = System.Activator.CreateInstance(t);
            object exec = t.InvokeMember("Exec", System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { cmd });


            //while (exec.Status == 0)
            while (GetPropertyValue<int>(exec, "Status") == 0)
            {
                System.Threading.Thread.Sleep(100);
            } // Whend 

            System.Console.WriteLine("Finished execution");
        } // End Sub ShellExecute 


        public static void Shell()
        {
            // System.Type t2 = System.Type.GetTypeFromProgID("Shell.Application");
            // dynamic obj2 = System.Activator.CreateInstance(t2);
            // obj2.FileRun();

            System.Type t = System.Type.GetTypeFromProgID("Shell.Application");
            object obj = System.Activator.CreateInstance(t);
            t.InvokeMember("FileRun", System.Reflection.BindingFlags.InvokeMethod, null, obj, null);
        } // End Sub Shell 



        // https://code.google.com/archive/p/openid-selector/
        // https://github.com/krtek4/openid-selector
        // https://github.com/damusnet/openid-selector
        public static void ControlImageMagick()
        {
            var imagemagick = @"C:\Program Files\ImageMagick-6.6.5-Q16/";

            var locale = "en";
            /*
            if (WScript.Arguments.length == 0) 
            {
	            // assuming english locale
            } 
            else 
            {
	            locale = WScript.Arguments(0);
            }
            */

            string s = null;
            try
            {
                s = System.IO.File.ReadAllText("js/openid-" + locale + ".js");
            }
            finally
            {
            }

            /*
            var  openid = {};
            eval(s);

            if (openid.locale != locale)
            {
	            Console.WriteLine("error: locale setting mismatch in js/openid-" + locale + ".js");
	            System.Environment.Exit(-1);
            }
            
            if (openid.locale != openid.sprite) 
            {
	            Console.WriteLine("error: " + openid.locale + " localization is reusing sprite from " + openid.sprite 
			            + " localization. refresh sprite on original localization or don\"t reuse the sprite");
	            System.Environment.Exit(-1);
            }
            */
            // generate small montage
            string cmd = imagemagick + "montage";
            int i = 0;


            string[] providers_large = null;
            string[] providers_small = null;

            foreach (string provider_id in providers_large)
            {
                cmd += " images.small/" + provider_id + ".ico.png";
                i++;
            } // Next provider_id


            foreach (string provider_id in providers_small)
            {
                cmd += " images.small/" + provider_id + ".ico.png";
                i++;
            } // Next provider_id

            // http://stackoverflow.com/questions/944483/how-to-get-temporary-folder-for-current-user
            string tempPath = System.IO.Path.GetTempPath();

            //var small = fso.GetTempName() + ".bmp";
            var small = System.IO.Path.Combine(tempPath, System.Guid.NewGuid().ToString() + ".bmp");
            cmd += " -tile " + i + "x1 -geometry 16x16+4+4 " + small;
            exec(cmd);

            // generate large montage
            cmd = imagemagick + "montage";
            i = 0;
            foreach (string provider_id in providers_large)
            {
                cmd += " images.large/" + provider_id + ".gif";
                i++;
            } // Next provider_id

            //System.Environment.GetFolderPath(Environment.SpecialFolder.t
            //var large = fso.GetTempName() + ".bmp";
            var large = System.IO.Path.Combine(tempPath, System.Guid.NewGuid().ToString() + ".bmp");
            cmd += " -tile " + i + "x1 -geometry 100x60>+0+0 " + large;
            exec(cmd);

            // generate final montage
            cmd = imagemagick + "convert " + large + " " + small + " -append images/openid-providers-" + locale + ".png";
            exec(cmd);

            System.IO.File.Delete(large);
            System.IO.File.Delete(small);

            System.Console.WriteLine("done");
        } // End Sub ControlImageMagick


        public static void exec(string cmd)
        {
            //var shell = new ActiveXObject('WScript.Shell');
            System.Type t = System.Type.GetTypeFromProgID("WScript.Shell");
            // dynamic shell = System.Activator.CreateInstance(t);

            //var exec = shell.Exec(cmd);
            //while (exec.Status == 0)
            //{
            //    System.Threading.Thread.Sleep(100);
            //}

            // New way:
            //System.Diagnostics.Process process = System.Diagnostics.Process.Start("filename", "arguments");
            // process.WaitForExit();

            System.Console.WriteLine("Finished");
        } // End Sub exec


    }


}
