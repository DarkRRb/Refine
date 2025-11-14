using System;
using Umrab.Options;

namespace Umrab.Options.Test;

public class CommandTests {
    [Fact]
    public void ShortGroup_WithEquals_BindsValueToLastKey_AndSetsOtherFlags() {
        // -key=value 应被识别为 -k -e -y=value
        Command cmd = new Command()
            .Flag("k-flag", 'k')
            .Flag("e-flag", 'e')
            .Option("y-option", 'y', s => s.ToString())
            .Argument(s => s.ToString());

        ParseResult result = cmd.Parse(new[] { "-key=value", "foo" });

        Assert.True(result.GetFlag("k-flag"));
        Assert.True(result.GetFlag("e-flag"));
        Assert.Equal("value", result.GetOption("y-option", () => string.Empty));
        Assert.Equal("foo", result.GetArgument<string>(0, () => string.Empty));
    }

    [Fact]
    public void ShortGroup_NextTokenAsValue_ForLastKey() {
        // -key 后面的第一个位置参数应作为 y 的值
        Command cmd = new Command()
            .Flag("k-flag", 'k')
            .Flag("e-flag", 'e')
            .Option("y-option", 'y', s => s.ToString());

        ParseResult result = cmd.Parse(new[] { "-key", "bar" });

        Assert.True(result.GetFlag("k-flag"));
        Assert.True(result.GetFlag("e-flag"));
        Assert.Equal("bar", result.GetOption("y-option", () => string.Empty));
    }
}

