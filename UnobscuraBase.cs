using System.Text.RegularExpressions;

/// <summary>
/// Provides a set of compiled regular expressions for matching and extracting common patterns in HTML, CSS, and
/// identifier strings.
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
}