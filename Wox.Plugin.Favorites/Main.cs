using System.Diagnostics;
using System.Net;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using File = System.IO.File;

namespace Wox.Plugin.Favorites
{
    public class Elem
    {
        public string type { get; set; }
        public string to { get; set; }
        public string keywords { get; set; }
    }

    public class Favorites
    {
        public List<Elem> Elements { get; set; }
    }

    public class Main : IPlugin
    {
        private string _json;
        private Favorites _myFavorites;

        public List<Result> Query(Query query)
        {
            var result = new List<Result>();

            if (query.ActionParameters.Count > 0)
            {
                var str = query.ActionParameters[0].ToLower();

                foreach (var f in _myFavorites.Elements)
                {

                    if (f.keywords.ToLower().Contains(str))
                    {
                        result.Add(new Result()
                        {
                            Title = f.keywords.Substring(0, f.keywords.IndexOf(',')),
                            SubTitle = f.to,
                            IcoPath = GetIco(f.to),
                            Action = c =>
                            {
                                Process.Start(f.to);
                                return true;
                            }
                        });
                    }
                }
            }

            return result;
        }

        public void Init(PluginInitContext context)
        {
            _json = new StreamReader(@"Plugins\Wox.Plugin.Favorites\favorites.json").ReadToEnd();
            _myFavorites = JsonConvert.DeserializeObject<Favorites>(_json);
        }

        public string GetIco(string href)
        {
            var cacheFodler = Directory.GetCurrentDirectory() + @"\Plugins\Wox.Plugin.Favorites\icons";

            if (!href.Contains("://"))
                href = "http://" + href;

            var url = new Uri(href).Host;
            var path = string.Format(@"{0}\{1}.png", cacheFodler, url);
            var ico = new Uri(@"http://www.google.com/s2/favicons?domain=" + url);

            // Download the image file            
            if (!File.Exists(path))
                new WebClient().DownloadFile(ico, path);
            return path;
        }

    }
}
