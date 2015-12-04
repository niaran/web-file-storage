using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebStorage.UI.Models
{
    public class WebStorageDoc
    {
        private Int32 byteLimitation;

        public WebStorageDoc()
        {
            byteLimitation = 100 * 1024;
        }

        public String FileName { get; set; }
        
        [AllowHtml]
        public String EditorContent { get; set; }

        public Int32? ParentId { get; set; }

        public void WriteToDocFile(String path, String _content)
        {
            /* 
            Задаем кодировку UTF8,
            может отобразить все выделенные символы Unicode и представлять 
            остальные символы в пременном количестве байтов
            */
            if (String.IsNullOrEmpty(_content))
            {
                _content = String.Empty;
            }
            Encoding u8 = Encoding.UTF8;            
            byte[] bytes = u8.GetBytes(_content);

            Int32 limit = Math.Min(bytes.Length, this.byteLimitation);

            using (Stream _editorFileSream = new FileStream(path, FileMode.Create))
            {
                _editorFileSream.Position = 0;
                _editorFileSream.Write(bytes, 0, limit);
            }
        }

        public String ReadDocFile(String path)
        {
            using (FileStream _editorFileSream = new FileStream(path, FileMode.OpenOrCreate))
            using (TextReader txtrdr = new StreamReader(_editorFileSream))
            {
                return txtrdr.ReadToEnd();
            }
        }
    }
}