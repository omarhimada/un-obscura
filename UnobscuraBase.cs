using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Provides a set of compiled regular expressions for matching and extracting common patterns in HTML, CSS, and
/// identifier strings. Additionally extended to download JavaScript files referenced in HTML &lt;script&gt; tags,
/// then update the local HTML output to reference the downloaded files.
/// </summary>
/// <remarks>
/// The static members of this class expose precompiled regular expressions for efficient reuse in
/// scenarios such as parsing HTML attributes, CSS selectors, GUIDs, and Webflow node identifiers. These regular
/// expressions are intended for use in validation, extraction, or reverse engineering of markup and identifier formats.
/// All regular expressions are compiled for performance and are suitable for repeated use in high-throughput or
/// performance-sensitive applications.
/// </remarks>
public partial class UnobscuraBase {
    public static readonly Regex CssClassSelector = CompiledCssClassSelector();
    public static readonly Regex CssIdSelector = CompiledCssIdSelector();
    public static readonly Regex Guid32 = CompiledReverseEngineerGuidRegexB();
    public static readonly Regex Guid36 = CompiledReverseEngineerGuidRegexA();
    public static readonly Regex Hashy = CompiledReverseEngineerHashyRegex();
    public static readonly Regex HtmlClassAttr = CompiledHtmlClassAttr();
    public static readonly Regex HtmlHashRefAttr = CompiledHtmlHashRefAttr();
    public static readonly Regex HtmlIdAttr = CompiledHtmlIdAttr();
    public static readonly Regex Meaningful = CompiledReverseEngineerMeaningfulRegex();
    public static readonly Regex WebflowNodeId = CompiledReverseEngineerWebflowRegex();

    /// <summary>
    /// Returns a compiled regular expression that matches an HTML or XML attribute named 'id' and captures its value.
    /// </summary>
    /// <remarks>The returned regular expression is case-insensitive and compiled for performance. It matches
    /// both single- and double-quoted attribute values. Use the 'v' named group to access the captured value of the
    /// 'id' attribute.</remarks>
    /// <returns>A <see cref="Regex"/> instance that matches 'id' attributes in markup and captures the attribute value in the
    /// 'v' named group.</returns>
    [GeneratedRegex(@"\bid\s*=\s*(?:""(?<v>[^""]*)""|'(?<v>[^']*)')", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    public static partial Regex CompiledCssClassSelector();

    /// <summary>
    /// Returns a compiled regular expression that matches CSS ID selectors in a string.
    /// </summary>
    /// <remarks>The returned regular expression matches a hash sign ('#') followed by a valid CSS identifier,
    /// ensuring the selector is not preceded by a valid identifier character. The named group "i" contains the matched
    /// ID value. This method is suitable for repeated use due to the compiled nature of the regular
    /// expression.</remarks>
    /// <returns>A compiled <see cref="Regex"/> instance that matches CSS ID selectors. The regular expression captures the ID
    /// name in a named group called "i".</returns>
    [GeneratedRegex(@"(?<![A-Za-z0-9_-])\#(?<i>[_A-Za-z-][A-Za-z0-9_-]*)", RegexOptions.Compiled)]
    public static partial Regex CompiledCssIdSelector();

    /// <summary>
    /// Returns a compiled regular expression that matches the value of the HTML 'class' attribute in a string.
    /// </summary>
    /// <remarks>The regular expression is case-insensitive and compiled for performance. It supports both
    /// double-quoted and single-quoted attribute values. Use the 'v' named group to access the extracted class
    /// attribute value from a match.</remarks>
    /// <returns>A <see cref="Regex"/> instance that matches 'class' attributes in HTML, capturing the attribute value in the 'v'
    /// named group.</returns>
    [GeneratedRegex(@"\bclass\s*=\s*(?:""(?<v>[^""]*)""|'(?<v>[^']*)')", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    public static partial Regex CompiledHtmlClassAttr();

    /// <summary>
    /// Returns a compiled regular expression that matches HTML or SVG attributes referencing fragment identifiers using
    /// 'href' or 'xlink:href'.
    /// </summary>
    /// <remarks>The regular expression is case-insensitive and matches both double-quoted and single-quoted
    /// attribute values. The named group 'v' captures the fragment identifier value, including the leading '#'. This
    /// method is culture-invariant and uses the compiled regular expression engine for improved performance.</remarks>
    /// <returns>A compiled <see cref="Regex"/> instance that matches 'href' or 'xlink:href' attributes whose values are fragment
    /// identifiers (beginning with '#').</returns>
    [GeneratedRegex(@"\b(href|xlink:href)\s*=\s*(?:""(?<v>#[^""]+)""|'(?<v>#[^']+)')", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    public static partial Regex CompiledHtmlHashRefAttr();

    /// <summary>
    /// Returns a compiled regular expression that matches CSS class selectors and captures valid HTML ID attribute
    /// values.
    /// </summary>
    /// <remarks>The returned regular expression uses the compiled option for improved performance on repeated
    /// matches. The pattern matches a period ('.') not preceded by an alphanumeric character, underscore, or hyphen,
    /// followed by a valid HTML ID attribute name (starting with an underscore, letter, or hyphen, and followed by
    /// alphanumeric characters, underscores, or hyphens).</remarks>
    /// <returns>A <see cref="Regex"/> instance that matches a period ('.') followed by a valid HTML ID attribute name, capturing
    /// the name in the 'c' group.</returns>
    [GeneratedRegex(@"(?<![A-Za-z0-9_-])\.(?<c>[_A-Za-z-][A-Za-z0-9_-]*)", RegexOptions.Compiled)]
    public static partial Regex CompiledHtmlIdAttr();

    /// <summary>
    /// Returns a compiled regular expression that matches strings in the standard 32-character hexadecimal GUID format
    /// with hyphens.
    /// </summary>
    /// <remarks>The returned regular expression is case-insensitive and matches only strings that strictly
    /// conform to the standard GUID format with hyphens. The compiled option improves performance for repeated
    /// use.</remarks>
    /// <returns>A compiled <see cref="Regex"/> instance that matches GUID strings formatted as
    /// "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", where each 'x' is a hexadecimal digit.</returns>
    [GeneratedRegex(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", RegexOptions.Compiled)]
    public static partial Regex CompiledReverseEngineerGuidRegexA();

    /// <summary>
    /// Returns a compiled regular expression that matches a 32-character hexadecimal string representing a GUID without
    /// separators.
    /// </summary>
    /// <remarks>This regular expression is case-insensitive and is intended for validating or parsing GUIDs
    /// in their compact hexadecimal form. The compiled option improves performance for repeated use.</remarks>
    /// <returns>A compiled <see cref="Regex"/> instance that matches strings consisting of exactly 32 hexadecimal digits (0-9,
    /// a-f, A-F) with no dashes or braces.</returns>
    [GeneratedRegex(@"^[0-9a-fA-F]{32}$", RegexOptions.Compiled)]
    public static partial Regex CompiledReverseEngineerGuidRegexB();

    /// <summary>
    /// Returns a compiled regular expression that matches strings consisting of 10 or more alphanumeric characters,
    /// underscores, or hyphens.
    /// </summary>
    /// <remarks>The returned regular expression is compiled for improved performance when used repeatedly.
    /// The pattern enforces that the entire input string matches the allowed character set and length
    /// requirements.</remarks>
    /// <returns>A compiled <see cref="Regex"/> instance that matches strings containing only uppercase and lowercase letters,
    /// digits, underscores, or hyphens, with a minimum length of 10 characters.</returns>
    [GeneratedRegex(@"^[A-Za-z0-9_-]{10,}$", RegexOptions.Compiled)]
    public static partial Regex CompiledReverseEngineerHashyRegex();

    /// <summary>
    /// Returns a compiled regular expression that matches strings starting with a lowercase letter, followed by up to
    /// 24 lowercase letters, digits, or hyphens.
    /// </summary>
    /// <remarks>The regular expression pattern is "^[a-z][a-z0-9-]{0,24}$". This can be used to validate
    /// identifiers or names that must begin with a lowercase letter and may include lowercase letters, digits, or
    /// hyphens, with a maximum length of 25 characters.</remarks>
    /// <returns>A compiled <see cref="Regex"/> that matches strings conforming to the pattern: a lowercase letter at the start,
    /// followed by zero to 24 lowercase letters, digits, or hyphens.</returns>
    [GeneratedRegex(@"^[a-z][a-z0-9-]{0,24}$", RegexOptions.Compiled)]
    public static partial Regex CompiledReverseEngineerMeaningfulRegex();

    /// <summary>
    /// Returns a compiled regular expression that matches Webflow node identifiers in the format
    /// "w-node-<GUID>-<name>".
    /// </summary>
    /// <remarks>Use this regular expression to identify or validate Webflow-generated node IDs that follow
    /// the standard naming convention. The compiled option improves performance for repeated matches.</remarks>
    /// <returns>A compiled <see cref="Regex"/> instance that matches strings beginning with "w-node-", followed by a 20 or more
    /// character hexadecimal and hyphen sequence, a hyphen, and a name consisting of alphanumeric characters,
    /// underscores, or hyphens.</returns>
    [GeneratedRegex(@"^w-node-[0-9a-fA-F-]{20,}-[A-Za-z0-9_-]+$", RegexOptions.Compiled)]
    public static partial Regex CompiledReverseEngineerWebflowRegex();

    /// <summary>
    /// Represents a compiled regular expression used to match the 'src' attribute of HTML <script> elements.
    /// </summary>
    /// <remarks>This regular expression is intended for internal use when parsing or analyzing HTML content
    /// to locate script source attributes. It is compiled for improved performance during repeated use.</remarks>
    private static readonly Regex HtmlScriptSrcAttr = CompiledScriptSourceAttributeRegex();

    /// <summary>
    /// Returns a compiled regular expression that matches <script> HTML tags with a 'src' attribute and captures the
    /// value of the 'src' attribute.
    /// </summary>
    /// <remarks>The returned regular expression is case-insensitive, culture-invariant, and compiled for
    /// performance. Use the 'src' named group to extract the value of the 'src' attribute from matched <script>
    /// tags.</remarks>
    /// <returns>A <see cref="Regex"/> instance that matches <script> tags containing a 'src' attribute. The regular expression
    /// captures the value of the 'src' attribute in a named group called 'src'.</returns>
    [GeneratedRegex(@"<script\b[^>]*?\bsrc\s*=\s*([""'])(?<src>[^""']+)\1[^>]*?>", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex CompiledScriptSourceAttributeRegex();

    /// <summary>
    /// Downloads external JS referenced by &lt;script src="..."&gt; tags, in the input HTML,
    /// into out/js, then rewrites the HTML to point to those local copies.
    /// </summary>
    /// <param name="htmlPath">Path to the HTML file to rewrite.</param>
    /// <param name="outputRoot">Output folder root (will create js subfolder).</param>
    /// <param name="baseUrl">
    /// Optional base URL used to resolve site-relative or relative src values (ex: "https://example.com/").
    /// If null/empty, only absolute http/https src values are downloaded.
    /// </param>
    /// <param name="localScriptPrefix">
    /// The prefix written into HTML src attributes (ex: "./js" or "/assets/js").
    /// Defaults to "./js".
    /// </param>
    public static async Task DownloadAndRelinkScriptsAsync(
        string htmlPath,
        string outputRoot,
        string? baseUrl = null,
        string localScriptPrefix = "./js",
        CancellationToken cancellationToken = default) {

        if (string.IsNullOrWhiteSpace(htmlPath))
            throw new ArgumentException($"{nameof(htmlPath)} is required.", nameof(htmlPath));
        if (string.IsNullOrWhiteSpace(outputRoot))
            throw new ArgumentException($"{nameof(outputRoot)} is required.", nameof(outputRoot));
        if (!File.Exists(htmlPath))
            throw new FileNotFoundException("HTML file not found.", htmlPath);

        string html = await File.ReadAllTextAsync(htmlPath, Encoding.UTF8, cancellationToken).ConfigureAwait(false);

        // Note: with Google web fonts you'll have to open the CSS files and download the .woff2 files too.
        string jsDirectory = Path.Combine(outputRoot, "js");

        Directory.CreateDirectory(jsDirectory);

        Uri? uri = null;
        if (!string.IsNullOrWhiteSpace(baseUrl)) {
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out uri)) {
                throw new ArgumentException($"{nameof(baseUrl)} must be an absolute URL like https://example.com/.", nameof(baseUrl));
            }
        }

        // Collect script src candidates.
        MatchCollection matches = HtmlScriptSrcAttr.Matches(html);

        if (matches.Count == 0)
            return;

        // Map original local filename to avoid downloading duplicates.
        Dictionary<string, string> rewritten = new(StringComparer.OrdinalIgnoreCase);

        using HttpClient http = new();

        foreach (Match m in matches) {
            cancellationToken.ThrowIfCancellationRequested();

            string src = m.Groups["src"].Value.Trim();

            if (string.IsNullOrWhiteSpace(src))
                continue;

            // Skip non-downloadable schemes or inline-ish.
            if (src.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                continue;
            if (src.StartsWith("blob:", StringComparison.OrdinalIgnoreCase))
                continue;

            // Resolve to an absolute http/https URL.
            if (!TryResolveHttpUrl(src, uri, out Uri? scriptUri)) {
                // Not resolvable to http(s). Leave it alone.
                continue;
            }

            string? srcKey = scriptUri?.ToString();
            if (scriptUri is null || rewritten.ContainsKey(srcKey!))
                continue;

            // Decide a stable local filename.
            // Prefer last path segment if it looks like a file; otherwise use a hash.
            string localFileName = MakeStableJsFileName(scriptUri);

            // Ensure no collisions in output folder (rare, but possible).
            localFileName = EnsureUniqueFileName(jsDirectory, localFileName);

            string localFullPath = Path.Combine(jsDirectory, localFileName);

            // Download JavaScript
            byte[] bytes = await http.GetByteArrayAsync(scriptUri, cancellationToken).ConfigureAwait(false);
            await File.WriteAllBytesAsync(localFullPath, bytes, cancellationToken).ConfigureAwait(false);

            // Rewrite to local
            string localSrc = CombineUrlish(localScriptPrefix, localFileName);
            rewritten[srcKey!] = localSrc;
        }

        if (rewritten.Count == 0)
            return;

        // Rewrite HTML for each .js file resolve and replace if was downloaded.
        string rewrittenHtml = HtmlScriptSrcAttr.Replace(html, match => {
            string originalSrc = match.Groups["src"].Value.Trim();
            if (!TryResolveHttpUrl(originalSrc, uri, out Uri? resolved))
                return match.Value;

            string? key = resolved?.ToString();
            if (!rewritten.TryGetValue(key, out string? localSrc))
                return match.Value;

            // Replace just the attribute value inside the tag.
            // We do a small targeted replacement: src="old" -> src="new".
            string tag = match.Value;
            return ReplaceSrcAttributeValue(tag, originalSrc, localSrc);
        });

        // Write back (or write to an output HTML path if you prefer).
        await File.WriteAllTextAsync(htmlPath, rewrittenHtml, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Attempts to resolve the specified URI string to an absolute HTTP or HTTPS URL.
    /// </summary>
    /// <remarks>This method returns false if the URI string is not a valid HTTP or HTTPS URL, or if a
    /// relative URI is provided without a base URI. Only URIs with the 'http' or 'https' scheme are considered
    /// valid.</remarks>
    /// <param name="src">The URI string to resolve. Can be an absolute or relative URI.</param>
    /// <param name="baseUri">The base URI to use when resolving a relative URI. If the URI string is relative, this parameter must not be
    /// null.</param>
    /// <param name="absoluteHttpUrl">When this method returns, contains the absolute HTTP or HTTPS URL if the resolution succeeds; otherwise, null.</param>
    /// <returns>true if the URI string can be resolved to an absolute HTTP or HTTPS URL; otherwise, false.</returns>
    private static bool TryResolveHttpUrl(string src, Uri? baseUri, out Uri? absoluteHttpUrl) {
        absoluteHttpUrl = null;

        if (Uri.TryCreate(src, UriKind.Absolute, out Uri? abs)) {
            if (abs.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) ||
                abs.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)) {
                absoluteHttpUrl = abs;
                return true;
            }
            return false;
        }

        // Relative paths aren't supported
        // (everything is all cdn cloudfront-esque anyways)
        if (baseUri == null)
            return false;

        if (Uri.TryCreate(baseUri, src, out Uri? combined)) {
            if (combined.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) ||
                combined.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)) {
                absoluteHttpUrl = combined;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Generates a stable JavaScript file name based on the specified URI.
    /// </summary>
    /// <remarks>If the URI's last segment resembles a file name, it is used directly (with sanitization). If
    /// not, a deterministic file name is generated using a hash of the URI to ensure uniqueness and stability. This
    /// method is useful for caching or referencing scripts loaded from dynamic or CDN sources.</remarks>
    /// <param name="uri">The URI from which to derive the JavaScript file name. Must not be null.</param>
    /// <returns>A file name string suitable for a JavaScript file, derived from the URI's last segment if possible; otherwise, a
    /// name based on a hash of the full URI.</returns>
    private static string MakeStableJsFileName(Uri uri) {
        // Try to use last segment.
        string last = uri.Segments.LastOrDefault()?.Trim('/') ?? string.Empty;

        // Strip query-ish from last segment if present (usually not in Segments, but safe).
        int q = last.IndexOf('?', StringComparison.Ordinal);
        if (q >= 0)
            last = last.Substring(0, q);

        if (!string.IsNullOrWhiteSpace(last) && last.Contains('.')) {
            // Normalize: ensure .js extension if it seems like JS.
            if (!last.EndsWith(".js", StringComparison.OrdinalIgnoreCase)) {
                // Many CDNs serve "script" or "index" without .js; don’t force it.
                // But if it has a different extension, keep it as-is.
            }
            return SanitizeFileName(last);
        }

        // Fallback: hash full URL.
        string hash = ShortSha256(uri.ToString());
        return $"script-{hash}.js";
    }

    /// <summary>
    /// Generates a unique file name within the specified directory by appending a numeric suffix if a file with the
    /// given name already exists.
    /// </summary>
    /// <remarks>This method does not create any files or modify the file system. It only checks for existing
    /// files and generates a unique name accordingly.</remarks>
    /// <param name="dir">The path to the directory in which to ensure the file name is unique. Cannot be null or empty.</param>
    /// <param name="fileName">The desired file name, including extension. Cannot be null or empty.</param>
    /// <returns>A file name that does not conflict with any existing file in the specified directory. If the original file name
    /// is available, it is returned unchanged; otherwise, a numeric suffix is appended to create a unique name.</returns>
    private static string EnsureUniqueFileName(string dir, string fileName) {
        string name = Path.GetFileNameWithoutExtension(fileName);
        string ext = Path.GetExtension(fileName);
        string candidate = fileName;

        int i = 2;
        while (File.Exists(Path.Combine(dir, candidate))) {
            candidate = $"{name}-{i}{ext}";
            i++;
        }

        return candidate;
    }

    /// <summary>
    /// Replaces invalid file name characters in the specified string with hyphens ('-') to produce a file-system-safe
    /// name.
    /// </summary>
    /// <remarks>This method does not check for reserved file names or enforce length restrictions imposed by
    /// the file system. The caller is responsible for ensuring the resulting name is valid for the intended
    /// use.</remarks>
    /// <param name="name">The file name to sanitize. Cannot be null.</param>
    /// <returns>A string in which all invalid file name characters have been replaced with hyphens.</returns>
    private static string SanitizeFileName(string name) {
        foreach (char c in Path.GetInvalidFileNameChars()) {
            name = name.Replace(c, '-');
        }
        return name;
    }

    /// <summary>
    /// Computes a shortened SHA-256 hash of the specified string and returns it as a lowercase hexadecimal string.
    /// </summary>
    /// <remarks>The returned hash is not suitable for cryptographic purposes or as a secure identifier, as it
    /// uses only a portion of the full SHA-256 output. Use this method for scenarios where a short, non-secure
    /// identifier is sufficient.</remarks>
    /// <param name="value">The input string to hash. Cannot be null.</param>
    /// <returns>A 20-character, lowercase hexadecimal string representing the first 10 bytes of the SHA-256 hash of the input.</returns>
    private static string ShortSha256(string value) {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        byte[] hash = SHA256.HashData(bytes);
        // 10 bytes -> 20 hex chars, enough to avoid collisions in practice.
        return Convert.ToHexString(hash.AsSpan(0, 10)).ToLowerInvariant();
    }

    /// <summary>
    /// Combines a prefix and a file name into a single path, ensuring exactly one '/' separator between them.
    /// </summary>
    /// <remarks>If the prefix already ends with a '/', no additional separator is added. This method does not
    /// validate whether the resulting string is a valid URL or file path.</remarks>
    /// <param name="prefix">The prefix to combine with the file name. May be an empty string or a path ending with '/'.</param>
    /// <param name="fileName">The file name to append to the prefix. Cannot be null.</param>
    /// <returns>A string representing the combined path. If the prefix is null, empty, or consists only of white-space
    /// characters, returns the file name.</returns>
    private static string CombineUrlish(string prefix, string fileName) {
        if (string.IsNullOrWhiteSpace(prefix))
            return fileName;
        if (prefix.EndsWith("/", StringComparison.Ordinal))
            return prefix + fileName;
        return prefix + "/" + fileName;
    }

    /// <summary>
    /// Replaces the value of the src attribute in a script tag if it matches a specified value, preserving the original
    /// quote style.
    /// </summary>
    /// <remarks>Only the src attribute whose value exactly matches oldSrc is replaced. The method preserves
    /// the original quote character (single or double) used in the src attribute. If no matching src attribute is
    /// found, the input string is returned unchanged.</remarks>
    /// <param name="scriptTag">The HTML string representing a single <script> tag in which to search for the src attribute.</param>
    /// <param name="oldSrc">The current value of the src attribute to be replaced. The comparison is case-sensitive and ignores leading and
    /// trailing whitespace.</param>
    /// <param name="newSrc">The new value to assign to the src attribute if the current value matches oldSrc.</param>
    /// <returns>A string containing the script tag with the src attribute value replaced if a match is found; otherwise, the
    /// original script tag.</returns>
    private static string ReplaceSrcAttributeValue(string scriptTag, string oldSrc, string newSrc) {
        // Replace src="oldSrc" or src='oldSrc' with newSrc (preserve quote type).
        // If oldSrc appears in other places, this is still safe because it’s anchored to src=.
        return Regex.Replace(
            scriptTag,
            @"(\bsrc\s*=\s*)([""'])(?<v>[^""']+)\2",
            match => {
                string v = match.Groups["v"].Value;
                // Only replace if it matches exactly what we saw (avoid rewriting other tags incorrectly).
                if (!string.Equals(v.Trim(), oldSrc, StringComparison.Ordinal))
                    return match.Value;

                string prefix = match.Groups[1].Value;
                string quote = match.Groups[2].Value;
                return $"{prefix}{quote}{newSrc}{quote}";
            },
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }
}

