using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Xml;

namespace signalGenerator2
{
    public class signalModel
    {
        public List<string> Args = new List<string>();
        public string AttributeEventName = "";
        public string UIClassName_caller = "";
        public string BaseClass = "";
    }
    public class TypeModel
    {
        public List<signalModel> SignalModels = new List<signalModel>();
        public string UIClassName_caller = "";
        public string BaseClass = "";
    }

    public partial class clsGtkCsParse
    {

        private static clsGtkCsParse _singleInstance = null;

        public string GtkFolder = "../../../Gtk/";
        public string GdkFolder = "../../../Gdk/";
        private string GtkSignalName = "signalGtk.xml";
        private string GdkSignalName = "signalGdk.xml";
        
        private string GtkPrefixName = "Gtk";   
        private string GdkPrefixName = "Gdk";

        public static clsGtkCsParse Instance()
        {
            if (_singleInstance == null) {
                _singleInstance = new clsGtkCsParse();
            }
            return _singleInstance;
        }

        public void _parseGtk()
        {
            _loadCs(GtkPrefixName);

            string saveFilePath = clsFile._getExePath_replace(GtkSignalName);

            _outPut(saveFilePath);
        }
        
        public void _parseGdk()
        {
            _loadCs(GdkPrefixName);

            string saveFilePath = clsFile._getExePath_replace(GdkSignalName);

            _outPut(saveFilePath);
        }     

        private void _loadCs(string prefix)
        {
            string GtkFolder = clsGtkCsParse.Instance().GtkFolder ;

            ArrayList csListArray = clsFile._getFileList(GtkFolder, "*.cs");

            if (csListArray.Count == 0)
            {
                Console.Write("csファイルがない");
                return;
            }

            List<string> classFileArray = new List<string>();
            foreach (string filePath in csListArray)
            {
                if (filePath._indexOf("Handler") != -1 || filePath._indexOf("Args") != -1)
                {

                }
                else
                {
                    classFileArray.Add(filePath);
                }
            }

            _parseGtkXML(classFileArray,prefix);
        }


        private List<TypeModel> TypeModelArray = null;
        private void _parseGtkXML(List<string>  classFileArray,string prefix)
        {

            TypeModelArray = new List<TypeModel>();
            
            foreach (string filePath in classFileArray)
            {

                string csContent = clsFile._load_static(filePath);
              
                string fileName = clsPath._getFileName(filePath);
                
                Console.WriteLine(fileName);

               // public class Button : Bin, IActionable, IWrapper, IActivatable

                string patarn0 = @"public class(.*?)\:(.*?)" + Environment.NewLine;
                ArrayList classArray = clsUtility._patarnMatch(csContent, patarn0, 
                    1, 2,RegexOptions.Singleline);

                if (classArray.Count < 2)
                {
                    Console.WriteLine("public class　例外 {0} {1}", fileName , classArray.Count);
                    if (csContent._indexOf("[Signal(") != -1)
                    {
                        Console.WriteLine("Signalあり　例外 {0} ", fileName);
                    }
                    continue;
                }

                string callerClassName = prefix + classArray[0].ToString().TrimStart().TrimEnd();
                string BaseClass = classArray[1].ToString();
    
                if (BaseClass._indexOf(",") != -1)
                {
                    ArrayList splitArray = BaseClass._split(",");
                    if (splitArray.Count > 0)
                    {
                        BaseClass = splitArray[0].ToString();
                    }
                }
                
                if (BaseClass._indexOf(":") > 1)
                {
                    ArrayList splitArray = BaseClass._split(":");
                    if (splitArray.Count > 0)
                    {
                        BaseClass = splitArray[0].ToString();
                    }
                }
                
                if (callerClassName._indexOf("{") > 0)
                {
                    ArrayList splitArray = callerClassName._split("{");
                    if (splitArray.Count > 0)
                    {
                        callerClassName = splitArray[0].ToString().TrimEnd();
                    }
                }   

                BaseClass = prefix + BaseClass.TrimStart().TrimEnd();

                TypeModel TypeModel1 = new TypeModel();

                TypeModel1.BaseClass = BaseClass;
                TypeModel1.UIClassName_caller = callerClassName;

                string patarn = @"\[Signal\(\"".*?add.*?base.AddSignalHandler\((.*?)"+ Environment.NewLine;
                ArrayList signalArray = clsUtility._patarnMatch(csContent, patarn, 1, 
                    -1,RegexOptions.Singleline);
               
                List<signalModel> signalModelArray = new List<signalModel>();

                foreach (string signalStr in signalArray)
                {
                    signalModel signalModel1 = new signalModel();
                    signalModel1.BaseClass = BaseClass;
                    signalModel1.UIClassName_caller = callerClassName;

                    string patarn2 = @"\""(.*?)\""(.*?)";
                    ArrayList addSignalArray = clsUtility._patarnMatch(signalStr, patarn2, 1, 2,RegexOptions.Singleline);

                    if (addSignalArray.Count != 2)
                    {
                        Console.WriteLine("addSignalArray　例外 {0} {1}",signalStr , addSignalArray.Count);
                        continue;
                    }

                    signalModel1.AttributeEventName = _convertEventName(addSignalArray[0].ToString());
                    string argsStr = addSignalArray[1].ToString();

                    List<string> Args = new List<string>();
                    if (signalStr._indexOf("typeof") != -1)
                    {
                        // base.AddSignalHandler("format-entry-text", value, typeof(FormatEntryTextArgs));
                        string patarn3 = @"typeof\((.*?)\)\)\;";
                        ArrayList typeOfArray = clsUtility._patarnMatch(signalStr, patarn3, 1, -1,RegexOptions.Singleline);
                        if (typeOfArray.Count != 1)
                        {
                            Console.WriteLine("typeOfArray　例外 {0} {1}",signalStr , typeOfArray.Count);
                            continue;
                        }
                        Args.Add("object sender");
                        string args_new = prefix + "." + typeOfArray[0].ToString().TrimEnd().TrimStart() + " e";
                        Args.Add(args_new);
                    } else
                    {
                        Args.Add("object sender");
                        Args.Add("EventArgs e");  
                    }

                    signalModel1.Args = Args;
                    signalModelArray.Add(signalModel1);
                }

                string patarn5 = @"\[DefaultSignalHandler.*?ConnectionMethod(.*?)" + Environment.NewLine;
                ArrayList signalArray2 = clsUtility._patarnMatch(csContent, patarn5, 1, -1,RegexOptions.Singleline);
 
                foreach (string signalStr in signalArray2)
                {
                    // ConnectionMethod = "OverrideIconPress")]
       
                    string patarn6 = @"\""(.*?)\""" ;
                    ArrayList signalArray3 = clsUtility._patarnMatch(signalStr, patarn6, 1, -1,RegexOptions.Singleline);

                    foreach (string signalStr2 in signalArray3)
                    {
                        signalModel signalModel1 = new signalModel();
                        signalModel1.BaseClass = BaseClass;
                        signalModel1.UIClassName_caller = callerClassName;

                        signalModel1.AttributeEventName = _convertEventName(signalStr2.ToString());

                        List<string> Args = new List<string>();
                        Args.Add("object sender");
                        Args.Add("EventArgs e");
                        signalModel1.Args = Args;
                        signalModelArray.Add(signalModel1);
                    }
                }       

                TypeModel1.SignalModels = signalModelArray;
                TypeModelArray.Add(TypeModel1);
            }
        }

    }
}