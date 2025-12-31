 ## (Un)obscura CLI
 ### This CLI tool reverse-engineers generated `html` and `css` to re-map class and ID tokens to new, unobscured stable names. 
 
 ##### Give it the `.ccs` and the `.html` and it will make turning things into components much easier for you.
 ---
 #### Purpose
 - Reversee engineering `React/Typescript` website templates in order to build similar components in `Blazor + SignalR` as an example. 
 - Turning things into re-usable `Blazor + SignalR` frontends whiile avoiding `JSInterop` is incredibly annoying. 
 - *I don't want `JSInterop`.*
 - **Example: I paid $179.00 USD for an admin C# .NET server-side template, so I could take some of the components for my own project, but the purchased template ended up `JSInterop` on every page and used over a hundred `npm` packages.**
 - Anyways...

 ---
  #### Usage
- `dotnet run -- --html path/to/index.html --css path/to/site.css --out ./out --prefix c`
  
    - Optional arguments: 
    - `--prefix c` to specify output prefix for class and ID names. 
    - Default is `c` for classes and `id` for IDs. You can make up your own.
    - `--mode guid` *GUID-like e.g.: Webflow and w-node-...)
      - (GUID-like or hashyopaque (default) + Webflow w-node-...)
    - `--mode hash` 
 
- For my example included I used neither of those modes and the default worked great,

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


