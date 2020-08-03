using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MailGroup
{
    public class Util
    {
        public readonly static Dictionary<USERTYPE, string> UserTypeDic = new Dictionary<USERTYPE, string>()
        {
            {USERTYPE.QQMAIL, "@qq.com" },
            {USERTYPE.HOTMAIL, "@hotmail.com" },
            {USERTYPE.GMAIL, "@gmail.com" },
            {USERTYPE.YAHOOMAIL, "@yahoo.com" },
            {USERTYPE.SOHUMAIL, "@sohu.com" },
            {USERTYPE.MSNMAIL, "@msn.com" },
            {USERTYPE.NATEMAIL, "@nate.com" },
            {USERTYPE.WY139MAIL, "@139.com" },
            {USERTYPE.WY163Mail, "@163.com" },
            {USERTYPE.WY126Mail, "@126.com" },
            {USERTYPE.SINAMAIL, "@sina" },
        };
        public readonly static Dictionary<USERTYPE, string> UserTypeFiles = new Dictionary<USERTYPE, string>()
        {
            {USERTYPE.OTHER, "other" },
            {USERTYPE.QQMAIL, "qq" },
            {USERTYPE.HOTMAIL, "hotmail" },
            {USERTYPE.GMAIL, "gmail" },
            {USERTYPE.YAHOOMAIL, "yahoo" },
            {USERTYPE.SOHUMAIL, "sohu" },
            {USERTYPE.MSNMAIL, "msn" },
            {USERTYPE.NATEMAIL, "nate" },
            {USERTYPE.WY139MAIL, "139wy" },
            {USERTYPE.WY163Mail, "163wy" },
            {USERTYPE.WY126Mail, "126wy" },
            {USERTYPE.SINAMAIL, "sina" },
        };

        public static readonly IEnumerable<USERTYPE> keys = UserTypeDic.Keys;
        
        private static Dictionary<USERTYPE, FileStream> fileStreams = new Dictionary<USERTYPE, FileStream>();
        private static Dictionary<USERTYPE, StreamWriter> filewriterStreams = new Dictionary<USERTYPE, StreamWriter>();


        private static Dictionary<USERTYPE, FileStream> GenrateFileStreamDic(string fileName)
        {
            string dirName = "Mail" + fileName;
            Directory.CreateDirectory(dirName);
            foreach (USERTYPE t in UserTypeFiles.Keys)
            {
                var stream = new FileStream(System.Environment.CurrentDirectory + "/" + dirName + "/" + UserTypeFiles[t] + fileName + ".txt", FileMode.Create);
                fileStreams.Add(t, stream);
            }
            return fileStreams;
        }

        public static Dictionary<USERTYPE, StreamWriter> GetFileStreamWriterDic(string fileName)
        {
            GenrateFileStreamDic(fileName);
            foreach (USERTYPE t in fileStreams.Keys)
            {
                var stream = new StreamWriter(fileStreams[t]);
                filewriterStreams.Add(t, stream);
            }

            return filewriterStreams;
        }


        public static void CloseStreams()
        {
            if (fileStreams.Count < 1) return;

            foreach (var s in filewriterStreams.Values)
            {
                s.Close();
            }
            foreach (var s in fileStreams.Values)
            {
                s.Close();
            }
            fileStreams = new Dictionary<USERTYPE, FileStream>();
            filewriterStreams = new Dictionary<USERTYPE, StreamWriter>();


        }

        public static USERTYPE GetUserType(string username)
        {
            if (username != null && username.Contains("@"))
            {
                foreach (USERTYPE k in keys)
                {
                    string value = UserTypeDic[k];
                    if (username.ToLower().Contains(value))
                    {
                        return k;
                    }
                }
                return USERTYPE.OTHER;
            }
            else
            {
                return USERTYPE.OTHER;
            }
        }

    }
}
