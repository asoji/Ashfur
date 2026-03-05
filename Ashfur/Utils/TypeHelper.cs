using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace Ashfur.Utils;

public static class TypeHelper {
    extension(string) {
        [PublicAPI]
        public static int ToInt(string s) => int.Parse(s);
    }

    // extension(ConfigurationManager) {
    //     [PublicAPI]
    //     public static int ToInt(string s) => int.Parse(s);
    // }
}