using NetCord.Rest;

namespace Ashfur.Utils;

public class ComponentUtils {
    public static ComponentContainerProperties BuildExceptionComponent(string message, Exception ex) {
        ComponentContainerProperties component = new ComponentContainerProperties()
            .WithAccentColor(new(0xFF0000))
            .AddComponents(new TextDisplayProperties("# Uh oh! Something went wrong!"))
            .AddComponents(new ComponentSeparatorProperties())
            .AddComponents(new TextDisplayProperties(message))
            .AddComponents(new TextDisplayProperties($"""
                                                     ```
                                                     {ex}
                                                     ```
                                                     """));

        return component;
    }
}