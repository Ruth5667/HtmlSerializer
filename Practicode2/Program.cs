using Practicode2;
using System.Text.RegularExpressions;


HtmlHelper helper = HtmlHelper.Instance;
HtmlElement e = helper.BuildTree(await helper.GetHtmlLinesFromUrl("https://hebrewbooks.org/beis"));
Selector s = SelectorHelper.GenerateSelectors("td#tdblog");
HashSet<HtmlElement> result = e.FindElements(s);
result.ToList().ForEach(g => Console.WriteLine($"{g.Name}  {g.Id}  {g.InnerHtml} "));

Console.WriteLine("------------------------------------");
HtmlElement e2 = helper.BuildTree(await helper.GetHtmlLinesFromUrl("https://netfree.link/#main"));
Selector s2 = SelectorHelper.GenerateSelectors("div#main");
HashSet<HtmlElement> result2 = e2.FindElements(s2);
result2.ToList().ForEach(g => Console.WriteLine($"{g.Name}  {g.Id}  {g.InnerHtml} "));





Console.ReadKey(); 