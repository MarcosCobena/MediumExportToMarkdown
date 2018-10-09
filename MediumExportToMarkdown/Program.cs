using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ReverseMarkdown;
using static ReverseMarkdown.Config;

namespace MediumExportToMarkdown
{
    class Program
    {
        const string DateFormat = "yyyy-mm-dd";
        const string HTMLExtension = ".html";
        const string ImportMessage = 
            "*(This post was imported, please [contact](#/contact) me if there's anything wrong with it. " +
            "Thanks in advance)*";
        const string OutputPath = "/Users/marcos/Repositorios/MediumExportToMarkdown/MediumExportToMarkdown"; //marcoscobena/items";

        static void Main(string[] args)
        {
            var posts = Directory.EnumerateFiles("../medium-export/posts", "*.html");

            foreach (var item in posts)
            {
                var rawFileName = Path.GetFileName(item);
                var dateAndUnderscoreLength = DateFormat.Length + 1;
                var fileName = rawFileName.Substring(dateAndUnderscoreLength)
                                          .Replace(HTMLExtension, string.Empty);

                var rawDate = rawFileName.Substring(0, DateFormat.Length);
                var date = DateTime.Parse(rawDate);

                var content = File.ReadAllText(item);
                var config = new Config(UnknownTagsOption.PassThrough);
                var markdownConverter = new Converter(config);
                var body = Extract("body", content);
                var markdown = markdownConverter.Convert(content);

                var title = Extract("title", content);

                markdown = markdown.Insert(0, $"{ImportMessage}\n\n");

                File.WriteAllText($"{OutputPath}{Path.DirectorySeparatorChar}{fileName}.md", markdown);

                Console.WriteLine($"addPost(\"{title}\", \"{fileName}\", \"{date.ToShortDateString()}\");");
            }
        }

        static string Extract(string tag, string content)
        {
            var value = Regex.Matches(content, $"<{tag}>(.+)</{tag}>")
                             .FirstOrDefault()
                             ?.Groups[1]
                             .Value;

            return value;
        }
    }
}
