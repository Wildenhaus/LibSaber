using System.Text.RegularExpressions;

namespace LibSaber.SpaceMarine2.Serialization.Scripting;

public class PsObjectTextSerializer
{
  public Dictionary<string, dynamic> Deserialize(TextReader reader)
  {
    string content = reader.ReadToEnd();
    return Deserialize(content);
  }

  public Dictionary<string, dynamic> Deserialize(string script)
  {
    Tokenizer tokenizer = new Tokenizer(script);
    List<Token> tokens = tokenizer.Tokenize();
    Parser parser = new Parser(tokens);
    return parser.Parse();
  }

  private class Tokenizer
  {
    private readonly string _content;
    private static readonly Regex TokenRegex = new Regex(
        @"(?<QuotedIdentifier>""[^""]*"")|(?<String>""[^""]*"")|(?<Number>\d+(\.\d+)?)|(?<Bool>true|false)|(?<ArrayStart>\[)|(?<ArrayEnd>\])|(?<Equals>=)|(?<Semicolon>;)|(?<Comma>,)|(?<BraceStart>{)|(?<BraceEnd>})|(?<Identifier>[a-zA-Z_][a-zA-Z0-9_]*)|(?<Whitespace>\s+)|(?<Unknown>.)",
        RegexOptions.Compiled);

    public Tokenizer(string content)
    {
      _content = content;
    }

    public List<Token> Tokenize()
    {
      var tokens = new List<Token>();
      var matches = TokenRegex.Matches(_content);

      foreach (Match match in matches)
      {
        if (match.Groups["QuotedIdentifier"].Success)
        {
          tokens.Add(new Token(TokenType.Identifier, match.Groups["QuotedIdentifier"].Value.Trim('"')));
        }
        else if (match.Groups["String"].Success)
        {
          tokens.Add(new Token(TokenType.String, match.Groups["String"].Value.Trim('"')));
        }
        else if (match.Groups["Number"].Success)
        {
          tokens.Add(new Token(TokenType.Number, match.Groups["Number"].Value));
        }
        else if (match.Groups["Bool"].Success)
        {
          tokens.Add(new Token(TokenType.Bool, match.Groups["Bool"].Value));
        }
        else if (match.Groups["ArrayStart"].Success)
        {
          tokens.Add(new Token(TokenType.ArrayStart, "["));
        }
        else if (match.Groups["ArrayEnd"].Success)
        {
          tokens.Add(new Token(TokenType.ArrayEnd, "]"));
        }
        else if (match.Groups["Equals"].Success)
        {
          tokens.Add(new Token(TokenType.Equals, "="));
        }
        else if (match.Groups["Semicolon"].Success)
        {
          tokens.Add(new Token(TokenType.Semicolon, ";"));
        }
        else if (match.Groups["Comma"].Success)
        {
          tokens.Add(new Token(TokenType.Comma, ","));
        }
        else if (match.Groups["BraceStart"].Success)
        {
          tokens.Add(new Token(TokenType.BraceStart, "{"));
        }
        else if (match.Groups["BraceEnd"].Success)
        {
          tokens.Add(new Token(TokenType.BraceEnd, "}"));
        }
        else if (match.Groups["Identifier"].Success)
        {
          tokens.Add(new Token(TokenType.Identifier, match.Groups["Identifier"].Value));
        }
        // Ignore whitespace and unknown tokens for simplicity.
      }

      return tokens;
    }
  }

  private class Parser
  {
    private readonly List<Token> _tokens;
    private int _index;

    public Parser(List<Token> tokens)
    {
      _tokens = tokens;
      _index = 0;
    }

    public Dictionary<string, dynamic> Parse()
    {
      return ParseObject();
    }

    private Dictionary<string, dynamic> ParseObject()
    {
      var result = new Dictionary<string, dynamic>();

      while (_index < _tokens.Count)
      {
        var token = _tokens[_index];

        if (token.Type == TokenType.Identifier)
        {
          var key = token.Value;
          _index++; // Move to '='
          Expect(TokenType.Equals);
          _index++; // Move to value or '{'

          if (_tokens[_index].Type == TokenType.BraceStart)
          {
            _index++; // Skip '{'
            result[key] = ParseObject();
            Expect(TokenType.BraceEnd);
            _index++; // Skip '}'
          }
          else if (_tokens[_index].Type == TokenType.ArrayStart)
          {
            result[key] = ParseArray();
          }
          else
          {
            result[key] = ParseValue();
          }

          Expect(TokenType.Semicolon);
          _index++; // Skip ';'
        }
        else if (token.Type == TokenType.BraceEnd)
        {
          break; // End of object
        }
        else
        {
          throw new InvalidOperationException("Unexpected token: " + token.Value);
        }
      }

      return result;
    }

    private List<dynamic> ParseArray()
    {
      var list = new List<dynamic>();
      _index++; // Skip '['

      while (_tokens[_index].Type != TokenType.ArrayEnd)
      {
        if (_tokens[_index].Type == TokenType.BraceStart)
        {
          _index++; // Skip '{'
          list.Add(ParseObject());
          Expect(TokenType.BraceEnd);
          _index++; // Skip '}'
        }
        else if (_tokens[_index].Type == TokenType.ArrayStart)
        {
          list.Add(ParseArray());
        }
        else
        {
          list.Add(ParseValue());
        }

        if (_tokens[_index].Type == TokenType.Comma)
        {
          _index++; // Skip ','
        }
      }

      _index++; // Skip ']'
      return list;
    }

    private dynamic ParseValue()
    {
      var token = _tokens[_index];

      switch (token.Type)
      {
        case TokenType.String:
          _index++;
          return token.Value;
        case TokenType.Number:
          _index++;
          return double.Parse(token.Value);
        case TokenType.Bool:
          _index++;
          return bool.Parse(token.Value);
        case TokenType.Identifier:
          // Treat unquoted identifiers as strings (e.g., "local__plane")
          _index++;
          return token.Value;
        default:
          throw new InvalidOperationException("Unexpected token: " + token.Value);
      }
    }

    private void Expect(TokenType type)
    {
      if (_tokens[_index].Type != type)
      {
        throw new InvalidOperationException($"Expected {type}, but got {_tokens[_index].Type}");
      }
    }
  }

  private enum TokenType
  {
    String,
    Number,
    Bool,
    ArrayStart,
    ArrayEnd,
    Equals,
    Semicolon,
    Comma,
    BraceStart,
    BraceEnd,
    Identifier
  }

  private class Token
  {
    public TokenType Type { get; }
    public string Value { get; }

    public Token(TokenType type, string value)
    {
      Type = type;
      Value = value;
    }
  }
}