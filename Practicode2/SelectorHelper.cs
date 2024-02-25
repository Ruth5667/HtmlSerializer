using Practicode2;
using System.Text.RegularExpressions;

public class SelectorHelper
{
    public static Selector GenerateSelectors(string select)
    {
        string[] selsctors = new Regex(" ").Split(select);
        Selector root = new Selector();
        root.ParseSelector(selsctors[0]);
        Selector current = root;
        for (int i = 1; i < selsctors.Length; i++)
        {
            current.Child = new Selector();
            current.Child.Parent = current;
            current = current.Child;
            current.ParseSelector(selsctors[i]);

        }
        return root;
    }
}
