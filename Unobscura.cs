
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

/// <summary>
/// Provides the entry point and main logic for the command-line tool that renames and maps CSS class and ID tokens in
/// HTML and CSS files, producing rewritten files and mapping outputs.
/// </summary>
/// <remarks>
/// This class is intended to be used as a console application. 
/// It parses command-line arguments to determine input and output file paths, processes the specified HTML and CSS files to identify class and ID tokens,
/// and generates new, stable names for selected tokens based on configurable rules. The rewritten HTML and CSS files,
/// along with JSON mapping files for classes and IDs, are written to the specified output directory. 
/// 
/// The tool is designed to help un-obfuscate to re-standardize class and ID names, such as for privacy, education for designers, code minimization, or deployment
/// scenarios. For usage details and supported arguments, run the application with no parameters or refer to the project
/// documentation.
/// </remarks>
public partial class Unobscura : UnobscuraBase {
    public static int Main(string[] args) {
        try {
            Options options = Options.Parse(args);

            string html = File.ReadAllText(options.HtmlPath, Encoding.UTF8);
            string css = File.ReadAllText(options.CssPath, Encoding.UTF8);

            // Extract class + id tokens from both HTML and CSS
            HashSet<string> htmlClasses = ExtractHtmlClassTokens(html);
            HashSet<string> cssClasses = ExtractCssClassTokens(css);

            HashSet<string> htmlIds = ExtractHtmlIdTokens(html);
            HashSet<string> cssIds = ExtractCssIdTokens(css);

            // Build mappings
            Dictionary<string, string> classMap = BuildMapping(
                candidates: htmlClasses.Concat(cssClasses),
                prefix: options.ClassPrefix,
                mode: options.Mode,
                shouldRename: ShouldRenameClassOrGeneric);

            Dictionary<string, string> idMap = BuildMapping(
                candidates: htmlIds.Concat(cssIds),
                prefix: options.IdPrefix,
                mode: options.Mode,
                shouldRename: ShouldRenameId);

            // Rewrite HTML
            string newHtml = RewriteHtmlClasses(html, classMap);
            newHtml = RewriteHtmlIds(newHtml, idMap);
            newHtml = RewriteHashRefs(newHtml, idMap); // optional but usually helpful

            // Rewrite CSS
            string newCss = RewriteCssClasses(css, classMap);
            newCss = RewriteCssIds(newCss, idMap);

            // Write outputs
            Directory.CreateDirectory(options.OutDir);

            string outHtml = Path.Combine(options.OutDir, "output.html");
            string outCss = Path.Combine(options.OutDir, "output.css");
            string outClassMap = Path.Combine(options.OutDir, "class-map.json");
            string outIdMap = Path.Combine(options.OutDir, "id-map.json");

            File.WriteAllText(outHtml, newHtml, Encoding.UTF8);
            File.WriteAllText(outCss, newCss, Encoding.UTF8);

            File.WriteAllText(outClassMap,
                JsonSerializer.Serialize(OrderByKey(classMap), new JsonSerializerOptions { WriteIndented = true }),
                Encoding.UTF8);

            File.WriteAllText(outIdMap,
                JsonSerializer.Serialize(OrderByKey(idMap), new JsonSerializerOptions { WriteIndented = true }),
                Encoding.UTF8);

            Console.WriteLine("Done.");
            Console.WriteLine($"HTML        : {outHtml}");
            Console.WriteLine($"CSS         : {outCss}");
            Console.WriteLine($"Class map   : {outClassMap}");
            Console.WriteLine($"ID map      : {outIdMap}");
            Console.WriteLine($"Renamed     : {classMap.Count} classes, {idMap.Count} ids");

            return 0;
        } catch (Exception ex) {
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    /// <summary>
    /// Determines whether the specified token should be renamed based on its format and the provided renaming mode.
    /// </summary>
    /// <remarks>Tokens with common icon prefixes (such as "fa-" or "bi-") or those considered meaningful are
    /// not renamed. When the mode is GuidOnly, only tokens matching GUID patterns are considered for renaming;
    /// otherwise, tokens matching GUID or hash-like patterns may be renamed.</remarks>
    /// <param name="token">The token to evaluate for renaming. Cannot be null, empty, or consist only of whitespace.</param>
    /// <param name="mode">The renaming mode that influences the criteria used to decide if the token should be renamed.</param>
    /// <returns>true if the token meets the criteria for renaming according to the specified mode; otherwise, false.</returns>
    private static bool ShouldRenameClassOrGeneric(string token, RenameMode mode) {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        // preserve common icon prefixes etc.
        if (token.StartsWith("fa-", StringComparison.OrdinalIgnoreCase))
            return false;
        if (token.StartsWith("bi-", StringComparison.OrdinalIgnoreCase))
            return false;

        // If it's already meaningful, keep it.
        if (Meaningful.IsMatch(token))
            return false;

        if (mode == RenameMode.GuidOnly) {
            return Guid36.IsMatch(token) || Guid32.IsMatch(token);
        }

        // default: GUID or hashy opaque
        if (Guid36.IsMatch(token) || Guid32.IsMatch(token))
            return true;
        if (Hashy.IsMatch(token))
            return true;

        return false;
    }

    /// <summary>
    /// Determines whether the specified identifier token should be renamed based on its format and the provided
    /// renaming mode.
    /// </summary>
    /// <remarks>Webflow node identifiers are always considered for renaming, as they affect grid placement
    /// rules. For other tokens, generic renaming rules are applied based on the provided mode.</remarks>
    /// <param name="token">The identifier token to evaluate for renaming. Cannot be null or whitespace.</param>
    /// <param name="mode">The renaming mode that influences the criteria used to decide if the token should be renamed.</param>
    /// <returns>true if the token meets the criteria for renaming according to its format and the specified mode; otherwise,
    /// false.</returns>
    private static bool ShouldRenameId(string token, RenameMode mode) {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        // Always rename Webflow node ids (these drive grid placement rules)
        if (token.StartsWith("w-node-", StringComparison.OrdinalIgnoreCase))
            return true;
        if (WebflowNodeId.IsMatch(token))
            return true;

        // otherwise fall back to generic rules
        return ShouldRenameClassOrGeneric(token, mode);
    }

    /// <summary>
    /// Generates a mapping from original names to new, uniquely generated names for a set of candidates, using the
    /// specified prefix and renaming mode.
    /// </summary>
    /// <remarks>Generated names are guaranteed to be unique within the returned mapping. The mapping is
    /// stable for a given set of inputs, ensuring consistent results between runs.</remarks>
    /// <param name="candidates">The collection of candidate names to consider for renaming. Only names for which <paramref name="shouldRename"/>
    /// returns <see langword="true"/> will be processed.</param>
    /// <param name="prefix">The prefix to use when generating new names. This value is prepended to each new name.</param>
    /// <param name="mode">The renaming mode that determines how names are selected for renaming. Passed to <paramref name="shouldRename"/>
    /// for each candidate.</param>
    /// <param name="shouldRename">A function that determines whether a given candidate name should be renamed, based on the name and the specified
    /// renaming mode.</param>
    /// <returns>A dictionary mapping each original name that was selected for renaming to its newly generated, unique name. The
    /// dictionary is empty if no candidates are selected for renaming.</returns>
    private static Dictionary<string, string> BuildMapping(
        IEnumerable<string> candidates,
        string prefix,
        RenameMode mode,
        Func<string, RenameMode, bool> shouldRename) {

        List<string> toRename = candidates
            .Where(t => shouldRename(t, mode))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(t => t, StringComparer.Ordinal)
            .ToList();

        HashSet<string> usedNewNames = new HashSet<string>(StringComparer.Ordinal);
        Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.Ordinal);

        int counter = 1;
        foreach (string oldName in toRename) {
            // stable suffix helps keep names consistent between runs
            string hashSuffix = ShortStableHash(oldName, 6); // e.g. "a1b2c3"
            string baseName = $"{prefix}{counter:000}_{hashSuffix}";

            string newName = baseName;
            int bump = 1;
            while (usedNewNames.Contains(newName)) {
                newName = $"{baseName}_{bump}";
                bump++;
            }

            usedNewNames.Add(newName);
            map[oldName] = newName;
            counter++;
        }

        return map;
    }

    /// <summary>
    /// Generates a short, stable hexadecimal hash string for the specified input using SHA-256.
    /// </summary>
    /// <remarks>The returned hash is deterministic for the same input and length. This method is suitable for
    /// generating compact, stable identifiers but is not intended for cryptographic security purposes.</remarks>
    /// <param name="input">The input string to be hashed. Cannot be null.</param>
    /// <param name="hexChars">The number of hexadecimal characters to include in the returned hash string. Must be greater than zero and less
    /// than or equal to 64.</param>
    /// <returns>A lowercase hexadecimal string of length up to <paramref name="hexChars"/> representing the hash of the input.</returns>
    private static string ShortStableHash(string input, int hexChars) {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = SHA256.HashData(bytes);

        StringBuilder sb = new StringBuilder(hexChars);
        for (int i = 0; sb.Length < hexChars && i < hash.Length; i++) {
            sb.Append(hash[i].ToString("x2"));
        }

        if (sb.Length > hexChars)
            sb.Length = hexChars;
        return sb.ToString();
    }

    /// <summary>
    /// Returns a new dictionary containing the entries of the specified map, ordered by key using ordinal string
    /// comparison.
    /// </summary>
    /// <remarks>The returned dictionary uses ordinal string comparison for key lookups. The input dictionary
    /// is not modified.</remarks>
    /// <param name="map">The dictionary whose entries are to be ordered by key. Cannot be null.</param>
    /// <returns>A dictionary containing the same key-value pairs as the input map, ordered by key according to ordinal string
    /// comparison.</returns>
    private static Dictionary<string, string> OrderByKey(Dictionary<string, string> map) =>
         map.OrderBy(k => k.Key, StringComparer.Ordinal)
            .ToDictionary(k => k.Key, v => v.Value, StringComparer.Ordinal);
    

    private static HashSet<string> ExtractHtmlClassTokens(string html) {
        HashSet<string> result = new HashSet<string>(StringComparer.Ordinal);

        foreach (Match m in HtmlClassAttr.Matches(html)) {
            string v = m.Groups["v"].Value;
            foreach (string token in SplitClassValue(v)) {
                if (!string.IsNullOrWhiteSpace(token)) {
                    result.Add(token);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Extracts all unique HTML element ID attribute values from the specified HTML markup.
    /// </summary>
    /// <remarks>ID attribute values are extracted using a regular expression and are trimmed of leading and
    /// trailing whitespace. Comparison is case-sensitive. The method does not validate whether the IDs conform to HTML
    /// specifications.</remarks>
    /// <param name="html">The HTML markup to search for element ID attributes. Cannot be null.</param>
    /// <returns>A set of strings containing all unique ID attribute values found in the HTML. The set will be empty if no IDs
    /// are present.</returns>
    private static HashSet<string> ExtractHtmlIdTokens(string html) {
        HashSet<string> result = new HashSet<string>(StringComparer.Ordinal);

        foreach (Match m in HtmlIdAttr.Matches(html)) {
            string v = m.Groups["v"].Value.Trim();
            if (!string.IsNullOrWhiteSpace(v)) {
                result.Add(v);
            }
        }

        return result;
    }

    /// <summary>
    /// Extracts all unique CSS class names from the specified CSS string.
    /// </summary>
    /// <remarks>Class names are extracted from standard CSS class selectors (e.g., ".my-class"). The
    /// comparison is case-sensitive and uses ordinal string comparison.</remarks>
    /// <param name="css">The CSS source text to search for class selectors. Cannot be null.</param>
    /// <returns>A set containing the distinct class names found in the input CSS. The set will be empty if no class selectors
    /// are present.</returns>
    private static HashSet<string> ExtractCssClassTokens(string css) {
        HashSet<string> result = new HashSet<string>(StringComparer.Ordinal);

        foreach (Match m in CssClassSelector.Matches(css)) {
            string c = m.Groups["c"].Value.Trim();
            if (!string.IsNullOrWhiteSpace(c)) {
                result.Add(c);
            }
        }

        return result;
    }

    /// <summary>
    /// Extracts all unique CSS ID selector tokens from the specified CSS string.
    /// </summary>
    /// <remarks>ID selector tokens are extracted from patterns matching the CSS ID selector syntax (e.g.,
    /// '#myId'). Comparison is case-sensitive using ordinal string comparison.</remarks>
    /// <param name="css">The CSS source text to search for ID selectors. Cannot be null.</param>
    /// <returns>A set of strings containing all unique ID selector tokens found in the input CSS. The set will be empty if no ID
    /// selectors are present.</returns>
    private static HashSet<string> ExtractCssIdTokens(string css) {
        HashSet<string> result = new HashSet<string>(StringComparer.Ordinal);

        foreach (Match m in CssIdSelector.Matches(css)) {
            string i = m.Groups["i"].Value.Trim();
            if (!string.IsNullOrWhiteSpace(i)) {
                result.Add(i);
            }
        }

        return result;
    }

    private static IEnumerable<string> SplitClassValue(string classValue) {
        return classValue
            .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim());
    }

    private static string RewriteHtmlClasses(string html, Dictionary<string, string> classMap) {
        string evaluator(Match m) {
            string full = m.Value;
            string v = m.Groups["v"].Value;

            List<string> tokens = SplitClassValue(v).ToList();
            for (int i = 0; i < tokens.Count; i++) {
                if (classMap.TryGetValue(tokens[i], out string? renamed)) {
                    tokens[i] = renamed;
                }
            }

            string newV = string.Join(" ", tokens);
            return ReplaceGroupValue(full, v, newV);
        }

        return HtmlClassAttr.Replace(html, new MatchEvaluator(evaluator));
    }

    private static string RewriteHtmlIds(string html, Dictionary<string, string> idMap) {
        string evaluator(Match m) {
            string full = m.Value;
            string v = m.Groups["v"].Value;

            if (idMap.TryGetValue(v, out string? renamed)) {
                return ReplaceGroupValue(full, v, renamed);
            }

            return full;
        }

        return HtmlIdAttr.Replace(html, new MatchEvaluator(evaluator));
    }

    private static string RewriteHashRefs(string html, Dictionary<string, string> idMap) {
        string evaluator(Match m) {
            string full = m.Value;
            string v = m.Groups["v"].Value; // "#someId"

            if (v.Length > 1) {
                string raw = v.Substring(1);
                if (idMap.TryGetValue(raw, out string? renamed)) {
                    string newV = "#" + renamed;
                    return ReplaceGroupValue(full, v, newV);
                }
            }

            return full;
        }

        return HtmlHashRefAttr.Replace(html, new MatchEvaluator(evaluator));
    }

    private static string RewriteCssClasses(string css, Dictionary<string, string> classMap) {
        string evaluator(Match m) {
            string c = m.Groups["c"].Value;
            if (classMap.TryGetValue(c, out string? renamed)) {
                return "." + renamed;
            }
            return m.Value;
        }

        return CssClassSelector.Replace(css, new MatchEvaluator(evaluator));
    }

    private static string RewriteCssIds(string css, Dictionary<string, string> idMap) {
        string evaluator(Match m) {
            string i = m.Groups["i"].Value;
            if (idMap.TryGetValue(i, out string? renamed)) {
                return "#" + renamed;
            }
            return m.Value;
        }

        return CssIdSelector.Replace(css, new MatchEvaluator(evaluator));
    }

    private static string ReplaceGroupValue(string fullMatch, string oldValue, string newValue) {
        int idx = fullMatch.IndexOf(oldValue, StringComparison.Ordinal);
        if (idx < 0)
            return fullMatch;
        return fullMatch.Substring(0, idx) + newValue + fullMatch.Substring(idx + oldValue.Length);
    }

    private sealed class Options {
        public string HtmlPath { get; private set; } = string.Empty;
        public string CssPath { get; private set; } = string.Empty;
        public string OutDir { get; private set; } = ".";
        public string ClassPrefix { get; private set; } = "c";
        public string IdPrefix { get; private set; } = "id";
        public RenameMode Mode { get; private set; } = RenameMode.GuidOrHashy;

        public static Options Parse(string[] args) {
            Dictionary<string, string> kv = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < args.Length; i++) {
                string a = args[i];
                if (!a.StartsWith("--", StringComparison.Ordinal))
                    continue;

                string key = a.Substring(2);
                string value = (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
                    ? args[++i]
                    : "true";

                kv[key] = value;
            }

            if (!kv.TryGetValue("html", out string? html) || string.IsNullOrWhiteSpace(html)) {
                throw new ArgumentException("Missing --html path/to/file.html");
            }
            if (!kv.TryGetValue("css", out string? css) || string.IsNullOrWhiteSpace(css)) {
                throw new ArgumentException("Missing --css path/to/file.css");
            }

            Options o = new Options {
                HtmlPath = Path.GetFullPath(html),
                CssPath = Path.GetFullPath(css),
            };

            if (kv.TryGetValue("out", out string? outDir) && !string.IsNullOrWhiteSpace(outDir)) {
                o.OutDir = Path.GetFullPath(outDir);
            }

            if (kv.TryGetValue("classPrefix", out string? cp) && !string.IsNullOrWhiteSpace(cp)) {
                o.ClassPrefix = SanitizeCssIdentifierPrefix(cp);
            }

            if (kv.TryGetValue("idPrefix", out string? ip) && !string.IsNullOrWhiteSpace(ip)) {
                o.IdPrefix = SanitizeCssIdentifierPrefix(ip);
            }

            if (kv.TryGetValue("mode", out string? mode) && !string.IsNullOrWhiteSpace(mode)) {
                if (mode.Equals("guid", StringComparison.OrdinalIgnoreCase)) {
                    o.Mode = RenameMode.GuidOnly;
                } else {
                    o.Mode = RenameMode.GuidOrHashy;
                }
            }

            if (!File.Exists(o.HtmlPath))
                throw new FileNotFoundException("HTML file not found.", o.HtmlPath);
            if (!File.Exists(o.CssPath))
                throw new FileNotFoundException("CSS file not found.", o.CssPath);

            return o;
        }

        private static string SanitizeCssIdentifierPrefix(string input) {
            string p = new string(input.Where(ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '-').ToArray());
            if (string.IsNullOrWhiteSpace(p))
                return "c";
            if (!char.IsLetter(p[0]) && p[0] != '_' && p[0] != '-')
                p = "c" + p;
            return p;
        }
    }

    private enum RenameMode {
        GuidOnly,
        GuidOrHashy
    }
}
