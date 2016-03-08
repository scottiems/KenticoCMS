/// <summary>
/// Summary description for KenticoTagParser
/// </summary>
public class KenticoTagParser
{
    public string parseTags(string nodeId, string htmlString)
    {


        //var myData = new { FirstName = "Joe", LastName = "Dirt", NodeID = NodeID };
        //var templateString = @"A template to show fullname / NodeID - {{FirstName}} {{LastName}} / <font color='red'>{{NodeID}}</font>";
        //var html2 = Render.StringToString(templateString, myData);
        bool tagsExist = htmlString.Contains(ThinkgateTemplateBuilder.StartDelimiter);
        //Look for tags << >>
        while (tagsExist)
        {
            string currentTagFound = ThinkgateTemplateBuilder.FindTag(htmlString);
            if (!string.IsNullOrEmpty(currentTagFound))
            {
                // call replace method
                htmlString = ThinkgateTemplateBuilder.TagReplace(currentTagFound, htmlString, nodeId);
                if (!htmlString.Contains(ThinkgateTemplateBuilder.StartDelimiter))
                    tagsExist = false;

            }
            else
            {
                tagsExist = false;
            }
        }
        return htmlString;
    }
}