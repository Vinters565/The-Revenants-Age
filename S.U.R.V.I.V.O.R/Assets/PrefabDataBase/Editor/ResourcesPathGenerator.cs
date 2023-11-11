using System.Text.RegularExpressions;
using Unity.VisualScripting;

namespace DataBase
{
    public class ResourcesPathGenerator
    {
        public string GenerateFromAssetPath(string assetPath)
        {
            var regex = new Regex($"^Assets/.*{PathsHelper.RESOURCES_DIRECTORY_NAME}/(?<relative_path>.+?)$",
                RegexOptions.RightToLeft);
            var matches = regex.Matches(assetPath);
            
            if (matches.Count == 0)
            {
                return string.Empty;
            }

            return matches[^1].Groups["relative_path"].Value;
        }
    }
}