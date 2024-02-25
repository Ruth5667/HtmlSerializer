using System.Text.RegularExpressions;

namespace Practicode2
{
    public class Selector
    {
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public string TagName { get; set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }
        public Selector()
        {
            Classes = new List<string>();
        }
        public void ParseSelector(string query)
        {
            string pattern = @"(?<tag>\w+)(?:#(?<id>\w+))?(?:\.(?<class>\w+))*";
            /*string patterntag = @"(?<tag>\w+)*";*/
            //string pattern = @"(?<tag>\w+)(?:#(?<id>\w+))?(?:\.(?<class>[a-zA-Z_][a-zA-Z0-9_-]*))*";
            //string pattern = @"(?<tag>\w +)(?:#(?<id>\w+))?(?:\.(?<class>[a-zA-Z_][a-zA-Z0-9_-]*))*";

            var result = Regex.Match(query, pattern);
            string tagName = result.Groups["tag"].Value;
            Id = result.Groups["id"].Value;
            string classes = string.Join(" ", result.Groups["class"].Captures.Cast<Capture>().Select(c => c.Value));
            string[] arr_class = classes.Split(" ",StringSplitOptions.RemoveEmptyEntries);
            if (HtmlHelper.Instance.AllHtmlTags.Contains(tagName) || HtmlHelper.Instance.SelfClosingHtmlTags.Contains(tagName))
            {
                TagName = tagName;
            }
            else
            {
                if (query.StartsWith("."))
                {
                    Classes.Add(tagName);
                }
                if (query.StartsWith("#"))
                {
                    Id = tagName;
                }
            }
            foreach (var item in arr_class)
            {
                Classes.Add(item);
            }
        }

      
    }
    
}
