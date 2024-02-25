using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Practicode2
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

        public HtmlElement()
        {
            Attributes = new List<string>();
            Classes = new List<string>();
            Children = new List<HtmlElement>();
        }



        public void PrintElement()
        {
            PrintElementHelper(this, 0);
        }

        private void PrintElementHelper(HtmlElement element, int indentLevel)
        {
            string indent = new string(' ', indentLevel * 4);
            Console.WriteLine($"{indent}<{element.Name} id=\"{element.Id}\" class=\"{string.Join(" ", element.Classes)}\">");

            foreach (var child in element.Children)
            {
                PrintElementHelper(child, indentLevel + 1);
            }

            Console.WriteLine($"{indent}</{element.Name}>");
        }
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                HtmlElement current = queue.Dequeue();
                yield return current;

                foreach (HtmlElement child in current.Children)
                {
                    queue.Enqueue(child);
                }

            }
        }
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this;
            while (current.Parent != null)
            {
                yield return current.Parent;
                current = current.Parent;
            }
        }
        public HashSet<HtmlElement> FindElements(Selector selector)
        {
            // return Descendants().Where(e => MatchSelector(e, selector)).ToList();
            //List<HtmlElement> elements = new List<HtmlElement>();
            var elements = new HashSet<HtmlElement>();
            //foreach (HtmlElement descendant in Descendants())
            //{
                FindElementsRecursively(this, selector, elements);
                //if (MatchSelector(descendant, selector))
                //{
                //    elements.Add(descendant);
                //}
            //}
            return elements;
        }
        private void FindElementsRecursively(HtmlElement element, Selector selector, HashSet<HtmlElement> results)
        {
            var descendants = element.Descendants();
            if (selector.Child == null)
            {
                // Recursively search in the children of the current element
                foreach (HtmlElement child in descendants)
                {
                    if(MatchSelector(child, selector))
                    {
                        results.Add(child);
                    }
                }
            }
            else
            {
                // Recursively search in the children of the current element
                foreach (HtmlElement child in descendants)
                {
                    FindElementsRecursively(child, selector.Child, results);
                }
            }
        }

        private bool MatchSelector(HtmlElement element, Selector selector)
        {
            if (selector.TagName != string.Empty && !string.Equals(element.Name, selector.TagName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (selector.Id != string.Empty && !string.Equals(element.Id, selector.Id, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (selector.Classes.Count >0  && !selector.Classes.All(c => element.Classes.Contains(c)))
            {
                return false;
            }

            return true;
        }
    }
}
