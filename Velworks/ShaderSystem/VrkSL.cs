using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Veldrid;
using Velworks.Rendering;

namespace Velworks.ShaderSystem
{
    public static class VrkSL
    {
        public const string DEFAULT_SHADER_VERSION = "450";

        // Matches all comments
        static Regex r_removeComments = new Regex(@"(\/\/.*)|(\/\*[\S\s]*\*\/)");

        // Header
        static Regex r_hasHeader = new Regex(@"\$header_end");
        static Regex r_getHeader = new Regex(@"([\s\S]*)(\$header_end)");
        static Regex r_headerStatements = new Regex(@"(\#)([^\s]*)(?=\s)");

        public static ShaderCompilerArgs Compile(
            string source,
            Action<string> log)
        {
            source = r_removeComments.Replace(source, "");

            var preprocMatch = r_headerStatements.Match(source);

            var args = new ShaderCompilerArgs();

            if (!preprocMatch.Success)
            {
                log("[ERROR] - Shader has no processor statements");
                args.IsFaulted = true;
                return args;
            }

            string shaderName = "";
            string shaderVersion = "";
            var inputs = new List<string>();

            #region HEADER
            {
                var header = source[..preprocMatch.Index];

                if (!Ini.Parse(header, out var ini, log))
                {
                    log("[ERR] - Header has errors");

                }
                else
                {
                    // NAME
                    var name = ini["", "Name"];
                    if (name is string ntxt)
                    {
                        shaderName = ntxt;
                    }
                    else
                    {
                        log("[ERR] - Missing Shader Name!");
                    }
                    // VERSION
                    var version = ini["", "Version"];
                    if (version is string vtxt)
                    {
                        shaderVersion = vtxt;
                    }
                    else
                    {
                        log("[WRN] - Missing Shader Version, assumed 450!");
                        shaderVersion = DEFAULT_SHADER_VERSION;
                    }
                }
            }
            #endregion

            // TODO: Passes
            
            foreach (var match in Iterate(preprocMatch))
            {
                log($"MATCH: {preprocMatch.Value}");
                switch (match.Value)
                {
                    case "#pass":
                    default: break;
                };
            }

            // TODO: Inputs

            // TODO: Build Source Code

            // TODO: Compile Source to SPIRV

            return args;

            // ========================== \\



        }

        static IEnumerable<Match> Iterate(Match match)
        {
            while (match.Success)
            {
                yield return match;
                match = match.NextMatch();
            }
        }
    }

    public struct ShaderCompilerArgs
    {
        
        public bool IsFaulted { get; internal set; }
        
        public void LogData(Action writeLine)
        {
            


        }
    }

    class Ini
    {
        Dictionary<string, Dictionary<string, object>> values
            = new Dictionary<string, Dictionary<string, object>>();

        Dictionary<string, object> defaultSection
            = new Dictionary<string, object>();

        #region REGEX
        static Regex r_tokName = new Regex(@"\w");
        static Regex r_tokNum = new Regex(@"\d*");
        static Regex r_tokString = new Regex("[\"'`]*");
        static Regex r_tokNL = new Regex(@"\n\r");

        #endregion

        Dictionary<string, object> GetSection(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
                return defaultSection;

            if (values.ContainsKey(sectionName))
            {
                return values[sectionName];
            }
            else
            {
                var newDic = new Dictionary<string, object>();
                values.Add(sectionName, newDic);
                return newDic;
            }
        }
        
        static object? Get(Dictionary<string, object> dic, string key)
        {
            if (dic.ContainsKey(key))
                return dic[key];
            return null;
        }
        static void Set(Dictionary<string, object> dic, string key, object? obj)
        {
            if (obj is null)
                return;
            if (dic.ContainsKey(key))
                dic[key] = obj;
            else
                dic.Add(key, obj);
        }

        public object? this[string section, string key]
        {
            get => Get(GetSection(section), key);
            set => Set(GetSection(section), key, value);
        }

        public static bool Parse(
            string source,
            out Ini result,
            Action<string> log,
            char commentChar = '\0')
        {
            var it = Utils.Lex(source);
            result = new Ini();
            string currentSection = "";
            int line = 0;

            var buffer = new List<string>();
            foreach (string tok in it)
            {
                // Skip Comments
                if (tok.StartsWith(commentChar))
                    continue;

                if (!Environment.NewLine.Any(x => tok.Contains(x)) && tok != ",")
                {
                    buffer.Add(tok);
                    continue;
                }
                
                // Handle buffer
                if (buffer.Count > 0)
                {
                    // Expression
                    if (r_tokName.IsMatch(buffer[0]))
                    {
                        if (buffer.Count != 3)
                        { log($"[ERR-{line}] - Expected 3 Tokens"); goto Next; }
                        if (buffer[1] != "=")
                        { log($"[ERR-{line}] - Value assignment must use '='"); goto Next; }
                        var value = ParseValue(buffer[2]);
                        if (value != null)
                        {
                            var section = result.GetSection(currentSection);
                            section[buffer[0]] = value;
                        }
                    }

                    // Section Change
                    if (buffer[0] == "[")
                    {
                        if (buffer.Count == 2 && buffer[1] == "]")
                        { currentSection = ""; goto Next; }
                        if (buffer.Count != 3)
                        { log($"[ERR-{line}] - Expected 3 Tokens"); goto Next; }
                        if (buffer[2] != "]")
                        { log($"[ERR-{line}] - Section Name change not closed!"); goto Next; }
                        if (!r_tokName.IsMatch(buffer[1]))
                        { log($"[ERR-{line}] - Section Name not valid!"); goto Next; }
                        currentSection = buffer[1];
                        goto Next;
                    }

                    // Error
                    log($"[ERR-{line}] - Unknown expression starting with {buffer[0]}");
                }
            Next:;
                buffer.Clear();
                if (tok != ",")
                    line++;
            }

            return true;
        }

        static object? ParseValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            // Is Name
            if (r_tokName.IsMatch(value))
                return value;
            
            // Is String Lit
            if (r_tokString.IsMatch(value)) {
                char qc = value[0];
                return value.Substring(1,
                    value[^1] == qc ?
                        value.Length - 2
                        : value.Length - 1
                    );
            }

            // Is Number
            if (double.TryParse(value, out double d))
                return d;
            

            return null;
        }

    }

    class Utils
    {

        public static IEnumerable<string> Lex(string text, char commentChar = '\0')
        {
            static bool IsName(char c) =>
                char.IsLetterOrDigit(c) || c == '_' || c == '.';
            static bool IsString(char c) =>
                c == '"' || c == '\'' || c == '`';

            for (int i = 0; i < text.Length; i++)
            {
                // Skip Whitespace
                while (char.IsWhiteSpace(text[i]))
                {
                    i++;
                    if (i == text.Length)
                        yield break;
                }

                // Names & Numbers
                if (IsName(text[i]))
                {
                    int count = 0;
                    do {
                        count++;
                    } while (IsName(text[i]) && i + count < text.Length);
                    yield return text.Substring(i, count);
                    i += count;
                    continue;
                }

                // Comments
                if (text[i] == commentChar)
                {
                    int count = 0;
                    do {
                        count++;
                    } while (!Environment.NewLine.Contains(text[i]) && i + count < text.Length);
                    yield return text.Substring(i, count);
                    i += count;
                    continue;
                }

                // Strings
                if (IsString(text[i]))
                {
                    int count = 0;
                    do
                    {
                        count++;
                    } while (IsString(text[i]) && i + count < text.Length);
                    yield return text.Substring(i, count);
                    i += count;
                    continue;
                }

                // New Line
                if (Environment.NewLine.Contains(text[i]))
                {
                    int count = 1;
                    while (Environment.NewLine.Contains(text[i + count]))
                        count++;
                    yield return text.Substring(i, count);
                    i += count;
                    continue;
                }

                // Operators
                yield return text[i].ToString();
            }
        }

    }
}
