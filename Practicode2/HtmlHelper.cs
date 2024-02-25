using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Practicode2
{
    public class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public string[] AllHtmlTags { get; private set; }
        public string[] SelfClosingHtmlTags { get; private set; }
        public HtmlElement RootElement { get; private set; }

        private HtmlHelper()
        {
            InitializeHtmlTags();
        }
        private void InitializeHtmlTags()
        {
            AllHtmlTags = LoadTagsFromFile("tags/AllTags.json");
            SelfClosingHtmlTags = LoadTagsFromFile("tags/SelfClosingTags.json");
        }
        private string[] LoadTagsFromFile(string filePath)
        {
            try
            {
                // Read the content of the JSON file
                string jsonContent = File.ReadAllText(filePath);
                // Deserialize the JSON content to string array
                return JsonSerializer.Deserialize<string[]>(jsonContent);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading JSON file: {ex.Message}");
                return new string[0]; // Return an empty array in case of an error
            }
        }
        public async Task<List<string>> GetHtmlLinesFromUrl(string url)
        {
            var html = await Load(url);
            var cleanHtml = new Regex("\\s").Replace(html, " ");
            IEnumerable<string> htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => !string.IsNullOrWhiteSpace(s));
            return htmlLines.ToList();

        }

        async Task<string> Load(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            return html;
        }
        public HtmlElement BuildTree(List<string> htmlStrings)
        {
            HtmlElement rootElement = new HtmlElement();

            HtmlElement currentElement = rootElement;

            foreach (string htmlString in htmlStrings)
            {
                currentElement = ProcessHtmlString(htmlString, currentElement);
            }
            return rootElement;
        }
        private HtmlElement ProcessHtmlString(string htmlString, HtmlElement currentElement)
        {
            string remainingString = htmlString;

            while (!string.IsNullOrWhiteSpace(remainingString))
            {
                // Check if it's the end of the HTML
                if (remainingString.StartsWith("html/"))
                {
                    return currentElement;
                }

                // Check if it's a closing tag
                if (remainingString.StartsWith("/") && currentElement != null)
                {
                    if (currentElement?.Parent != null)
                    {
                        return currentElement.Parent;
                    }

                    //remainingString = remainingString.Substring(1).Trim();
                    remainingString = string.Empty;
                }
                else
                {
                    // Extract the first word from the string
                    string firstWord = Regex.Match(remainingString, @"\S+").Value;

                    // Check if it's a tag
                    if (AllHtmlTags.Contains(firstWord))
                    {
                        HtmlElement newElement = new HtmlElement
                        {
                            Name = firstWord
                        };

                        // Set parent
                        newElement.Parent = currentElement;

                        // Add to children
                        currentElement.Children.Add(newElement);

                        // Update current element
                        if (!SelfClosingHtmlTags.Contains(firstWord) && !remainingString.EndsWith("/"))
                        {
                            currentElement = newElement;
                        }
                        // Remove the processed part from the string
                        remainingString = remainingString.Substring(firstWord.Length).Trim();

                        // Handle attributes
                        var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(remainingString);
                        int endIndex = 0;

                        foreach (var attr in attributes)
                        {
                            string attributePart = attr.ToString();
                            if (attributePart.StartsWith("class="))
                            {
                                string[] classes = CleanAttributes(attributePart.Substring("class=".Length)).Split(' ');
                                classes.ToList().ForEach(x => x = CleanAttributes(x));
                                currentElement.Classes.AddRange(classes);
                            }
                            else if (attributePart.StartsWith("id="))
                            {
                                currentElement.Id = CleanAttributes(attributePart.Substring("id=".Length));

                            }
                            else
                            {
                                currentElement.Attributes.Add(attributePart);
                            }
                        }

                        remainingString = string.Empty;
                    }
                    else if (currentElement != null)
                    {
                        // It's inner text, update InnerHtml
                        currentElement.InnerHtml = remainingString;
                        return currentElement;

                    }
                    else
                    {
                        remainingString = string.Empty;
                    }
                }
            }

            return currentElement;
        }

        private string CleanAttributes(string attribute)
        {
            var endIndex = 0;
            if (attribute.EndsWith("\""))
            {
                endIndex = attribute.LastIndexOf("\"");
                attribute = attribute.Substring(0, endIndex);
            }
            if (attribute.EndsWith("\\"))
            {
                attribute = attribute.Substring(0, attribute.Length - 2);
            }
            if (attribute.StartsWith("\""))
            {
                attribute = attribute.Substring(1);
            }
            if (attribute.StartsWith("\\"))
            {
                attribute = attribute.Substring(2);
            }
            return attribute.Trim();
        }
    }
}