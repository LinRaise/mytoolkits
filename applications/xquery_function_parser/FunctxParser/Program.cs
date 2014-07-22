using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace FunctxParser
{
    public static class Program
    {
        public static string OUTPUT_PATH = "D:\\ws\\esms\\svn\\devtest\\qryproc\\udf\\";
        public static string TESTCASE_FILE = "D:\\ws\\esms\\svn\\devtest\\qryproc\\udf_tests.seg";

        public static string UnescapeXML(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            string returnString = s;
            returnString = returnString.Replace("&apos;", "'");
            returnString = returnString.Replace("&quot;", "\"");
            returnString = returnString.Replace("&gt;", ">");
            returnString = returnString.Replace("&lt;", "<");
            returnString = returnString.Replace("&amp;", "&");

            return returnString;
        }

        static void Main(string[] args)
        {
            string[] funcListUrls = {
                                        //"http://www.xqueryfunctions.com/xq/c0008.html",
                                        //"http://www.xqueryfunctions.com/xq/c0011.html",
                                        //"http://www.xqueryfunctions.com/xq/c0002.html",
                                        //"http://www.xqueryfunctions.com/xq/c0013.html",
                                        //"http://www.xqueryfunctions.com/xq/c0015.html",
                                        "http://www.xqueryfunctions.com/xq/c0026.html",
                                        "http://www.xqueryfunctions.com/xq/c0033.html",
                                        "http://www.xqueryfunctions.com/xq/c0021.html",
                                        "http://www.xqueryfunctions.com/xq/c0023.html",
                                        "http://www.xqueryfunctions.com/xq/c0072.html"
                                    };

            foreach (string funcListUrl in funcListUrls)
            {
                FuncList[] funcLists = ParseFuncList(funcListUrl);
                foreach (FuncList funcList in funcLists)
                {
                    System.IO.StreamWriter testFile = new System.IO.StreamWriter(TESTCASE_FILE, true);
                    testFile.WriteLine("\n<!-- " + funcList.funcName + " - " + funcList.subcat + ": " + funcList.funclist.Length + " cases -->");
                    testFile.Close();
                    testFile = null;
                    foreach (string name in funcList.funclist)
                    {
                        string url = "http://www.xqueryfunctions.com/xq/functx_" + name + ".html";
                        Console.WriteLine("Parsing " + url + " ...");
                        ParseURL(funcList.funcName, funcList.subcat, url);
                    }
                }
            }
        }

        public class FuncList
        {
            public string funcName;
            public string subcat;
            public string[] funclist;
        }

        public static FuncList[] ParseFuncList(string url)
        {
            HtmlWeb webClient = new HtmlWeb();

            HtmlDocument doc = webClient.Load(url);

            List<FuncList> funcList = new List<FuncList>();

            string xpath = "//h1[@class='funcName']";
            HtmlNode node = doc.DocumentNode.SelectSingleNode(xpath);
            string funcName = node.InnerText.Trim();

            xpath = "//h2[@class='subcat']";
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(xpath);
            int i = 0;
            foreach (HtmlNode aNode in nodes)
            {
                FuncList list = new FuncList();
                list.funcName = funcName;
                list.subcat = aNode.InnerText.Trim();
                funcList.Add(list);
                HtmlNodeCollection nodes1 = aNode.NextSibling.SelectNodes("tr/td[1]/a");
                List<string> funcs = new List<string>();
                for (int j = 0; j < nodes1.Count; j++)
                {
                    string name = nodes1[j].InnerText.Trim();
                    if (name.StartsWith("functx:"))
                        funcs.Add(name.Replace("functx:", "").ToLower());
                }
                list.funclist = funcs.ToArray();
                i++;
            }

            doc = null;
            node = null;
            webClient = null;

            return funcList.ToArray();
        }

        public static void ParseURL(string family, string subcat, string url)
        {
            HtmlWeb webClient = new HtmlWeb();

            HtmlDocument doc = webClient.Load(url);

            // function name
            string xpath = "//h1[@class='funcName']";
            HtmlNode node = doc.DocumentNode.SelectSingleNode(xpath);
            string funcName = node.InnerText.Trim().Replace(':', '_');
            string qryFileName = funcName + ".xql";
            string refFileName = funcName + "_ref.xml";
            System.IO.StreamWriter qryFile = new System.IO.StreamWriter(OUTPUT_PATH + qryFileName);
            System.IO.StreamWriter refFile = new System.IO.StreamWriter(OUTPUT_PATH + refFileName);
            System.IO.StreamWriter testFile = new System.IO.StreamWriter(TESTCASE_FILE, true);
            Console.WriteLine("utf_tests.xml segement: ");
            Console.WriteLine("  <test filename=\"udf/" + qryFileName + "\">");
            testFile.WriteLine("  <test filename=\"udf/" + qryFileName + "\">");

            // function short summray
            xpath = "//h1[@class='funcName']/../p[2]/i";
            node = doc.DocumentNode.SelectSingleNode(xpath);
            Console.WriteLine("    <desc>" + node.InnerText.Trim() + "</desc>");
            testFile.WriteLine("    <desc>" + node.InnerText.Trim() + "</desc>");
            Console.WriteLine("    <case ref=\"udf/" + refFileName + "\" input=\"udf/dummy.xml\"/>");
            testFile.WriteLine("    <case ref=\"udf/" + refFileName + "\" input=\"udf/dummy.xml\"/>");
            Console.WriteLine("  </test>");
            testFile.WriteLine("  </test>");
            testFile.Close();
            testFile = null;

            // function description (it's actually not needed in this task)
            xpath = "//h2[text()='Description']/../p[1]";
            node = doc.DocumentNode.SelectSingleNode(xpath);

            // function syntax
            xpath = "//h2[text()='XQuery Function Declaration']/../table[2]/tr[1]/td";
            node = doc.DocumentNode.SelectSingleNode(xpath);
            int declIndex = -1;
            if (node == null)
            {
                declIndex = 2;
            } else if (node.InnerText.Trim().StartsWith("See XSLT definition"))
            {
                declIndex = 3;
            }
            xpath = "//h2[text()='XQuery Function Declaration']/../table[2]/tr[" + declIndex + "]/td/pre";
            node = doc.DocumentNode.SelectSingleNode(xpath);
            Console.WriteLine("\nQuery File Content: ");
            Console.WriteLine(UnescapeXML(node.InnerText.Trim()));
            Console.WriteLine("");
            qryFile.WriteLine(UnescapeXML(node.InnerText.Trim()));
            qryFile.WriteLine("");

            xpath = "//h2[text()='Examples']/following-sibling::*";
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(xpath);
            int tableCnt = 0;
            foreach (HtmlNode aNode in nodes)
            {
                if (aNode.Name == "table")
                    tableCnt++;
                else
                    break;
            }

            // examples
            if (tableCnt == 2)
            {
                xpath = "//h2[text()='Examples']/../table[3]/tr/td";
                nodes = doc.DocumentNode.SelectNodes(xpath);
                foreach (HtmlNode aNode in nodes)
                {
                    Console.WriteLine(UnescapeXML(aNode.InnerText.Trim()));
                    qryFile.WriteLine(UnescapeXML(aNode.InnerText.Trim()));
                }
            }
            else
            {
                Console.WriteLine("return");
            }

            xpath = "//h2[text()='Examples']/../table[" + (2 + tableCnt) + "]/tr/td[1]";
            nodes = doc.DocumentNode.SelectNodes(xpath);
            string xpath1 = "//h2[text()='Examples']/../table[" + (2 + tableCnt) + "]/tr/td[2]";
            HtmlNodeCollection nodes1 = doc.DocumentNode.SelectNodes(xpath1);
            Console.WriteLine("<o>");
            qryFile.WriteLine("<o>");
            refFile.WriteLine("<o>");
            int i = 1;
            foreach (HtmlNode aNode in nodes)
            {
                Console.Write("<e" + i + ">{");
                qryFile.Write("<e" + i + ">{");
                refFile.Write("<e" + i + ">");
                Console.Write(UnescapeXML(aNode.InnerText.Trim()));
                qryFile.Write(UnescapeXML(aNode.InnerText.Trim()));
                refFile.Write(nodes1.ElementAt(i - 1).InnerText.Trim());
                Console.WriteLine("}</e" + i + ">");
                qryFile.WriteLine("}</e" + i + ">");
                refFile.WriteLine("</e" + i + ">");
                i++;
            }
            Console.WriteLine("</o>");
            qryFile.WriteLine("</o>");
            refFile.WriteLine("</o>");

            doc = null;
            node = null;
            nodes = null;
            nodes1 = null;
            webClient = null;

            qryFile.Close();
            refFile.Close();
            qryFile = null;
            refFile = null;

            Console.WriteLine("Completed.");
        }
    }
}
