using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace WebMap.Web
{
    public class Wildcard : Regex

    {

        public Wildcard( string pattern ) : base( WildcardToRegex( pattern ) )
        {

        }
     

        public Wildcard( string pattern, RegexOptions options ) : base( WildcardToRegex( pattern ), options )
        {

        }

        public static string WildcardToRegex( string pattern )
        {

            return "^" + Regex.Escape( pattern ).

            Replace( "\\*", ".*" ).

            Replace( "\\?", "." ) + "$";

        }

    }
}