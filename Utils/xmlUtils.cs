using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Do_An_2.Model;

namespace Do_An_2.Utils
{
    class xmlUtils
    {
        static string pathXML = frmRename.XmlPath;
        static XmlDocument doc;
        static XmlNode root;

        static xmlUtils()
        {
            doc = new XmlDocument();
            doc.Load(pathXML);
            root = doc.DocumentElement;
        }
        public static void Save()
        {
            doc.Save(pathXML);
        }
        public static List<driveInfo> Drives()
        {
            var l = new List<driveInfo>();
            var ld = root.SelectNodes("Drive");
            foreach (XmlNode nd in ld)
            {
                l.Add(new driveInfo
                {
                    Name = nd.Attributes["name"].Value,
                    Type = nd.Attributes["type"].Value,
                    VolumeLabel = nd.Attributes["volumeLabel"]?.Value,
                    Node = nd
                });
            }
            return l;
        }
        public static List<folderInfo> Folders(ioInfo cur)
        {
            var node = cur.Node;
            var l = new List<folderInfo>();
            var lfo = node.SelectNodes("Folder");
            foreach (XmlNode nfo in lfo)
            {
                l.Add(new folderInfo
                {
                    Name = nfo.Attributes["name"].Value,
                    Node = nfo,
                    Parent = cur
                });
            }
            return l;
        }
        public static List<fileInfo> Files(ioInfo cur)
        {
            var node = cur.Node;
            var l = new List<fileInfo>();
            var lf = node.SelectNodes("File");
            foreach (XmlNode nf in lf)
            {
                l.Add(new fileInfo
                {
                    Name = nf.Attributes["name"].Value,
                    Ext = nf.Attributes["ext"].Value,
                    Size = long.Parse(nf.Attributes["size"].Value),
                    Node = nf,
                    Parent = cur
                });
            }
            return l;
        }

        public static string Getlocation(ioInfo cur, bool iscurrent)
        {
            string path = "";
            if (iscurrent)
                path = cur.Name;
            var curpar = cur?.Parent;
            while (curpar != null)
            {
                path = string.Format($@"{curpar.Name.Replace(@"\", "")}\{path}");
                curpar = curpar.Parent;
            }
            return path;
        }

        public static void GetRecursionFile(ioInfo target, ref List<fileInfo> outfile)
        {
            foreach (var fi in xmlUtils.Files(target))
            {
                outfile.Add(fi);
            }
            foreach (var foi in xmlUtils.Folders(target))
            {
                GetRecursionFile(foi, ref outfile);
            }
        }
    }
}
