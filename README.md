un-obscura CLI 🜂

A .NET CLI tool that reverse-engineers obfuscated HTML + CSS tokens and re-maps class/ID names to stable, unobscured component-friendly names — making it dramatically easier to port React templates into Blazor + SignalR frontends without touching JSInterop.

**Yes, I am still mad about paying for a $179 admin template that imported the entire npm registry and then glued JSInterop to every page like decorative pasta.**

 ---
  #### Usage
  ```
  dotnet run -- 
      --html ../../../Example/example.html 
      --css ../../../Example/example.css 
      --out ./out 
      --prefix c`
  ```

#### Advamced Isage
  ```
dotnet run -- 
    --html ../../../Example/example.html 
    --css ../../../Example/example.css 
    --out ./out
    --downloadScripts 
    --prefix inspired-ty
    --scriptOut ./out/www/scripts
  ```
 
 If you don't like command-line you can also open VS Code or Visual Studio or whatever IDE and just run it yourself.
 The whole pgrogram takes about a second to finish.

 ![Example Inout and Output](https://github.com/omarhimada/un-obscura/blob/master/reason.png)


- Optional arguments: 
- `--prefix c` to specify output prefix for class and ID names. 
    - Default is `c` for classes and `id` for IDs. You can make up your own.
    - `--mode guid` *GUID-like e.g.: Webflow and w-node-...*
    - `--mode hash`  *Targets hashed/opaque tokens*
    - `--mode opaque` *(Default) Matches anything that looks emotionally cryptic*
 
In my sample, I used none of the fancy modes and the defaults worked great. It’s like the tool could sense my disappointment and adapted automatically.

---
#### Description

  - It parses command-line arguments to determine input and output file paths, processes the specified HTML and CSS files to identify class and ID tokens,
 and generates new, stable names for selected tokens based on configurable rules. 
 - The rewritten HTML and CSS files, along with JSON mapping files for classes and IDs, are written to the specified output directory. 
 - The tool is designed to help un-obfuscate to re-standardize class and ID names, such as for privacy, education for designers, code minimization, or deployment
 scenarios. 
 - For usage details and supported arguments, run the application with no parameters or refer to the project
 documentation.

 - Provides the entry point and main logic for the command-line tool that renames and maps CSS class and ID tokens in
 HTML and CSS files, producing rewritten files and mapping outputs.
 - Renames BOTH class tokens (HTML `class="..."`; CSS `.class`) and `id` tokens (HTML `id="..."`)
 - Configurable output prefixes after reverse engineering. 
   - Writes:
     - `out/output.html`
     - `out/output.css`
     - `out/class-map.json`
     - `out/id-map.json`
---
#### Use cases
- Porting React → Blazor components
- Un-obfuscating design templates for learning or privacy
- Standardizing CSS tokens before componentization
- Reducing markup entropy
- Avoiding npm-based existential regret
